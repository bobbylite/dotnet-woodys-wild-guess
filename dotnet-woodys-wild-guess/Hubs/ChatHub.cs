using System.Text.Json;
using dotnet.woodyswildguess.Configuration;
using dotnet.woodyswildguess.Constants;
using dotnet.woodyswildguess.Extensions;
using dotnet.woodyswildguess.Models;
using Flurl;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;

namespace dotnet.woodyswildguess.Hubs;

/// <summary>
/// Represents a SignalR hub for global chat features.
/// </summary>
public class ChatHub : Hub
{
    private readonly IHubContext<ChatHub> _hubContext;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly HuggingFaceOptions _huggingFaceOptions;
    private readonly ILogger<ChatHub> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatHub"/> class.
    /// </summary>
    /// <param name="hubContext">The context for the hub.</param>
    /// <param name="httpClientFactory">The factory for creating <see cref="HttpClient"/> instances.</param>
    /// <param name="huggingFaceOptions">The options for the Hugging Face API.</param>
    /// <param name="logger">The logger.</param>
    /// <remarks>
    /// This constructor initializes a new instance of the <see cref="ChatHub"/> class.
    /// </remarks>
    public ChatHub(
        IHubContext<ChatHub> hubContext,
        IHttpClientFactory httpClientFactory,
        IOptionsSnapshot<HuggingFaceOptions> huggingFaceOptions,
        ILogger<ChatHub> logger)
    {
        ArgumentNullException.ThrowIfNull(hubContext);
        ArgumentNullException.ThrowIfNull(httpClientFactory);
        ArgumentNullException.ThrowIfNull(huggingFaceOptions);
        ArgumentNullException.ThrowIfNull(logger);

        _hubContext = hubContext;
        _httpClientFactory = httpClientFactory;
        _huggingFaceOptions = huggingFaceOptions.Value;
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

        var httpClient = _httpClientFactory.CreateClient(HttpClientNames.HuggingFaceApiClient);
        httpClient.AddHuggingFaceTokenAuthorizationHeaders(_huggingFaceOptions.ApiKey);

        var content = new { inputs = message };
        var serializedContent = JsonSerializer.Serialize(content);

        var sentimentAnalysisModelUrl = _huggingFaceOptions.BaseUrl
            .AppendPathSegments("models", "cardiffnlp", "twitter-roberta-base-sentiment-latest");

        var sentimentResponse = await httpClient.PostAsJsonAsync(sentimentAnalysisModelUrl, serializedContent);
        var sentimentJsonResponse = await sentimentResponse.Content.ReadAsStringAsync();

        var toxicityModelUrl = _huggingFaceOptions.BaseUrl
            .AppendPathSegments("models", "unitary", "toxic-bert");
        var response = await httpClient.PostAsJsonAsync(modelUrl, serializedContent);

        if (response.IsSuccessStatusCode)
        {
            var jsonResponse = await response.Content.ReadAsStringAsync();
            
            var huggingFaceResponse = JsonSerializer.Deserialize<List<List<LabelScore>>>(jsonResponse);

            if (huggingFaceResponse != null && huggingFaceResponse.Count > 0)
            {
                foreach (var labelScoreList in huggingFaceResponse)
                {
                    foreach (var labelScore in labelScoreList)
                    {
                        if (labelScore.Label == "toxic" && labelScore.Score > 0.7f)
                        {
                            _logger.LogDebug("Message from {User} flagged as toxic", user);
                            await _hubContext.Clients.All.SendAsync(
                                "ReceiveMessage",
                                "Hugging Face Moderator",
                                "This message has been flagged as toxic and has been removed."
                            );
                            return;
                        }
                    }
                }
            }
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
