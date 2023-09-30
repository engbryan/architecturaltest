using Bryan.Dynamo.Abstractions.Interfaces;

namespace Bryan.Proof.Auth.Domain.Entities;
//TODO FAZER OQ COM ISSO?
public class TraceRecord : IEntity<TraceRecord>
{
    public string? Reference { get; set; } //PK

    public DateTime DateTime { get; set; } //SK

    public string? User { get; set; }

    public ActionEnum Action { get; set; }

    public object? Input { get; set; }

    public string? Description { get; set; }
}