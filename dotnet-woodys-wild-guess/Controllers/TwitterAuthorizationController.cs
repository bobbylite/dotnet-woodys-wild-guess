using System.Net;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text.Json;
using dotnet.woodyswildguess.Configuration;
using dotnet.woodyswildguess.Constants;
using dotnet.woodyswildguess.Extensions;
using dotnet.woodyswildguess.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace dotnet.woodyswildguess.Controllers;

/// <summary>
/// TwitterController
/// </summary>
[Route("authorize/twitter/callback")]
public class TwitterAuthorizationController : Controller
{
    private readonly ILogger<TwitterAuthorizationController> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly TwitterOptions _twitterOptions;

    /// <summary>
    /// Creates an instance of <see cref="TwitterController"/>
    /// </summary>
    /// <param name="httpClientFactory"></param>
    /// <param name="logger"></param>
    public TwitterAuthorizationController(IHttpClientFactory httpClientFactory, 
        ILogger<TwitterAuthorizationController> logger,
        IOptionsSnapshot<TwitterOptions> twitterOptions)
    { 
        ArgumentNullException.ThrowIfNull(httpClientFactory);
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(twitterOptions.Value);

        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _twitterOptions = twitterOptions.Value;
    }

    /// <summary>
    /// GET api/twitter
    /// Handles the GET request for the autorization code w/ pkce grant type
    /// https://developer.x.com/en/docs/authentication/oauth-2-0/user-access-token
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string code, [FromQuery] string state)
    {
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

        _logger.LogDebug("Performing POST request to Twitter API for Access Token");
        var response = await httpClient.PostAsync(_twitterOptions.TokenUrl, content);
        _logger.LogDebug("Reading access token response content");
        var responseContent = await response.Content.ReadAsStringAsync();

        if (string.IsNullOrEmpty(responseContent)) 
        {
            _logger.LogError("Access token response content is null or empty");
            return BadRequest("Access token response content is null or empty");
        }

        _logger.LogDebug("Deserializing access token response content");
        var deserializedResponseContent = JsonSerializer.Deserialize<TwitterAuthorizationResponseModel>(responseContent);

        if (deserializedResponseContent is null)
        {
            _logger.LogError("Failed to deserialize access token response content");
            return BadRequest("Failed to deserialize access token response content");
        }

        if (response.StatusCode is HttpStatusCode.OK)
        {
            _logger.LogDebug("Returning access token response content");
            return Ok($"Access Token: {deserializedResponseContent.AccessToken}");
        }

        if (response.StatusCode is HttpStatusCode.Unauthorized)
        {
            _logger.LogError("Unauthorized access token request");
            return Unauthorized("Unauthorized access token request");
        }
        
        _logger.LogDebug("Failed to retrieve access token");
        return BadRequest("Failed to retrieve access token");
    }
}
