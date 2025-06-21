namespace shared.V1.HelperClasses.Contracts;

public interface IHttpClientService
{
    Task<HttpResponseMessage> SendAsync<TRequest>(
        HttpMethod method,
        string baseUrl,
        string apiPath,
        TRequest content = default!,
        string bearerToken = null!,
        Dictionary<string, string> headers = null!,
        Dictionary<string, string> queryParams = null!,
        CancellationToken cancellationToken = default
    );
}
