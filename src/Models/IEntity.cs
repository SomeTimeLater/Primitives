namespace SomeTimeLater.Primitives;

public interface IEntity<out TId>
{
    TId Id { get; }
}