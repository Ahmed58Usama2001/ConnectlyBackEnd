
using Microsoft.AspNetCore.Identity;

namespace Connectly.API.SingalR;

[Authorize]
public class MessageHub(IMessageRepository messageRepository, UserManager<AppUser> userManager, IMapper mapper):Hub
{
    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        var otherUserPublicId = httpContext?.Request.Query["userId"].ToString()??throw new HubException("Other user not found");

        var groupName = GetGroupName(GetUserId(), otherUserPublicId!);
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

        var currentUser = await GetUserByPublicId(GetUserId());
        var otherUser = await GetUserByPublicId(otherUserPublicId);
        var messages = await messageRepository.GetMessageThread(currentUser!.Id, otherUser!.Id);

        await Clients.Group(groupName).SendAsync("ReceiveMessageThread", messages);
    }

    public async Task SendMessage(CreateMessageDto createMessageDto)
    {
        var sender = await GetUserByPublicId(GetUserId());
        var recipient = await GetUserByPublicId(createMessageDto.RecipientId.ToString());

        if (sender == null || recipient == null || sender.PublicId == createMessageDto.RecipientId)
            throw new HubException("Cannot send this message");

        var message = new Message
        {
            SenderId = sender.Id,
            RecipientId = recipient.Id,
            Content = createMessageDto.Content
        };

        messageRepository.Add(message);

        if (await messageRepository.SaveAllAsync())
        {
            var group = GetGroupName(sender.PublicId.ToString(), recipient.PublicId.ToString());
            await Clients.Group(group).SendAsync("NewMessage", mapper.Map<MessageDto>(message));
        }
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

    private async Task<AppUser?> GetUserByPublicId(string publicIdString)
    {
        if (!Guid.TryParse(publicIdString, out var publicId))
            return null;

        return await userManager.Users.SingleOrDefaultAsync(u => u.PublicId == publicId);
    }
}
