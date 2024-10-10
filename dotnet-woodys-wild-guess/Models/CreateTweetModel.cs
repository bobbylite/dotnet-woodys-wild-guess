using System.Text.Json.Serialization;

namespace dotnet.woodyswildguess.Models;

public class CreateTweetModel
{
    /// <summary>
    /// Gets or sets the tweet message.
    /// </summary>
    [JsonPropertyName("text")]
    public string? Text { get; set; }
}

