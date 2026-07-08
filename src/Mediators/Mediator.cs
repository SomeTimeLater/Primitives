using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using SomeTimeLater.Primitives.Cache;
using SomeTimeLater.Primitives.Events;
using SomeTimeLater.Primitives.Requests;

namespace SomeTimeLater.Primitives;

[DebuggerStepThrough]
internal class Mediator : IMediator
{
    private readonly IEventQueue _eventQueue;
    private readonly IServiceProvider _serviceProvider;
    private readonly ExecutorCache _executorCache = new ();

    public Mediator(IEventQueue eventQueue, IServiceProvider serviceProvider)
    {
        _eventQueue = eventQueue;
        _serviceProvider = serviceProvider;
    }
    
    public async Task<TOutput> SendRequestAsync<TOutput>(Request<TOutput> request, CancellationToken token = default)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        var executor = _executorCache.GetRequestHandlerExecutor<TOutput>(request.GetType());
        var outputObject = await executor.ExecuteWithOutputAsync(request, scope.ServiceProvider, token);
        return (TOutput) outputObject ?? throw new InvalidOperationException("Handler returned an invalid output type.");
    }
    
    public async Task SendRequestAsync(Request request, CancellationToken token = default)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        var executor = _executorCache.GetRequestHandlerExecutor(request.GetType());
        await executor.ExecuteAsync(request, scope.ServiceProvider, token);
    }
    
    public async Task<IEnumerable<TOutput>> PublishEventAsync<TOutput>(AppEvent<TOutput> appEvent, CancellationToken token = default)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        var executor = _executorCache.GetEventHandlerExecutor<TOutput>(appEvent.GetType());
        var outputEnumerable = await executor.ExecuteWithOutputAsync(appEvent, scope.ServiceProvider, token);
        return outputEnumerable.Cast<TOutput>();
    }
    
    public async Task PublishEventAsync(AppEvent appEvent, CancellationToken token = default)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        var executor = _executorCache.GetEventHandlerExecutor(appEvent.GetType());
        await executor.ExecuteAsync(appEvent, scope.ServiceProvider, token);
    }

    public bool EnqueueEvent(AppEvent appEvent)
    {
        var executor = _executorCache.GetEventHandlerExecutor(appEvent.GetType());
        return _eventQueue.Enqueue(executor, appEvent);
    }
    
    public bool EnqueueEvent<TOutput>(AppEvent<TOutput> appEvent)
    {
        var executor = _executorCache.GetEventHandlerExecutor(appEvent.GetType());
        return _eventQueue.Enqueue(executor, appEvent);
    }

    public ValueTask EnqueueEventAsync(AppEvent appEvent, CancellationToken token = default)
    {
        var executor = _executorCache.GetEventHandlerExecutor(appEvent.GetType());
        return _eventQueue.EnqueueAsync(executor, appEvent, token);
    }
    
    public ValueTask EnqueueEventAsync<TOutput>(AppEvent<TOutput> appEvent, CancellationToken token = default)
    {
        var executor = _executorCache.GetEventHandlerExecutor(appEvent.GetType());
        return _eventQueue.EnqueueAsync(executor, appEvent, token);
    }
}