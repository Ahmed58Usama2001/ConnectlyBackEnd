
namespace Connectly.API.Controllers;


public class AccountController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager,
    IAuthService authService,
    ITokenBlacklistService tokenBlacklistService,
    IPhotoService photoService) : BaseApiController
{
    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register([FromBody] RegisterDto registerDto)
    {
        var user = new AppUser
        {
            Email = registerDto.Email,
            UserName = registerDto.UserName,
            Gender = registerDto.Gender,
            City = registerDto.City,
            Country = registerDto.Country,
            DateOfBirth = registerDto.DateOfBirth,
        };

        var result = await userManager.CreateAsync(user, registerDto.Password);
        if (!result.Succeeded)
            return BadRequest(new ApiResponse(400, string.Join(", ", result.Errors.Select(e => e.Description))));

        await userManager.AddToRoleAsync(user, "Member");

        await SetRefreshTokenCookie(user);
        

        return Ok(new UserDto
        {
            Id = user.PublicId.ToString(),
            UserName = user.UserName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            ImageUrl = user.ImageUrl,
            Token = await authService.CreateAccessTokenAsync(user, userManager)
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


            await SetRefreshTokenCookie(user);

            return Ok(new UserDto
            {
                Id = user.PublicId.ToString(),
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                ImageUrl = user.ImageUrl,
                Token = await authService.CreateAccessTokenAsync(user, userManager)
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

        if (file == null || file.Length == 0)
            return BadRequest(new ApiResponse(400, "No photo file provided"));

        var uploadResult = await photoService.UploadPhotoAsync(file.OpenReadStream(), file.FileName);

        if (string.IsNullOrEmpty(uploadResult.Url) || string.IsNullOrEmpty(uploadResult.PublicId))
            return BadRequest(new ApiResponse(400, "Photo upload failed"));

        var photo = new Photo
        {
            Url = uploadResult.Url,
            PublicId = uploadResult.PublicId,
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
            return BadRequest("Cannot set this as main image.");

        user.ImageUrl = photo.Url;

        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return BadRequest(new ApiResponse(400, "Failed to update user main image"));

        return NoContent();
    }

    [Authorize]
    [HttpDelete("delete-photo/{photoId}")]
    public async Task<ActionResult> DeletePhoto(int photoId)
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
        if (photo == null)
            return NotFound(new ApiResponse(404, "Photo not found"));

        if (user.ImageUrl == photo.Url)
            return BadRequest(new ApiResponse(400, "Cannot delete main photo"));

        if (!string.IsNullOrEmpty(photo.PublicId))
        {
            var deleteResult = await photoService.DeletePhotoAsync(photo.PublicId);

            if (!deleteResult.Success)
                return BadRequest(new ApiResponse(400, "Failed to delete photo from Cloudinary"));
        }

        user.Photos.Remove(photo);

        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return BadRequest(new ApiResponse(400, "Failed to update user after photo deletion"));

        return Ok();
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<UserDto>> RefreshToken()
    {
        if (!Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
            return Unauthorized(new ApiResponse(401, "No refresh token provided"));

        var user = await userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken && u.RefreshTokenExpiry>DateTime.UtcNow);

        if (user == null || user.RefreshTokenExpiry <= DateTime.UtcNow)
            return Unauthorized(new ApiResponse(401, "Invalid or expired refresh token"));

        var newAccessToken = await authService.CreateAccessTokenAsync(user, userManager);
        await SetRefreshTokenCookie(user);

        return Ok(new UserDto
        {
            Id = user.PublicId.ToString(),
            UserName = user.UserName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            ImageUrl = user.ImageUrl,
            Token = newAccessToken
        });
    } 

    private async Task SetRefreshTokenCookie(AppUser user)
    {
        var refreshToken = authService.GenereateRefreshToken();
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
        await userManager.UpdateAsync(user);

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = user.RefreshTokenExpiry,
            SameSite = SameSiteMode.Strict, 
            Secure = true 

        };

        Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
    }
}
