using AutoMapper;
using Microsoft.AspNetCore.Identity;

namespace Connectly.API.Controllers;


public class AccountController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager,
    IAuthService authService, ITokenBlacklistService tokenBlacklistService, IMapper mapper) : BaseApiController
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
            Token = token
        });
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto model)
    {
        if (ModelState.IsValid)
        {
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user is null)
                return Unauthorized(new ApiResponse(401));

            var result = await signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded)
                return Unauthorized(new ApiResponse(401));

            var token = await authService.CreateAccessTokenAsync(user, userManager);

            return Ok(new UserDto
            {   Id = user.PublicId.ToString(),
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
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

        return Ok(new { message = "Logged out successfully" });
    }

    [Authorize]
    [HttpGet("get-current-user")]
    public async Task<ActionResult<UserDto>> GetCurrentUser()
    {
        var email = User.FindFirstValue(ClaimTypes.Email);
        if (string.IsNullOrEmpty(email))
            return Unauthorized(new ApiResponse(401));

        var user = await userManager.FindByEmailAsync(email);
        if (user == null) return Unauthorized(new ApiResponse(401));

        var token = await authService.CreateAccessTokenAsync(user, userManager);
        var userDto = mapper.Map<UserDto>(user);
        userDto.Token = token;

        return Ok(userDto);
    }

}
