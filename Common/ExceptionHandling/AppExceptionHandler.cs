using Common.ExceptionHandling.CustomExceptions;

namespace Common.ExceptionHandling;

public static class AppExceptionHandler
{
    public static AppErrorExceptionData? HandleException(Exception exception)
    {
        var error = exception switch
        {
            ConflictException => AppErrorExceptionData.Conflict,
            UnauthorizedException => AppErrorExceptionData.Unauthorized,
            PermissionException => AppErrorExceptionData.Forbidden,
            ResourceNotFoundException => AppErrorExceptionData.ResourceNotFound,
            BusinessArgumentException => AppErrorExceptionData.UnprocessableEntity,
            _ => null,
        };

        if (error != null)
            error.Description = exception.Message;

        return error;
    }
}
