namespace SomeTimeLater.Primitives.Exceptions;

public class GetIdException(string message) : InvalidOperationException(message);