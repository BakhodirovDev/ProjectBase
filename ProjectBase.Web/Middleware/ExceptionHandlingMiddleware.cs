using Domain.Abstraction.Base;
using Serilog;
using System.Net;
using System.Text.Json;

namespace ProjectBase.WebApi.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IWebHostEnvironment _environment;

    public ExceptionHandlingMiddleware(RequestDelegate next, IWebHostEnvironment environment)
    {
        _next = next;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var traceId = context.TraceIdentifier;

        // Determine status code and message based on exception type
        var (statusCode, code, message) = exception switch
        {
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "Unauthorized", "Access denied"),
            NotFoundException notFound => (HttpStatusCode.NotFound, "NotFound", notFound.Message),
            BadRequestException badRequest => (HttpStatusCode.BadRequest, "BadRequest", badRequest.Message),
            ValidationException validation => (HttpStatusCode.BadRequest, "ValidationError", validation.Message),
            ConflictException conflict => (HttpStatusCode.Conflict, "Conflict", conflict.Message),
            ForbiddenException => (HttpStatusCode.Forbidden, "Forbidden", "You don't have permission to perform this action"),
            ArgumentNullException argNull => (HttpStatusCode.BadRequest, "BadRequest", $"Required field: {argNull.ParamName}"),
            ArgumentException arg => (HttpStatusCode.BadRequest, "BadRequest", arg.Message),
            _ => (HttpStatusCode.InternalServerError, "InternalServerError", "An error occurred on the server")
        };

        // Log the exception
        LogException(exception, context, statusCode);

        // Configure response
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        // Create ProblemDetails
        var problemDetails = CreateProblemDetails(
            context,
            statusCode,
            code,
            message,
            traceId,
            exception
        );

        // Write JSON response
        var json = JsonSerializer.Serialize(problemDetails, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = _environment.IsDevelopment()
        });

        await context.Response.WriteAsync(json);
    }

    private ApiProblemDetails CreateProblemDetails(
        HttpContext context,
        HttpStatusCode statusCode,
        string code,
        string message,
        string traceId,
        Exception exception)
    {
        var problemDetails = new ApiProblemDetails
        {
            Type = $"https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/{(int)statusCode}",
            Title = code,
            Status = (int)statusCode,
            Detail = message,
            Instance = context.Request.Path,
            Extensions = new Dictionary<string, object>
            {
                ["timestamp"] = DateTime.UtcNow,
                ["traceId"] = traceId
            }
        };

        if (_environment.IsDevelopment())
        {
            problemDetails.Extensions["exceptionType"] = exception.GetType().Name;
            problemDetails.Extensions["stackTrace"] = exception.StackTrace ?? string.Empty;

            if (exception.InnerException != null)
            {
                problemDetails.Extensions["innerException"] = new
                {
                    type = exception.InnerException.GetType().Name,
                    message = exception.InnerException.Message
                };
            }
        }

        // Add validation errors for ValidationException
        if (exception is ValidationException validationException &&
            validationException.Errors != null)
        {
            problemDetails.Extensions["errors"] = validationException.Errors;
        }

        return problemDetails;
    }

    private void LogException(Exception exception, HttpContext context, HttpStatusCode statusCode)
    {
        var logMessage = "Request {Method} {Path} failed with {StatusCode}";
        var logArgs = new object[]
        {
            context.Request.Method,
            context.Request.Path,
            (int)statusCode
        };

        if (statusCode >= HttpStatusCode.InternalServerError)
        {
            Log.Error(exception, logMessage, logArgs);
        }
        else if (statusCode >= HttpStatusCode.BadRequest)
        {
            Log.Warning(exception, logMessage, logArgs);
        }
        else
        {
            Log.Information(exception, logMessage, logArgs);
        }
    }

    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }
        public NotFoundException(string entityName, object key)
            : base($"{entityName} with key ({key}) was not found") { }
    }

    public class BadRequestException : Exception
    {
        public BadRequestException(string message) : base(message) { }
    }

    public class ValidationException : Exception
    {
        public Dictionary<string, string[]>? Errors { get; }

        public ValidationException(string message) : base(message) { }

        public ValidationException(Dictionary<string, string[]> errors)
            : base("One or more validation errors occurred")
        {
            Errors = errors;
        }
    }

    public class ConflictException : Exception
    {
        public ConflictException(string message) : base(message) { }
    }

    public class ForbiddenException : Exception
    {
        public ForbiddenException(string message = "Access forbidden") : base(message) { }
    }
}