using SomeTimeLater.Primitives.Events;
using SomeTimeLater.Primitives.Requests;

namespace SomeTimeLater.Primitives;

public interface IMediatorConfiguration
{
    IMediatorConfiguration SetEventQueueConfiguration(string sectionName);
    IMediatorConfiguration SetEventQueueConfiguration(Action<EventQueueConfiguration> configureOptions);
    IMediatorConfiguration DisableEventQueue();

    IMediatorConfiguration AddRequestHandler<TRequest, TOutput, TClass>()
        where TRequest : Request<TOutput>
        where TClass : class, IRequestHandler<TRequest, TOutput>;

    IMediatorConfiguration AddRequestHandler<TRequest, TClass>()
        where TRequest : Request
        where TClass : class, IRequestHandler<TRequest>;

    IMediatorConfiguration AddEventHandler<TEvent, TClass>()
        where TEvent : AppEvent
        where TClass : class, IEventHandler<TEvent>;

    IMediatorConfiguration AddEventHandler<TEvent, TOutput, TClass>()
        where TEvent : AppEvent<TOutput>
        where TClass : class, IEventHandler<TEvent, TOutput>;
}