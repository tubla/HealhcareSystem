namespace shared.Models;

public class Response<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? ErrorMessage { get; set; }

    public int StatusCode { get; set; }

    public static Response<T> Ok(T data) => new() { Success = true, Data = data };

    public static Response<T> Fail(string error) => new() { Success = false, ErrorMessage = error };
    public static Response<T> Fail(string error, int statusCode) => new() { Success = false, ErrorMessage = error, StatusCode = statusCode };
}
