namespace Domain.Abstraction.Errors;

public static class ErrorCodes
{
    // Success
    public const string None = "SUCCESS";

    // Client Errors (4xx)
    public const string BadRequest = "BAD_REQUEST";
    public const string Unauthorized = "UNAUTHORIZED";
    public const string Forbidden = "FORBIDDEN";
    public const string NotFound = "NOT_FOUND";
    public const string Conflict = "CONFLICT";
    public const string ValidationError = "VALIDATION_ERROR";
    public const string NullValue = "NULL_VALUE";
    public const string Timeout = "TIMEOUT";
    public const string TooManyRequests = "TOO_MANY_REQUESTS";
    public const string InvalidOperation = "INVALID_OPERATION";
    public const string DuplicateEntry = "DUPLICATE_ENTRY";

    // Server Errors (5xx)
    public const string InternalError = "INTERNAL_ERROR";
    public const string DatabaseError = "DATABASE_ERROR";
    public const string ServiceUnavailable = "SERVICE_UNAVAILABLE";
    public const string NotImplemented = "NOT_IMPLEMENTED";

    // Business Logic Errors
    public const string BusinessRuleViolation = "BUSINESS_RULE_VIOLATION";
    public const string InsufficientPermissions = "INSUFFICIENT_PERMISSIONS";
    public const string InvalidState = "INVALID_STATE";
    public const string ConcurrencyError = "CONCURRENCY_ERROR";

    // External Service Errors
    public const string ExternalServiceError = "EXTERNAL_SERVICE_ERROR";
    public const string ApiError = "API_ERROR";
    public const string NetworkError = "NETWORK_ERROR";
}
