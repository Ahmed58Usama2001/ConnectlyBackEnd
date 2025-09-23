using Connectly.API.DTOs.MessagesDtos;

namespace Connectly.API.Controllers;

[Authorize]
public class MessagesController(IMessageRepository _messageRepository,
    UserManager<AppUser> _userManager,
    IMapper _mapper) : BaseApiController
{

    [HttpPost]
    public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
    {
        var publicIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var sender = await GetUserByPublicId(publicIdString!);

        var recipient = await GetUserByPublicId(createMessageDto.RecipientId.ToString());

        if (sender == null || recipient == null || sender.PublicId == createMessageDto.RecipientId)
            return NotFound("Can not send this message");

        var message = new Message
        {
            SenderId = sender.Id,
            RecipientId = recipient.Id,
            Content = createMessageDto.Content
        };

        _messageRepository.Add(message);

        if (await _messageRepository.SaveAllAsync())
            return _mapper.Map<MessageDto>(message);

        return BadRequest("Failed to send message");
    }

    private async Task<AppUser?> GetUserByPublicId(string publicIdString)
    {
        if (!Guid.TryParse(publicIdString, out var publicId))
            return null;

        return await _userManager.Users.SingleOrDefaultAsync(u => u.PublicId == publicId);
    }
}
