namespace SomeTimeLater.Primitives;

public interface ISingleValue<out TValue>
{
    public TValue Value { get; }
}