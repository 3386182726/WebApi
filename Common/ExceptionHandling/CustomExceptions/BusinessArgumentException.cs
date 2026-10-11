using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Common.ExceptionHandling.CustomExceptions;

public class BusinessArgumentException : ArgumentException
{
    public BusinessArgumentException() { }

    public BusinessArgumentException(string? message)
        : base(message) { }

    public BusinessArgumentException(string? message, Exception? innerException)
        : base(message, innerException) { }

    public BusinessArgumentException(string? message, string? paramName)
        : base(message, paramName) { }

    public BusinessArgumentException(string? message, string? paramName, Exception? innerException)
        : base(message, paramName, innerException) { }

    public static void ThrowIfNull(
        [NotNull] object? argument,
        [CallerArgumentExpression(nameof(argument))] string? paramName = null
    )
    {
        if (argument is null)
        {
            Throw(paramName);
        }
    }

    [DoesNotReturn]
    internal static void Throw(string? paramName) => throw new BusinessArgumentException(paramName);
}
