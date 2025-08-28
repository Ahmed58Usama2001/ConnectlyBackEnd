namespace Connectly.Core.Repositories.Contracts;

public interface IUserRepository
{
    Task<IReadOnlyList<Photo>> GetPhotosForUserAsync(string userId);
}
