namespace Connectly.Infrastructure.Data.Configurations;

public class MemberLikeConfigurations : IEntityTypeConfiguration<MemberLike>
{
    public void Configure(EntityTypeBuilder<MemberLike> builder)
    {
        builder.HasKey(k => new { k.SourceMemberId, k.TargetMemberId });

        builder.HasOne(l => l.SourceMember)
            .WithMany(u => u.LikedMembers)
            .HasForeignKey(l => l.SourceMemberId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(l => l.TargetMember)
            .WithMany(u => u.LikedByMembers)
            .HasForeignKey(l => l.TargetMemberId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(l => l.TargetMemberId);
    }


}
