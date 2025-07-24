

namespace Connectly.Core.Entities;

public class AppUser:IdentityUser<int>
{
    public Guid PublicId { get; set; } = new Guid();
}
