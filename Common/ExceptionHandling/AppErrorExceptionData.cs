using System.Net;
using System.Text.Json.Serialization;
using Common.Serialization;

namespace Common.ExceptionHandling;

public class AppErrorExceptionData(
    HttpStatusCode errorCode,
    string customCode = "",
    string description = ""
)
{
    [JsonConverter(typeof(JsonCamelStringEnumConverter))]
    [JsonPropertyName("ErrorCode")]
    public HttpStatusCode ErrorCode { get; set; } = errorCode;

    [JsonPropertyName("CustomCode")]
    public string? CustomCode { get; set; } = customCode;

    [JsonPropertyName("Description")]
    public string? Description { get; set; } = description;

    [JsonPropertyName("Data")]
    public object? Data { get; set; }

    // Custom error data.
    public readonly static AppErrorExceptionData UndefinedError = new(
        HttpStatusCode.Conflict,
        "UndefinedError",
        "Unknown error"
    );
    public static readonly AppErrorExceptionData ResourceNotFound = new(
        HttpStatusCode.NotFound,
        "ResourceNotFound",
        "Resource not found"
    );
    public static readonly AppErrorExceptionData Unauthorized = new(
        HttpStatusCode.Unauthorized,
        "Unauthorized",
        "Unauthorized access"
    );

    public static readonly AppErrorExceptionData Conflict = new(
        HttpStatusCode.Conflict,
        "Conflict",
        "Resource conflict"
    );
    public static readonly AppErrorExceptionData Forbidden = new(
        HttpStatusCode.Forbidden,
        "Forbidden",
        "Access denied"
    );
    public static readonly AppErrorExceptionData UnprocessableEntity = new(
        HttpStatusCode.UnprocessableEntity,
        "UnprocessableEntity",
        "Unprocessable entity"
    );
}
