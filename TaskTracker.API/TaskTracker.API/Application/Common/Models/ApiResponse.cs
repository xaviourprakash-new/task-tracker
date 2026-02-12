using System.Text.Json.Serialization;

namespace TaskTracker.API.Application.Common.Models;

public class ApiResponse
{
    public bool Success { get; init; }
    public string Message { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? TraceId { get; init; }

    protected ApiResponse(string message, bool success = true, string? traceId = null)
    {
        Success = success;
        Message = message;
        TraceId = traceId;
    }

    public static ApiResponse SuccessResponse(string message = "Request completed successfully.")
        => new(message);

    public static ApiResponse<T> SuccessResponse<T>(T data, string message = "Request completed successfully.")
        => new(data, message);
}

public class ApiResponse<T> : ApiResponse
{
    public T Data { get; init; }

    internal ApiResponse(T data, string message)
        : base(message)
    {
        Data = data;
    }
}
