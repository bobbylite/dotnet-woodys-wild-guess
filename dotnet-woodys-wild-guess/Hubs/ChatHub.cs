using dotnet.woodyswildguess.Services.HuggingFace;
using Microsoft.AspNetCore.SignalR;

namespace dotnet.woodyswildguess.Hubs;

/// <summary>
/// Represents a SignalR hub for global chat features.
/// </summary>
public class ChatHub : Hub
{
    private readonly IHubContext<ChatHub> _hubContext;
    private readonly IHuggingFaceToxicityService _huggingFaceToxicityService;
    private readonly IHuggingFaceSentimentService _huggingFaceSentimentService;
    private readonly ILogger<ChatHub> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatHub"/> class.
    /// </summary>
    /// <param name="hubContext">The context for the hub.</param>
    /// <param name="huggingFaceToxicityService">The service for interacting with the Hugging Face API for toxicity analysis.</param>
    /// <param name="huggingFaceSentimentService">The service for interacting with the Hugging Face API for sentiment analysis.</param>
    /// <param name="logger">The logger.</param>
    /// <remarks>
    /// This constructor initializes a new instance of the <see cref="ChatHub"/> class.
    /// </remarks>
    public ChatHub(
        IHubContext<ChatHub> hubContext,
        IHuggingFaceToxicityService huggingFaceToxicityService,
        IHuggingFaceSentimentService huggingFaceSentimentService,
        ILogger<ChatHub> logger)
    {
        ArgumentNullException.ThrowIfNull(hubContext);
        ArgumentNullException.ThrowIfNull(huggingFaceToxicityService);
        ArgumentNullException.ThrowIfNull(huggingFaceSentimentService);
        ArgumentNullException.ThrowIfNull(logger);

        _hubContext = hubContext;
        _huggingFaceToxicityService = huggingFaceToxicityService;
        _huggingFaceSentimentService = huggingFaceSentimentService;
        _logger = logger;
    }

    /// <summary>
    /// Sends a message to all clients.
    /// </summary>
    /// <param name="user">The user sending the message.</param>
    /// <param name="message">The message to send.</param>
    /// <remarks> 
    /// Determines toxicity of the message using Hugging Face API and sends the message to all clients. 
    /// </remarks>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task SendMessage(string user, string message)
    {
        _logger.LogDebug("Received message from {User}", user);

        var toxicityScores = await _huggingFaceToxicityService.PerformToxicBertAnalysisAsync(message);
        var isToxic = _huggingFaceToxicityService.IsToxic(toxicityScores);

        if (isToxic)
        {
            _logger.LogDebug("Message from {User} flagged as toxic", user);
            await _hubContext.Clients.All.SendAsync(
                "ReceiveMessage",
                "Toxicity Moderator",
                "This message has been flagged as toxic and has been removed."
            );

            return;
        }

        var sentimentScores = await _huggingFaceSentimentService.PerformTwitterRobertaSentimentAnalysisAsync(message);
        var isPositive = _huggingFaceSentimentService.IsPositive(sentimentScores);
        var isNegative = _huggingFaceSentimentService.IsNegative(sentimentScores);

        if (isPositive && !isNegative)
        {
            _logger.LogDebug("Message from {User} flagged as positive", user);
            await _hubContext.Clients.All.SendAsync(
                "ReceiveMessage",
                "Sentiment Moderator",
                "This message has been flagged as positive."
            );

            return;
        }

        if (isNegative && !isPositive)
        {
            _logger.LogDebug("Message from {User} flagged as negative", user);
            await _hubContext.Clients.All.SendAsync(
                "ReceiveMessage",
                "Sentiment Moderator",
                "This message has been flagged as negative."
            );

            return;
        }

        _logger.LogDebug("Message is appropriate");
        var username = Context?.User?.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
        await _hubContext.Clients.All.SendAsync("ReceiveMessage", username, message);
    }

    /// <summary>
    /// Invoked when a new connection is established with the hub.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <remarks>
    /// This method is called when a new connection is established with the hub.
    /// </remarks>
    public override async Task OnConnectedAsync()
    {
        var username = Context?.User?.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
        await _hubContext.Clients.All.SendAsync("ReceiveMessage", string.Empty, $"{username} joined the Woody's Wild Guess chat room.");
        await base.OnConnectedAsync();
    }

    /// <summary>
    /// Invoked when a connection with the hub is terminated.
    /// </summary>
    /// <param name="exception">The exception that occurred.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <remarks>
    /// This method is called when a connection with the hub is terminated.
    /// </remarks>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var username = Context?.User?.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
        await _hubContext.Clients.All.SendAsync("ReceiveMessage", string.Empty, $"{username} has left the chat room.");
        await base.OnDisconnectedAsync(exception);
    }
}