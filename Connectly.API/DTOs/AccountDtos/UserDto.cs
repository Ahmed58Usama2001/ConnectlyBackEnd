namespace Connectly.API.DTOs.AccountDtos;

public class UserDto
{
    public string Id { get; set; }

    public string Email { get; set; }

    public string UserName { get; set; }

    public string Token { get; set; }

    public string? PictureUrl { get; set; }

    public string? Gender { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public DateTime Created { get; set; }

    public DateTime LastActive { get; set; }

    public string? Description { get; set; }

    public string? City { get; set; }

    public string? Country { get; set; }
}
