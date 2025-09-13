namespace Connectly.Core.Specifications.MemberSpecs;

public class MemberSpecifications : BaseSpecifications<AppUser>
{
    public MemberSpecifications(MemberSpecificationsParams speceficationsParams)
        : base(u =>
            string.IsNullOrEmpty(speceficationsParams.Search)
              || u.UserName.ToLower().Contains(speceficationsParams.Search)
            )

    {

        if (!string.IsNullOrEmpty(speceficationsParams.sort))
        {
            switch (speceficationsParams.sort)
            {
                case "CreationDataAsc":
                    AddOrderBy(p => p.Created);
                    break;

                case "CreationDataDesc":
                    AddOrderByDesc(p => p.Created);
                    break;

                case "DateOfBirthAsc":
                    AddOrderBy(p => p.DateOfBirth!);
                    break;

                case "DateOfBirthDesc":
                    AddOrderByDesc(p => p.DateOfBirth!);
                    break;

                default:
                    AddOrderByDesc(p => p.Created);
                    break;
            }
        }
        else
            AddOrderByDesc(p => p.Created);

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