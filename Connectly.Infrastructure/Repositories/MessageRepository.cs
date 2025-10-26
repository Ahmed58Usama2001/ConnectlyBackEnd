namespace Connectly.Infrastructure.Repositories;

public class MessageRepository(ApplicationContext context) : IMessageRepository
{
    public void Add(Message message)
    {
        context.Add(message);
    }

    public void AddGroup(Group group)
    {
        context.Groups.Add(group);
    }

    public void Delete(Message message)
    {
        context.Remove(message);
    }

    public async Task<Connection?> GetConnection(string connectionId)
    {
        return await context.Connections.FindAsync(connectionId);
    }

    public async Task<Group?> GetGroupForConnection(string connectionId)
    {
        return await context.Groups.Include(x => x.connections)
            .Where(x => x.connections.Any(c => c.ConnectionId == connectionId))
            .FirstOrDefaultAsync();
    }

    public async Task<Message?> GetMessage(int messgeId)
    {
        return await context.Messages.FindAsync(messgeId);
    }

    public async Task<Group?> GetMessageGroup(string groupName)
    {
        return await context.Groups.Include(x => x.connections)
            .FirstOrDefaultAsync(x => x.Name == groupName);
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

    public async Task RemoveConnection(string connectionId)
    {
        await context.Connections
            .Where(c => c.ConnectionId == connectionId)
            .ExecuteDeleteAsync();
    }

    public async Task<bool> SaveAllAsync()
    {
         return await context.SaveChangesAsync() > 0;
    }
}
