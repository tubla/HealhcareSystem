using AutoMapper;
using prescription.models.V1.Db;
using prescription.models.V1.Dto;

namespace prescription.services.V1.Mappings;

public class PrescriptionMappingProfile : Profile
{
    public PrescriptionMappingProfile()
    {
        CreateMap<CreatePrescriptionDto, Prescription>();
        CreateMap<Prescription, CreatePrescriptionDto>();
    }
}
