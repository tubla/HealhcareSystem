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
            dto.IsFirstNameSet = body.Contains("\"first_name\"");
            dto.IsLastNameSet = body.Contains("\"last_name\"");
            dto.IsDateOfBirthSet = body.Contains("\"date_of_birth\"");
            dto.IsGenderSet = body.Contains("\"gender\"");
            dto.IsContactNumberSet = body.Contains("\"contact_number\"");
            dto.IsEmailSet = body.Contains("\"email\"");
            dto.IsAddressSet = body.Contains("\"address\"");
            bindingContext.Result = ModelBindingResult.Success(dto);
        }
        else
        {
            bindingContext.Result = ModelBindingResult.Failed();
        }
    }
}
