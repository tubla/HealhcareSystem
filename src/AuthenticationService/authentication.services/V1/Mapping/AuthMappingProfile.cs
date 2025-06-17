using authentication.models.V1.Db;
using authentication.models.V1.Dtos;
using AutoMapper;

namespace authentication.services.V1.Mapping;

public class AuthMappingProfile : Profile
{
    public AuthMappingProfile()
    {
        CreateMap<UserDto, User>()
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Username))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.UserRoles, opt => opt.MapFrom(src => src.RoleIds.Select(roleId => new UserRole { RoleId = roleId })));

        CreateMap<User, UserDto>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.UserName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.RoleIds, opt => opt.MapFrom(src => src.UserRoles.Select(ur => ur.RoleId).ToList()));

        CreateMap<RegisterRequestDto, User>()
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Username))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.UserRoles, opt => opt.MapFrom(src => src.RoleIds.Select(roleId => new UserRole { RoleId = roleId })));
    }
}
