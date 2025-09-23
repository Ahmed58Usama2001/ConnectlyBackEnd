namespace Connectly.Core.Repositories.Contracts;

public interface IMessageRepository
{
    void Add(Message message);
    void Delete(Message message);
    Task<Message?> GetMessage(int messgeId);

    Task<int> GetMessagesCountAsync(ISpecification<Message> spec);
    Task<IReadOnlyList<Message>> GetMessagesWithSpecAsync(ISpecification<Message> spec);

    Task<IReadOnlyList<Message>> GetMessageThread(int CurrentMemberId, int receipientId);

    Task<bool> SaveAllAsync();
}
