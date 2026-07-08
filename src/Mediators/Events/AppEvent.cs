namespace SomeTimeLater.Primitives.Events;

public abstract record AppEvent
{
    public virtual Type? GetOutputType() => null;
}

public abstract record AppEvent<TOutput> : AppEvent
{
    public override Type GetOutputType() => typeof(TOutput);
}