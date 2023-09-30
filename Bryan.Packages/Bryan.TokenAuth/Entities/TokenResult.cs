namespace Bryan.TokenAuth.Entities;

public class TokenResult
{
    public string Token_type { get; set; } = default!;

    public int Expires_in { get; set; } = default!;

    public string Access_token { get; set; } = default!;

    public string Refresh_token { get; set; } = default!;
}