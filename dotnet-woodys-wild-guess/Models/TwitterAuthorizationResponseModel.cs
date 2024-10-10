using System.Text.Json.Serialization;

namespace dotnet.woodyswildguess.Models;

/// <summary>
/// Represents the response model for Twitter authorization.
/// </summary>
public class TwitterAuthorizationResponseModel
{
    private ICollection<string>? _scope;

    /// <summary>
    /// Gets or sets the token type.
    /// </summary>
    [JsonPropertyName("token_type")]
    public string? TokenType { get; set; }

    /// <summary>
    /// Gets or sets the expires in.
    /// </summary>
    [JsonPropertyName("expires_in")]
    public int? ExpiresIn { get; set; }

    /// <summary>
    /// Gets or sets the access token.
    /// </summary>
    [JsonPropertyName("access_token")]
    public string? AccessToken { get; set; }

    /// <summary>
    /// Gets or sets the scope.
    /// </summary>
    [JsonPropertyName("scope")]
    public string? Scope 
    { 
        get
        {
            return string.Join(' ', _scope!);
        }
        set
        {
            string scopeValue = value?.ToString() ?? string.Empty;
            _scope = [.. scopeValue.Split(' ')];
        }
    }

    /// <summary>
    /// Gets or sets the refresh token.
    /// </summary>
    [JsonPropertyName("refresh_token")]
    public string? RefreshToken { get; set; }
}
