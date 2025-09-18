namespace Connectly.API.Controllers
{
    [Authorize]
    public class LikesController : BaseApiController
    {
        private readonly ILikesRepository _likesRepository;
        private readonly UserManager<AppUser> _userManager;

        public LikesController(ILikesRepository likesRepository, UserManager<AppUser> userManager)
        {
            _likesRepository = likesRepository;
            _userManager = userManager;
        }

        [HttpPost("toggle/{targetUserPublicId:guid}")]
        public async Task<ActionResult> ToggleLike(Guid targetUserPublicId)
        {
            var sourceUser = await _userManager.GetUserAsync(User);
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

    }
}
