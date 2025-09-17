
namespace Connectly.API.Helpers;

public class LogUserActivity : IAsyncActionFilter
{
    private readonly UserManager<AppUser> _userManager;

    public LogUserActivity(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var resultContext = await next();

        if (!resultContext.HttpContext.User.Identity?.IsAuthenticated ?? true)
            return;

        var userPublicId = resultContext.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (Guid.TryParse(userPublicId, out var guidId))
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.PublicId == guidId);
            if (user != null)
            {
                user.LastActive = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);
            }
        }
    }
}
