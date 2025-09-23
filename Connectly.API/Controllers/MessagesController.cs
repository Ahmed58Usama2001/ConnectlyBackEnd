using Connectly.API.DTOs.MessagesDtos;
using Connectly.Core.Specifications.MessagesSpecs;

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

    [HttpGet]
    public async Task<ActionResult<Pagination<MessageDto>>> GetMessagsByContainer([FromQuery] MessageSpecificationsParams specParams)
    {
        var publicIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var member  =await GetUserByPublicId(publicIdString!);
        specParams.MemberId = member?.Id.ToString();

        var spec = new MessageSpecifications(specParams);
        var countSpec = new MessageForCountSpecifications(specParams);

        var messages = await _messageRepository.GetMessagesWithSpecAsync(spec);
        var totalCount = await _messageRepository.GetMessagesCountAsync(countSpec);

        var messagesToReturn = _mapper.Map<IReadOnlyList<MessageDto>>(messages);

        return Ok(new Pagination<MessageDto>(
            specParams.PageSize,
            specParams.PageIndex,
            totalCount,
            messagesToReturn
        ));
    }

    private async Task<AppUser?> GetUserByPublicId(string publicIdString)
    {
        if (!Guid.TryParse(publicIdString, out var publicId))
            return null;

        return await _userManager.Users.SingleOrDefaultAsync(u => u.PublicId == publicId);
    }
}
