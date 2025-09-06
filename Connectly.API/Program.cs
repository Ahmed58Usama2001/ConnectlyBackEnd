
using Connectly.API.Extensions;
using Connectly.API.Middlewares;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

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


        builder.Services.AddApplicationServices();
        builder.Services.AddIdentityServices(builder.Configuration);

        builder.Services.AddCors();

        var app = builder.Build();

        app.UseCors(options =>
        {
            options.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:4200", "https://localhost:4200");
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
                await context.Database.MigrateAsync();
                await ApplicationContextSeed.SeedAsync(context);
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

        app.Run();
    }
}
