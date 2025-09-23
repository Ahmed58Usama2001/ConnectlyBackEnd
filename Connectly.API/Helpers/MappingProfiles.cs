namespace Connectly.API.Helpers;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<AppUser, MemberDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.PublicId.ToString()));
        CreateMap<Message, MessageDto>()
            .ForMember(dest => dest.SenderId, opt => opt.MapFrom(src => src.Sender.PublicId.ToString()))
            .ForMember(dest => dest.SenderUserName, opt => opt.MapFrom(src => src.Sender.UserName))
            .ForMember(dest => dest.SenderImageUrl, opt => opt.MapFrom(src => src.Sender.ImageUrl))

            .ForMember(dest => dest.RecipientId, opt => opt.MapFrom(src => src.Recipient.PublicId.ToString()))
            .ForMember(dest => dest.RecipientUserName, opt => opt.MapFrom(src => src.Recipient.UserName))
            .ForMember(dest => dest.RecipientImageUrl, opt => opt.MapFrom(src => src.Recipient.ImageUrl));

    }
}
