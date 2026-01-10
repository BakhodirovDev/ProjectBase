using Application.Mappers.AutoMapper;
using AutoMapper;
using Domain.EfClasses.Authentication;

namespace Application.Mappers;

public class PermissionMappingProfile : BaseMappingProfile
{
    public PermissionMappingProfile()
    {
        // Permission -> PermissionDto
        CreateMap<Permission, PermissionDto>();

        // CreatePermissionDto -> Permission
        CreateMap<CreatePermissionDto, Permission>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.StatusId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.RolePermissions, opt => opt.Ignore());

        // UpdatePermissionDto -> Permission (partial update)
        CreateMap<UpdatePermissionDto, Permission>()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
    }
}
