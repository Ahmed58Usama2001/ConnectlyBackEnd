namespace Connectly.Core.Specifications.MemberSpecs
{
    public class MemberForCountSpecifications : BaseSpecifications<AppUser>
    {
        public MemberForCountSpecifications(MemberSpecificationsParams speceficationsParams)
            : base(u =>
                (string.IsNullOrEmpty(speceficationsParams.Gender) || u.Gender == speceficationsParams.Gender)
            )
        {
        }
    }
}
