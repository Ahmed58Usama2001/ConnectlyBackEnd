


namespace Connectly.API.SingalR;

[Authorize]
public class PresenceHub(PresenceTracker presenceTracker):Hub
{
    public override async Task OnConnectedAsync()
    {
        await presenceTracker.UserConnected(GetUserId(), Context.ConnectionId);
        await Clients.Others.SendAsync("UserIsOnline", GetUserId());

        var currentUsers = await presenceTracker.GetOnlineUsers();
        await Clients.All.SendAsync("GetOnlineUsers", currentUsers);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await presenceTracker.UserDisonnected(GetUserId(), Context.ConnectionId);
        await Clients.Others.SendAsync("UserIsOffline", GetUserId());

        var currentUsers = await presenceTracker.GetOnlineUsers();
        await Clients.All.SendAsync("GetOnlineUsers", currentUsers);

        await base.OnDisconnectedAsync(exception);
    }

    private string GetUserId()
    {
        return Context.User?.FindFirstValue(ClaimTypes.NameIdentifier)??throw new HubException("Cannot get memberId");
    }
}
