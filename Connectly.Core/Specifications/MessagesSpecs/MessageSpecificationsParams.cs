namespace Connectly.Core.Specifications.MessagesSpecs;

public class MessageSpecificationsParams
{
    public string? MemberId { get; set; }
    public string Container { get; set; } = "Inbox";

    private const int maxPageSize = 50;
    private int pageSize = 5;

    public int PageSize
    {
        get { return pageSize; }
        set { pageSize = value > maxPageSize ? maxPageSize : value; }
    }

    public int PageIndex { get; set; } = 1;
}
