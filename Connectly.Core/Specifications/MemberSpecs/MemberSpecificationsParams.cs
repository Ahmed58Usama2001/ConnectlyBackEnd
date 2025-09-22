namespace Connectly.Core.Specifications.MemberSpecs;

public class MemberSpecificationsParams
{
    public string? sort { get; set; }

    public string? Gender { get; set; }

    public int MinAge { get; set; } = 18;
    public int MaxAge { get; set; } = 100;


    private const int maxPageSize = 50;
    private int pageSize = 5;

    public int PageSize
    {
        get { return pageSize; }
        set { pageSize = value > maxPageSize ? maxPageSize : value; }
    }

    public int PageIndex { get; set; } = 1;
}
