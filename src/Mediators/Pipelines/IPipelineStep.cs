namespace SomeTimeLater.Primitives.Pipelines;

public interface IPipelineStep<TInput>
{
    Task ExecuteAsync(TInput input, NextDelegate<TInput> next, CancellationToken token = default);
}

public interface IPipelineStep<TInput, TOutput>
{
    Task<TOutput> ExecuteAsync(TInput input, NextDelegate<TInput, TOutput> next, CancellationToken token = default);
}