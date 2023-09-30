namespace Bryan.Proof.Auth.Api.Client.Entities;

public record User(string Name, string Username, string Language, List<string> Roles);