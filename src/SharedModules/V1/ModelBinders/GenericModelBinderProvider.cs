using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.DependencyInjection;

namespace shared.V1.ModelBinders;

public class GenericModelBinderProvider<T> : IModelBinderProvider where T : class
{
    private readonly IServiceProvider _serviceProvider;

    public GenericModelBinderProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IModelBinder GetBinder(ModelBinderProviderContext context)
    {
        if (context.Metadata.ModelType == typeof(T))
        {
            var checker = (IPropertySetChecker<T>)_serviceProvider.GetRequiredService(typeof(IPropertySetChecker<T>));
            return new BinderTypeModelBinder(typeof(GenericModelBinder<T>));
        }

        return null!;
    }
}
