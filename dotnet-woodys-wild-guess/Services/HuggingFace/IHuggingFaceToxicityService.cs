using dotnet.woodyswildguess.Models;

namespace dotnet.woodyswildguess.Services.HuggingFace;

/// <summary>
/// Represents a service for interacting with the Hugging Face API.
/// </summary>
public interface IHuggingFaceToxicityService
{
    /// <summary>
    /// Performs toxicity analysis on the provided text.
    /// </summary>
    /// <param name="text">The text to analyze.</param>
    /// <returns>A collection of toxicity scores.</returns>
    Task<IEnumerable<IEnumerable<LabelScoreModel>>> PerformToxicBertAnalysisAsync(string text);

    /// <summary>
    /// Determines if the provided scores are toxic.
    /// </summary>
    /// <param name="scores">The scores to evaluate which are returned from PerformToxicBertAnalysisAsync(string text).</param>
    /// <returns><see langword="true"/> if the scores are toxic; otherwise, <see langword="false"/>.</returns>
    bool IsToxic(IEnumerable<IEnumerable<LabelScoreModel>> scores);

    /// <summary>
    /// Determines if the provided scores are severely toxic.
    /// </summary>
    /// <param name="scores">The scores to evaluate which are returned from PerformToxicBertAnalysisAsync(string text).</param>
    /// <returns><see langword="true"/> if the scores are severely toxic; otherwise, <see langword="false"/>.</returns>
    bool IsServerelyToxic(IEnumerable<IEnumerable<LabelScoreModel>> scores);
}