using Microsoft.AspNetCore.Mvc.Testing;
using NoteService.Test.TestAuthentication;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace NoteService.Test;

internal static class TestAppFactoryTestAuthExtensions
{
    public static HttpClient CreateClientWithTestAuth(
        this CustomWebApplicationFactory app,
        IEnumerable<Claim> claims,
        WebApplicationFactoryClientOptions? options = null
    )
    {

        var client = options != null ? app.CreateClient(options) : app.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            TestAuthHandler.AuthenticationScheme,
            TestAuthHelper.SerializeClaims(claims)
        );
        return client;
    }

    public static HttpClient CreateClientWithTestAuth(
        this CustomWebApplicationFactory app,
        string userId,
        WebApplicationFactoryClientOptions? options = null
    )
    {
        List<Claim> claims =
        [
            new(ClaimTypes.NameIdentifier, userId),
            new(ClaimTypes.Email, $"{Guid.NewGuid()}@test.com")
        ];

        return app.CreateClientWithTestAuth(claims, options);
    }

    public static HttpClient CreateClientWithTestAuth(
        this CustomWebApplicationFactory app,
        WebApplicationFactoryClientOptions? options = null
    )
    {
        List<Claim> claims =
        [
            new(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            new(ClaimTypes.Email, $"{Guid.NewGuid()}@test.com")
        ];

        return app.CreateClientWithTestAuth(claims, options);
    }
}
