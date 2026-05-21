using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace NoteService.Test.TestAuthentication
{
    public class TestAuthHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder
) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
    {
        public const string AuthenticationScheme = "Test";

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var authorizationHeader = Request.Headers.Authorization.ToString();

            if (string.IsNullOrEmpty(authorizationHeader))
            {
                return Task.FromResult(AuthenticateResult.Fail("Authorization header is empty."));
            }

            if (
                authorizationHeader.StartsWith(
                    $"{AuthenticationScheme} ",
                    StringComparison.OrdinalIgnoreCase
                )
            )
            {
                var token = authorizationHeader[$"{AuthenticationScheme} ".Length..].Trim();

                if (
                    string.IsNullOrEmpty(token)
                    || TestAuthHelper.DeserializeClaims(token) is not Claim[] claims
                )
                {
                    return Task.FromResult(AuthenticateResult.Fail("Authorization header is invalid."));
                }
                else
                {
                    var identity = new ClaimsIdentity(claims, AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);
                    var ticket = new AuthenticationTicket(principal, AuthenticationScheme);

                    var result = AuthenticateResult.Success(ticket);

                    return Task.FromResult(result);
                }
            }
            else
            {
                return Task.FromResult(
                    AuthenticateResult.Fail(
                        $"Authorization header is not {AuthenticationScheme} scheme."
                    )
                );
            }
        }
    }
}
