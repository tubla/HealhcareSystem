namespace authentication.services.V1.CustomExceptions;

public class ConflictException(string message) : Exception(message) { }
