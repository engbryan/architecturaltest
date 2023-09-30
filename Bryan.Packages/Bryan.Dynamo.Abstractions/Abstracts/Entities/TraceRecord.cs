using Bryan.Dynamo.Abstractions.Enums;
using Bryan.Dynamo.Abstractions.Interfaces;

namespace Bryan.Dynamo.Abstractions.Abstracts.Entities;

public class TraceRecord : IEntity<TraceRecord>
{
    public string? Reference { get; set; } //PK

    public DateTime DateTime { get; set; } //SK

    public string? User { get; set; }

    public ActionEnum Action { get; set; }

    public object? Input { get; set; }

    public string? Description { get; set; }
}