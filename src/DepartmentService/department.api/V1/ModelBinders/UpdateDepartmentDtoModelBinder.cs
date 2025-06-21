using department.models.V1.Dto;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json;

namespace department.api.V1.ModelBinders;

internal class UpdateDepartmentDtoModelBinder : IModelBinder
{
    /// <summary/>
    public async Task BindModelAsync(ModelBindingContext bindingContext)
    {
        ArgumentNullException.ThrowIfNull(bindingContext);

        using var reader = new StreamReader(bindingContext.HttpContext.Request.Body);
        var body = await reader.ReadToEndAsync();

        UpdateDepartmentRequestDto? dto;

        try
        {
            dto = JsonSerializer.Deserialize<UpdateDepartmentRequestDto>(body);
        }
        catch (JsonException)
        {
            bindingContext.Result = ModelBindingResult.Failed();
            return;
        }

        if (dto != null)
        {
            dto.IsNameSet = body.Contains("\"name\"");
            bindingContext.Result = ModelBindingResult.Success(dto);
        }
        else
        {
            bindingContext.Result = ModelBindingResult.Failed();
        }
    }
}
