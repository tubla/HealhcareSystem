using appointment.models.V1.Dtos;
using shared.V1.ModelBinders;

namespace appointment.api.V1.ModelBinders;

public class UpdateAppointmentDtoPropertyChecker : IPropertySetChecker<UpdateAppointmentRequestDto>
{
    public void CheckProperties(UpdateAppointmentRequestDto dto, string rawBody)
    {
        dto.IsPatientIdSet = rawBody.Contains("\"patient_id\"");
        dto.IsDoctorIdSet = rawBody.Contains("\"doctor_id\"");
        dto.IsAppointmentDateTimeSet = rawBody.Contains("\"appointment_datetime\"");
        dto.IsNotesSet = rawBody.Contains("\"notes\"");
    }
}
