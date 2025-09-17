namespace Connectly.Core.Specifications.MemberSpecs;

public class MemberSpecifications : BaseSpecifications<AppUser>
{
    public MemberSpecifications(MemberSpecificationsParams speceficationsParams)
        : base(u => (string.IsNullOrEmpty(speceficationsParams.Gender) ||
                 u.Gender == speceficationsParams.Gender)
                &&
                (u.DateOfBirth.HasValue &&
                 u.DateOfBirth.Value <= DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-speceficationsParams.MinAge)) &&
                 u.DateOfBirth.Value >= DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-speceficationsParams.MaxAge)))
            )

    {

        if (!string.IsNullOrEmpty(speceficationsParams.sort))
        {
            switch (speceficationsParams.sort)
            {

                case "created":
                    AddOrderByDesc(p => p.Created);
                    break;

                default:
                    AddOrderByDesc(p => p.Created);
                    break;
            }
        }
        else
            AddOrderByDesc(p => p.LastActive);

        ApplyPagination((speceficationsParams.PageIndex - 1) * speceficationsParams.PageSize, speceficationsParams.PageSize);
    }

    public MemberSpecifications(string publicId)
        : base(u => u.PublicId == Guid.Parse(publicId))
    {
        AddIncludes();
    }

    private void AddIncludes()
    {
        Includes.Add(u => u.Photos);
    }

}