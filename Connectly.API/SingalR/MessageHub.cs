
using Connectly.Core.Entities;
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
        await AddToGroup(groupName);

        var currentUser = await GetUserByPublicId(GetUserId());
        var otherUser = await GetUserByPublicId(otherUserPublicId);
        var messages = await messageRepository.GetMessageThread(currentUser!.Id, otherUser!.Id);
        var mappedMessages = mapper.Map<IReadOnlyList<Message>, IReadOnlyList<MessageDto>>(messages);

        await Clients.Group(groupName).SendAsync("ReceiveMessageThread", mappedMessages);
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

        var groupName = GetGroupName(sender.PublicId.ToString(), recipient.PublicId.ToString());
        var group = await messageRepository.GetMessageGroup(groupName);
        if (group != null && group.connections.Any(c => c.UserId == recipient.PublicId.ToString()))
        {
            message.DateRead = DateTime.UtcNow;
        }

        messageRepository.Add(message);

        if (await messageRepository.SaveAllAsync())
            await Clients.Group(groupName).SendAsync("NewMessage", mapper.Map<MessageDto>(message));
        
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await messageRepository.RemoveConnection(Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }

    private async Task<bool> AddToGroup (string groupName)
    {
        var group = await messageRepository.GetMessageGroup(groupName);
        var connection = new Connection(Context.ConnectionId, GetUserId());

        if(group is null)
        {
            group = new Group(groupName);
            messageRepository.AddGroup(group);
        }

        group.connections.Add(connection);
        return await messageRepository.SaveAllAsync();
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
