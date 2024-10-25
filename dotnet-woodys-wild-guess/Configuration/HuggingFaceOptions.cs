namespace dotnet.woodyswildguess.Configuration
{
    /// <summary>
    /// Represents the configuration options for HuggingFace integration.
    /// </summary>
    public class HuggingFaceOptions
    {
        /// <summary>
        /// The section key for HuggingFace options in the configuration.
        /// </summary>
        public static readonly string SectionKey = "HuggingFaceOptions";

        /// <summary>
        /// Gets or sets the base URL for HuggingFace API.
        /// </summary>
        public string BaseUrl { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the API key for HuggingFace API.
        /// </summary>
        public string ApiKey { get; set; } = string.Empty;
    }
}