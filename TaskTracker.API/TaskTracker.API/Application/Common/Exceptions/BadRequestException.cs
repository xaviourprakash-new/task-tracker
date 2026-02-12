namespace TaskTracker.API.Application.Common.Exceptions;

public class BadRequestException : Exception
{
    public IDictionary<string, string[]> Errors { get; }

    public BadRequestException(string message)
        : base(message)
    {
        Errors = new Dictionary<string, string[]>();
    }

    public BadRequestException(string message, IDictionary<string, string[]> errors)
        : base(message)
    {
        Errors = errors;
    }
}
