using Microsoft.AspNetCore.Mvc.ModelBinding;
using prescription.models.V1.Dto;
using System.Text.Json;

namespace prescription.api.V1.ModelBinders;

internal class UpdatePrescriptionDtoModelBinder : IModelBinder
{
    /// <summary/>
    public async Task BindModelAsync(ModelBindingContext bindingContext)
    {
        ArgumentNullException.ThrowIfNull(bindingContext);

        using var reader = new StreamReader(bindingContext.HttpContext.Request.Body);
        var body = await reader.ReadToEndAsync();

        UpdatePrescriptionDto? dto;

        try
        {
            dto = JsonSerializer.Deserialize<UpdatePrescriptionDto>(body);
        }
        catch (JsonException)
        {
            bindingContext.Result = ModelBindingResult.Failed();
            return;
        }

        if (dto != null)
        {
            dto.IsAppointmentIdSet = body.Contains("\"appointment_id\"");
            dto.IsMedicationIdSet = body.Contains("\"medication_id\"");
            dto.IsDosageSet = body.Contains("\"dosage\"");
            dto.IsDurationSet = body.Contains("\"duration\"");
            bindingContext.Result = ModelBindingResult.Success(dto);
        }
        else
        {
            bindingContext.Result = ModelBindingResult.Failed();
        }
    }
}
