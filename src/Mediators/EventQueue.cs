using System.Threading.Channels;
using Microsoft.Extensions.Options;
using SomeTimeLater.Primitives.Events;

namespace SomeTimeLater.Primitives;

internal class EventQueue : IEventQueue
{
    public EventQueueConfiguration Configuration { get; }
    private readonly Channel<QueuedEvent> _channel;

    public EventQueue(IOptions<EventQueueConfiguration> options)
    {
        Configuration = options.Value;
        _channel = Channel.CreateBounded<QueuedEvent>(new BoundedChannelOptions(Configuration.QueueCapacity)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = false,
            SingleWriter = false
        });
    }

    public bool Enqueue(EventHandlerExecutor executor, AppEvent appEvent)
    {
        var queuedEvent = new QueuedEvent(executor, appEvent);
        return _channel.Writer.TryWrite(queuedEvent);
    }

    public ValueTask EnqueueAsync(EventHandlerExecutor executor, AppEvent appEvent, CancellationToken cancellationToken = default)
    {
        var queuedEvent = new QueuedEvent(executor, appEvent);
        return _channel.Writer.WriteAsync(queuedEvent, cancellationToken);
    }

    public IAsyncEnumerable<QueuedEvent> DequeueAllAsync(CancellationToken token)
    {
        return _channel.Reader.ReadAllAsync(token);
    }

    
}