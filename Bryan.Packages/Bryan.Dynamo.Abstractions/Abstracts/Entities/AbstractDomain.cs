using Bryan.Dynamo.Abstractions.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace Bryan.Dynamo.Abstractions.Abstracts.Entities;

[ExcludeFromCodeCoverage]
public abstract class AbstractDomain<T> : IEntity<T>
    where T : class
{
    public virtual string? Group { get; }

    public virtual string? Code { get; set; }

    public virtual DateTime? LastUpdate { get; set; }

    public virtual bool Active { get; set; }

    public virtual string? Description { get; set; }

    public virtual string? ENDescription { get; set; }

    public virtual string? ESDescription { get; set; }

    public virtual string? User { get; set; }
}