using AutoMapper;
using patient.models.V1.Db;
using patient.models.V1.Dto;

namespace patient.services.V1.Mapping;

public class PatientMappingProfile : Profile
{
    public PatientMappingProfile()
    {
        CreateMap<CreatePatientRequestDto, Patient>();
        CreateMap<Patient, PatientResponseDto>();
    }
}
