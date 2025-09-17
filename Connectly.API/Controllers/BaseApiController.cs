namespace Connectly.API.Controllers;

[ServiceFilter(typeof(LogUserActivity))]
[Route("api/[controller]")]
[ApiController]
public class BaseApiController : ControllerBase
{
}