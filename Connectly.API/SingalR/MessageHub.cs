
namespace Connectly.API.SingalR;

[Authorize]
public class MessageHub(IMessageRepository messageRepository, UserManager<AppUser> userManager):Hub
{
    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        var otherUserPublicId = httpContext?.Request.Query["userId"].ToString()??throw new HubException("Other user not found");

        var groupName = GetGroupName(GetUserId(), otherUserPublicId!);
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

        var currentUser = await userManager.Users.FirstOrDefaultAsync(u=>u.PublicId.ToString() == GetUserId());
        var otherUser = await userManager.Users.FirstOrDefaultAsync(u => u.PublicId.ToString() == otherUserPublicId);
        var messages = await messageRepository.GetMessageThread(currentUser!.Id, otherUser!.Id);

        await Clients.Group(groupName).SendAsync("ReceiveMessageThread", messages);
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        return base.OnDisconnectedAsync(exception);
    }

    private static string GetGroupName(string? caller, string? other)
    {
        var stringCompare = string.CompareOrdinal(caller, other) < 0;
        return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}"; 
    }

    private string GetUserId()
    {
        return Context.User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new HubException("Cannot get memberId");
    }
}
