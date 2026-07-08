using SomeTimeLater.Primitives.Events;
using SomeTimeLater.Primitives.Requests;

namespace SomeTimeLater.Primitives;

public interface IMediator
{
    Task<TOutput> SendRequestAsync<TOutput>(Request<TOutput> request, CancellationToken token = default);
    Task SendRequestAsync(Request request, CancellationToken token = default);
    Task<IEnumerable<TOutput>> PublishEventAsync<TOutput>(AppEvent<TOutput> domainAppEvent, CancellationToken token = default);
    Task PublishEventAsync(AppEvent appAppEvent, CancellationToken token = default);
    bool EnqueueEvent(AppEvent appAppEvent);
    ValueTask EnqueueEventAsync(AppEvent appEvent, CancellationToken token = default);
}