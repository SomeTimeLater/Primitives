using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using SomeTimeLater.Primitives.Exceptions;

namespace SomeTimeLater.Primitives;

public abstract class Entity : IEquatable<Entity>
{
    private readonly int _cachedHashCode;
    
    protected Entity()
    {
        _cachedHashCode = BuildHashCode();
    }

    protected virtual Type GetUnproxiedType() => GetType();

    public virtual bool IsTransient() 
        => GetIdentity().All(v => v is null);

    public override bool Equals(object? obj)
    {
        var compareTo = obj as Entity;
        return Equals(compareTo);
    }

    public bool Equals(Entity? other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }
        if (ReferenceEquals(this, other))
        {
            return true;
        }
        if (IsTransient() || other.IsTransient())
        {
            return true;
        }
        return GetUnproxiedType() == other.GetUnproxiedType() 
               && Equals(GetIdentity(), other.GetIdentity());
    }

    public override int GetHashCode() => _cachedHashCode;
    
    private int BuildHashCode()
    {
        if (IsTransient())
        {
            return RuntimeHelpers.GetHashCode(this);
        }
        var hash = new HashCode();
        hash.Add(GetUnproxiedType());
        foreach (var id in GetIdentity())
        {
            hash.Add(id);
        }
        return hash.ToHashCode();
    }
    
    public static bool operator ==(Entity? left, Entity? right)
        => Equals(left, right);
    
    public static bool operator !=(Entity? left, Entity? right)
        => !Equals(left, right);
    
    protected abstract IEnumerable<object?> GetIdentity();
}

public abstract class Entity<TId> : Entity, IEntity<TId>
    where TId : ValueObject
{
    public TId Id => _id ?? throw new GetIdException("Entity Id has not been set.");
    
    [NotMapped]
    private TId? _id;

    protected Entity(TId id)
    {
        ArgumentNullException.ThrowIfNull(id);
        _id = id;
    }

    protected Entity()
    {
        _id = null;
    }
    
    protected override IEnumerable<object?> GetIdentity() => [_id];

    public void SetIdIfNotNull(TId id)
    {
        ArgumentNullException.ThrowIfNull(id);
        if (_id is not null)
        {
            _id = id;
        }
    }
}