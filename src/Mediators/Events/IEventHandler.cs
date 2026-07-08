namespace SomeTimeLater.Primitives.Events;

public interface IEventHandler<in TEvent, TOutput>
    where TEvent : AppEvent<TOutput>
{
    Task<TOutput> HandleAsync(TEvent domainEvent, CancellationToken token = default);
}

public interface IEventHandler<in TEvent>
    where TEvent : AppEvent
{
    Task HandleAsync(TEvent domainEvent, CancellationToken token = default);
}