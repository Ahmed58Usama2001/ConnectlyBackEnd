

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
        var member = await GetUserByPublicId(publicIdString!);
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

    [HttpGet("thread/{recipientId}")]
    public async Task<ActionResult<IReadOnlyList<MessageDto>>> GetMessageThread(string recipientId)
    {
        var publicIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var currentMember = await GetUserByPublicId(publicIdString!);

        var recipient = await GetUserByPublicId(recipientId.ToString());
        if (recipient == null)
            return NotFound();

        if (currentMember!.Id == recipient.Id)
            return BadRequest();

        var messages = await _messageRepository.GetMessageThread(currentMember!.Id, recipient.Id);

        return Ok(_mapper.Map<IReadOnlyList<Message>, IReadOnlyList<MessageDto>>(messages));
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteMessage(int id)
    {

        var message = await _messageRepository.GetMessage(id);
        if (message is null) return BadRequest();

        var publicIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var member = await GetUserByPublicId(publicIdString!);
        if (member!.Id != message.SenderId && member.Id != message.RecipientId)
            return Unauthorized();

        if(member.Id == message.SenderId)
            message.SenderDeleted = true;

        if (member.Id == message.RecipientId)
            message.RecipientDeleted = true;

        if(message is { RecipientDeleted: true, SenderDeleted : true })
            _messageRepository.Delete(message);

        if(await _messageRepository.SaveAllAsync())
            return Ok();

        return BadRequest("Problem deleting the message");
    }

    private async Task<AppUser?> GetUserByPublicId(string publicIdString)
    {
        if (!Guid.TryParse(publicIdString, out var publicId))
            return null;

        return await _userManager.Users.SingleOrDefaultAsync(u => u.PublicId == publicId);
    }
}
