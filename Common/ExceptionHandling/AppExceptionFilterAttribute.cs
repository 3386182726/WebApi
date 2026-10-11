using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Common.ExceptionHandling
{
    /* https://stackoverflow.com/questions/38630076/asp-net-core-web-api-exception-handling
     * https://stackoverflow.com/questions/54307170/exceptionfilter-vs-exceptionlogger-vs-exceptionhandler-in-web-api-2
     */
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AppExceptionFilterAttribute : ExceptionFilterAttribute
    {
        static readonly JsonSerializerOptions Options = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };

        public override void OnException(ExceptionContext context)
        {
            var error = AppExceptionHandler.HandleException(context.Exception);

            if (error != null)
            {
                context.Result = new JsonResult(error, Options)
                {
                    StatusCode = (int)error.ErrorCode,
                };
                context.ExceptionHandled = true;
            }
        }
    }
}
