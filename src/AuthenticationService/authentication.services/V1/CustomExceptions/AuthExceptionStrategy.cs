using shared.V1.HelperClasses.Contracts;
using System.Net;

namespace authentication.services.V1.CustomExceptions;

public class AuthExceptionStrategy : IExceptionHandlerStrategy
{
    public (HttpStatusCode, string)? TryMap(Exception ex) => ex switch
    {
        RecordNotFoundException => (HttpStatusCode.NotFound, ex.Message),
        UnauthorizedAccessException => (HttpStatusCode.Forbidden, ex.Message),
        InvalidOperationException => (HttpStatusCode.Conflict, ex.Message),
        ArgumentException => (HttpStatusCode.BadRequest, ex.Message),
        _ => null
    };
}
