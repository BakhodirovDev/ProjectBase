using Application.Extensions;
using Application.Service;

namespace ProjectBase.WebApi.Middleware;

public class TokenValidationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TokenValidationMiddleware> _logger;

    private static readonly HashSet<string> _bypassPaths = new(StringComparer.OrdinalIgnoreCase)
    {
        "/swagger",
        "/health",
        "/Auth/SignIn",
    };

    public TokenValidationMiddleware(RequestDelegate next, ILogger<TokenValidationMiddleware> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task InvokeAsync(HttpContext context, IServiceProvider serviceProvider)
    {
        var path = context.Request.Path;

        if (ShouldBypassValidation(path))
        {
            await _next(context);
            return;
        }

        var authHeader = context.Request.Headers.Authorization.ToString();
        if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        var token = authHeader[7..].Trim();

        using var scope = serviceProvider.CreateScope();
        var jwtTokenService = scope.ServiceProvider.GetRequiredService<JwtTokenService>();

        try
        {
            var isAuthenticated = await jwtTokenService.IsAuthenticatedAsync();

            if (!isAuthenticated)
            {
                _logger.LogWarning("Unauthorized access attempt: Invalid or Revoked token. IP: {Ip}", context.GetClientIpAddress());
                await WriteUnauthorizedResponse(context, "Token is invalid, expired or revoked.");
                return;
            }

            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during token validation.");
            await WriteUnauthorizedResponse(context, "An internal error occurred during authentication.");
        }
    }

    private static bool ShouldBypassValidation(PathString path)
    {
        return _bypassPaths.Any(p => path.StartsWithSegments(p, StringComparison.OrdinalIgnoreCase));
    }

    private static async Task WriteUnauthorizedResponse(HttpContext context, string message)
    {
        if (context.Response.HasStarted) return;

        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsJsonAsync(new
        {
            title = "Unauthorized",
            status = 401,
            detail = message,
            traceId = context.TraceIdentifier,
            timestamp = DateTime.UtcNow
        });
    }
}