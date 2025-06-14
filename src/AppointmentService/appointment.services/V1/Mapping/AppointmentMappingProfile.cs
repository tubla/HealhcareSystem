using appointment.models.V1.Db;
using appointment.models.V1.Dtos;
using AutoMapper;

namespace appointment.services.V1.Mapping;

public class AppointmentMappingProfile : Profile
{
    public AppointmentMappingProfile()
    {
        CreateMap<AppointmentDto, Appointment>()
            .ForMember(dest => dest.AppointmentID, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.PatientID, opt => opt.MapFrom(src => src.PatientID))
            .ForMember(dest => dest.DoctorID, opt => opt.MapFrom(src => src.DoctorID))
            .ForMember(
                dest => dest.AppointmentDateTime,
                opt => opt.MapFrom(src => src.AppointmentDateTime)
            );

        CreateMap<Appointment, AppointmentDto>()
            .ForMember(dest => dest.AppointmentID, opt => opt.MapFrom(src => src.AppointmentID))
            .ForMember(dest => dest.PatientID, opt => opt.MapFrom(src => src.PatientID))
            .ForMember(dest => dest.DoctorID, opt => opt.MapFrom(src => src.DoctorID))
            .ForMember(
                dest => dest.AppointmentDateTime,
                opt => opt.MapFrom(src => src.AppointmentDateTime)
            )
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));
    }
}
