
namespace Connectly.Infrastructure.Data.Configurations;

public class AppUserConfigurations : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.HasIndex(u => u.PublicId).IsUnique();

        builder.Property(u => u.PublicId)
               .HasDefaultValueSql("NEWID()");
    }
}
