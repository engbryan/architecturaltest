using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Bryan.Proof.Auth.Api.Tests.TestBase;

public class MockAuthenticationWithUserHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly MockUser _user;

    public MockAuthenticationWithUserHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        MockUser user
        ) : base(options, logger, encoder, clock) => _user = user;

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        => Task.FromResult(
            AuthenticateResult.Success(
                new(
                    new(
                        new ClaimsIdentity(
                            new Claim[] { new Claim("UserId", _user.UserId) },
                            JwtBearerDefaults.AuthenticationScheme
                        )
                    ),
                    JwtBearerDefaults.AuthenticationScheme
                )
            )
        );
}

public record MockUser(string UserId = "1234");