using Microsoft.Extensions.DependencyInjection;
using SomeTimeLater.Primitives.Pipelines;

namespace SomeTimeLater.Primitives.Events;

internal abstract class EventHandlerExecutor 
{
    public abstract Task ExecuteAsync(object eventObject, IServiceProvider serviceProvider,
        CancellationToken token = default);
}

internal class EventHandlerExecutor<TEvent> : EventHandlerExecutor
    where TEvent : AppEvent
{
    public override Task ExecuteAsync(object eventObject, IServiceProvider serviceProvider, CancellationToken token = default)
    {
        return eventObject is not TEvent appEvent
            ? throw new InvalidEventTypeException($"Expected {typeof(TEvent).Name} but got {eventObject.GetType().Name}")
            : ExecuteAsync(appEvent, serviceProvider, token);
    }
    
    private static Task ExecuteAsync(TEvent appEvent, IServiceProvider serviceProvider, CancellationToken token = default)
    {
        var handlerList = serviceProvider.GetServices<IEventHandler<TEvent>>().ToList();
        var pipelineEnumerable = PipelineBuilder.Create(serviceProvider, handlerList);
        var tasksEnumerable = pipelineEnumerable.Select(p => p.ExecuteAsync(appEvent, token));
        return Task.WhenAll(tasksEnumerable);
    }
}