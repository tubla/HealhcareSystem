namespace authentication.services.V1.CustomExceptions;

public class RecordNotFoundException(string message) : Exception(message) { }
