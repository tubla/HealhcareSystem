using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json;

namespace shared.V1.ModelBinders;

public class GenericModelBinder<T> : IModelBinder where T : class
{
    private readonly IPropertySetChecker<T> _propertyChecker;

    public GenericModelBinder(IPropertySetChecker<T> propertyChecker)
    {
        _propertyChecker = propertyChecker;
    }

    public async Task BindModelAsync(ModelBindingContext bindingContext)
    {
        ArgumentNullException.ThrowIfNull(bindingContext);

        using var reader = new StreamReader(bindingContext.HttpContext.Request.Body);
        var body = await reader.ReadToEndAsync();

        T? dto;
        try
        {
            dto = JsonSerializer.Deserialize<T>(body);
        }
        catch (JsonException)
        {
            bindingContext.Result = ModelBindingResult.Failed();
            return;
        }

        if (dto != null)
        {
            _propertyChecker.CheckProperties(dto, body);
            bindingContext.Result = ModelBindingResult.Success(dto);
        }
        else
        {
            bindingContext.Result = ModelBindingResult.Failed();
        }
    }
}

