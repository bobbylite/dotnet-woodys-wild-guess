using System.Text.Json.Serialization;

namespace dotnet.woodyswildguess.Configuration;

/// <summary>
/// Represents the configuration options for Twitter integration.
/// </summary>
public class TwitterOptions 
{
    /// <summary>
    /// The section key for Twitter options in the configuration.
    /// </summary>
    public static readonly string SectionKey = "TwitterOptions";

    /// <summary>
    /// Gets or sets the authorization URL for Twitter API.
    /// </summary>
    [JsonPropertyName("token_url")]
    public string ApiBaseUrl { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the base URL for Twitter API.
    /// </summary>
    [JsonPropertyName("base_url")]
    public string BaseUrl { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the response type for Twitter API.
    /// </summary>
    [JsonPropertyName("response_type")]
    public string ResponseType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the client ID for Twitter API.
    /// </summary>
    [JsonPropertyName("client_id")]
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the client secret for Twitter API.
    /// </summary>
    [JsonPropertyName("client_secret")]
    public string ClientSecret { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the redirect URI for Twitter API.
    /// </summary>
    [JsonPropertyName("redirect_uri")]
    public string RedirectUri { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the scope for Twitter API.
    /// </summary>
    [JsonPropertyName("scope")]
    public List<string> Scope { get; set; } = new List<string>();

    /// <summary>
    /// Gets or sets the state for Twitter API.
    /// </summary>
    [JsonPropertyName("state")]
    public string State { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the code challenge for Twitter API.
    /// </summary>
    [JsonPropertyName("code_challenge")]
    public string CodeChallenge { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the code challenge method for Twitter API.
    /// </summary>
    [JsonPropertyName("code_challenge_method")]
    public string CodeChallengeMethod { get; set; } = "SHA-256";
}