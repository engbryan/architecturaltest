using System.Diagnostics.CodeAnalysis;

namespace Bryan.TokenAuth.Entities;

[ExcludeFromCodeCoverage]
public class AppRoleAssignments
{
    public string? ResourceDisplayName { get; set; }

    public string? AppRoleId { get; set; }
}