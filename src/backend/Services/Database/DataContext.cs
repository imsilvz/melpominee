using Dapper;
using Npgsql;
using System.Data;
using Melpominee.app.Services.Startup;
namespace Melpominee.app.Services.Database;

public class DataContext
{
    private static readonly Lazy<DataContext> _instance = new(() => new DataContext());
    public static DataContext Instance => _instance.Value;

    private NpgsqlDataSource? _dataSource;
    private readonly SemaphoreSlim _initGate = new(1, 1);
    private bool _initialized;
    private StartupReadiness? _readiness;

    public DataContext() { }

    /// <summary>
    /// Wires up the readiness gate used by <see cref="ConnectAsync"/>.
    /// Called exactly once from the DI factory in <c>Program.cs</c> during
    /// service registration, before any request can arrive.
    /// </summary>
    internal void AttachReadiness(StartupReadiness readiness)
    {
        _readiness = readiness;
    }

    /// <summary>
    /// Legacy sync connection path kept for the ~30 model-layer callers that
    /// still use <c>DataContext.Instance.Connect()</c>. Delegates to the
    /// shared <see cref="NpgsqlDataSource"/>. Does NOT consult
    /// <see cref="_readiness"/>: the k8s readiness probe holds inbound traffic
    /// until startup completes, and the shared <c>_initialized</c> flag
    /// means the async init runs exactly once across both paths.
    ///
    /// Returns a CLOSED <see cref="NpgsqlConnection"/> (via
    /// <see cref="NpgsqlDataSource.CreateConnection"/>) to preserve the
    /// contract of the original pre-refactor method — every legacy caller
    /// still follows the <c>Connect() + conn.Open()</c> pattern, and
    /// <see cref="NpgsqlDataSource.OpenConnection"/> would return an
    /// already-open connection that would throw on the next <c>Open</c>.
    /// </summary>
    public IDbConnection Connect()
    {
        EnsureDataSource();
        return _dataSource!.CreateConnection();
    }

    /// <summary>
    /// Async connection path used by the auth hot path during the cold-start
    /// window. Fast-paths when the readiness TCS is already resolved; when
    /// still pending, awaits it with a 10s safety cap.
    /// </summary>
    public async Task<NpgsqlConnection> ConnectAsync(CancellationToken ct = default)
    {
        // Fast path: plain bool check, zero cost when ready.
        if (_readiness is not null && !_readiness.IsReady)
        {
            await _readiness.WhenReadyAsync(ct)
                .WaitAsync(TimeSpan.FromSeconds(10), ct);
        }
        EnsureDataSource();
        return await _dataSource!.OpenConnectionAsync(ct);
    }

    /// <summary>
    /// Registers Dapper type handlers (exactly once) and runs the idempotent
    /// CREATE TABLE IF NOT EXISTS batch. Guarded by a semaphore so concurrent
    /// callers collapse to a single invocation.
    /// </summary>
    public async Task InitializeAsync(CancellationToken ct = default)
    {
        await _initGate.WaitAsync(ct);
        try
        {
            if (_initialized)
            {
                return;
            }
            SqlMapper.AddTypeHandler(new Models.Characters.VTMV5.VampireClanTypeHandler());
            SqlMapper.AddTypeHandler(new Models.Characters.VTMV5.VampirePredatorTypeHandler());
            EnsureDataSource();
            await using var conn = await _dataSource!.OpenConnectionAsync(ct);
            await conn.ExecuteAsync(SchemaSql);
            _initialized = true;
        }
        finally
        {
            _initGate.Release();
        }
    }

    /// <summary>
    /// Opens two parallel connections and disposes them, returning warm
    /// sockets to the Npgsql pool so the first real request doesn't pay the
    /// handshake cost.
    /// </summary>
    public async Task WarmPoolAsync(CancellationToken ct = default)
    {
        EnsureDataSource();
        await Task.WhenAll(
            OpenAndDisposeAsync(ct),
            OpenAndDisposeAsync(ct));
    }

    private async Task OpenAndDisposeAsync(CancellationToken ct)
    {
        await using var conn = await _dataSource!.OpenConnectionAsync(ct);
    }

    private void EnsureDataSource()
    {
        if (_dataSource is not null)
        {
            return;
        }
        var host = SecretManager.Instance.GetSecret("db_host");
        var user = SecretManager.Instance.GetSecret("db_user");
        var password = SecretManager.Instance.GetSecret("db_password");
        var database = SecretManager.Instance.GetSecret("db_database");
        var newSource = NpgsqlDataSource.Create(
            $"Host={host};Username={user};Password={password};Database={database}");
        // InitializeAsync and WarmPoolAsync both land here concurrently via
        // the Task.WhenAll fan-out in StartupInitializationService. CAS ensures
        // exactly one NpgsqlDataSource is stored; the loser disposes its own
        // so we don't leak a pool + background timer per cold start.
        if (Interlocked.CompareExchange(ref _dataSource, newSource, null) is not null)
        {
            newSource.Dispose();
        }
    }

    /// <summary>
    /// Legacy sync init retained as a blocking delegator so any unmigrated
    /// caller still works. Prefer <see cref="InitializeAsync"/>.
    /// </summary>
    [Obsolete("Use InitializeAsync")]
    public void Initalize() =>
        InitializeAsync(CancellationToken.None).GetAwaiter().GetResult();

