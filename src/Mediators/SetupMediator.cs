using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SomeTimeLater.Primitives.Events;
using SomeTimeLater.Primitives.Requests;

namespace SomeTimeLater.Primitives;

public static class SetupMediator
{
    extension(IHostApplicationBuilder builder)
    {
        public IMediatorConfiguration AddMediator()
        {
            return MediatorConfiguration.Enable(builder.Services, builder.Configuration);
        }
    }
    
    extension(IServiceCollection services)
    {
        public IServiceCollection AddRequestHandler<TRequest, TOutput, TClass>()
            where TRequest : Request<TOutput>
            where TClass : class, IRequestHandler<TRequest, TOutput>
        {
            return services.AddTransient<IRequestHandler<TRequest, TOutput>, TClass>();
        }
        
        public IServiceCollection AddRequestHandler<TRequest, TClass>()
            where TRequest : Request
            where TClass : class, IRequestHandler<TRequest>
        {
            return services.AddTransient<IRequestHandler<TRequest>, TClass>();
        }
        
        public IServiceCollection AddEventHandler<TEvent, TClass>()
            where TEvent : AppEvent
            where TClass : class, IEventHandler<TEvent>
        {
            return services.AddTransient<IEventHandler<TEvent>, TClass>();
        }
        
        public IServiceCollection AddEventHandler<TEvent, TOutput, TClass>()
            where TEvent : AppEvent<TOutput>
            where TClass : class, IEventHandler<TEvent, TOutput>
        {
            return services.AddTransient<IEventHandler<TEvent, TOutput>, TClass>();
        }
    }
}