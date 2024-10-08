using System;
using dotnet.woodyswildguess.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;

namespace dotnet.woodyswildguess.Extensions;

public static class WebApplicationBuilderExtensions
{
    /// <summary>
    /// Add support for refreshing access tokens using refresh tokens.
    /// With .NET 8, the configuration for this can be controled completely from 'appsettings.json'.
    /// </summary>
    /// <param name="webApplicationBuilder">A builder for web applications and services.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    /// <remarks>
    /// This method adds support for refreshing access tokens using refresh tokens.
    /// </remarks>
    public static WebApplicationBuilder AddAccessControl(this WebApplicationBuilder webApplicationBuilder)
    {
        ArgumentNullException.ThrowIfNull(webApplicationBuilder);

        // Authentication with OpenID Connect: https://github.com/dotnet/blazor-samples/tree/main/8.0/BlazorWebAppOidcBff
        webApplicationBuilder.Services.AddAuthentication("OpenIdConnect")
            .AddOpenIdConnect()
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);

        webApplicationBuilder.Services.AddAuthorization();
        webApplicationBuilder.Services.AddRefreshTokenSupport();
        webApplicationBuilder.Services.AddCascadingAuthenticationState();
        
        // Add support for flowing the server authentication state to the WebAssembly client.
        // Sync authentication state between server and client: https://auth0.com/blog/auth0-authentication-blazor-web-apps/
        webApplicationBuilder.Services.AddScoped<AuthenticationStateProvider, PersistingAuthenticationStateProvider>();
        
        return webApplicationBuilder;
    }
}
