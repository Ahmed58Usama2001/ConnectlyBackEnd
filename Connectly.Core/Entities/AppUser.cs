

namespace Connectly.Core.Entities;

public class AppUser:IdentityUser<int>
{
    public Guid PublicId { get; set; } = Guid.NewGuid();
    public string? ImageUrl { get; set; }

    public string? Gender { get; set; }
    public DateOnly? DateOfBirth { get; set; }  
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public DateTime LastActive { get; set; } = DateTime.UtcNow;
    public string? Description { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
}
