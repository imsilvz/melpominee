using System.Text.Json;
using System.Collections.Concurrent;
namespace Melpominee.app.Services;

public class SecretManager
{
    private static Lazy<SecretManager> _instance = new Lazy<SecretManager>(() => new SecretManager());
    public static SecretManager Instance => _instance.Value;

    private static readonly string[] _defaultSecretFiles = new[]
    {
        "discord-oauth", "pg-credentials", "redis-credentials", "mail-secrets"
    };

    private ConcurrentDictionary<string, string> _store;
    public SecretManager()
    {
        _store = new ConcurrentDictionary<string, string>();
    }

    private static string ResolveSecretPath(string filename)
    {
        if (Directory.Exists("/etc/melpominee/secrets/"))
        {
            return $"/etc/melpominee/secrets/{filename}.json";
        }
        return $"{Directory.GetCurrentDirectory()}/secrets/{filename}.json";
    }

    private void MergeSecretJson(string? secretText)
    {
        if (string.IsNullOrEmpty(secretText))
        {
            return;
        }
        var secretDict = JsonSerializer.Deserialize<Dictionary<string, string>>(secretText);
        if (secretDict is null)
        {
            return;
        }
        foreach (var item in secretDict)
        {
            _store.AddOrUpdate(item.Key, item.Value, (key, oldValue) => item.Value);
        }
    }

    public void LoadSecret(string filename)
    {
        var filePath = ResolveSecretPath(filename);
        if (File.Exists(filePath))
        {
            MergeSecretJson(File.ReadAllText(filePath));
        }
    }

    /// <summary>
    /// Async counterpart of <see cref="LoadSecret"/>. Safe to call from the
    /// background startup pipeline.
    /// </summary>
    public async Task LoadSecretAsync(string filename, CancellationToken ct = default)
    {
        var filePath = ResolveSecretPath(filename);
        if (File.Exists(filePath))
        {
            var secretText = await File.ReadAllTextAsync(filePath, ct);
            MergeSecretJson(secretText);
        }
    }

    /// <summary>
    /// Loads all four standard secret files in parallel. Used by
    /// <c>StartupInitializationService</c> during the <c>secrets</c> phase.
    /// </summary>
    public async Task LoadAllAsync(CancellationToken ct = default)
    {
        await Task.WhenAll(_defaultSecretFiles.Select(n => LoadSecretAsync(n, ct)));
    }

    public string? GetSecret(string key)
    {
        string? val;
        if (_store.TryGetValue(key, out val))
        {
            return val;
        }
        return null;
    }
}