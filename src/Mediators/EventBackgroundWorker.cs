using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SomeTimeLater.Primitives;

internal class EventBackgroundWorker : BackgroundService
{
    private readonly IEventQueue _queue;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<EventBackgroundWorker> _logger;

    public EventBackgroundWorker(
        IEventQueue queue,
        IServiceProvider serviceProvider,
        ILogger<EventBackgroundWorker> logger)
    {
        _queue = queue;
        _serviceProvider = serviceProvider;
        _logger  = logger;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_logger.IsEnabled(LogLevel.Debug))
            _logger.LogDebug("EventBackgroundWorker started with {Workers} workers.", _queue.Configuration.ExecutionConcurrency);

        var workers = Enumerable.Range(0, _queue.Configuration.ExecutionConcurrency)
            .Select(_ => ProcessEventsAsync(stoppingToken))
            .ToArray();

        await Task.WhenAll(workers);

        if (_logger.IsEnabled(LogLevel.Debug))
            _logger.LogDebug("EventBackgroundWorker stopped.");
    }


    private async Task ProcessEventsAsync(CancellationToken stoppingToken)
    {
        await foreach (var queuedEvent in _queue.DequeueAllAsync(stoppingToken))
        {
            try
            {
                if (_logger.IsEnabled(LogLevel.Debug))
                    _logger.LogDebug("Processing event: {EventType}", queuedEvent.AppEvent.GetType().Name);

                await using var scope = _serviceProvider.CreateAsyncScope();

                await queuedEvent.Executor.ExecuteAsync(queuedEvent.AppEvent, scope.ServiceProvider, stoppingToken);

                if (_logger.IsEnabled(LogLevel.Debug))
                    _logger.LogDebug("Processed event: {EventType}", queuedEvent.AppEvent.GetType().Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing event: {EventType}", queuedEvent.AppEvent.GetType().Name);
            }
        }
    }
}