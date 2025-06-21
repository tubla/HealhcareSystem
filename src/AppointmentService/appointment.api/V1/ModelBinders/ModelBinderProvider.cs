using appointment.models.V1.Dtos;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace appointment.api.V1.ModelBinders;

internal class ModelBinderProvider : IModelBinderProvider
{
    public IModelBinder GetBinder(ModelBinderProviderContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (context.Metadata.ModelType == typeof(UpdateAppointmentRequestDto))
        {
            return new BinderTypeModelBinder(typeof(UpdateAppointmentRequestDtoModelBinder));
        }
        return null!;
    }
}
