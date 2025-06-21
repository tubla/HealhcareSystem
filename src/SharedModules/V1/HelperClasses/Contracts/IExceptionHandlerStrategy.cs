using System.Net;

namespace shared.V1.HelperClasses.Contracts
{
    public interface IExceptionHandlerStrategy
    {
        (HttpStatusCode StatusCode, string Message)? TryMap(Exception exception);
    }
}
