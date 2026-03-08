using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR;
using Melpominee.app.Hubs.VTMV5;
using Melpominee.app.Hubs.Clients.VTMV5;
namespace Melpominee.app.Services.Hubs;

public class ConnectionService
{
    public ConcurrentDictionary<string, string> ConnectionMap =
        new ConcurrentDictionary<string, string>();
    public ConcurrentDictionary<string, ConcurrentDictionary<string, bool>> UserMap = 
        new ConcurrentDictionary<string, ConcurrentDictionary<string, bool>>();
    public ConcurrentDictionary<string, ConcurrentDictionary<string, bool>> GroupMap =
        new ConcurrentDictionary<string, ConcurrentDictionary<string, bool>>();
    
    public void OnConnect(IHubContext<CharacterHub, ICharacterClient> hub, string identifier, string connectionId)
    {
        ConnectionMap.AddOrUpdate
        (
            connectionId,
            identifier,
            (k, l) =>
            {
                return l;
            }
        );
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

    public void OnGroupAdd(IHubContext<CharacterHub, ICharacterClient> hub, string connId, string groupId)
    {
        // manage concurrent dictionary
        GroupMap.AddOrUpdate
        (
            groupId, 
            new ConcurrentDictionary<string, bool>() 
            { 
                [connId] = true
            }, 
            (k, l) => 
            { 
                l.TryAdd(connId, true);
                return l; 
            }
        );
    }

    public void OnDisconnect(IHubContext<CharacterHub, ICharacterClient> hub, string identifier, string connectionId)
    {
        ConnectionMap.TryRemove(
            new KeyValuePair<string, string>(identifier, connectionId)
        );
        ConcurrentDictionary<string, bool>? connectionMap;
        if(UserMap.TryGetValue(identifier, out connectionMap))
        {
            connectionMap.TryRemove(
                new KeyValuePair<string, bool>(connectionId, true)
            );
        }

        foreach(var groupKvp in GroupMap)
        {
            // remove user from group
            if(groupKvp.Value.ContainsKey(connectionId))
            {
                // remove connection from group
                groupKvp.Value.Remove(connectionId, out _);

                // alert group of change
                var connList = GroupMap[groupKvp.Key].Keys.ToList();
                var watcherDict = new Dictionary<string, bool>();
                foreach(var conn in connList)
                    watcherDict.Add(GetConnectedUser(conn), true);
                hub.Clients.Group(groupKvp.Key).WatcherUpdate(
                    int.Parse(groupKvp.Key.Split('_')[1]), 
                    watcherDict.Keys.ToList()
                );
            }
            // if no more users remain, remove group listener
            if(groupKvp.Value.Count <= 0)
                GroupMap.Remove(groupKvp.Key, out _);
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

    public string GetConnectedUser(string connId)
    {
        return ConnectionMap[connId];
    }
}