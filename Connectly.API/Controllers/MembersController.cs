
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
    public async Task<ActionResult<Pagination<MemberDto>>> GetMembers([FromQuery] MemberSpecificationsParams specParams)
    {
        var spec = new MemberSpecifications(specParams);
        var countSpec = new MemberForCountSpecifications(specParams);

        var users = await _userRepository.GetUsersWithSpecAsync(spec);
        var totalCount = await _userRepository.GetUsersCountAsync(countSpec);

        var members = _mapper.Map<IReadOnlyList<MemberDto>>(users);

        return Ok(new Pagination<MemberDto>(
            specParams.PageSize,
            specParams.PageIndex,
            totalCount,
            members
        ));
    }

    [HttpGet("{publicId}")]
    public async Task<ActionResult<MemberDto>> GetMember(string publicId)
    {
        var spec = new MemberSpecifications(publicId);

        var user = await _userRepository.GetUserWithSpecAsync(spec);

        if (user is null) return NotFound();

        return Ok(_mapper.Map<MemberDto>(user));
    }



    [HttpGet("{id}/photos")]
    public async Task<ActionResult<IReadOnlyList<PhotoDto>>> GetMemberPhotos(string id)
    {
        var photos = await _userRepository.GetPhotosForUserAsync(id);

        ICollection<PhotoDto> photosToReturn = [];
        foreach (var photo in photos)
        {

            photosToReturn.Add(new PhotoDto()
            {
                Id = photo.Id,
                Url = photo.Url,
                PublicId = photo.PublicId!,
                MemberId = id
            });

        }

        return Ok(photosToReturn);
    }
}
