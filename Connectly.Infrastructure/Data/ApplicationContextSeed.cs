namespace Connectly.Infrastructure.Data
{
    public static class ApplicationContextSeed
    {
        public static async Task SeedAsync(
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole<int>> roleManager)
        {
            var roles = new List<string> { "Member", "Moderator", "Admin" };
            foreach (var role in roles)
            {
                if (!await roleManager.Roles.AnyAsync(r => r.Name == role))
                {
                    await roleManager.CreateAsync(new IdentityRole<int>
                    {
                        Name = role,
                        NormalizedName = role.ToUpper()
                    });
                }
            }

            if (!await userManager.Users.AnyAsync())
            {
                var usersData = File.ReadAllText("../Connectly.Infrastructure/Data/DataSeed/UserSeedData.json");
                var users = JsonSerializer.Deserialize<List<AppUser>>(usersData);

                if (users?.Count > 0)
                {
                    foreach (var user in users)
                    {
                        user.UserName = user.Email; 
                        user.NormalizedEmail = user.Email.ToUpper();
                        user.NormalizedUserName = user.Email.ToUpper();

                        var result = await userManager.CreateAsync(user, "P@ssw0rd");
                        if (result.Succeeded)
                        {
                            await userManager.AddToRoleAsync(user, "Member");
                        }
                    }
                }
            }

            var adminEmail = "admin@test.com";
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var adminUser = new AppUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    NormalizedEmail = adminEmail.ToUpper(),
                    NormalizedUserName = adminEmail.ToUpper(),
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, "P@ssw0rd");
                if (result.Succeeded)
                {
                    await userManager.AddToRolesAsync(adminUser, new[] { "Admin", "Moderator" });
                }
            }
        }
    }
}
