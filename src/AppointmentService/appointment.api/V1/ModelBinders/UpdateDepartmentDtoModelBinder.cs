using appointment.models.V1.Dtos;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json;

namespace appointment.api.V1.ModelBinders;

internal class UpdateAppointmentRequestDtoModelBinder : IModelBinder
{
    /// <summary/>
    public async Task BindModelAsync(ModelBindingContext bindingContext)
    {
        ArgumentNullException.ThrowIfNull(bindingContext);

        using var reader = new StreamReader(bindingContext.HttpContext.Request.Body);
        var body = await reader.ReadToEndAsync();

        UpdateAppointmentRequestDto? dto;

        try
        {
            dto = JsonSerializer.Deserialize<UpdateAppointmentRequestDto>(body);
        }
        catch (JsonException)
        {
            bindingContext.Result = ModelBindingResult.Failed();
            return;
        }

        if (dto != null)
        {
            dto.IsPatientIdSet = body.Contains("\"patient_id\"");
            dto.IsDoctorIdSet = body.Contains("\"doctor_id\"");
            dto.IsAppointmentDateTimeSet = body.Contains("\"appointment_datetime\"");
            dto.IsNotesSet = body.Contains("\"notes\"");
            bindingContext.Result = ModelBindingResult.Success(dto);
        }
        else
        {
            bindingContext.Result = ModelBindingResult.Failed();
        }
    }
}
