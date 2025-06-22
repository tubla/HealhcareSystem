using prescription.models.V1.Dto;
using shared.V1.ModelBinders;

namespace prescription.api.V1.ModelBinders;

public class UpdatePrescriptionDtoPropertyChecker : IPropertySetChecker<UpdatePrescriptionRequestDto>
{
    public void CheckProperties(UpdatePrescriptionRequestDto dto, string rawBody)
    {
        dto.IsAppointmentIdSet = rawBody.Contains("\"appointment_id\"");
        dto.IsMedicationIdSet = rawBody.Contains("\"medication_id\"");
        dto.IsDosageSet = rawBody.Contains("\"dosage\"");
        dto.IsDurationSet = rawBody.Contains("\"duration\"");
    }
}
