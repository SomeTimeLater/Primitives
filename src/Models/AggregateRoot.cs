using System.ComponentModel.DataAnnotations.Schema;
using SomeTimeLater.Primitives.Exceptions;

namespace SomeTimeLater.Primitives;

public abstract class AggregateRoot : Entity;

public abstract class AggregateRoot<TId> : AggregateRoot, IAggregate<TId>
    where TId : ValueObject
{
    public TId Id => _id ?? throw new GetIdException("Aggregate Id has not been set.");
    
    [NotMapped]
    private TId? _id;

    protected AggregateRoot(TId id)
    {
        ArgumentNullException.ThrowIfNull(id);
        _id = id;
    }

    protected AggregateRoot()
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