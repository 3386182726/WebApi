namespace Common.ExceptionHandling.CustomExceptions;

public class ResourceNotFoundException : Exception
{
    public ResourceNotFoundException() { }

    public ResourceNotFoundException(string? message)
        : base(message) { }

    public ResourceNotFoundException(string? message, Exception? innerException)
        : base(message, innerException) { }
}
