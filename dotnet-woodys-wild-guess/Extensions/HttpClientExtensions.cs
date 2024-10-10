using System.Net.Http.Headers;
using System.Text;

namespace dotnet.woodyswildguess.Extensions;

public static class HttpClientExtensions
{
    /// <summary>
    /// Adds the basic authorization headers for Twitter
    /// </summary>
    /// <param name="httpClient"></param>
    /// <param name="clientId"></param>
    /// <param name="clientSecret"></param>
    /// <returns></returns>
    public static HttpClient AddTwitterAuthorizationHeaders(this HttpClient httpClient, string clientId, string clientSecret)
    {
        string base64AuthString = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{clientId}:{clientSecret}"));
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64AuthString);

        return httpClient;
    }

    /// <summary>
    /// Adds the bearer token to the request headers
    /// </summary>
    /// <param name="httpClient"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public static HttpClient AddTwitterBearerTokenAuthorizationHeaders(this HttpClient httpClient, string token)
    {
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return httpClient;
    }
}
