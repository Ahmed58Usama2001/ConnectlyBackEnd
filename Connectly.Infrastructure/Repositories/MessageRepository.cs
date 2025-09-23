namespace Connectly.Infrastructure.Repositories;

public class MessageRepository(ApplicationContext context) : IMessageRepository
{
    public void Add(Message message)
    {
        context.Add(message);
    }

    public void Delete(Message message)
    {
        context.Remove(message);
    }

    public async Task<Message?> GetMessage(int messgeId)
    {
        return await context.Messages.FindAsync(messgeId);
    }

    public async Task<int> GetMessagesCountAsync(ISpecification<Message> spec)
    {   
        var query = SpecificationsEvaluator<Message>.GetQuery(context.Messages,spec);
        return await query.CountAsync();
    }

    public async Task<IReadOnlyList<Message>> GetMessagesWithSpecAsync(ISpecification<Message> spec)
    {
        var query = SpecificationsEvaluator<Message>.GetQuery(context.Messages, spec);
        return await query.ToListAsync();
    }

    public async Task<IReadOnlyList<Message>> GetMessageThread(int CurrentMemberId, int receipientId)
    {
        await context.Messages
            .Where(m => m.RecipientId == CurrentMemberId  
            && m.SenderId == receipientId && m.DateRead == null)
            .ExecuteUpdateAsync(setters=>setters.SetProperty(m => m.DateRead, DateTime.UtcNow)); //changing the readDatefor the messages

        return await context.Messages
            .Where(m => (m.RecipientId == CurrentMemberId &&
            !m.RecipientDeleted &&
            m.SenderId == receipientId) 
            || (m.SenderId == CurrentMemberId &&
            !m.SenderDeleted
            &&m.RecipientId == receipientId))
            .OrderBy(m=>m.MessageSent).ToListAsync();
            


    }

    public async Task<bool> SaveAllAsync()
    {
         return await context.SaveChangesAsync() > 0;
    }
}
