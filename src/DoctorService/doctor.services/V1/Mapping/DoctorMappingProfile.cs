using AutoMapper;
using doctor.models.V1.Db;
using doctor.models.V1.Dto;

namespace doctor.services.V1.Mapping;

public class DoctorMappingProfile : Profile
{
    public DoctorMappingProfile()
    {
        CreateMap<CreateDoctorDto, Doctor>();
        CreateMap<Doctor, DoctorDto>();
    }
}
