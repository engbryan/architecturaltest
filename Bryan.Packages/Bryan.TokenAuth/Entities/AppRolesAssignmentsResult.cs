using System.Diagnostics.CodeAnalysis;

namespace Bryan.TokenAuth.Entities;
[ExcludeFromCodeCoverage]
public class AppRoleAssignmentsResult
{
    public List<AppRoleAssignments>? Value { get; set; }
}