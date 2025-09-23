namespace Connectly.API.Extensions;

public static class IdentityServiceExtensions
{
    public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddSingleton<ITokenBlacklistService, TokenBlacklistService>();

        services.AddIdentity<AppUser, IdentityRole<int>>(options =>
        {   options.User.RequireUniqueEmail = true;
            options.Password.RequiredUniqueChars = 2;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;
        }).AddEntityFrameworkStores<ApplicationContext>();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            var secretKey = Encoding.UTF8.GetBytes(configuration["JWT:SecretKey"]);
            var requiredKeyLength = 256 / 8;
            if (secretKey.Length < requiredKeyLength)
            {
                Array.Resize(ref secretKey, requiredKeyLength);
            }
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateAudience = true,
                ValidAudience = configuration["JWT:ValidAudience"],
                ValidateIssuer = true,
                ValidIssuer = configuration["JWT:ValidIssuer"],
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(secretKey),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromDays(double.Parse(configuration["JWT:AccessTokenDurationInMinutes"] ?? string.Empty))
            };

            
        });

        return services;
    }
}
