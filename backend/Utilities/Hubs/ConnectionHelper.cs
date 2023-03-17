using System.Collections.Concurrent;
namespace Melpominee.app.Utilities.Hubs;

public class ConnectionHelper
{
    public ConcurrentDictionary<string, ConcurrentDictionary<string, bool>> UserMap = 
        new ConcurrentDictionary<string, ConcurrentDictionary<string, bool>>();
    
    public void OnConnect(string identifier, string connectionId)
    {
        UserMap.AddOrUpdate
        (
            identifier, 
            new ConcurrentDictionary<string, bool>() 
            { 
                [connectionId] = true
            }, 
            (k, l) => 
            { 
                l.TryAdd(connectionId, true);
                return l; 
            }
        );
    }

    public void OnDisconnect(string identifier, string connectionId)
    {
        ConcurrentDictionary<string, bool>? connectionMap;
        if(UserMap.TryGetValue(identifier, out connectionMap))
        {
            connectionMap.TryRemove(
                new KeyValuePair<string, bool>(connectionId, true)
            );
        }
    }

    public List<string> GetUserConnections(string identifier)
    {
        ConcurrentDictionary<string, bool>? connectionMap;
        if(UserMap.TryGetValue(identifier, out connectionMap))
        {
            return connectionMap.Keys.ToList<string>();
        }
        return new List<string>();
    }
}