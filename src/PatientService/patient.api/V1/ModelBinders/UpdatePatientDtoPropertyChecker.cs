using patient.models.V1.Dto;
using shared.V1.ModelBinders;

namespace patient.api.V1.ModelBinders;

public class UpdatePatientDtoPropertyChecker : IPropertySetChecker<UpdatePatientRequestDto>
{
    public void CheckProperties(UpdatePatientRequestDto dto, string rawBody)
    {
        dto.IsNameSet = rawBody.Contains("\"name\"");
        dto.IsDobSet = rawBody.Contains("\"dob\"");
        dto.IsGenderSet = rawBody.Contains("\"gender\"");
        dto.IsEmailSet = rawBody.Contains("\"email\"");
        dto.IsPhoneSet = rawBody.Contains("\"phone\"");
        dto.IsAddressSet = rawBody.Contains("\"address\"");
        dto.IsInsuranceProviderIdSet = rawBody.Contains("\"insurance_provider_id\"");
        dto.IsUserIdSet = rawBody.Contains("\"user_id\"");
    }
}
