namespace Connectly.API.Controllers;

[Authorize]
public class LikesController(ILikesRepository _likesRepository,
    UserManager<AppUser> _userManager,
    IMapper _mapper) : BaseApiController
{
  
  

    [HttpPost("toggle/{targetUserPublicId:guid}")]
    public async Task<ActionResult> ToggleLike(Guid targetUserPublicId)
    {
        var sourceUser = await GetCurrentUserAsync();
        if (sourceUser == null)
            return Unauthorized("User not found");

        var targetUser = await _userManager.Users
            .SingleOrDefaultAsync(u => u.PublicId == targetUserPublicId);

        if (targetUser == null)
            return NotFound("Target user not found");

        if (sourceUser.Id == targetUser.Id)
            return BadRequest("You cannot like yourself");

        var existingLike = await _likesRepository.GetMemberLike(sourceUser.Id, targetUser.Id);

        if (existingLike != null)
        {
            _likesRepository.DeleteLike(existingLike);
        }
        else
        {
            var like = new MemberLike
            {
                SourceMemberId = sourceUser.Id,
                TargetMemberId = targetUser.Id
            };
            _likesRepository.AddLike(like);
        }

        if (await _likesRepository.SaveAllChangesAsync())
            return Ok(new { message = existingLike != null ? "Unliked successfully" : "Liked successfully" });

        return BadRequest("Failed to toggle like");
    }

    [HttpGet("list")]
    public async Task<ActionResult<IReadOnlyList<Guid>>> GetCurrentMemberLikeIds()
    {
        var sourceUser = await GetCurrentUserAsync();
        if (sourceUser == null)
            return Unauthorized("User not found");

        var likedPublicIds = await _likesRepository.GetCurrentMemberLikeIds(sourceUser.Id);
        return Ok(likedPublicIds);
    }


    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<AppUser>>> GetMemberLikes(string predicate)
    {
        var sourceUser = await GetCurrentUserAsync();
        if (sourceUser == null)
            return Unauthorized("User not found");

        var likedUsers = await _likesRepository.GetMemberLikes(sourceUser.Id, predicate);
        var members = _mapper.Map<IReadOnlyList<MemberDto>>(likedUsers);
        return Ok(members);
    }

    private async Task<AppUser?> GetCurrentUserAsync()
    {
        var publicIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(publicIdString, out var publicId))
            return null;

        return await _userManager.Users.SingleOrDefaultAsync(u => u.PublicId == publicId);
    }
}
