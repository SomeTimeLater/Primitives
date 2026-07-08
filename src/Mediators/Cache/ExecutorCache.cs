using System.Collections.Concurrent;
using System.Diagnostics;
using SomeTimeLater.Primitives.Events;
using SomeTimeLater.Primitives.Requests;

namespace SomeTimeLater.Primitives.Cache;

[DebuggerStepThrough]
internal class ExecutorCache
{
    private readonly ConcurrentDictionary<Type, RequestOutputHandlerExecutor> _requestCache = [];
    private readonly ConcurrentDictionary<Type, RequestHandlerExecutor> _requestWithOutputCache = [];
    private readonly ConcurrentDictionary<Type, EventHandlerExecutor> _eventCache = [];
    private readonly ConcurrentDictionary<Type, EventOutputHandlerExecutor> _eventWithOutputCache = [];

    public RequestOutputHandlerExecutor GetRequestHandlerExecutor<TOutput>(Type requestType) 
    {
        return _requestCache.GetOrAdd(requestType, CreateRequestHandlerExecutor<TOutput>);
    }
    
    public RequestHandlerExecutor GetRequestHandlerExecutor(Type requestType) 
    {
        return _requestWithOutputCache.GetOrAdd(requestType, CreateRequestHandlerExecutor);
    }
    
    public EventHandlerExecutor GetEventHandlerExecutor(Type eventType) 
    {
        return _eventCache.GetOrAdd(eventType, CreateEventHandlerExecutor);
    }
    
    public EventOutputHandlerExecutor GetEventHandlerExecutor<TOutput>(Type eventType) 
    {
        return _eventWithOutputCache.GetOrAdd(eventType, CreateEventHandlerExecutor<TOutput>);
    }
    
    private static RequestHandlerExecutor CreateRequestHandlerExecutor(Type requestType) 
    {
        var genericType = typeof(RequestHandlerExecutor<>).MakeGenericType(requestType);
        var executor = Activator.CreateInstance(genericType) as RequestHandlerExecutor;
        return executor ?? throw new DispatcherHandlerCreationException($"Error creating request handler executor {genericType.Name}");
    }
    
    private static RequestOutputHandlerExecutor CreateRequestHandlerExecutor<TOutput>(Type requestType) 
    {
        var genericType = typeof(RequestOutputHandlerExecutor<,>).MakeGenericType(requestType, typeof(TOutput));
        var executor = Activator.CreateInstance(genericType) as RequestOutputHandlerExecutor;
        return executor ?? throw new DispatcherHandlerCreationException($"Error creating request handler executor {genericType.Name}");
    }
    
    private static EventHandlerExecutor CreateEventHandlerExecutor(Type eventType) 
    {
        var genericType = typeof(EventHandlerExecutor<>).MakeGenericType(eventType);
        var executor = Activator.CreateInstance(genericType) as EventHandlerExecutor;
        return executor ?? throw new DispatcherHandlerCreationException($"Error creating event handler executor {genericType.Name}");
    }
    
    private static EventOutputHandlerExecutor CreateEventHandlerExecutor<TOutput>(Type eventType) 
    {
        var genericType = typeof(EventOutputHandlerExecutor<,>).MakeGenericType(eventType, typeof(TOutput));
        var executor = Activator.CreateInstance(genericType) as EventOutputHandlerExecutor;
        return executor ?? throw new DispatcherHandlerCreationException($"Error creating event handler executor {genericType.Name}");
    }
}