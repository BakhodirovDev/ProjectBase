using Domain.Abstraction;

namespace Application.Mappers.AutoMapper;

public class IpLocationMappingProfile : BaseMappingProfile
{
    public IpLocationMappingProfile()
    {
        CreateMap<IpApiResponse, IpLocationInfo>()
            .ForMember(dest => dest.Ip, opt => opt.MapFrom(src => src.Ip ?? string.Empty))
            .ForMember(dest => dest.IsLocal, opt => opt.MapFrom(_ => false));
    }
}