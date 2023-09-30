using System.Text.Json.Serialization;

namespace Bryan.Proof.Auth.Domain.App;

[JsonSerializable(typeof(InfoByEmailCmd))]
public record InfoByEmailCmd(string Email, [property: JsonIgnore] string Password) : IRequest<Result<InfoRs>>;