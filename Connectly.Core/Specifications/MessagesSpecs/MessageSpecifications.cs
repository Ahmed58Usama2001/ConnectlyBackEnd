namespace Connectly.Core.Specifications.MessagesSpecs;

public class MessageSpecifications : BaseSpecifications<Message>
{
    public MessageSpecifications(MessageSpecificationsParams speceficationsParams)
        : base()

    {
        AddIncludes();

        if (speceficationsParams.Container == "Outbox")
            Criteria = m => m.SenderId == int.Parse(speceficationsParams.MemberId!);
        
        else     
            Criteria = m => m.RecipientId == int.Parse(speceficationsParams.MemberId!);

        AddOrderByDesc(m => m.MessageSent);

        ApplyPagination((speceficationsParams.PageIndex - 1) * speceficationsParams.PageSize, speceficationsParams.PageSize);
    }

    public MessageSpecifications(int id)
        : base(u => u.Id == id)
    {
        AddIncludes();
    }

    private void AddIncludes()
    {
        Includes.Add(u => u.Sender);
        Includes.Add(u => u.Recipient);
    }

}