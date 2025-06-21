using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using prescription.models.V1.Dto;

namespace prescription.api.V1.ModelBinders;

internal class ModelBinderProvider : IModelBinderProvider
{
    public IModelBinder GetBinder(ModelBinderProviderContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (context.Metadata.ModelType == typeof(UpdatePrescriptionRequestDto))
        {
            return new BinderTypeModelBinder(typeof(UpdatePrescriptionDtoModelBinder));
        }
        return null!;
    }
}
