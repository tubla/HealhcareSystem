namespace appointment.services.V1.Exceptions;

public class AppointmentConflictException(string message) : Exception(message) { }
