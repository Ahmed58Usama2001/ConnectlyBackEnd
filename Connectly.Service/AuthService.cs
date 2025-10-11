

using System.Security.Cryptography;

namespace Connectly.Service;


public class AuthService(IConfiguration configuration) : IAuthService
{
    public async Task<string> CreateAccessTokenAsync(AppUser user, UserManager<AppUser> userManager)
    {
        var authClaims = new List<Claim>()
        {
            new Claim(ClaimTypes.NameIdentifier, user.PublicId.ToString()),
            new Claim(ClaimTypes.GivenName, user.UserName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var userRoles = await userManager.GetRolesAsync(user);
        foreach (var role in userRoles)
        {
            authClaims.Add(new Claim("role", role));
        }

        var secretKey = Encoding.UTF8.GetBytes(configuration["JWT:SecretKey"]);
        var requiredKeyLength = 256 / 8;
        if (secretKey.Length < requiredKeyLength)
        {
            Array.Resize(ref secretKey, requiredKeyLength);
        }

        var token = new JwtSecurityToken(
            audience: configuration["JWT:ValidAudience"],
            issuer: configuration["JWT:ValidIssuer"],
            expires: DateTime.UtcNow.AddMinutes(double.Parse(configuration["JWT:AccessTokenDurationInMinutes"])),
            claims: authClaims,
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256Signature)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenereateRefreshToken()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(64);

        return Convert.ToBase64String(randomBytes);
    }
}