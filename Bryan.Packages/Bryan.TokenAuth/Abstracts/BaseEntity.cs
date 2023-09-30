using System.Diagnostics.CodeAnalysis;

namespace Bryan.TokenAuth.Abstracts;

[ExcludeFromCodeCoverage]
public abstract class BaseEntity
{
    public virtual Guid Id { get; protected set; }

    protected BaseEntity()
    {
        if (Id == Guid.Empty)
        {
            Id = Guid.NewGuid();
        }
    }

    public void SetId(Guid id) => Id = id;

    public override bool Equals(object obj)
    {
        var compareTo = obj as BaseEntity;

        if (ReferenceEquals(this, compareTo)) return true;
        if (compareTo is null) return false;

        return Id.Equals(compareTo.Id);
    }

#pragma warning disable S3875 // "operator==" should not be overloaded on reference types
    public static bool operator ==(BaseEntity a, BaseEntity b)
#pragma warning restore S3875 // "operator==" should not be overloaded on reference types
    {
        if (a is null && b is null)
            return true;

        if (a is null || b is null)
            return false;

        return a.Equals(b);
    }

    public static bool operator !=(BaseEntity a, BaseEntity b) => !(a == b);

    public override int GetHashCode() => (GetType().GetHashCode() * 907) + Id.GetHashCode();

    public override string ToString() => $"{GetType().Name} [Id={Id}]";
}