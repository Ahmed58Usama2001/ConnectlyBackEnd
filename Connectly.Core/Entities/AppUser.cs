


namespace Connectly.Core.Entities;

public class AppUser:IdentityUser<int>
{
    public Guid PublicId { get; set; } = Guid.NewGuid();
    public string? ImageUrl { get; set; }

    public DateOnly? DateOfBirth { get; set; }  
    public DateTime Created { get; set; } = DateTime.UtcNow; 
    public DateTime LastActive { get; set; } 
    public string? Gender { get; set; }
    public string? Description { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }

    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiry { get; set; }

    public List<Photo> Photos { get; set; } = new();

    [JsonIgnore]
    public List<MemberLike> LikedByMembers { get; set; } = [];
    [JsonIgnore]
    public List<MemberLike> LikedMembers { get; set; } = [];

    [JsonIgnore]
    public List<Message> MessagesSent { get; set; } = [];
    [JsonIgnore]
    public List<Message> MessagesRecieved { get; set; } = [];

}
