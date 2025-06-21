using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Web;

namespace shared.V1.HelperClasses;

public static class UrlHelper
{
    public static string BuildUrl(
        string baseUrl,
        string apiPath,
        IReadOnlyCollection<KeyValuePair<string, string>>? queryParameters,
        params object[] placeholders
    )
    {
        string pattern = "\\{(.+?)\\}";
        int pos = 0;
        string formattedPath = Regex.Replace(Path.Combine(baseUrl, apiPath).Replace("\\", "/"), pattern, (_) => $"{{{pos++}}}");
        string urlText = string.Format(CultureInfo.InvariantCulture, formattedPath, placeholders);
        if (queryParameters == null || queryParameters.Count == 0)
        {
            return urlText;
        }

        return new UriBuilder(urlText)
        {
            Query = BuildQueryParameter(queryParameters)
        }.Uri.ToString();
    }

    private static string BuildQueryParameter(IReadOnlyCollection<KeyValuePair<string, string>> queryParameters)
    {
        return string.Join("&", queryParameters.Select((kvp) =>
        HttpUtility.UrlEncode(kvp.Key) + "=" + HttpUtility.UrlEncode(kvp.Value)));
    }

    public static string GetVersionedApiPath(
        IHttpContextAccessor httpContextAccessor,
        string apiPath
    )
    {
        string pattern = @"v{\d+}/";
        Match match = Regex.Match(apiPath, pattern);
        if (match.Success)
        {
            if (!httpContextAccessor.HttpContext!.GetRouteData().Values.TryGetValue("version", out object? version))
            {
                throw new ArgumentException("Version not found in the API path.", nameof(apiPath));
            }
            return string.Format(apiPath, version);
        }
        return apiPath;
    }
}
