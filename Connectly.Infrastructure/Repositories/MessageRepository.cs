

using Microsoft.AspNetCore.Identity;

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

    public Task<IReadOnlyList<Message>> GetMessageThread(Guid CurrentMemberId, Guid receipientId)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> SaveAllAsync()
    {
         return await context.SaveChangesAsync() > 0;
    }
}
