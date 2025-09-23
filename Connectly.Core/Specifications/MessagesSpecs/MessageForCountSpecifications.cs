namespace Connectly.Core.Specifications.MessagesSpecs;

public class MessageForCountSpecifications : BaseSpecifications<Message>
{
    public MessageForCountSpecifications(MessageSpecificationsParams speceficationsParams)
        : base()
    {
        if (speceficationsParams.Container == "Outbox")
            Criteria = m => m.SenderId == int.Parse(speceficationsParams.MemberId!);

        else
            Criteria = m => m.RecipientId == int.Parse(speceficationsParams.MemberId!);
    }

}
