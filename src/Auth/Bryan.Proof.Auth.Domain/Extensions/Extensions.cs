using SecurityApiConfig = Bryan.TokenAuth.Entities.ApiConfig;
using SecurityAppRole = Bryan.TokenAuth.Entities.AppRole;
using SecurityUser = Bryan.TokenAuth.Entities.User;

namespace Bryan.Proof.Auth.Domain.Extensions;

public static class Extensions
{
    public static List<ApiConfig> ToApiClient(this List<SecurityApiConfig> list)
        => list.Select(p => new ApiConfig(p.Key, (ApiEnum)p.Api)).ToList();

    public static List<AppRole> ToApiClient(this List<SecurityAppRole> list)
        => list.Select(p => new AppRole(
            p.Description,
            p.DisplayName,
            p.Id,
            p.IsEnabled,
            p.Value
        )).ToList();

    public static User ToApiClient(this SecurityUser user)
        => new(user.Name, user.Username, user.Language, user.Roles);
}