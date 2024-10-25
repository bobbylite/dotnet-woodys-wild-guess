using dotnet.woodyswildguess.Models;

namespace dotnet.woodyswildguess.Services.HuggingFace;

/// <summary>
/// Represents a service for interacting with the Hugging Face API.
/// </summary>
public interface IHuggingFaceSentimentService
{
    /// <summary>
    /// Performs sentiment analysis on the provided text.
    /// </summary>
    /// <param name="text">The text to analyze.</param>
    /// <returns>A collection of toxicity scores.</returns>
    Task<IEnumerable<IEnumerable<LabelScoreModel>>> PerformTwitterRobertaSentimentAnalysisAsync(string text);

    /// <summary>
    /// Determines if the provided scores are positive.
    /// </summary>
    /// <param name="scores">The scores to evaluate which are returned from PerformTwitterRobertaSentimentAnalysisAsync(string text).</param>
    /// <returns><see langword="true"/> if the scores are toxic; otherwise, <see langword="false"/>.</returns>
    bool IsPositive(IEnumerable<IEnumerable<LabelScoreModel>> scores);

    /// <summary>
    /// Determines if the provided scores are neutral.
    /// </summary>
    /// <param name="scores">The scores to evaluate which are returned from PerformTwitterRobertaSentimentAnalysisAsync(string text).</param>
    /// <returns><see langword="true"/> if the scores are severely toxic; otherwise, <see langword="false"/>.</returns>
    bool IsNeutral(IEnumerable<IEnumerable<LabelScoreModel>> scores);

    /// <summary>
    /// Determines if the provided scores are negative.
    /// </summary>
    /// <param name="scores">The scores to evaluate which are returned from PerformTwitterRobertaSentimentAnalysisAsync(string text).</param>
    /// <returns><see langword="true"/> if the scores are severely toxic; otherwise, <see langword="false"/>.</returns>
    bool IsNegative(IEnumerable<IEnumerable<LabelScoreModel>> scores);
}

