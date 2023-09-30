using Refit;

namespace Bryan.Proof.Auth.Api.Client;

public interface IProofAuthService
{
    private const string CONTROLLER = "";
    public const string URI_INFO_BY_EMAIL = $"{CONTROLLER}api/v1/info-by-email";
    public const string URI_APPLICATION_ROLES = $"{CONTROLLER}api/v1/application-roles";
    public const string URI_TEST = $"{CONTROLLER}api/v1/test";

    [Get($"{URI_INFO_BY_EMAIL}")]
    Task<ApiResponse<InfoRs>> InfoByEmail(InfoByEmailRq rq);

    [Get($"{URI_APPLICATION_ROLES}")]
    Task<ApiResponse<ApplicationRolesRs>> ApplicationRoles();
}