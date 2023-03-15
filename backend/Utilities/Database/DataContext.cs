using Dapper;
using System.Data;
using Microsoft.Data.Sqlite;
using Melpominee.app.Models;
namespace Melpominee.app.Utilities.Database;


public class DataContext
{
    private static Lazy<DataContext> _instance = new Lazy<DataContext>(() => new DataContext());
    public static DataContext Instance => _instance.Value;

    public DataContext() {}

    public IDbConnection Connect()
    {
        return new SqliteConnection("Data Source=data/melpominee.db");
    }

    public void Initalize()
    {
        SqlMapper.AddTypeHandler(new Models.CharacterSheets.VTMV5.VampireClanTypeHandler());
        SqlMapper.AddTypeHandler(new Models.CharacterSheets.VTMV5.VampirePredatorTypeHandler());
        using(var conn = Connect())
        {
            var sql = @"
                CREATE TABLE IF NOT EXISTS melpominee_users (
                    Email TEXT NOT NULL PRIMARY KEY,
                    Password TEXT NOT NULL,
                    ActivationKey TEXT,
                    ActivationRequested DATETIME,
                    ActivationCompleted DATETIME,
                    Active BOOL DEFAULT false
                );
                CREATE TABLE IF NOT EXISTS melpominee_users_rescue (
                    Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                    Email TEXT NOT NULL,
                    RescueKey TEXT,
                    RescueRequested DATETIME,
                    RescueCompleted DATETIME,
                    FOREIGN KEY(Email) REFERENCES melpominee_users(Email)
                );
                CREATE TABLE IF NOT EXISTS melpominee_characters (
                    Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
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
                    XpTotal INTEGER NOT NULL
                );
                CREATE TABLE IF NOT EXISTS melpominee_character_attributes (
                    Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                    CharId INTEGER NOT NULL,
                    Attribute TEXT NOT NULL,
                    Score INTEGER NOT NULL,
                    UNIQUE(CharId, Attribute),
                    FOREIGN KEY(CharId) REFERENCES melpominee_characters(Id)
                );
                CREATE TABLE IF NOT EXISTS melpominee_character_skills (
                    Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                    CharId INTEGER NOT NULL,
                    Skill TEXT NOT NULL,
                    Speciality TEXT NOT NULL,
                    Score INTEGER NOT NULL,
                    UNIQUE(CharId, Skill),
                    FOREIGN KEY(CharId) REFERENCES melpominee_characters(Id)
                );
                CREATE TABLE IF NOT EXISTS melpominee_character_secondary (
                    Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                    CharId INTEGER NOT NULL,
                    StatName TEXT NOT NULL,
                    BaseValue INTEGER NOT NULL,
                    SuperficialDamage INTEGER NOT NULL,
                    AggravatedDamage INTEGER NOT NULL,
                    UNIQUE(CharId, StatName),
                    FOREIGN KEY(CharId) REFERENCES melpominee_characters(Id)
                );
                CREATE TABLE IF NOT EXISTS melpominee_character_disciplines (
                    Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                    CharId INTEGER NOT NULL,
                    Discipline TEXT NOT NULL,
                    Score INTEGER NOT NULL,
                    UNIQUE(CharId, Discipline),
                    FOREIGN KEY(CharId) REFERENCES melpominee_characters(Id)
                );
                CREATE TABLE IF NOT EXISTS melpominee_character_discipline_powers (
                    Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                    CharId INTEGER NOT NULL,
                    PowerId TEXT NOT NULL,
                    UNIQUE(CharId, PowerId),
                    FOREIGN KEY(CharId) REFERENCES melpominee_characters(Id)
                );
                CREATE TABLE IF NOT EXISTS melpominee_character_beliefs (
                    Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                    CharId INTEGER NOT NULL,
                    Tenets TEXT NOT NULL,
                    Convictions TEXT NOT NULL,
                    Touchstones TEXT NOT NULL,
                    UNIQUE(CharId),
                    FOREIGN KEY(CharId) REFERENCES melpominee_characters(Id)
                );
                CREATE TABLE IF NOT EXISTS melpominee_character_meritflawbackgrounds (
                    Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                    CharId INTEGER NOT NULL,
                    ItemType TEXT NOT NULL,
                    SortOrder INTEGER NOT NULL,
                    Name TEXT NOT NULL,
                    Score INTEGER NOT NULL,
                    UNIQUE(CharId, ItemType, SortOrder),
                    FOREIGN KEY(CharId) REFERENCES melpominee_characters(Id)
                );
                CREATE TABLE IF NOT EXISTS melpominee_character_profile (
                    Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
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
            ";
            conn.Execute(sql);
        }
    }
}