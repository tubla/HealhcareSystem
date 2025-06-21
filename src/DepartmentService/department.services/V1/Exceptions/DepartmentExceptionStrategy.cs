using shared.V1.HelperClasses.Contracts;
using System.Net;

namespace department.services.V1.Exceptions;

public class DepartmentExceptionStrategy : IExceptionHandlerStrategy
{
    public (HttpStatusCode, string)? TryMap(Exception ex) => ex switch
    {
        RecordNotFoundException => (HttpStatusCode.NotFound, ex.Message),
        DepartmentAccessPermissionException => (HttpStatusCode.Forbidden, ex.Message),
        InvalidOperationException => (HttpStatusCode.Conflict, ex.Message),
        ArgumentException => (HttpStatusCode.BadRequest, ex.Message),
        _ => null
    };
}
