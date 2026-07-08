using SomeTimeLater.Primitives.Events;

namespace SomeTimeLater.Primitives;

internal interface IEventQueue
{
    EventQueueConfiguration Configuration { get; }
    bool Enqueue(EventHandlerExecutor executor, AppEvent appEvent);
    ValueTask EnqueueAsync(EventHandlerExecutor executor, AppEvent appEvent, CancellationToken cancellationToken = default);
    IAsyncEnumerable<QueuedEvent> DequeueAllAsync(CancellationToken token);
}