
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
                foreach (var user in users)
                {
                    user.Photos.Add(new Photo
                    {
                        Url = user.ImageUrl!,
                        AppUserId = user.Id,
                    });

                    context.Users.Add(user);
                }

            }
            else
            {
                Console.WriteLine("There are no users to seed");
                return;
            }

            await context.SaveChangesAsync();
        }
    }
}
