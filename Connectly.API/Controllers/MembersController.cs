using System.Collections.Generic;

namespace Connectly.API.Controllers;

[Authorize]
public class MembersController(
    UserManager<AppUser> userManager,
    IUserRepository userRepository,
    IMapper mapper) : BaseApiController
{
    private readonly UserManager<AppUser> _userManager = userManager;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IMapper _mapper = mapper;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<MemberDto>>> GetMembers()
    {
        var members = await _userManager.Users
            .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        return Ok(members);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MemberDto>> GetMember(string id)
    {
        if (!Guid.TryParse(id, out var publicId))
            return BadRequest("Invalid ID format.");

        var member = await _userManager.Users
            .Where(u => u.PublicId == publicId)
            .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();

        if (member == null)
            return NotFound();

        return Ok(member);
    }

    [HttpGet("{id}/photos")]
    public async Task<ActionResult<IReadOnlyList<PhotoDto>>> GetMemberPhotos(string id)
    {
        var photos = await _userRepository.GetPhotosForUserAsync(id);

        if (!photos.Any())
            return NotFound("No photos found for this user.");

        ICollection<PhotoDto> photosToReturn = [];
        foreach (var photo in photos)
        {

            photosToReturn.Add(new PhotoDto()
            {
                Url = photo.Url,
                PublicId = photo.PublicId!,
                AppUserId = id
            });

        }

        return Ok(photosToReturn);
    }
}
