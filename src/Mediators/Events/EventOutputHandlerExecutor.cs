using Microsoft.Extensions.DependencyInjection;
using SomeTimeLater.Primitives.Pipelines;

namespace SomeTimeLater.Primitives.Events;

internal abstract class EventOutputHandlerExecutor : EventHandlerExecutor
{
    public abstract Task<IEnumerable<object>> ExecuteWithOutputAsync(object eventObject, IServiceProvider serviceProvider,
        CancellationToken token = default);
}

internal class EventOutputHandlerExecutor<TEvent, TOutput> : EventOutputHandlerExecutor
    where TEvent : AppEvent<TOutput>
{
    
    public override Task ExecuteAsync(object eventObject, IServiceProvider serviceProvider, CancellationToken token = default)
    {
        return ExecuteWithOutputAsync(eventObject, serviceProvider, token);
    }
    
    public override Task<IEnumerable<object>> ExecuteWithOutputAsync(object eventObject, IServiceProvider serviceProvider, CancellationToken token = default)
    {
        return eventObject is not TEvent appEvent 
            ? throw new InvalidEventTypeException($"Expected {typeof(TEvent).Name} but got {eventObject.GetType().Name}")
            : ExecuteWithOutputAsync(appEvent, serviceProvider, token);
    }
    
    private static async Task<IEnumerable<object>> ExecuteWithOutputAsync(
        TEvent eventObject,
        IServiceProvider serviceProvider,
        CancellationToken token = default)
    {
        var handlerList = serviceProvider.GetServices<IEventHandler<TEvent, TOutput>>().ToList();
        var pipelineEnumerable = PipelineBuilder.Create(serviceProvider, handlerList);
        var tasksEnumerable = pipelineEnumerable.Select(p => p.ExecuteAsync(eventObject, token));
        var output = await Task.WhenAll(tasksEnumerable);
        return output.Cast<object>();
    }
}