

namespace Connectly.API.Controllers;

public class MembersController(UserManager<AppUser> userManager, IMapper mapper) : BaseApiController
{
    

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<UserDto>>> GetMembers()
    {
        var members = await userManager.Users
            .ProjectTo<UserDto>(mapper.ConfigurationProvider)
            .ToListAsync();

        return Ok(members);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetMember(string id)
    {
        if (!Guid.TryParse(id, out var publicId))
            return BadRequest("Invalid ID format.");

        var member = await userManager.Users
            .Where(u => u.PublicId == publicId)
            .ProjectTo<UserDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();

        if (member == null)
            return NotFound();

        return Ok(member);
    }
}
