namespace Connectly.API.DTOs.MessagesDtos;

public class CreateMessageDto
{
    public Guid RecipientId { get; set; }
    public required string Content { get; set; }
}
