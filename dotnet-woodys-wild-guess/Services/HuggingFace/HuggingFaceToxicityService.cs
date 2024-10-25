using System.Text.Json;
using dotnet.woodyswildguess.Configuration;
using dotnet.woodyswildguess.Constants;
using dotnet.woodyswildguess.Extensions;
using dotnet.woodyswildguess.Models;
using Flurl;
using Microsoft.Extensions.Options;

namespace dotnet.woodyswildguess.Services.HuggingFace;

/// <inheritdoc/>
public class HuggingFaceToxicityService : IHuggingFaceToxicityService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly HuggingFaceOptions _huggingFaceOptions;
    private readonly ILogger<HuggingFaceToxicityService> _logger;
    private const float ToxicityThreshold = 0.8f;

    /// <summary>
    /// Initializes a new instance of the <see cref="HuggingFaceToxicityService"/> class.
    /// </summary>
    /// <param name="httpClientFactory"></param>
    /// <param name="huggingFaceOptions"></param>
    /// <param name="logger"></param>
    public HuggingFaceToxicityService(
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
    public async Task<IEnumerable<IEnumerable<LabelScoreModel>>> PerformToxicBertAnalysisAsync(string text)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(text);
        
        var huggingFaceClient = _httpClientFactory.CreateClient(HttpClientNames.HuggingFaceApiClient);
        _logger.LogDebug("Adding Hugging Face token authorization headers");
        huggingFaceClient.AddHuggingFaceTokenAuthorizationHeaders(_huggingFaceOptions.ApiKey);

        var content = new { inputs = text };
        _logger.LogDebug("Serializing content for toxicity analysis");
        var serializedContent = JsonSerializer.Serialize(content);

        var toxicityModelUrl = _huggingFaceOptions.BaseUrl
            .AppendPathSegments("models", "unitary", "toxic-bert");
        _logger.LogDebug("Performing toxicity analysis on text");
        var toxicityResponse = await huggingFaceClient.PostAsJsonAsync(toxicityModelUrl, serializedContent);

        if (!toxicityResponse.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to analyze text for toxicity. Status code: {StatusCode}", toxicityResponse.StatusCode);
            return null!;
        }

        _logger.LogDebug("Deserializing toxicity scores");
        var toxicityScores = await toxicityResponse.Content.ReadFromJsonAsync<IEnumerable<IEnumerable<LabelScoreModel>>>();
        ArgumentNullException.ThrowIfNull(toxicityScores);

        return toxicityScores;
    }

    /// <inheritdoc/>
    public bool IsToxic(IEnumerable<IEnumerable<LabelScoreModel>> scores)
    {
        ArgumentNullException.ThrowIfNull(scores);
        var scoresList = scores.ToList();

        _logger.LogDebug("Checking for toxic label in scores");
        foreach (var labelScoreList in scoresList)
        {
            foreach (var labelScore in labelScoreList)
            {
                if (labelScore.Label is not "toxic")
                {
                    continue;
                }

                return labelScore.Score >= ToxicityThreshold;
            }
        }

        _logger.LogDebug("No toxic label found in scores");
        return false;
    }

    /// <inheritdoc/>
    public bool IsServerelyToxic(IEnumerable<IEnumerable<LabelScoreModel>> scores)
    {
        ArgumentNullException.ThrowIfNull(scores);
        var scoresList = scores.ToList();

        _logger.LogDebug("Checking for severe toxic label in scores");
        foreach (var labelScoreList in scoresList)
        {
            foreach (var labelScore in labelScoreList)
            {
                if (labelScore.Label is not "severe_toxic")
                {
                    continue;
                }

                return labelScore.Score >= ToxicityThreshold;
            }
        }

        _logger.LogDebug("No toxic label found in scores");
        return false;
    }
}