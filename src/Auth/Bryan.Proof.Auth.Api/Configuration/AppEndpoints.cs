using Bryan.Proof.Auth.Api.Client;
using Bryan.Proof.Auth.Domain.App;
using static Bryan.TokenAuth.Config.SecurityConfig.ClaimConfig;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace Bryan.Proof.Auth.Api.Configuration;

public static class AppEndpoints
{
    public static void MapEndPoints(this WebApplication app)
    {
        app.InfoByEmailMap();
        app.ApplicationRolesMap();
        app.TestMap();
    }

    public static void InfoByEmailMap(this WebApplication app) =>
        app.MapPost(IProofAuthService.URI_INFO_BY_EMAIL, AppEndpoints.InfoByEmail)
        .WithName(IProofAuthService.URI_INFO_BY_EMAIL).WithDisplayName("#Description")
        .WithTags("User").WithMetadata(new[] { "http" })
        .Produces(200, typeof(InfoRs));

    public static async Task<IResult> InfoByEmail(IMediator m, InfoByEmailRq rq)
        => await m.SendCommand(new InfoByEmailCmd(rq.Email, rq.Password));

    public static void ApplicationRolesMap(this WebApplication app) =>
        app.MapGet(IProofAuthService.URI_APPLICATION_ROLES, AppEndpoints.ApplicationRoles)
        .WithName(IProofAuthService.URI_APPLICATION_ROLES).WithDisplayName("#Description")
        .WithTags("User").WithMetadata(new[] { "http" })
        .Produces(200, typeof(ApplicationRolesRs))
        .RequireAuthorization();

    public static async Task<IResult> ApplicationRoles(IMediator m)
        => await m.SendCommand(new ApplicationRolesCmd());

    public static void TestMap(this WebApplication app) =>
        app.MapGet(IProofAuthService.URI_TEST, AppEndpoints.Test)
        .WithName(IProofAuthService.URI_TEST).WithDisplayName("Method test for IT_SIGHHUB policies")
        .WithTags("User").WithMetadata(new[] { "http" })
        .Produces(200)
        .RequireAuthorization(new string[] { "IT-Proof" });

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
    public static IResult Test(IMediator m) => Results.Ok();
}