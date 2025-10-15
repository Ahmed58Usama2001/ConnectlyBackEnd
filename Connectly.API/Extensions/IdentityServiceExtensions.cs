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

            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Query["access_token"];

                    var path = context.HttpContext.Request.Path;
                    if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                    {
                        context.Token = accessToken;
                    }
                    return Task.CompletedTask;
                }
            };
        });

        services.AddAuthorizationBuilder()
            .AddPolicy("ModeratePhotoRole", policy => policy.RequireRole("Admin", "Moderator"))
            .AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));

        return services;
    }
}
