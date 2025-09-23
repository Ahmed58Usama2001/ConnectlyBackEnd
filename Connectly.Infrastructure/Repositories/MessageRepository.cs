
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

    public Task<IReadOnlyList<Message>> GetMessagesWithSpecAsync()
    {
        throw new NotImplementedException();
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
