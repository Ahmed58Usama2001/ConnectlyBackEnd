
namespace Connectly.Infrastructure.Data.Configurations;

public class MessageConfigurations : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.HasOne(x => x.Recipient)
            .WithMany(m => m.MessagesRecieved)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Sender)
          .WithMany(m => m.MessagesSent)
          .OnDelete(DeleteBehavior.Restrict);
    }
}
