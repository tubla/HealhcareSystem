using appointment.models.V1.Db;
using appointment.models.V1.Dtos;
using AutoMapper;

namespace appointment.services.V1.Mapping;

public class AppointmentMappingProfile : Profile
{
    public AppointmentMappingProfile()
    {
        CreateMap<AppointmentDto, Appointment>()
            .ForMember(dest => dest.AppointmentId, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.PatientId, opt => opt.MapFrom(src => src.PatientId))
            .ForMember(dest => dest.DoctorId, opt => opt.MapFrom(src => src.DoctorId))
            .ForMember(
                dest => dest.AppointmentDateTime,
                opt => opt.MapFrom(src => src.AppointmentDateTime)
            );

        CreateMap<Appointment, AppointmentDto>()
            .ForMember(dest => dest.AppointmentId, opt => opt.MapFrom(src => src.AppointmentId))
            .ForMember(dest => dest.PatientId, opt => opt.MapFrom(src => src.PatientId))
            .ForMember(dest => dest.DoctorId, opt => opt.MapFrom(src => src.DoctorId))
            .ForMember(
                dest => dest.AppointmentDateTime,
                opt => opt.MapFrom(src => src.AppointmentDateTime)
            )
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));

        CreateMap<CreateAppointmentDto, Appointment>()
            .ForMember(dest => dest.AppointmentId, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.PatientId, opt => opt.MapFrom(src => src.PatientId))
            .ForMember(dest => dest.DoctorId, opt => opt.MapFrom(src => src.DoctorId))
            .ForMember(
                dest => dest.AppointmentDateTime,
                opt => opt.MapFrom(src => src.AppointmentDateTime)
            );
    }
}
