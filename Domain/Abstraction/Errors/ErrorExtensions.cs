using Domain.Abstraction.Base;
using Serilog;

namespace Domain.Abstraction.Errors;

public static class ErrorExtensions
{
    /// <summary>
    /// Converts error to problem details for API responses
    /// </summary>
    public static ApiProblemDetails ToProblemDetails(this Error error, string? traceId = null)
    {
        var problemDetails = new ApiProblemDetails
        {
            Type = $"https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/{(int)error.StatusCode}",
            Title = error.Code,
            Status = (int)error.StatusCode,
            Detail = error.Message,
            Extensions = new Dictionary<string, object>
            {
                ["timestamp"] = error.Timestamp,
                ["errorCode"] = error.Code
            }
        };

        if (!string.IsNullOrWhiteSpace(traceId))
        {
            problemDetails.Extensions["traceId"] = traceId;
        }

        if (error.Details != null && error.Details.Any())
        {
            foreach (var detail in error.Details)
            {
                problemDetails.Extensions[detail.Key] = detail.Value;
            }
        }

        if (error.InnerError != null)
        {
            problemDetails.Extensions["innerError"] = new
            {
                code = error.InnerError.Code,
                message = error.InnerError.Message
            };
        }

        return problemDetails;
    }

    /// <summary>
    /// Checks if error is client error (4xx)
    /// </summary>
    public static bool IsClientError(this Error error)
    {
        var statusCode = (int)error.StatusCode;
        return statusCode >= 400 && statusCode < 500;
    }

    /// <summary>
    /// Checks if error is server error (5xx)
    /// </summary>
    public static bool IsServerError(this Error error)
    {
        var statusCode = (int)error.StatusCode;
        return statusCode >= 500 && statusCode < 600;
    }

    /// <summary>
    /// Logs error with appropriate level
    /// </summary>
    public static void Log(this Error error, ILogger logger)
    {
        if (error.IsServerError())
            logger.Error("Error occurred: {Error}", error);
        else if (error.IsClientError())
            logger.Warning("Client error: {Error}", error);
        else
            logger.Information("Error: {Error}", error);
    }
}