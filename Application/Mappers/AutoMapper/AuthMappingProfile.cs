using Application.Extensions;
using Domain.EfClasses;

namespace Application.Mappers.AutoMapper;

public class AuthMappingProfile : BaseMappingProfile
{
    public AuthMappingProfile()
    {
        CreateMap<DeviceInfoDto, DeviceInfo>()
            .ForMember(dest => dest.DeviceModel, opt => opt.MapFrom(src => src.DeviceName ?? "Unknown"))
            .ForMember(dest => dest.OsName, opt => opt.MapFrom(src => src.OperatingSystem ?? "Unknown"))
            .ForMember(dest => dest.OsVersion, opt => opt.MapFrom(src => src.BrowserVersion ?? "Unknown"))
            .ForMember(dest => dest.BrowserName, opt => opt.MapFrom(src => src.Browser ?? "Unknown"))
            .ForMember(dest => dest.IpAddress, opt => opt.Ignore())
            .ForMember(dest => dest.TokenId, opt => opt.Ignore())
            .ForMember(dest => dest.Token, opt => opt.Ignore())
            .ForMember(dest => dest.IsTrusted, opt => opt.MapFrom(_ => false))
            .ForMember(dest => dest.DeviceNickname, opt => opt.MapFrom(src => $"{src.DeviceType} - {src.Browser}"))
            .ForMember(dest => dest.LastActivityAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.LoginCount, opt => opt.MapFrom(_ => 1))
            .ForMember(dest => dest.Location, opt => opt.Ignore())
            .ForMember(dest => dest.CountryCode, opt => opt.Ignore())
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.StatusId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore());
    }
}