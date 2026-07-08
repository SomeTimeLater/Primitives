namespace SomeTimeLater.Primitives.Requests;

public abstract record Request
{
    public virtual Type? GetOutputType() => null;
}

public abstract record Request<TOutput> : Request
{
    public override Type GetOutputType() => typeof(TOutput);
}