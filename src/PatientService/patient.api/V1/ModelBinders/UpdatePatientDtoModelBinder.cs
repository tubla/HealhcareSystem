using Microsoft.AspNetCore.Mvc.ModelBinding;
using patient.models.V1.Dto;
using System.Text.Json;

namespace patient.api.V1.ModelBinders;

internal class UpdatePatientDtoModelBinder : IModelBinder
{
    /// <summary/>
    public async Task BindModelAsync(ModelBindingContext bindingContext)
    {
        ArgumentNullException.ThrowIfNull(bindingContext);

        using var reader = new StreamReader(bindingContext.HttpContext.Request.Body);
        var body = await reader.ReadToEndAsync();

        UpdatePatientDto? dto;

        try
        {
            dto = JsonSerializer.Deserialize<UpdatePatientDto>(body);
        }
        catch (JsonException)
        {
            bindingContext.Result = ModelBindingResult.Failed();
            return;
        }

        if (dto != null)
        {
            dto.IsNameSet = body.Contains("\"name\"");
            dto.IsDobSet = body.Contains("\"dob\"");
            dto.IsGenderSet = body.Contains("\"gender\"");
            dto.IsEmailSet = body.Contains("\"email\"");
            dto.IsPhoneSet = body.Contains("\"phone\"");
            dto.IsAddressSet = body.Contains("\"address\"");
            dto.IsInsuranceProviderIdSet = body.Contains("\"insurance_provider_id\"");
            dto.IsUserIdSet = body.Contains("\"user_id\"");
            bindingContext.Result = ModelBindingResult.Success(dto);
        }
        else
        {
            bindingContext.Result = ModelBindingResult.Failed();
        }
    }
}
