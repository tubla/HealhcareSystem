using doctor.models.V1.Dto;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json;

namespace doctor.api.V1.ModelBinders;

internal class UpdateDoctorDtoModelBinder : IModelBinder
{
    /// <summary/>
    public async Task BindModelAsync(ModelBindingContext bindingContext)
    {
        ArgumentNullException.ThrowIfNull(bindingContext);

        using var reader = new StreamReader(bindingContext.HttpContext.Request.Body);
        var body = await reader.ReadToEndAsync();

        UpdateDoctorRequestDto? dto;

        try
        {
            dto = JsonSerializer.Deserialize<UpdateDoctorRequestDto>(body);
        }
        catch (JsonException)
        {
            bindingContext.Result = ModelBindingResult.Failed();
            return;
        }

        if (dto != null)
        {
            dto.IsNameSet = body.Contains("\"name\"");
            dto.IsLicenseNumberSet = body.Contains("\"license_number\"");
            dto.IsSpecializationSet = body.Contains("\"specialization\"");
            dto.IsPhoneSet = body.Contains("\"phone\"");
            dto.IsEmailSet = body.Contains("\"email\"");
            dto.IsDeptIdSet = body.Contains("\"dept_id\"");
            dto.IsUserIdSet = body.Contains("\"user_id\"");
            bindingContext.Result = ModelBindingResult.Success(dto);
        }
        else
        {
            bindingContext.Result = ModelBindingResult.Failed();
        }
    }
}
