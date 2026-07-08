using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using SomeTimeLater.Primitives.Pipelines;

namespace SomeTimeLater.Primitives.Requests;

[DebuggerStepThrough]
internal abstract class RequestHandlerExecutor
{
    public abstract Task ExecuteAsync(object objectRequest, IServiceProvider serviceProvider,
        CancellationToken token = default);
}

[DebuggerStepThrough]
internal class RequestHandlerExecutor<TRequest> : RequestHandlerExecutor
    where TRequest : Request
{
    public override Task ExecuteAsync(object objectRequest, IServiceProvider serviceProvider, CancellationToken token = default)
    {
        return objectRequest is not TRequest typedInput 
            ? throw new InvalidHandlerRequestException("Input must be of type " + typeof(TRequest)) 
            : ExecuteAsync(typedInput, serviceProvider, token);
    }
    
    private static Task ExecuteAsync(TRequest objectRequest, IServiceProvider serviceProvider, CancellationToken token = default)
    {
        var handler = serviceProvider.GetRequiredService<IRequestHandler<TRequest>>();
        var pipeline = PipelineBuilder.Create(serviceProvider, handler);
        return pipeline.ExecuteAsync(objectRequest, token);
    }

}