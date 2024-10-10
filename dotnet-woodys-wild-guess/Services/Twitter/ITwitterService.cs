using System.Net;
using dotnet.woodyswildguess.Models;

namespace dotnet.woodyswildguess.Services.Twitter;

public interface ITwitterService
{
    /// <summary>
    /// The bearer token returned by a previously successful authentication operation.
    /// </summary>
    /// remarks>
    /// This operation is thread-safe.
    /// </remarks>
    string AccessToken { get; set; }

    /// <summary>
    /// The authorization response returned by a previously successful authentication operation.
    /// </summary>
    TwitterAuthorizationResponseModel? AuthorizationResponse { get; set; }

    /// <summary>
    /// Authenticates the user with Twitter asynchronously.
    /// </summary>
    /// <param name="code">Code returned from authorization code w/ pkce OAuth 2.0 grant type.</param>
    /// <returns><see cref="HttpStatusCode"/> asynchronously</returns>
    Task<HttpStatusCode> AuthenticateAsync(string code);

    /// <summary>
    /// Sends a tweet asynchronously.
    /// </summary>
    /// <param name="tweet">The tweet to send.</param>
    /// <returns><see cref="HttpStatusCode"/> asynchronously</returns>
    Task<HttpStatusCode> SendTweetAsync(string tweetMessage);

    /// <summary>
    /// Gets the user's state for OAuth2 Authorization Code w/ PKCE grant type.
    /// </summary>
    /// remarks>
    /// This operation is used to prevent CSRF attacks.
    /// </remarks>
    public string? StateHash();
}
