namespace Connectly.Core.Services.Conracts;

public interface IAuthService
{
    Task<string> CreateAccessTokenAsync(AppUser user, UserManager<AppUser> userManager);
    string GenereateRefreshToken();
}

