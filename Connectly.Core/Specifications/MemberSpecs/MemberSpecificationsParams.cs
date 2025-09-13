namespace Connectly.Core.Specifications.MemberSpecs;

public class MemberSpecificationsParams
{
    public string? sort { get; set; }

    private string? search;
    public string? Search
    {
        get { return search; }
        set { search = value?.ToLower(); }
    }


    private const int maxPageSize = 50;
    private int pageSize = 5;

    public int PageSize
    {
        get { return pageSize; }
        set { pageSize = value > maxPageSize ? maxPageSize : value; }
    }

    public int PageIndex { get; set; } = 1;
}
