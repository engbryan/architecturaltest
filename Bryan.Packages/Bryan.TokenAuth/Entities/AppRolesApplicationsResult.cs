using System.Diagnostics.CodeAnalysis;

namespace Bryan.TokenAuth.Entities;

[ExcludeFromCodeCoverage]
public class AppRolesApplicationsResult
{
    public List<AppRolesApplications>? Value { get; set; }

    public class AppRolesApplications
    {
        public string? DisplayName { get; set; }

        public string? AppId { get; set; }

        public List<AppRole>? AppRoles { get; set; }
    }
}