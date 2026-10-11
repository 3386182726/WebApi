namespace Common.ExceptionHandling.CustomExceptions;

public class UnauthorizedException : PermissionException
{
    public UnauthorizedException()
        : base("Unauthorized.") { }

    public UnauthorizedException(string? message)
        : base(message) { }

    public UnauthorizedException(string? message, Exception? innerException)
        : base(message, innerException) { }

    public static void ThrowIfNull<T>(T value)
        where T : class?
    {
        if (value == null)
        {
            throw new UnauthorizedException();
        }
    }
}
