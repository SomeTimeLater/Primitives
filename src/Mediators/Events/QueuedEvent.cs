namespace SomeTimeLater.Primitives.Events;

internal record QueuedEvent(EventHandlerExecutor Executor, AppEvent AppEvent);