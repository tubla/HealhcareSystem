using shared.V1.HelperClasses.Contracts;
using System.Net;

namespace patient.services.V1.Exceptions;

public class PatientExceptionStrategy : IExceptionHandlerStrategy
{
    public (HttpStatusCode, string)? TryMap(Exception ex) => ex switch
    {
        RecordNotFoundException => (HttpStatusCode.NotFound, ex.Message),
        PatientAccessPermissionException => (HttpStatusCode.Forbidden, ex.Message),
        InvalidOperationException => (HttpStatusCode.Conflict, ex.Message),
        ArgumentException => (HttpStatusCode.BadRequest, ex.Message),
        _ => null
    };
}
