using System.Net;

namespace Domain.Abstraction.Errors;

public sealed record Error
{
    /// <summary>
    /// Error code (e.g., "NOT_FOUND", "VALIDATION_ERROR")
    /// </summary>
    public string Code { get; init; }

    /// <summary>
    /// Human-readable error message
    /// </summary>
    public string Message { get; init; }

    /// <summary>
    /// HTTP status code associated with this error
    /// </summary>
    public HttpStatusCode StatusCode { get; init; }

    /// <summary>
    /// Additional error details or metadata
    /// </summary>
    public Dictionary<string, object>? Details { get; init; }

    /// <summary>
    /// Inner error for error chaining
    /// </summary>
    public Error? InnerError { get; init; }

    /// <summary>
    /// Timestamp when error occurred
    /// </summary>
    public DateTime Timestamp { get; init; }

    private Error(string code, string message,
        HttpStatusCode statusCode = HttpStatusCode.InternalServerError,
        Dictionary<string, object>? details = null,
        Error? innerError = null)
    {
        Code = code;
        Message = message;
        StatusCode = statusCode;
        Details = details;
        InnerError = innerError;
        Timestamp = DateTime.UtcNow;
    }

    // ========================================================================
    // PREDEFINED ERRORS
    // ========================================================================

    /// <summary>
    /// No error occurred
    /// </summary>
    public static readonly Error None = new(
        ErrorCodes.None,
        "No error",
        HttpStatusCode.OK);

    /// <summary>
    /// Resource not found
    /// </summary>
    public static readonly Error NotFound = new(
        ErrorCodes.NotFound,
        "The requested resource was not found",
        HttpStatusCode.NotFound);

    /// <summary>
    /// Bad request / Invalid input
    /// </summary>
    public static readonly Error BadRequest = new(
        ErrorCodes.BadRequest,
        "The request is invalid",
        HttpStatusCode.BadRequest);

    /// <summary>
    /// Unauthorized access
    /// </summary>
    public static readonly Error Unauthorized = new(
        ErrorCodes.Unauthorized,
        "Authentication is required",
        HttpStatusCode.Unauthorized);

    /// <summary>
    /// Forbidden access
    /// </summary>
    public static readonly Error Forbidden = new(
        ErrorCodes.Forbidden,
        "You don't have permission to access this resource",
        HttpStatusCode.Forbidden);

    /// <summary>
    /// Conflict / Resource already exists
    /// </summary>
    public static readonly Error Conflict = new(
        ErrorCodes.Conflict,
        "The resource already exists",
        HttpStatusCode.Conflict);

    /// <summary>
    /// Validation error
    /// </summary>
    public static readonly Error ValidationError = new(
        ErrorCodes.ValidationError,
        "One or more validation errors occurred",
        HttpStatusCode.BadRequest);

    /// <summary>
    /// Internal server error
    /// </summary>
    public static readonly Error InternalError = new(
        ErrorCodes.InternalError,
        "An internal server error occurred",
        HttpStatusCode.InternalServerError);

    /// <summary>
    /// Database error
    /// </summary>
    public static readonly Error DatabaseError = new(
        ErrorCodes.DatabaseError,
        "A database error occurred",
        HttpStatusCode.InternalServerError);

    /// <summary>
    /// Null value provided
    /// </summary>
    public static readonly Error NullValue = new(
        ErrorCodes.NullValue,
        "A null value was provided",
        HttpStatusCode.BadRequest);

    /// <summary>
    /// Service unavailable
    /// </summary>
    public static readonly Error ServiceUnavailable = new(
        ErrorCodes.ServiceUnavailable,
        "The service is temporarily unavailable",
        HttpStatusCode.ServiceUnavailable);

    /// <summary>
    /// Request timeout
    /// </summary>
    public static readonly Error Timeout = new(
        ErrorCodes.Timeout,
        "The request timed out",
        HttpStatusCode.RequestTimeout);

    /// <summary>
    /// Database error
    /// </summary>
    public static readonly Error DataBaseError = new(
        ErrorCodes.DatabaseError,
        "A database error occurred",
        HttpStatusCode.InternalServerError);

    // ========================================================================
    // FACTORY METHODS
    // ========================================================================

    /// <summary>
    /// Creates a custom error
    /// </summary>
    public static Error Custom(
        string code,
        string message,
        HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
    {
        return new Error(code, message, statusCode);
    }

    /// <summary>
    /// Creates an error with details
    /// </summary>
    public static Error WithDetails(
        string code,
        string message,
        Dictionary<string, object> details,
        HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
    {
        return new Error(code, message, statusCode, details);
    }

    /// <summary>
    /// Creates a not found error with entity name
    /// </summary>
    public static Error NotFoundWithEntity(string entityName, Guid? id = null)
    {
        var message = id.HasValue
            ? $"{entityName} with id '{id}' was not found"
            : $"{entityName} was not found";

        var details = new Dictionary<string, object>
        {
            { "entityName", entityName }
        };

        if (id.HasValue)
            details.Add("entityId", id.Value);

        return new Error(ErrorCodes.NotFound, message, HttpStatusCode.NotFound, details);
    }

    /// <summary>
    /// Creates a validation error with field errors
    /// </summary>
    public static Error Validation(Dictionary<string, string[]> errors)
    {
        var details = new Dictionary<string, object>
        {
            { "errors", errors }
        };

        return new Error(
            ErrorCodes.ValidationError,
            "One or more validation errors occurred",
            HttpStatusCode.BadRequest,
            details);
    }

    /// <summary>
    /// Creates an error from exception
    /// </summary>
    public static Error FromException(Exception exception)
    {
        var details = new Dictionary<string, object>
        {
            { "exceptionType", exception.GetType().Name },
            { "stackTrace", exception.StackTrace ?? string.Empty }
        };

        Error? innerError = exception.InnerException != null
            ? FromException(exception.InnerException)
            : null;

        return new Error(
            ErrorCodes.InternalError,
            exception.Message,
            HttpStatusCode.InternalServerError,
            details,
            innerError);
    }

    // ========================================================================
    // METHODS
    // ========================================================================

    /// <summary>
    /// Creates a copy with a different message
    /// </summary>
    public Error WithMessage(string message)
    {
        return this with { Message = message };
    }

    /// <summary>
    /// Creates a copy with additional details
    /// </summary>
    public Error WithDetail(string key, object value)
    {
        var newDetails = Details != null
            ? new Dictionary<string, object>(Details)
            : new Dictionary<string, object>();

        newDetails[key] = value;

        return this with { Details = newDetails };
    }

    /// <summary>
    /// Creates a copy with inner error
    /// </summary>
    public Error WithInnerError(Error innerError)
    {
        return this with { InnerError = innerError };
    }

    /// <summary>
    /// Checks if this is a specific error
    /// </summary>
    public bool Is(string code)
    {
        return Code.Equals(code, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Gets detail value
    /// </summary>
    public T? GetDetail<T>(string key)
    {
        if (Details == null || !Details.TryGetValue(key, out var value))
            return default;

        return value is T typedValue ? typedValue : default;
    }

    /// <summary>
    /// Converts error to string representation
    /// </summary>
    public override string ToString()
    {
        var result = $"[{Code}] {Message}";

        if (Details != null && Details.Any())
        {
            var detailsStr = string.Join(", ", Details.Select(d => $"{d.Key}={d.Value}"));
            result += $" | Details: {detailsStr}";
        }

        if (InnerError != null)
            result += $" | InnerError: {InnerError}";

        return result;
    }
}
