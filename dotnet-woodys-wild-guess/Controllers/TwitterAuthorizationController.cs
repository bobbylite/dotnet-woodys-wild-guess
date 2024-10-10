using System.Net;
using dotnet.woodyswildguess.Services.Twitter;
using Microsoft.AspNetCore.Mvc;

namespace dotnet.woodyswildguess.Controllers;

/// <summary>
/// TwitterAuthorizationController
/// </summary>
[Route("authorize/twitter/callback")]
public class TwitterAuthorizationController : Controller
{
    private readonly ILogger<TwitterAuthorizationController> _logger;
    private readonly ITwitterService _twitterService;

    /// <summary>
    /// Creates an instance of <see cref="TwitterController"/>
    /// </summary>
    /// <param name="httpClientFactory"></param>
    /// <param name="logger"></param>
    public TwitterAuthorizationController(ILogger<TwitterAuthorizationController> logger,
        ITwitterService twitterService)
    { 
        ArgumentNullException.ThrowIfNull(logger);

        _logger = logger;
        _twitterService = twitterService;
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
        ArgumentNullException.ThrowIfNull(code);
        ArgumentNullException.ThrowIfNull(state);

        if (state != _twitterService.StateHash())
        {
            _logger.LogError("State mismatch");
            return Unauthorized("State mismatch");
        }

        var statusCode = await _twitterService.AuthenticateAsync(code);

        if (statusCode is HttpStatusCode.OK)
        {
            _logger.LogDebug("Returning access token response content");
            return Redirect("/Tweet");
        }

        if (statusCode is HttpStatusCode.Unauthorized)
        {
            _logger.LogError("Unauthorized access token request");
            return Unauthorized("Unauthorized access token request");
        }
        
        _logger.LogDebug("Failed to retrieve access token");
        return BadRequest("Failed to retrieve access token");
    }
}