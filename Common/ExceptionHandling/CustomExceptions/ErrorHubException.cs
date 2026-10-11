using System.Text.Json;
using Microsoft.AspNetCore.SignalR;

namespace Common.ExceptionHandling.CustomExceptions;

public class ErrorHubException<T> : HubException
    where T : AppErrorExceptionData
{
    static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    public ErrorHubException(T response)
        : base(JsonSerializer.Serialize(response, Options)) { }

    public ErrorHubException(T response, Exception? innerException)
        : base(JsonSerializer.Serialize(response, Options), innerException) { }
}
