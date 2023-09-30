namespace Bryan.TokenAuth.Entities;

public record User(string Name, string Username, string Language)
{
    public List<string> Roles { get; } = new List<string>();
}