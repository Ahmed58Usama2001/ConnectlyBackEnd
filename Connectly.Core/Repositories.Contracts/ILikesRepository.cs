namespace Connectly.Core.Repositories.Contracts;

public interface ILikesRepository
{
    Task<MemberLike> GetMemberLike(int sourceMemberId, int targetMemberId);
}
