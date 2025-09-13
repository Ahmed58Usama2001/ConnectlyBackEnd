namespace Connectly.Core.Repositories.Contracts;

public interface IUserRepository
{
    Task<IReadOnlyList<Photo>> GetPhotosForUserAsync(string userId);
    Task<IReadOnlyList<AppUser>> GetUsersWithSpecAsync(ISpecification<AppUser> spec);
    Task<AppUser?> GetUserWithSpecAsync(ISpecification<AppUser> spec);
    Task<int> GetUsersCountAsync(ISpecification<AppUser> spec);
}
