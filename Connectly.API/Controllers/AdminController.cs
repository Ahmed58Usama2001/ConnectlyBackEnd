namespace Connectly.API.Controllers;


public class AdminController : BaseApiController
{
    [Authorize(Policy ="ModeratePhotoRole")]
    [HttpGet("photos-to-moderate")]
    public ActionResult GetPhotosForModeration()
    {
        return Ok("Admins or moderators can see this");
    }
}
