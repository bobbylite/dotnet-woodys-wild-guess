using System.Text.Json;
using dotnet.woodyswildguess.Configuration;
using dotnet.woodyswildguess.Constants;
using dotnet.woodyswildguess.Extensions;
using dotnet.woodyswildguess.Models;
using Flurl;
using Microsoft.Extensions.Options;

namespace dotnet.woodyswildguess.Services.HuggingFace;

/// <inheritdoc/>
public class HuggingFaceSentimentService : IHuggingFaceSentimentService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly HuggingFaceOptions _huggingFaceOptions;
    private readonly ILogger<HuggingFaceToxicityService> _logger;
    private const float PositivityThreshold = 0.8f;
    private const float NegativityThreshold = 0.8f;

    /// <summary>
    /// Initializes a new instance of the <see cref="HuggingFaceSentimentService"/> class.
    /// </summary>
    /// <param name="httpClientFactory"></param>
    /// <param name="huggingFaceOptions"></param>
    /// <param name="logger"></param>
    public HuggingFaceSentimentService(
        IHttpClientFactory httpClientFactory,
        IOptionsSnapshot<HuggingFaceOptions> huggingFaceOptions,
        ILogger<HuggingFaceToxicityService> logger)
    {
        ArgumentNullException.ThrowIfNull(httpClientFactory);
        ArgumentNullException.ThrowIfNull(huggingFaceOptions?.Value);
        ArgumentNullException.ThrowIfNull(logger);

        _httpClientFactory = httpClientFactory;
        _huggingFaceOptions = huggingFaceOptions.Value;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<IEnumerable<LabelScoreModel>>> PerformTwitterRobertaSentimentAnalysisAsync(string text)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(text);
        
        var huggingFaceClient = _httpClientFactory.CreateClient(HttpClientNames.HuggingFaceApiClient);
        _logger.LogDebug("Adding Hugging Face token authorization headers");
        huggingFaceClient.AddHuggingFaceTokenAuthorizationHeaders(_huggingFaceOptions.ApiKey);

        var content = new { inputs = text };
        _logger.LogDebug("Serializing content for sentiment analysis");
        var serializedContent = JsonSerializer.Serialize(content);

        var sentimentModelUrl = _huggingFaceOptions.BaseUrl
            .AppendPathSegments("models", "cardiffnlp", "twitter-roberta-base-sentiment-latest");
        _logger.LogDebug("Performing sentiment analysis on text");
        var sentimentResponse = await huggingFaceClient.PostAsJsonAsync(sentimentModelUrl, serializedContent);

        if (!sentimentResponse.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to analyze text for sentiment. Status code: {StatusCode}", sentimentResponse.StatusCode);
            return null!;
        }

        _logger.LogDebug("Deserializing sentiment scores");
        var sentimentScores = await sentimentResponse.Content.ReadFromJsonAsync<IEnumerable<IEnumerable<LabelScoreModel>>>();
        ArgumentNullException.ThrowIfNull(sentimentScores);

        return sentimentScores;
    }

    /// <inheritdoc/>
    public bool IsNegative(IEnumerable<IEnumerable<LabelScoreModel>> scores)
    {
        ArgumentNullException.ThrowIfNull(scores);

        _logger.LogDebug("Evaluating sentiment scores for negativity");
        foreach (var scoreResponse in scores)
        {
            foreach (var score in scoreResponse)
            {
                if (score.Label is not "negative")
                {
                    continue;
                }

                return score.Score >= NegativityThreshold;
            }
        }

        _logger.LogDebug("No negative label found in scores");
        return false;
    }

    /// <inheritdoc/>
    public bool IsPositive(IEnumerable<IEnumerable<LabelScoreModel>> scores)
    {
        ArgumentNullException.ThrowIfNull(scores);

        _logger.LogDebug("Evaluating sentiment scores for positivity");
        foreach (var scoreResponse in scores)
        {
            foreach (var score in scoreResponse)
            {
                if (score.Label is not "positive")
                {
                    continue;
                }

                return score.Score >= PositivityThreshold;
            }
        }

        _logger.LogDebug("No positive label found in scores");
        return false;
    }

    /// <inheritdoc/>
    public bool IsNeutral(IEnumerable<IEnumerable<LabelScoreModel>> scores)
    {
        return !IsNegative(scores) && !IsPositive(scores);
    }
}