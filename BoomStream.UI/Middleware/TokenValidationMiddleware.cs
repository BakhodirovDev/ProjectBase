using Application.Extensions;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BoomStream.UI.Middleware;

public class TokenValidationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<TokenValidationMiddleware> _logger;

    public TokenValidationMiddleware(RequestDelegate next, IServiceScopeFactory serviceScopeFactory, 
                                     IHttpContextAccessor httpContextAccessor, ILogger<TokenValidationMiddleware> logger)
    {
        _next = next;
        _serviceScopeFactory = serviceScopeFactory;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        // Bypass token validation for the refresh token endpoint
        if (httpContext.Request.Path.StartsWithSegments("/api/v1/home/auth/token_refresh", StringComparison.OrdinalIgnoreCase))
        {
            await _next(httpContext);
            return;
        }

        _logger.LogInformation($"IP Addresss By HttpContextExtensions: {HttpContextExtensions.GetClientIpAddress(httpContext)}, TokenValidationMiddleware");
        Log.Information("IP Addresss By HttpContextExtensions: {ClientIpAddress}, TokenValidationMiddleware", HttpContextExtensions.GetClientIpAddress(httpContext));

        var token = httpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "").Trim();
        var userAgent = httpContext.Request.Headers["User-Agent"].ToString();

        using (var scope = _serviceScopeFactory.CreateScope())
        {
            // Log request details
            var request = httpContext.Request;

            // Enable buffering to read the request body multiple times
            httpContext.Request.EnableBuffering();
            using (var reader = new StreamReader(httpContext.Request.Body, Encoding.UTF8, leaveOpen: true))
            {
                var body = await reader.ReadToEndAsync();
                Log.Information("Request Method: {Method}, Path: {Path}, QueryString: {QueryString}, Headers: {Headers}, Request Body: {Body}",
                request.Method, request.Path, request.QueryString, string.Join(", ", request.Headers.Select(h => $"{h.Key}: {h.Value}")), body);

                httpContext.Request.Body.Position = 0; // Reset the request body position
            }
        }

        if (!string.IsNullOrEmpty(token))
        {
            var handler = new JwtSecurityTokenHandler();
            if (!handler.CanReadToken(token))
            {
                Log.Warning("Tokenni o'qib bo'lmadi. Token: {Token}, Qurilma: {UserAgent} IpManzil: {IP}", token, userAgent, httpContext.GetClientIpAddress());
                
                /*using (var scope = _serviceScopeFactory.CreateScope())
                {
                    //var loggerService = scope.ServiceProvider.GetRequiredService<ILoggerService>();
                    //Log.Warning("Tokenni o'qib bo'lmadi. Token: {Token}, Qurilma: {UserAgent} IpManzil: {IP}", token, userAgent, httpContext.GetClientIpAddress());
                    //await loggerService.LogToTelegram($"⚠️WARNING: Tokenni o'qib bo'lmadi. \n🔑Token: {token}, \n📱Qurilma: {userAgent}\n🌐IpManzil: {httpContext.GetClientIpAddress()} ");
                }*/

                httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await httpContext.Response.WriteAsJsonAsync(new { Unauthorized = "Token yaroqsiz" });
                return;
            }

            var jwtToken = handler.ReadToken(token) as JwtSecurityToken;
            if (jwtToken != null)
            {
                var userIdClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier);
                var userId = userIdClaim?.Value;
                var tokenIpAddressClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "IpAddress");
                var tokenIpAddress = tokenIpAddressClaim?.Value;

                var currentIpAddress = _httpContextAccessor.HttpContext?.GetClientIpAddress();

                Log.Information($"IP Addresss By HttpContextExtensions: {HttpContextExtensions.GetClientIpAddress(httpContext)}");

                Console.WriteLine(HttpContextExtensions.GetClientIpAddress(httpContext));

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    //var loggerService = scope.ServiceProvider.GetRequiredService<ILoggerService>();
                    Log.Information("Token tekshirilmoqda. UserId: {UserId}, IP: {IpAddress}, Qurilma: {UserAgent}", userId, currentIpAddress, userAgent);
                    //await loggerService.LogToTelegram($"ℹ️INFO: Token tekshirilmoqda. \n🆔UserId: {userId}, \n🌐IP: {currentIpAddress}, \n📱Qurilma: {userAgent}");

                    //var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

                    /*if (!(await userRepository.IsTokenValidAsync(Guid.Parse(userId), token) && Convert.ToString(tokenIpAddress) == Convert.ToString(currentIpAddress)))
                    {
                        Log.Warning("Token yaroqsiz yoki IP mos kelmaydi. UserId: {UserId}, IP: {IpAddress}, Qurilma: {UserAgent}", userId, currentIpAddress, userAgent);
                        //await loggerService.LogToTelegram($"⚠️WARNING: Token yaroqsiz yoki IP mos kelmaydi. \n🆔UserId: {userId}, \n🌐IP: {currentIpAddress}, \n📱Qurilma: {userAgent}");

                        httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await httpContext.Response.WriteAsJsonAsync(new { Unauthorized = "Token yaroqsiz" });
                        return;
                    }*/

                    /*var userInfo = await userRepository.GetByIdAsync(Guid.Parse(userId));

                    if (!userInfo.Value.EmailConfirmAt)
                    {
                        // Bir nechta pathlarni tekshirish
                        if (httpContext.Request.Path.StartsWithSegments("/api/v1/home/auth/token_refresh", StringComparison.OrdinalIgnoreCase) ||
                            httpContext.Request.Path.StartsWithSegments("/api/v1/home/auth/logout", StringComparison.OrdinalIgnoreCase) ||
                            httpContext.Request.Path.StartsWithSegments("/api/v1/home/auth/get_user_role", StringComparison.OrdinalIgnoreCase) ||
                            httpContext.Request.Path.StartsWithSegments("/api/v1/home/email/verification_code_send", StringComparison.OrdinalIgnoreCase) ||
                            httpContext.Request.Path.StartsWithSegments("/api/v1/home/email/email_verify", StringComparison.OrdinalIgnoreCase) ||
                            httpContext.Request.Path.StartsWithSegments("/api/v1/home/user/get_info", StringComparison.OrdinalIgnoreCase))
                        {
                            await _next(httpContext);
                            return;
                        }
                        else
                        {
                            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

                            var response = new
                            {
                                isSuccess = false,
                                successMessage = string.Empty,
                                isFailure = true,
                                error = new
                                {
                                    code = "400",
                                    message = "Email has not yet been confirmed"
                                }
                            };

                            await httpContext.Response.WriteAsJsonAsync(response);
                            return;
                        }
                    }*/

                }
            }
            else
            {
                Log.Error("Tokenni dekod qilishda xatolik yuz berdi. Token: {Token}, Qurilma: {UserAgent}", token, userAgent);

                /*using (var scope = _serviceScopeFactory.CreateScope())
                {
                    //var loggerService = scope.ServiceProvider.GetRequiredService<ILoggerService>();
                    Log.Error("Tokenni dekod qilishda xatolik yuz berdi. Token: {Token}, Qurilma: {UserAgent}", token, userAgent);
                    //await loggerService.LogToTelegram($"❌ERROR: Tokenni dekod qilishda xatolik yuz berdi. \n🔑Token: {token}, \n📱Qurilma : {userAgent}");
                }*/

                httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await httpContext.Response.WriteAsJsonAsync(new { Unauthorized = "Tokenni dekod qilishda xatolik." });
                return;
            }
        }

        await _next(httpContext);
    }
}
