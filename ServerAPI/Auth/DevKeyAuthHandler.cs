using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

public class DevKeyAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public DevKeyAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock)
        : base(options, logger, encoder, clock) { }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // Header vi kræver
        if (!Request.Headers.TryGetValue("X-Whist-Key", out var key))
            return Task.FromResult(AuthenticateResult.Fail("Missing X-Whist-Key"));

        // Sæt din hemmelige nøgle her (evt. flyt til appsettings senere)
        const string expected = "whist-dev-key";

        if (!string.Equals(key.ToString(), expected, StringComparison.Ordinal))
            return Task.FromResult(AuthenticateResult.Fail("Invalid X-Whist-Key"));

        // Markér som authenticated
        var claims = new[] { new Claim(ClaimTypes.Name, "DevUser") };
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}