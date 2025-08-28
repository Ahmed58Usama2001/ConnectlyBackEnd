

namespace Connectly.Core.Entities;

public class AppUser:IdentityUser<int>
{
    public Guid PublicId { get; set; } = Guid.NewGuid();
    public string? ImageUrl { get; set; }

    public DateOnly? DateOfBirth { get; set; }  
    public DateTime Created { get; set; } = DateTime.UtcNow; //Utc consistent time zone and the browser converts it to local time
    public DateTime LastActive { get; set; } 
    public string? Gender { get; set; }
    public string? Description { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }

    public List<Photo> Photos { get; set; } = new();

}
