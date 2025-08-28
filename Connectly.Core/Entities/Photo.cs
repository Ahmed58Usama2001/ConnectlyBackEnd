namespace Connectly.Core.Entities;

public class Photo : BaseEntity
{
    public string? PublicId { get; set; }
    public required string Url { get; set; }

    public int AppUserId { get; set; }
    public AppUser AppUser { get; set; } = null!;

}
