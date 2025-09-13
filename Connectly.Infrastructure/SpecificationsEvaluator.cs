namespace Connectly.Infrastructure;

public class SpecificationsEvaluator<T> where T : class
{
    public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecification<T> specs)
    {
        var query = inputQuery;

        if (specs.Criteria is not null)
            query = query.Where(specs.Criteria);

        if (specs.OrderBy is not null)
            query = query.OrderBy(specs.OrderBy);
        else if (specs.OrderByDesc is not null)
            query = query.OrderByDescending(specs.OrderByDesc);

        if (specs.IsPaginationEnabled)
            query = query.Skip(specs.Skip).Take(specs.Take);

        return specs.Includes.Aggregate(query, (currentQuery, includeExpression) => currentQuery.Include(includeExpression));



        return query;
    }
}
