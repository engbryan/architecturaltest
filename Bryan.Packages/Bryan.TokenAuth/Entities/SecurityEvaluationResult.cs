namespace Bryan.TokenAuth.Entities;

public class SecurityEvaluationResult
{
    public List<ApiConfig> ApiList { get; set; } = default!;

    public bool Passed { get; set; }

    public List<string> Roles { get; set; } = default!;
}