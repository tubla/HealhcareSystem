using shared.V1.HelperClasses.Contracts;
using System.Net;

namespace doctor.services.V1.Exceptions;

public class DoctorExceptionStrategy : IExceptionHandlerStrategy
{
    public (HttpStatusCode, string)? TryMap(Exception ex) => ex switch
    {
        RecordNotFoundException => (HttpStatusCode.NotFound, ex.Message),
        DoctorAccessPermissionException => (HttpStatusCode.Forbidden, ex.Message),
        InvalidOperationException => (HttpStatusCode.Conflict, ex.Message),
        ArgumentException => (HttpStatusCode.BadRequest, ex.Message),
        _ => null
    };
}
