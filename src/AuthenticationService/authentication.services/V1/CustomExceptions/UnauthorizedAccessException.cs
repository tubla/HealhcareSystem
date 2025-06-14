namespace authentication.services.V1.CustomExceptions
{
    public class UnauthorizedAccessException(string message) : Exception(message) { }
}
