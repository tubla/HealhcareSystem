using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using patient.models.V1.Dto;

namespace patient.api.V1.ModelBinders;

internal class ModelBinderProvider : IModelBinderProvider
{
    public IModelBinder GetBinder(ModelBinderProviderContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (context.Metadata.ModelType == typeof(UpdatePatientRequestDto))
        {
            return new BinderTypeModelBinder(typeof(UpdatePatientDtoModelBinder));
        }
        return null!;
    }
}
