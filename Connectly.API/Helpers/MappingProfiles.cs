namespace Connectly.API.Helpers;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<AppUser, MemberDto>()
            .ForMember(dest => dest.PictureUrl, opt => opt.MapFrom(src => src.ImageUrl))
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.PublicId.ToString()));
    }
}