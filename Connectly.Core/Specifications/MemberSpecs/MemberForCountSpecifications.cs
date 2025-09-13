namespace Connectly.Core.Specifications.MemberSpecs;

public class MemberForCountSpecifications : BaseSpecifications<AppUser>
{
    public MemberForCountSpecifications(MemberSpecificationsParams speceficationsParams) :
        base(u =>
            string.IsNullOrEmpty(speceficationsParams.Search)
              || u.UserName.ToLower().Contains(speceficationsParams.Search)
            )
    {

    }
}
