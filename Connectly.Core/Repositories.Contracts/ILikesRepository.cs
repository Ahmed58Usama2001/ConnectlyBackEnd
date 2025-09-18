namespace Connectly.Core.Repositories.Contracts;

public interface ILikesRepository
{
    Task<MemberLike> GetMemberLike(int sourceMemberId, int targetMemberId);
    Task<IReadOnlyList<AppUser>> GetMemberLikes(int sourceMemberId, string predicate);
    Task<int> GetCurrentMemberLikeIds(int sourceMemberId);

    void DeleteLike(MemberLike like);
    void AddLike(MemberLike like);

    Task<bool> SaveAllChangesAsync();
}
