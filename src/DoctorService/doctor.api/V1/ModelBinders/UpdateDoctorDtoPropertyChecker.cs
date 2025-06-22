using doctor.models.V1.Dto;
using shared.V1.ModelBinders;

namespace doctor.api.V1.ModelBinders;

public class UpdateDoctorDtoPropertyChecker : IPropertySetChecker<UpdateDoctorRequestDto>
{
    public void CheckProperties(UpdateDoctorRequestDto dto, string rawBody)
    {
        dto.IsNameSet = rawBody.Contains("\"name\"");
        dto.IsLicenseNumberSet = rawBody.Contains("\"license_number\"");
        dto.IsSpecializationSet = rawBody.Contains("\"specialization\"");
        dto.IsPhoneSet = rawBody.Contains("\"phone\"");
        dto.IsEmailSet = rawBody.Contains("\"email\"");
        dto.IsDeptIdSet = rawBody.Contains("\"dept_id\"");
        dto.IsUserIdSet = rawBody.Contains("\"user_id\"");
    }
}
