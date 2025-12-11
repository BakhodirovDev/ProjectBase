using Application.Extensions;
using Microsoft.Extensions.Primitives;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ProjectBase.WebApi.Middleware;

public class TokenValidationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TokenValidationMiddleware> _logger;

    private static readonly HashSet<string> _bypassPaths = new(StringComparer.OrdinalIgnoreCase)
    {
        "/swagger",
        "/health"
    };

    public TokenValidationMiddleware(
        RequestDelegate next,
        ILogger<TokenValidationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (ShouldBypassValidation(context.Request.Path))
        {
            await _next(context);
            return;
        }

        var token = ExtractToken(context.Request.Headers.Authorization);

        if (string.IsNullOrEmpty(token))
        {
            await _next(context);
            return;
        }

        var validationResult = ValidateToken(token);

        if (!validationResult.IsValid)
        {
            var clientIp = context.GetClientIpAddress();
            var userAgent = context.Request.Headers.UserAgent.ToString();

            _logger.LogWarning(
                "Invalid token. Reason: {Reason}, IP: {IpAddress}, UserAgent: {UserAgent}",
                validationResult.ErrorMessage, clientIp, userAgent);

            await WriteUnauthorizedResponse(context, validationResult.ErrorMessage);
            return;
        }

        _logger.LogDebug("Token validated successfully. UserId: {UserId}", validationResult.UserId);

        await _next(context);
    }

    private static bool ShouldBypassValidation(PathString path)
    {
        return _bypassPaths.Any(bypassPath =>
            path.StartsWithSegments(bypassPath, StringComparison.OrdinalIgnoreCase));
    }

    private static string? ExtractToken(StringValues authorizationHeader)
    {
        var headerValue = authorizationHeader.ToString();

        if (string.IsNullOrWhiteSpace(headerValue))
            return null;

        return headerValue.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
            ? headerValue[7..].Trim()
            : headerValue.Trim();
    }

    private static TokenValidationResult ValidateToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();

        if (!handler.CanReadToken(token))
        {
            return TokenValidationResult.Failed("Invalid token format");
        }

        JwtSecurityToken jwtToken;
        try
        {
            jwtToken = handler.ReadJwtToken(token);
        }
        catch (Exception ex)
        {
            return TokenValidationResult.Failed($"Token parsing error: {ex.Message}");
        }

        var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return TokenValidationResult.Failed("Token missing required claims");
        }

        return TokenValidationResult.Success(userId);
    }

    private static async Task WriteUnauthorizedResponse(HttpContext context, string? message)
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        context.Response.ContentType = "application/json";

        var response = new
        {
            type = "https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/401",
            title = "Unauthorized",
            status = 401,
            detail = message ?? "Invalid token",
            traceId = context.TraceIdentifier,
            timestamp = DateTime.UtcNow
        };

        await context.Response.WriteAsJsonAsync(response);
    }

    private class TokenValidationResult
    {
        public bool IsValid { get; }
        public string? UserId { get; }
        public string? ErrorMessage { get; }

        private TokenValidationResult(bool isValid, string? userId, string? errorMessage)
        {
            IsValid = isValid;
            UserId = userId;
            ErrorMessage = errorMessage;
        }

        public static TokenValidationResult Success(string userId)
            => new(true, userId, null);

        public static TokenValidationResult Failed(string errorMessage)
            => new(false, null, errorMessage);
    }
}