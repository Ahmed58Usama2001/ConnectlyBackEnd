
namespace Connectly.Infrastructure.Data;

public static class ApplicationContextSeed
{
    public static async Task SeedAsync(ApplicationContext context)
    {
        if (!await context.Users.AnyAsync())
        {
            var usersData = File.ReadAllText("../Connectly.Infrastructure/Data/DataSeed/UserSeedData.json");
            var users = JsonSerializer.Deserialize<List<AppUser>>(usersData);

            if (users?.Count > 0)
            {
                await context.Users.AddRangeAsync(users);
                await context.SaveChangesAsync();
            }
        }
    }
}
