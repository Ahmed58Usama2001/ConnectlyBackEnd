


namespace Connectly.API.SingalR;

[Authorize]
public class PresenceHub:Hub
{
    public override async Task OnConnectedAsync()
    {
        await Clients.Others.SendAsync("UserIsOnline", Context.User?.FindFirstValue(ClaimTypes.Email));
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {

        await Clients.Others.SendAsync("UserIsOffline", Context.User?.FindFirstValue(ClaimTypes.Email));
        await base.OnDisconnectedAsync(exception);
    }
}
