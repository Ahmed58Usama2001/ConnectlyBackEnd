namespace Connectly.Infrastructure.Repositories;

public class LikesRepository(ApplicationContext context
    ) : ILikesRepository
{
    public void AddLike(MemberLike like)
    {
        context.Likes.Add(like);
    }

    public void DeleteLike(MemberLike like)
    {
        context.Likes.Remove(like);
    }

    public async Task<IReadOnlyList<int>> GetCurrentMemberLikeIds(int sourceMemberId)
    {
        return await context.Likes
            .Where(like => like.SourceMemberId == sourceMemberId)
            .Select(like => like.TargetMemberId)
            .ToListAsync();
    }

    public async Task<MemberLike?> GetMemberLike(int sourceMemberId, int targetMemberId)
    {
        return await context.Likes.FindAsync(sourceMemberId, targetMemberId);
    }

    public async Task<IReadOnlyList<AppUser>> GetMemberLikes(int memberId, string predicate)
    {
        IQueryable<AppUser> query;

        switch (predicate.ToLower())
        {
            case "liked":
                query = context.Likes
                    .Where(like => like.SourceMemberId == memberId)
                    .Select(like => like.TargetMember);
                break;

            case "likedby":
                query = context.Likes
                    .Where(like => like.TargetMemberId == memberId)
                    .Select(like => like.SourceMember);
                break;

            default: // mutual likes
                var likedIds = await GetCurrentMemberLikeIds(memberId);

                query = context.Likes
                    .Where(like => like.TargetMemberId == memberId
                                   && likedIds.Contains(like.SourceMemberId))
                    .Select(like => like.SourceMember);
                break;
        }

        return await query.ToListAsync();
    }



    public async Task<bool> SaveAllChangesAsync()
    {
        return await context.SaveChangesAsync() > 0;
    }
}
