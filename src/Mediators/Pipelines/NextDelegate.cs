namespace SomeTimeLater.Primitives.Pipelines;

public delegate Task NextDelegate<in TInput>(TInput input, CancellationToken token = default);

public delegate Task<TOutput> NextDelegate<in TInput, TOutput>(TInput input, CancellationToken token = default);