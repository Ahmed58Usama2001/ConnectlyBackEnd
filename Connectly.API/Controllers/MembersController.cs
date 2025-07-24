using Microsoft.EntityFrameworkCore;

namespace Connectly.API.Controllers;


public class MembersController(UserManager<AppUser> userManager) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<AppUser>>> GetMembers()
    {
        var members = await userManager.Users.ToListAsync();

        return members;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AppUser>> GetMember(string id)
    {
        if (!Guid.TryParse(id, out var publicId))
            return BadRequest("Invalid ID format.");

        var member = await userManager.Users
            .FirstOrDefaultAsync(u => u.PublicId == publicId);

        if (member == null)
            return NotFound();

        return Ok(member);
    }

}
