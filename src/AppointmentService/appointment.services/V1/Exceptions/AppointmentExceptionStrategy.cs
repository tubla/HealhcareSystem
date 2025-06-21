using shared.V1.HelperClasses.Contracts;
using System.Net;

namespace appointment.services.V1.Exceptions;

public class AppointmentExceptionStrategy : IExceptionHandlerStrategy
{
    public (HttpStatusCode, string)? TryMap(Exception ex) => ex switch
    {
        RecordNotFoundException => (HttpStatusCode.NotFound, ex.Message),
        AppointmentConflictException => (HttpStatusCode.Conflict, ex.Message),
        AppointmentAccessPermissionException => (HttpStatusCode.Forbidden, ex.Message),
        InvalidOperationException => (HttpStatusCode.Conflict, ex.Message),
        ArgumentException => (HttpStatusCode.BadRequest, ex.Message),
        _ => null
    };
}
