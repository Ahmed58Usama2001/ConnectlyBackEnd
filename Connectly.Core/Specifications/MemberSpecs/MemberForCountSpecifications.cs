namespace Connectly.Core.Specifications.MemberSpecs
{
    public class MemberForCountSpecifications : BaseSpecifications<AppUser>
    {
        public MemberForCountSpecifications(MemberSpecificationsParams speceficationsParams)
            : base(u =>
                (string.IsNullOrEmpty(speceficationsParams.Gender) ||
                 u.Gender == speceficationsParams.Gender)
                &&
                (u.DateOfBirth.HasValue &&
                 u.DateOfBirth.Value <= DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-speceficationsParams.MinAge)) &&
                 u.DateOfBirth.Value >= DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-speceficationsParams.MaxAge)))
            )
        {
        }
    }
}
