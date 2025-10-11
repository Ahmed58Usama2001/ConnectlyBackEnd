using Microsoft.AspNetCore.Identity;

namespace Connectly.API.Controllers;


public class AdminController(UserManager<AppUser> userManager) : BaseApiController
{
    [Authorize(Policy = "RequireAdminRole")]
    [HttpGet("users-with-roles")]
    public async Task<ActionResult> GetUsersWithRoles()
    {
        var users = await userManager.Users.ToListAsync();
        var usersList = new List<object>();

        foreach (var user in users)
        {
            var roles = await userManager.GetRolesAsync(user);
            usersList.Add(new
            {
                user.Id,
                user.UserName,
                Roles = roles
            });
        }

        return Ok(usersList);
    }

    [Authorize(Policy = "RequireAdminRole")]
    [HttpPut("edit-roles/{userId}")]
    public async Task<ActionResult<IList<string>>> EditRoles(string userId,[FromQuery] string roles)
    {
        if(string.IsNullOrEmpty(roles)) return BadRequest("You must select at least one role");

        var selectedRoles = roles.Split(',').ToArray();

        if (!Guid.TryParse(userId, out var guidId))
            return BadRequest("Couldn't retrieve user");
        var user = await userManager.Users
        .FirstOrDefaultAsync(u => u.PublicId == guidId);
        if(user == null) return BadRequest("Could not find user");

        var userRoles =await userManager.GetRolesAsync(user);

        var result = await userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));
        if (!result.Succeeded) return BadRequest("Failed to add to roles");

        result = await userManager.AddToRolesAsync(user, userRoles.Except(selectedRoles));
        if (!result.Succeeded) return BadRequest("Failed to remove from roles");

        return Ok(await userManager.GetRolesAsync(user));

    }

    [Authorize(Policy ="ModeratePhotoRole")]
    [HttpGet("photos-to-moderate")]
    public ActionResult GetPhotosForModeration()
    {
        return Ok("Admins or moderators can see this");
    }
}
