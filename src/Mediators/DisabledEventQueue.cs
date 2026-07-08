using Microsoft.Extensions.Logging;
using SomeTimeLater.Primitives.Events;

namespace SomeTimeLater.Primitives;

internal class DisabledEventQueue : IEventQueue
{
    public EventQueueConfiguration Configuration { get; }
    private readonly ILogger<DisabledEventQueue> _logger;
    
    public DisabledEventQueue(ILogger<DisabledEventQueue> logger)
    {
        Configuration = new EventQueueConfiguration();
        _logger = logger;
    }
    
    public bool Enqueue(EventHandlerExecutor executor, AppEvent appEvent)
    {
        _logger.LogWarning("Event queue is disabled. Event {EventType} will not be executed", appEvent.GetType().Name);
        return false;
    }

    public ValueTask EnqueueAsync(EventHandlerExecutor executor, AppEvent appEvent, CancellationToken cancellationToken = default)
    {
        _logger.LogWarning("Event queue is disabled. Event {EventType} will not be executed", appEvent.GetType().Name);
        return ValueTask.CompletedTask;
    }

    public IAsyncEnumerable<QueuedEvent> DequeueAllAsync(CancellationToken token)
    {
        _logger.LogWarning("Event queue is disabled. No events will be dequeued");
        return AsyncEnumerable.Empty<QueuedEvent>();
    }
}