    private const string SchemaSql = @"
        CREATE TABLE IF NOT EXISTS melpominee_users (
            Email TEXT NOT NULL PRIMARY KEY,
            DiscordName TEXT DEFAULT '',
            Password TEXT,
            Role TEXT NOT NULL DEFAULT 'user',
            ActivationKey TEXT,
            ActivationRequested TIMESTAMP,
            ActivationCompleted TIMESTAMP,
            LastLogin TIMESTAMP DEFAULT NOW(),
            Active BOOL DEFAULT false
        );
        CREATE TABLE IF NOT EXISTS melpominee_users_rescue (
            Id BIGSERIAL NOT NULL PRIMARY KEY,
            Email TEXT NOT NULL,
            RescueKey TEXT,
            RescueRequested TIMESTAMP,
            RescueCompleted TIMESTAMP,
            FOREIGN KEY(Email) REFERENCES melpominee_users(Email)
        );
        CREATE TABLE IF NOT EXISTS melpominee_characters (
            Id BIGSERIAL NOT NULL PRIMARY KEY,
            Owner TEXT NOT NULL,
            Name TEXT NOT NULL,
            Concept TEXT NOT NULL,
            Chronicle TEXT NOT NULL,
            Ambition TEXT NOT NULL,
            Desire TEXT NOT NULL,
            Sire TEXT NOT NULL,
            Generation INTEGER NOT NULL,
            Clan TEXT NOT NULL,
            PredatorType TEXT NOT NULL,
            Hunger INTEGER NOT NULL,
            Resonance TEXT NOT NULL,
            BloodPotency INTEGER NOT NULL,
            XpSpent INTEGER NOT NULL,
            XpTotal INTEGER NOT NULL,
            Active BOOL DEFAULT false,
            FOREIGN KEY(Owner) REFERENCES melpominee_users(Email)
        );
        CREATE TABLE IF NOT EXISTS melpominee_character_attributes (
            Id BIGSERIAL NOT NULL PRIMARY KEY,
            CharId INTEGER NOT NULL,
            Attribute TEXT NOT NULL,
            Score INTEGER NOT NULL,
            UNIQUE(CharId, Attribute),
            FOREIGN KEY(CharId) REFERENCES melpominee_characters(Id)
        );
        CREATE TABLE IF NOT EXISTS melpominee_character_skills (
            Id BIGSERIAL NOT NULL PRIMARY KEY,
            CharId INTEGER NOT NULL,
            Skill TEXT NOT NULL,
            Speciality TEXT NOT NULL,
            Score INTEGER NOT NULL,
            UNIQUE(CharId, Skill),
            FOREIGN KEY(CharId) REFERENCES melpominee_characters(Id)
        );
        CREATE TABLE IF NOT EXISTS melpominee_character_secondary (
            Id BIGSERIAL NOT NULL PRIMARY KEY,
            CharId INTEGER NOT NULL,
            StatName TEXT NOT NULL,
            BaseValue INTEGER NOT NULL,
            SuperficialDamage INTEGER NOT NULL,
            AggravatedDamage INTEGER NOT NULL,
            UNIQUE(CharId, StatName),
            FOREIGN KEY(CharId) REFERENCES melpominee_characters(Id)
        );
        CREATE TABLE IF NOT EXISTS melpominee_character_disciplines (
            Id BIGSERIAL NOT NULL PRIMARY KEY,
            CharId INTEGER NOT NULL,
            Discipline TEXT NOT NULL,
            Score INTEGER NOT NULL,
            UNIQUE(CharId, Discipline),
            FOREIGN KEY(CharId) REFERENCES melpominee_characters(Id)
        );
        CREATE TABLE IF NOT EXISTS melpominee_character_discipline_powers (
            Id BIGSERIAL NOT NULL PRIMARY KEY,
            CharId INTEGER NOT NULL,
            PowerId TEXT NOT NULL,
            UNIQUE(CharId, PowerId),
            FOREIGN KEY(CharId) REFERENCES melpominee_characters(Id)
        );
        CREATE TABLE IF NOT EXISTS melpominee_character_beliefs (
            Id BIGSERIAL NOT NULL PRIMARY KEY,
            CharId INTEGER NOT NULL,
            Tenets TEXT NOT NULL,
            Convictions TEXT NOT NULL,
            Touchstones TEXT NOT NULL,
            UNIQUE(CharId),
            FOREIGN KEY(CharId) REFERENCES melpominee_characters(Id)
        );
        CREATE TABLE IF NOT EXISTS melpominee_character_meritflawbackgrounds (
            Id BIGSERIAL NOT NULL PRIMARY KEY,
            CharId INTEGER NOT NULL,
            ItemType TEXT NOT NULL,
            SortOrder INTEGER NOT NULL,
            Name TEXT NOT NULL,
            Score INTEGER NOT NULL,
            UNIQUE(CharId, ItemType, SortOrder),
            FOREIGN KEY(CharId) REFERENCES melpominee_characters(Id)
        );
        CREATE TABLE IF NOT EXISTS melpominee_character_profile (
            Id BIGSERIAL NOT NULL PRIMARY KEY,
            CharId INTEGER NOT NULL,
            TrueAge INTEGER NOT NULL,
            ApparentAge INTEGER NOT NULL,
            DateOfBirth TEXT NOT NULL,
            DateOfDeath TEXT NOT NULL,
            Description TEXT NOT NULL,
            History TEXT NOT NULL,
            Notes TEXT NOT NULL,
            UNIQUE(CharId),
            FOREIGN KEY(CharId) REFERENCES melpominee_characters(Id)
        );
        CREATE TABLE IF NOT EXISTS melpominee_users_favorites (
            Id BIGSERIAL NOT NULL PRIMARY KEY,
            Email TEXT NOT NULL,
            CharId BIGINT NOT NULL,
            FOREIGN KEY(Email) REFERENCES melpominee_users(Email),
            FOREIGN KEY(CharId) REFERENCES melpominee_characters(Id)
        );
    ";
}
