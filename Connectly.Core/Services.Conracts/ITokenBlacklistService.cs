namespace Connectly.Core.Services.Conracts;

public interface ITokenBlacklistService
{
    Task BlacklistTokenAsync(string token);
    Task<bool> IsTokenBlacklistedAsync(string token);
}