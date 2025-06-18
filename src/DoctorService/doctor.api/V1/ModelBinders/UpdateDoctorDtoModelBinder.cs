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

        UpdateDoctorDto? dto;

        try
        {
            dto = JsonSerializer.Deserialize<UpdateDoctorDto>(body);
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
            dto.IsSpecializationSet = body.Contains("\"specialization\"");
            dto.IsContactNumberSet = body.Contains("\"contact_number\"");
            dto.IsEmailSet = body.Contains("\"email\"");
            dto.IsHospitalAffiliationSet = body.Contains("\"hospital_affiliation\"");
            bindingContext.Result = ModelBindingResult.Success(dto);
        }
        else
        {
            bindingContext.Result = ModelBindingResult.Failed();
        }
    }
}
