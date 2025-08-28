namespace Connectly.Infrastructure.Repositories;

public class UserRepository(UserManager<AppUser> userManager) : IUserRepository
{
    public async Task<IReadOnlyList<Photo>> GetPhotosForUserAsync(string userId)
    {
        return await userManager.Users
     .Where(u => u.PublicId.ToString() == userId)
     .SelectMany(u => u.Photos)
     .ToListAsync();  //Loads only the photos of the selected user.


        //    await _context.Users
        //.Include(u => u.Photos)
        //.FirstOrDefaultAsync(u => u.PublicId.ToString() == userId);
        //    Loads the selected user plus their photos.
        //}
    }
}
