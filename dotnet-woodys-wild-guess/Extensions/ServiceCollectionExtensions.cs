using dotnet.woodyswildguess.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace dotnet.woodyswildguess.Extensions;

/// <summary>
/// Provides extension methods for <see cref="IServiceCollection"/>.
/// </summary>
/// <remarks>
/// This class provides extension methods for <see cref="IServiceCollection"/>.
/// </remarks>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds support for refreshing access tokens using refresh tokens.
    /// </summary>
    /// <param name="services">The collection of service descriptors.</param>
    /// <param name="cookieScheme">The scheme for the cookie authentication handler.</param>
    /// <param name="oidcScheme">The scheme for the OpenID Connect authentication handler.</param>
    public static IServiceCollection AddRefreshTokenSupport(this IServiceCollection services, 
        string cookieScheme = CookieAuthenticationDefaults.AuthenticationScheme, string oidcScheme = "OpenIdConnect")
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentException.ThrowIfNullOrWhiteSpace(cookieScheme);
        ArgumentException.ThrowIfNullOrWhiteSpace(oidcScheme);
        
        services.AddSingleton<AccessTokenRefresher>();
        services.AddOptions<CookieAuthenticationOptions>(cookieScheme).Configure<AccessTokenRefresher>((cookieOptions, refresher) =>
        {
            cookieOptions.Events.OnValidatePrincipal = context => refresher.RefreshAccessTokenAsync(context, oidcScheme);
        });
        
        services.AddOptions<OpenIdConnectOptions>(oidcScheme).Configure(oidcOptions =>
        {
            oidcOptions.Scope.Add(OpenIdConnectScope.OfflineAccess);
            oidcOptions.SaveTokens = true;
        });
        
        return services;
    }
}
