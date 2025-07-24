namespace Connectly.API.DTOs;

public class PagedResultParameters<T, TDto>
    where T : BaseEntity
{
    public IGenericRepository<T> Repository { get; set; }
    public ISpecification<T> Specification { get; set; }
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public int Count { get; set; }
    public IMapper Mapper { get; set; }
}