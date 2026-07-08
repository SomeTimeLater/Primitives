using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using SomeTimeLater.Primitives.Events;
using SomeTimeLater.Primitives.Requests;

namespace SomeTimeLater.Primitives;

public class MediatorConfiguration : IMediatorConfiguration
{
    public IServiceCollection Services { get; init; }
    public IConfiguration Configuration { get; init; }
    
    private MediatorConfiguration(IServiceCollection services, IConfiguration configuration)
    {
        Services = services;
        Configuration = configuration;
    }

    public static IMediatorConfiguration Enable(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IMediator, Mediator>();
        services.AddHostedService<EventBackgroundWorker>();
        services.AddSingleton<IEventQueue, EventQueue>();
        services.AddOptions<EventQueueConfiguration>();
        return new MediatorConfiguration(services, configuration);
    }
    
    public IMediatorConfiguration SetEventQueueConfiguration(string sectionName)
    {
        Services.RemoveAll<IConfigureOptions<EventQueueConfiguration>>();
        Services.AddOptions<EventQueueConfiguration>().BindConfiguration(sectionName);
        return this;
    }

    public IMediatorConfiguration SetEventQueueConfiguration(Action<EventQueueConfiguration> configureOptions)
    {
        Services.RemoveAll<IConfigureOptions<EventQueueConfiguration>>();
        Services.AddOptions<EventQueueConfiguration>().Configure(configureOptions);
        return this;
    }
    
    public IMediatorConfiguration DisableEventQueue()
    {
        Services.RemoveAll<IEventQueue>();
        var descriptor = Services.FirstOrDefault(
            d => d.ServiceType == typeof(IHostedService) &&
                 d.ImplementationType == typeof(EventBackgroundWorker));
        if (descriptor is not null)
            Services.Remove(descriptor);
        Services.AddSingleton<IEventQueue, DisabledEventQueue>();
        return this;
    }
    
    public IMediatorConfiguration AddRequestHandler<TRequest, TOutput, TClass>()
        where TRequest : Request<TOutput>
        where TClass : class, IRequestHandler<TRequest, TOutput>
    {
        Services.AddRequestHandler<TRequest, TOutput, TClass>();
        return this;
    }
        
    public IMediatorConfiguration AddRequestHandler<TRequest, TClass>()
        where TRequest : Request
        where TClass : class, IRequestHandler<TRequest>
    {
        Services.AddRequestHandler<TRequest, TClass>();
        return this;
    }
        
    public IMediatorConfiguration AddEventHandler<TEvent, TClass>()
        where TEvent : AppEvent
        where TClass : class, IEventHandler<TEvent>
    {
        Services.AddEventHandler<TEvent, TClass>();
        return this;
    }
        
    public IMediatorConfiguration AddEventHandler<TEvent, TOutput, TClass>()
        where TEvent : AppEvent<TOutput>
        where TClass : class, IEventHandler<TEvent, TOutput>
    {
        Services.AddEventHandler<TEvent, TOutput, TClass>();
        return this;
    }
}