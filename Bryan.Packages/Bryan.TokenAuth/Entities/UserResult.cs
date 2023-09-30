using System.Diagnostics.CodeAnalysis;

namespace Bryan.TokenAuth.Entities;

[ExcludeFromCodeCoverage]
public class UserResult
{
    public string? OnPremisesSamAccountName { get; set; }

    public string? UsageLocation { get; set; }

    public string? GivenName { get; set; }

    public string? Surname { get; set; }
}