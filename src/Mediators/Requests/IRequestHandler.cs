namespace SomeTimeLater.Primitives.Requests;

public interface IRequestHandler<in TRequest>
    where TRequest : Request
{
    Task HandleAsync(TRequest request, CancellationToken token = default);
}

public interface IRequestHandler<in TRequest, TOutput>
    where TRequest : Request<TOutput>
{
    Task<TOutput> HandleAsync(TRequest request, CancellationToken token = default);
}