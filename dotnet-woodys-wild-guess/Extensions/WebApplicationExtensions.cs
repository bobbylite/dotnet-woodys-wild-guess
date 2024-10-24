using dotnet.woodyswildguess.Hubs;

namespace dotnet.woodyswildguess.Extensions;

public static class WebApplicationExtensions
{
    /// <summary>
    /// Maps the chat hub.
    /// </summary>
    /// <param name="webApplication">The web application used to configure the HTTP pipeline, and routes.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    /// <remarks>
    /// This method maps the chat hub.
    /// </remarks>
    public static WebApplication MapChatHub(this WebApplication webApplication)
    {
        ArgumentNullException.ThrowIfNull(webApplication);

        webApplication.MapHub<ChatHub>("/chathub");

        return webApplication;
    }
}
