using department.models.V1.Dto;
using shared.V1.ModelBinders;

namespace department.api.V1.ModelBinders;

public class UpdateDepartmentDtoPropertyChecker : IPropertySetChecker<UpdateDepartmentRequestDto>
{
    public void CheckProperties(UpdateDepartmentRequestDto dto, string rawBody)
    {
        dto.IsNameSet = rawBody.Contains("\"name\"");
    }
}
