using System.Collections.Generic;

namespace Connectly.Core.Repositories.Contracts;

public interface ILikesRepository
{
    Task<MemberLike?> GetMemberLike(int sourceMemberId, int targetMemberId);
    Task<IReadOnlyList<AppUser>> GetMemberLikes(int MemberId, string predicate);
    Task<IReadOnlyList<int>> GetCurrentMemberLikeIds(int sourceMemberId);

    void DeleteLike(MemberLike like);
    void AddLike(MemberLike like);

    Task<bool> SaveAllChangesAsync();
}
