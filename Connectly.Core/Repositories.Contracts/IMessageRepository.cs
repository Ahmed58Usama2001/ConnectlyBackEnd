namespace Connectly.Core.Repositories.Contracts;

public interface IMessageRepository
{
    void Add(Message message);
    void Delete(Message message);
    Task<Message?> GetMessage(int messgeId);
    Task<IReadOnlyList<Message>> GetMessagesWithSpecAsync();

    Task<IReadOnlyList<Message>> GetMessageThread(Guid CurrentMemberId, Guid receipientId);

    Task<bool> SaveAllAsync()
}
