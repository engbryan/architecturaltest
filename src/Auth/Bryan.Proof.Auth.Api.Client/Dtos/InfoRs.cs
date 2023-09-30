namespace Bryan.Proof.Auth.Api.Client.Dtos;

public record InfoRs(string Token, string Refresh, DateTime Expiry, List<ApiConfig> ApiList, User User);