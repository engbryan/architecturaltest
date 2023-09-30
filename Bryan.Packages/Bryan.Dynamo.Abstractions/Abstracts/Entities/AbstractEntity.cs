using Bryan.Dynamo.Abstractions.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace Bryan.Dynamo.Abstractions.Abstracts.Entities;

[ExcludeFromCodeCoverage]
public abstract class AbstractEntity<T, TId> : IEntity<T>
    where T : class
{
    public virtual TId? PK { get; set; }

    public virtual string? SK { get; set; }
}