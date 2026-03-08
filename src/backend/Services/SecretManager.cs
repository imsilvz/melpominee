using System.Text.Json;
using System.Collections.Concurrent;
namespace Melpominee.app.Services;

public class SecretManager
{
    private static Lazy<SecretManager> _instance = new Lazy<SecretManager>(() => new SecretManager());
    public static SecretManager Instance => _instance.Value;

    private ConcurrentDictionary<string, string> _store;
    public SecretManager() 
    {
        _store = new ConcurrentDictionary<string, string>();
    }

    public void LoadSecret(string filename)
    {
        // check docker directory
        string filePath;
        if(Directory.Exists("/etc/melpominee/secrets/"))
        {
            // load as docker secret
            filePath = $"/etc/melpominee/secrets/{filename}.json";
        } 
        else
        {
            // load from local directory
            filePath = $"{Directory.GetCurrentDirectory()}/secrets/{filename}.json";
        }

        if (File.Exists(filePath))
        {
            // read file
            string secretText = File.ReadAllText(filePath);
            // load to store
            var secretDict = JsonSerializer.Deserialize<Dictionary<string, string>>(secretText);
            if (secretDict is not null)
            {
                foreach (var item in secretDict)
                {
                    _store.AddOrUpdate(item.Key, item.Value, (key, oldValue) => item.Value);
                }
            }
        }
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