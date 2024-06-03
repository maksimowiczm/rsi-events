using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Events.Presentation.Authentication;

public class BasicAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    ISystemClock clock
) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder, clock)
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        Claim[]? claims = Request.Headers["Authorization"].FirstOrDefault() switch
        {
            null => [new Claim(ClaimTypes.Role, Roles.Guest)],
            "Bearer Basic" => [new Claim(ClaimTypes.Role, Roles.Admin)],
            _ => null
        };

        if (claims is null)
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid authorization header"));
        }

        var identity = new ClaimsIdentity(claims, "Basic");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, Scheme.Name)));
    }
}