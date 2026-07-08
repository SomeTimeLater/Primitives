using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using SomeTimeLater.Primitives.Pipelines;

namespace SomeTimeLater.Primitives.Requests;

[DebuggerStepThrough]
internal abstract class RequestOutputHandlerExecutor  : RequestHandlerExecutor
{
    public abstract Task<object> ExecuteWithOutputAsync(object request, IServiceProvider serviceProvider,
        CancellationToken token = default);
}

[DebuggerStepThrough]
internal class RequestOutputHandlerExecutor<TRequest, TOutput> : RequestOutputHandlerExecutor 
    where TRequest : Request<TOutput>
{
    public override Task ExecuteAsync(object objectRequest, IServiceProvider serviceProvider, CancellationToken token = default)
    {
        return ExecuteWithOutputAsync(objectRequest, serviceProvider, token);
    }
    
    public override Task<object> ExecuteWithOutputAsync(object request, IServiceProvider serviceProvider, CancellationToken token = default)
    {
        return request is not TRequest typedInput 
            ? throw new InvalidHandlerRequestException("Input must be of type " + typeof(TRequest)) 
            : ExecuteWithOutputAsync(typedInput, serviceProvider, token);
    }
    
    private static async Task<object> ExecuteWithOutputAsync(TRequest request, IServiceProvider serviceProvider, CancellationToken token = default)
    {
        var handler = serviceProvider.GetRequiredService<IRequestHandler<TRequest, TOutput>>();
        var pipeline = PipelineBuilder.Create(serviceProvider, handler);
        var output = await pipeline.ExecuteAsync(request, token);
        return output ?? throw new InvalidHandlerRequestException("Input must be of type " + typeof(TRequest));
    }
}


