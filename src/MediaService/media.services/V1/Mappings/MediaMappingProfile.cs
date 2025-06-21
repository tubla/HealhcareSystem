using AutoMapper;
using media.models.V1.Db;
using media.models.V1.Dto;

namespace media.services.V1.Mappings;

public class MediaMappingProfile : Profile
{
    public MediaMappingProfile()
    {
        CreateMap<Media, MediaResponseDto>()
                 .ForMember(dest => dest.FileUrlWithSas, opt => opt.Ignore());
    }
}