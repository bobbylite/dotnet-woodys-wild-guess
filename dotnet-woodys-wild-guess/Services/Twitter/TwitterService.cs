using System.Net;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using dotnet.woodyswildguess.Configuration;
using dotnet.woodyswildguess.Constants;
using dotnet.woodyswildguess.Extensions;
using dotnet.woodyswildguess.Models;
using Flurl;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace dotnet.woodyswildguess.Services.Twitter;

/// <summary>
/// Represents the Twitter service.
/// </summary>
public class TwitterService : ITwitterService
{
    private readonly ReaderWriterLockSlim _tokenLock = new();
    private string _token = string.Empty;
    private readonly ILogger<TwitterService> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly TwitterOptions _twitterOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="TwitterService"/> class.
    /// </summary>
    public TwitterService(ILogger<TwitterService> logger,
        IHttpClientFactory httpClientFactory,
        IOptions<TwitterOptions> twitterOptions)
    {
        ArgumentNullException.ThrowIfNull(httpClientFactory);
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(twitterOptions.Value);

        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _twitterOptions = twitterOptions.Value;
    }

    /// <inheritdoc />
    public string AccessToken
    {
        get
        {
            _logger.LogDebug("Entering read lock");
            _tokenLock.EnterReadLock();
            try
            {
                _logger.LogDebug("Reading token");
                return _token;
            }
            finally
            {
                _logger.LogDebug("Exiting read lock");
                _tokenLock.ExitReadLock();
            }
        }
        set
        {
            _logger.LogDebug("Entering write lock");
            _tokenLock.EnterWriteLock();
            try
            {
                _logger.LogDebug("Writing token");
                _token = value;
            }
            finally
            {
                _logger.LogDebug("Exiting write lock");
                _tokenLock.ExitWriteLock();
            }
        }
    }

    /// <inheritdoc/>
    public string? StateHash() => ComputeSha256Hash(_twitterOptions.State);

    /// <inheritdoc/>
    public TwitterAuthorizationResponseModel? AuthorizationResponse { get; set; }

    /// <inheritdoc/>
    public async Task<HttpStatusCode> AuthenticateAsync(string code)
    {
        ArgumentNullException.ThrowIfNull(code);

        _logger.LogDebug("Creating Twitter/X HTTP Client");
        var httpClient = _httpClientFactory.CreateClient();
        _logger.LogDebug("Adding Basic Authorization Headers for Twitter HTTP Client");
        httpClient.AddTwitterAuthorizationHeaders(_twitterOptions.ClientId, _twitterOptions.ClientSecret);

        var formData = new Dictionary<string, string>
        {
            { "code", code },
            { "redirect_uri", _twitterOptions.RedirectUri },
            { "grant_type", TwitterAuthorizationConstants.GrantType },
            { "code_verifier", TwitterAuthorizationConstants.CodeVerifier }
        };

        var content = new FormUrlEncodedContent(formData);
        content.Headers.ContentType = new MediaTypeHeaderValue(MediaTypeNames.Application.FormUrlEncoded);
        
        var tokenUrl = _twitterOptions.ApiBaseUrl
            .AppendPathSegment("2")
            .AppendPathSegments("oauth2", "token");

        _logger.LogDebug("Performing POST request to Twitter API for Access Token");
        var response = await httpClient.PostAsync(tokenUrl, content);
        _logger.LogDebug("Reading access token response content");
        var responseContent = await response.Content.ReadAsStringAsync();

        if (string.IsNullOrEmpty(responseContent)) 
        {
            _logger.LogError("Access token response content is null or empty");
            throw new ArgumentNullException("Access token response content is null or empty");
        }

        _logger.LogDebug("Deserializing access token response content");
        var deserializedResponseContent = JsonSerializer.Deserialize<TwitterAuthorizationResponseModel>(responseContent);

        if (deserializedResponseContent is null)
        {
            _logger.LogError("Failed to deserialize access token response content");
            throw new ArgumentNullException("Access token response content is null or empty");
        }

        if (response.StatusCode is HttpStatusCode.OK)
        {
            _logger.LogDebug("Returning access token response content");
            AuthorizationResponse = deserializedResponseContent;
            AccessToken = deserializedResponseContent.AccessToken ?? 
                throw new ArgumentNullException($"{deserializedResponseContent.AccessToken} is null");
        }

        if (response.StatusCode is HttpStatusCode.Unauthorized)
        {
            _logger.LogError("Unauthorized access token request");
        }
        
        return response.StatusCode;
    }

    /// <inheritdoc/>
    public async Task<HttpStatusCode> SendTweetAsync(string tweetMessage)
    {
        ArgumentNullException.ThrowIfNull(tweetMessage);

        _logger.LogDebug("Creating Twitter/X HTTP Client");
        var httpClient = _httpClientFactory.CreateClient();
        _logger.LogDebug("Adding Bearer Token Authorization Headers for Twitter HTTP Client");
        httpClient.AddTwitterBearerTokenAuthorizationHeaders(AccessToken);

        var tweetData = new CreateTweetModel{
            Text = tweetMessage 
        };

        _logger.LogDebug("Serializing tweet data");
        var serializedTweetData = JsonSerializer.Serialize(tweetData);
        _logger.LogDebug("Creating content for tweet HTTP request");
        var content = new StringContent(serializedTweetData, Encoding.UTF8, MediaTypeNames.Application.Json);

        var tweetUrl = _twitterOptions.ApiBaseUrl
            .AppendPathSegment("2")
            .AppendPathSegment("tweets");

        _logger.LogDebug("Performing POST request to Twitter API for Access Token");
        var response = await httpClient.PostAsync(tweetUrl, content);
        _logger.LogDebug("Reading access token response content");
        var responseContent = await response.Content.ReadAsStringAsync();

        return response.StatusCode;
    }

    private static string ComputeSha256Hash(string rawData)
    {
        using (SHA256 sha256Hash = SHA256.Create())
        {
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

            StringBuilder builder = new StringBuilder();
            foreach (var b in bytes)
            {
                builder.Append(b.ToString("x2"));
            }

            return builder.ToString();
        }
    }
}
