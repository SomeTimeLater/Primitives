namespace SomeTimeLater.Primitives.Pipelines;

internal class PipelineExecutor<TInput>
{
    private readonly NextDelegate<TInput> _chain;

    internal PipelineExecutor(NextDelegate<TInput> chain)
    {
        _chain = chain;
    }

    public async Task ExecuteAsync(TInput input, CancellationToken token = default)
    {
        await _chain(input, token);
    }
}

internal class PipelineExecutor<TInput, TOutput>
{
    private readonly NextDelegate<TInput, TOutput> _chain;

    internal PipelineExecutor(NextDelegate<TInput, TOutput> chain)
    {
        _chain = chain;
    }

    public async Task<TOutput> ExecuteAsync(TInput input, CancellationToken token = default)
    {
        return await _chain(input, token);
    }
    
}