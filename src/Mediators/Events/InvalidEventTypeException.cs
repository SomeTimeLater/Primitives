namespace SomeTimeLater.Primitives.Events;

internal class InvalidEventTypeException(string message) : Exception(message);