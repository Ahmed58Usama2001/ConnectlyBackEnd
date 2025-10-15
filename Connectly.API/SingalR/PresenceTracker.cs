
namespace Connectly.API.SingalR;

public class PresenceTracker
{
    private static readonly ConcurrentDictionary<string,
        ConcurrentDictionary<string, byte>> OnlineUsers = new();

    public Task UserConnected(string userId , string connectionId)
    {
        var connections = OnlineUsers.GetOrAdd(userId, _ => new ConcurrentDictionary<string, byte>());
        connections.TryAdd(connectionId, 0);
        return Task.CompletedTask;
    }

    public Task UserDisonnected(string userId, string connectionId)
    {
        if (OnlineUsers.TryGetValue(userId, out var connections))
        {
            connections.TryRemove(userId, out _);
            if (connections.IsEmpty)
            {
                OnlineUsers.TryRemove(userId, out _);
            }
        }
        return Task.CompletedTask;
    }

    public Task<string[]> GetOnlineUsers()
    {
        var onlineUsers = OnlineUsers.Keys.OrderBy(k => k).ToArray();
        return Task.FromResult(onlineUsers);
    }
}
