using System.Text.Json;
using dotnet.woodyswildguess.Extensions;
using dotnet.woodyswildguess.Models;
using Microsoft.AspNetCore.SignalR;

namespace dotnet.woodyswildguess.Hubs;

/// <summary>
/// Represents a SignalR hub for global chat features.
/// </summary>
public class ChatHub : Hub
{
    private readonly IHubContext<ChatHub> _hubContext;
    private readonly IHttpClientFactory _httpClientFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatHub"/> class.
    /// </summary>
    /// <param name="hubContext">The context for the hub.</param>
    /// <remarks>
    /// This constructor initializes a new instance of the <see cref="ChatHub"/> class.
    /// </remarks>
    public ChatHub(
        IHubContext<ChatHub> hubContext,
        IHttpClientFactory httpClientFactory)
    {
        ArgumentNullException.ThrowIfNull(hubContext);
        ArgumentNullException.ThrowIfNull(httpClientFactory);
        _hubContext = hubContext;
        _httpClientFactory = httpClientFactory;
    }

    /// <summary>
    /// Sends a message to all clients.
    /// </summary>
    /// <param name="user">The user sending the message.</param>
    /// <param name="message">The message to send.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task SendMessage(string user, string message)
    {
        var username = Context?.User?.Claims.FirstOrDefault(c => c.Type == "name")?.Value;

        // TODO: Send the message to all clients after inspecting with huggingface 
        var httpClient = _httpClientFactory.CreateClient("HuggingFace");

        var apiKey = "";
        var modelUrl = "https://api-inference.huggingface.co/models/unitary/toxic-bert";
        httpClient.AddHuggingFaceTokenAuthorizationHeaders(apiKey);

        var content = new { inputs = message };
        var serializedContent = JsonSerializer.Serialize(content);
        var response = await httpClient.PostAsJsonAsync(modelUrl, serializedContent);

        if (response.IsSuccessStatusCode)
        {
            var jsonResponse = await response.Content.ReadAsStringAsync();
            
            // Deserialize into List<List<LabelScore>> directly
            var huggingFaceResponse = JsonSerializer.Deserialize<List<List<LabelScore>>>(jsonResponse);

            // Check if message is toxic
            if (huggingFaceResponse != null && huggingFaceResponse.Count > 0)
            {
                foreach (var labelScoreList in huggingFaceResponse)
                {
                    foreach (var labelScore in labelScoreList)
                    {
                        if (labelScore.Label == "toxic" && labelScore.Score > 0.7f)
                        {
                            Console.WriteLine("Message flagged as toxic");
                            await _hubContext.Clients.All.SendAsync("ReceiveMessage", "Hugging Face Moderator", "This message has been flagged as toxic and has been removed.");
                            return;
                        }
                    }
                }

                // If no toxic message was found
                Console.WriteLine("Message is appropriate");
            }
        }

        // Send the message to all clients
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
