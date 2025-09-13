namespace Connectly.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly UserManager<AppUser> _userManager;

    public UserRepository(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IReadOnlyList<AppUser>> GetUsersWithSpecAsync(ISpecification<AppUser> spec)
    {
        var query = SpecificationsEvaluator<AppUser>.GetQuery(_userManager.Users, spec);
        return await query.ToListAsync();
    }

    public async Task<AppUser?> GetUserWithSpecAsync(ISpecification<AppUser> spec)
    {
        var query = SpecificationsEvaluator<AppUser>.GetQuery(_userManager.Users, spec);
        return await query.FirstOrDefaultAsync();
    }

    public async Task<int> GetUsersCountAsync(ISpecification<AppUser> spec)
    {
        var query = SpecificationsEvaluator<AppUser>.GetQuery(_userManager.Users, spec);
        return await query.CountAsync();
    }

    public async Task<IReadOnlyList<Photo>> GetPhotosForUserAsync(string userPublicId)
    {
        return await _userManager.Users
            .Where(u => u.PublicId.ToString() == userPublicId)
            .SelectMany(u => u.Photos)
            .ToListAsync();
    }
}
