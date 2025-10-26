namespace Connectly.Core.Entities;

public class Connection(string connectionId, string userId)
{
    public string ConnectionId { get; set; } = connectionId;
    public string UserId { get; set; } = userId.ToString();

    public Group Group { get; set; } = null!;
}
