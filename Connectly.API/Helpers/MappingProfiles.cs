namespace Connectly.API.Helpers;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<AppUser, MemberDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.PublicId.ToString()));
    }
}