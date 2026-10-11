using Common.ExceptionHandling;
using Common.ExceptionHandling.CustomExceptions;
using Microsoft.AspNetCore.SignalR;

namespace Common.ExceptionHandling;

// https://github.com/aspnet/SignalR/issues/3009
public class ErrorResponseHubFilter : IHubFilter
{
    public async ValueTask<object?> InvokeMethodAsync(
        HubInvocationContext invocationContext,
        Func<HubInvocationContext, ValueTask<object?>> next
    )
    {
        try
        {
            return await next(invocationContext);
        }
        catch (Exception ex)
        {
            var error = AppExceptionHandler.HandleException(ex);

            if (error != null)
            {
                throw new ErrorHubException<AppErrorExceptionData>(error, ex);
            }

            throw;
        }
    }
}
