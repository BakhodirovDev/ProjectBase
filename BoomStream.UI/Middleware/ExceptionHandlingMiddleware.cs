using System.Net;
using System.Text.Json;
using Serilog;

namespace BoomStream.UI.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (UnauthorizedAccessException ex)
            {
                await HandleExceptionAsync(httpContext, ex, HttpStatusCode.Unauthorized, ex.Message);
            }
            catch (NotFoundException ex)
            {
                await HandleExceptionAsync(httpContext, ex, HttpStatusCode.NotFound, ex.Message);
            }
            catch (ArgumentNullException ex)
            {
                await HandleExceptionAsync(httpContext, ex, HttpStatusCode.BadRequest, ex.Message);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex, HttpStatusCode.InternalServerError, "Ichki tizimda xatolik yuz berdi.");
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception, HttpStatusCode statusCode, string message)
        {
            // Xatolikni loglash
            Log.Error(exception, "Xatolik yuz berdi: {Path}", context.Request.Path);

            // Javobni sozlash
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            // Javob JSON formatida bo'ladi
            var response = new
            {
                status = context.Response.StatusCode,
                error = message,
                path = context.Request.Path,
                timestamp = DateTime.UtcNow
            };

            // JSON javobni yozish
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }

    // Maxsus xatoliklar uchun misol
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }
    }
}
