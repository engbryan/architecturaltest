using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Bryan.Proof.Auth.Api.Tests.TestBase;

public class MockAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public MockAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock
        ) : base(options, logger, encoder, clock)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        => Task.FromResult(
            AuthenticateResult.Success(
                new(
                    new(
                        new ClaimsIdentity(
                            new Claim[] { new Claim("UserId", "1234") },
                            JwtBearerDefaults.AuthenticationScheme
                        )
                    ),
                    JwtBearerDefaults.AuthenticationScheme
                )
            )
        );
}
