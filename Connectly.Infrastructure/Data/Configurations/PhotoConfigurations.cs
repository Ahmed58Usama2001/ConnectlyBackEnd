
namespace Connectly.Infrastructure.Data.Configurations;

public class PhotoConfigurations : IEntityTypeConfiguration<Photo>
{
    public void Configure(EntityTypeBuilder<Photo> builder)
    {
        builder.HasOne(p => p.AppUser)
               .WithMany(u => u.Photos)
               .HasForeignKey(p => p.AppUserId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
