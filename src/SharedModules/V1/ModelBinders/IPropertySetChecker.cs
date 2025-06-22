namespace shared.V1.ModelBinders;

public interface IPropertySetChecker<T>
{
    void CheckProperties(T dto, string rawBody);
}

