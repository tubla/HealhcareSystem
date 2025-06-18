using doctor.models.V1.Dto;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace doctor.api.V1.ModelBinders;

internal class ModelBinderProvider : IModelBinderProvider
{
    public IModelBinder GetBinder(ModelBinderProviderContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (context.Metadata.ModelType == typeof(UpdateDoctorDto))
        {
            return new BinderTypeModelBinder(typeof(UpdateDoctorDtoModelBinder));
        }
        return null!;
    }
}
