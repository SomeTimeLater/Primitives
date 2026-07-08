using Microsoft.Extensions.DependencyInjection;
using SomeTimeLater.Primitives.Events;
using SomeTimeLater.Primitives.Requests;

namespace SomeTimeLater.Primitives.Pipelines;

internal static class PipelineBuilder
{
    public static PipelineExecutor<TRequest, TOutput> Create<TRequest, TOutput>(IServiceProvider serviceProvider, IRequestHandler<TRequest, TOutput> handler)
        where TRequest : Request<TOutput>
    {
        return PipelineBuilder<TOutput>.Create(serviceProvider, handler);
    }
    
    public static IEnumerable<PipelineExecutor<TEvent, TOutput>> Create<TEvent, TOutput>(IServiceProvider serviceProvider, List<IEventHandler<TEvent, TOutput>> eventHandlers)
        where TEvent : AppEvent<TOutput>
    {
        return PipelineBuilder<TOutput>.Create(serviceProvider, eventHandlers);
    }
    
    public static PipelineExecutor<TRequest> Create<TRequest>(IServiceProvider serviceProvider, IRequestHandler<TRequest> handler)
        where TRequest : Request
    {
        var pipelineBehaviorsList = GetPipelineBehaviors<TRequest>(serviceProvider);
        return CreatePipelineExecutor(pipelineBehaviorsList, handler.HandleAsync);
    }
    
    public static IEnumerable<PipelineExecutor<TEvent>> Create<TEvent>(IServiceProvider serviceProvider, List<IEventHandler<TEvent>> eventHandlers)
        where TEvent : AppEvent
    {
        var finalHandlers = eventHandlers
            .Select(eh => new Func<TEvent, CancellationToken, Task>(eh.HandleAsync))
            .ToList();
        return CreatePipelineExecutors(serviceProvider, finalHandlers);
    }

    private static IEnumerable<PipelineExecutor<TInput>> CreatePipelineExecutors<TInput>(IServiceProvider serviceProvider,
        List<Func<TInput, CancellationToken, Task>> finalHandlers)
    {
        var pipelineBehaviorsList = GetPipelineBehaviors<TInput>(serviceProvider);
        return finalHandlers.Select(finalHandler => CreatePipelineExecutor(pipelineBehaviorsList, finalHandler));
    }
    
    private static PipelineExecutor<TInput> CreatePipelineExecutor<TInput>(List<IPipelineStep<TInput>>  pipelineSteps, Func<TInput, CancellationToken, Task> finalHandler)
    {
        NextDelegate<TInput> next = (i, c) => finalHandler(i, c);

        for (var i = pipelineSteps.Count - 1; i >= 0; i--)
        {
            var step = pipelineSteps[i];
            var currentNext = next;
            next = (input, token) => step.ExecuteAsync(input, currentNext, token);
        }
        
        return new PipelineExecutor<TInput>(next);
    }

    private static List<IPipelineStep<TInput>> GetPipelineBehaviors<TInput>(IServiceProvider serviceProvider)
    {
        try
        {
            return serviceProvider.GetServices<IPipelineStep<TInput>>().ToList();
        }
        catch (Exception)
        {
            return [];
        }
    }
}

internal static class PipelineBuilder<TOutput>
{
    public static PipelineExecutor<TRequest, TOutput> Create<TRequest>(IServiceProvider serviceProvider, IRequestHandler<TRequest, TOutput> handler)
        where TRequest : Request<TOutput>
    {
        var pipelineBehaviorsList = GetPipelineBehaviors<TRequest>(serviceProvider);
        return CreatePipelineExecutor(pipelineBehaviorsList, handler.HandleAsync);
    }
    
    public static IEnumerable<PipelineExecutor<TEvent, TOutput>> Create<TEvent>(IServiceProvider serviceProvider, List<IEventHandler<TEvent, TOutput>> eventHandlers)
        where TEvent : AppEvent<TOutput>
    {
        var finalHandlers = eventHandlers
            .Select(eh => new Func<TEvent, CancellationToken, Task<TOutput>>(eh.HandleAsync))
            .ToList();
        return CreatePipelineExecutors(serviceProvider, finalHandlers);
    }

    private static IEnumerable<PipelineExecutor<TInput, TOutput>> CreatePipelineExecutors<TInput>(IServiceProvider serviceProvider,
        List<Func<TInput, CancellationToken, Task<TOutput>>> finalHandlers)
    {
        var pipelineBehaviorsList = GetPipelineBehaviors<TInput>(serviceProvider);
        return finalHandlers.Select(finalHandler => CreatePipelineExecutor(pipelineBehaviorsList, finalHandler));
    }
    
    private static PipelineExecutor<TInput, TOutput> CreatePipelineExecutor<TInput>(List<IPipelineStep<TInput, TOutput>>  pipelineSteps, Func<TInput, CancellationToken, Task<TOutput>> finalHandler)
    {
        NextDelegate<TInput, TOutput> next = (i, c) => finalHandler(i, c);

        for (var i = pipelineSteps.Count - 1; i >= 0; i--)
        {
            var step = pipelineSteps[i];
            var currentNext = next;
            next = (input, token) => step.ExecuteAsync(input, currentNext, token);
        }
        
        return new PipelineExecutor<TInput, TOutput>(next);
    }

    private static List<IPipelineStep<TInput, TOutput>> GetPipelineBehaviors<TInput>(IServiceProvider serviceProvider)
    {
        try
        {
            return serviceProvider.GetServices<IPipelineStep<TInput, TOutput>>().ToList();
        }
        catch (Exception)
        {
            return [];
        }
    }
}