namespace Connectly.API.Controllers;


public class AccountController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager,
    IAuthService authService,
    ITokenBlacklistService tokenBlacklistService,
    IPhotoService photoService) : BaseApiController
{
    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        var user = new AppUser
        {
            Email = registerDto.Email,
            UserName = registerDto.Email.Split('@')[0]
        };
        var result = await userManager.CreateAsync(user, registerDto.Password);

        if (!result.Succeeded)
            return BadRequest(new ApiResponse(400));

        var token = await authService.CreateAccessTokenAsync(user, userManager);

        return Ok(new UserDto
        {
            Id = user.PublicId.ToString(),
            UserName = user.UserName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            ImageUrl = user.ImageUrl,
            Token = token
        });
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto model)
    {
        if (ModelState.IsValid)
        {

            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return Unauthorized(new ApiResponse(401));

            var result = await signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded)
                return Unauthorized(new ApiResponse(401));

            var token = await authService.CreateAccessTokenAsync(user, userManager);

            return Ok(new UserDto
            {
                Id = user.PublicId.ToString(),
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                ImageUrl = user.ImageUrl,
                Token = token
            });
        }

        return Unauthorized(new ApiResponse(401));
    }


    [Authorize]
    [HttpPost("logout")]
    public async Task<ActionResult> Logout()
    {
        var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(userId))
            return BadRequest(new ApiResponse(400, "Invalid token"));

        await tokenBlacklistService.BlacklistTokenAsync(token);

        return NoContent();
    }

    [Authorize]
    [HttpGet("get-current-user")]
    public async Task<ActionResult<UserDto>> GetCurrentUser()
    {
        var userPublicId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userPublicId, out var guidId))
            return BadRequest(new ApiResponse(400));

        var user = await userManager.Users
        .FirstOrDefaultAsync(u => u.PublicId == guidId);

        if (user == null) return Unauthorized(new ApiResponse(401));
        var token = await authService.CreateAccessTokenAsync(user, userManager);


        return Ok(new UserDto
        {
            Id = user.PublicId.ToString(),
            UserName = user.UserName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            ImageUrl = user.ImageUrl,
            Token = token
        });
    }

    [Authorize]
    [HttpPut]
    public async Task<ActionResult> UpdateUser(MemberUpdateDto updateDto)
    {
        var userPublicId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userPublicId, out var guidId))
            return BadRequest(new ApiResponse(400));

        var user = await userManager.Users
       .FirstOrDefaultAsync(u => u.PublicId == guidId);

        if (user == null)
            return Unauthorized(new ApiResponse(401));

        user.UserName = updateDto.UserName ?? user.UserName;
        user.Description = updateDto.Description ?? user.Description;
        user.City = updateDto.City ?? user.City;
        user.Country = updateDto.Country ?? user.Country;



        var result = await userManager.UpdateAsync(user);

        if (!result.Succeeded)
            return BadRequest(new ApiResponse(400, "Failed to update user"));

        return NoContent();
    }

    [Authorize]
    [HttpPost("add-photo")]
    public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
    {
        var userPublicId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userPublicId, out var guidId))
            return BadRequest(new ApiResponse(400));

        var user = await userManager.Users
            .Include(u => u.Photos) 
            .FirstOrDefaultAsync(u => u.PublicId == guidId);

        if (user == null)
            return Unauthorized(new ApiResponse(401));

        var uploadPhotoResult = await photoService.UploadPhotoAsync(file);
        if (uploadPhotoResult.Error != null)
            return BadRequest(new ApiResponse(400, uploadPhotoResult.Error.Message));

        var photo = new Photo
        {
            Url = uploadPhotoResult.SecureUrl.AbsoluteUri,
            PublicId = uploadPhotoResult.PublicId,
            AppUserId = user.Id,
            AppUser = user
        };

        if (string.IsNullOrEmpty(user.ImageUrl))
            user.ImageUrl = photo.Url;

        user.Photos.Add(photo);

        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return BadRequest(new ApiResponse(400, "Failed to update user"));

        var photoDto = new PhotoDto
        {
            Id = photo.Id,
            PublicId = photo.PublicId,
            Url = photo.Url,
            MemberId = user.PublicId.ToString()
        };

        return Ok(photoDto);
    }

    [Authorize]
    [HttpPut("set-main-photo/{photoId}")]
    public async Task<ActionResult> SetMainPhoto(int photoId)
    {
        var userPublicId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userPublicId, out var guidId))
            return BadRequest(new ApiResponse(400));

        var user = await userManager.Users
            .Include(u => u.Photos)
            .FirstOrDefaultAsync(u => u.PublicId == guidId);

        if (user == null)
            return Unauthorized(new ApiResponse(401));

        var photo = user.Photos.FirstOrDefault(p => p.Id == photoId);
        if (photo == null || user.ImageUrl == photo.Url)
            return BadRequest("Cannot set this as main image." );

        user.ImageUrl = photo.Url;

        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return BadRequest(new ApiResponse(400, "Failed to update user main image"));

        return NoContent();
    }
}
