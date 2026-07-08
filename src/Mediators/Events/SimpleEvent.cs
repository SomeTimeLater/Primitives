namespace SomeTimeLater.Primitives.Events;

public record SimpleEvent<TValue> : AppEvent
{
    public TValue Value { get; }

    public SimpleEvent(TValue value)
    {
        Value = value;
    }
}