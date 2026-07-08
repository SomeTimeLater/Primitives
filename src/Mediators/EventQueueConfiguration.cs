namespace SomeTimeLater.Primitives;

public record EventQueueConfiguration
{
    public int QueueCapacity { get; init; } = 1000;
    public int ExecutionConcurrency { get; init; } = 1;
}