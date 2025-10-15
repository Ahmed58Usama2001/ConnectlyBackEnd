



namespace Connectly.API;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        builder.Services.AddSwaggerServices();

        builder.Services.AddDbContext<ApplicationContext>(options => {
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
        });

        builder.Services.AddSingleton<IConnectionMultiplexer>((config) =>
        {
            var connectionString = builder.Configuration.GetConnectionString("Redis") ?? throw new Exception("Cannot get redis connections string ");
            var configuration = ConfigurationOptions.Parse(connectionString, true);
            return ConnectionMultiplexer.Connect(configuration);
        });

        builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));
        builder.Services.AddSignalR();
        builder.Services.AddSingleton<PresenceTracker>();


        builder.Services.AddApplicationServices();
        builder.Services.AddIdentityServices(builder.Configuration);

        builder.Services.AddCors();

        var app = builder.Build();

        app.Use(async (context, next) =>
        {
            var cookies = context.Request.Headers["Cookie"];
            Console.WriteLine($"Cookies sent: {cookies}");
            await next();
        });


        app.UseCors(options =>
        {
            options.WithOrigins("http://localhost:4200")
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials();
        });

        app.UseMiddleware<ExceptionMiddleware>();
        app.UseMiddleware<JwtBlacklistMiddleware>();

        app.UseAuthentication();
        app.UseAuthorization();

        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            try
            {
                var context = services.GetRequiredService<ApplicationContext>();
                var userManager = services.GetRequiredService<UserManager<AppUser>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole<int>>>();

                await context.Database.MigrateAsync();
                await ApplicationContextSeed.SeedAsync(userManager, roleManager);
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred during migration");
            }
        }

        if (app.Environment.IsDevelopment())
        {
            app.UseSwaggerMiddlewares();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.MapControllers();
        app.MapHub<PresenceHub>("hubs/presence");

        app.Run();
    }
}
