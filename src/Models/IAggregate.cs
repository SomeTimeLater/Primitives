namespace SomeTimeLater.Primitives;

public interface IAggregate<out TId> : IEntity<TId>;