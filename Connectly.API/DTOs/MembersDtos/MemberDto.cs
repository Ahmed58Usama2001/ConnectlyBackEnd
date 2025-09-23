namespace Connectly.API.DTOs.MembersDtos;

public class MemberDto
{
    public string Id { get; set; }

    public string UserName { get; set; }

    public string? ImageUrl { get; set; }

    public string? Gender { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public DateTime? Created { get; set; }

    public DateTime? LastActive { get; set; }

    public string? Description { get; set; }

    public string? City { get; set; }

    public string? Country { get; set; }
}
