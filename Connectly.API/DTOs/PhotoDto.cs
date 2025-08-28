namespace Connectly.API.DTOs;

public class PhotoDto
{
    public string PublicId { get; set; }
    public required string Url { get; set; }

    public string AppUserId { get; set; }
}
