using Domain.Abstraction.Errors;
using Domain.Abstraction.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net;

namespace ProjectBase.WebApi.Extensions;

/// <summary>
/// Result pattern uchun ActionResult extension methodlar
/// </summary>
public static class ResultExtensions
{
    #region Basic ToActionResult

    /// <summary>
    /// Result ni ActionResult ga aylantiradi (Error.StatusCode asosida)
    /// </summary>
    public static ActionResult ToActionResult(this Result result)
    {
        if (result.IsSuccess)
            return new OkObjectResult(result);

        return CreateErrorResult(result.Error!, result);
    }

    /// <summary>
    /// Result<T> ni ActionResult ga aylantiradi (Error.StatusCode asosida)
    /// </summary>
    public static ActionResult<T> ToActionResult<T>(this Result<T> result)
    {
        if (result.IsSuccess)
            return new OkObjectResult(result);

        return CreateErrorResult(result.Error!, result);
    }

    #endregion

    #region Custom Status Code

    /// <summary>
    /// Result ni ActionResult ga aylantiradi (custom success status code bilan)
    /// </summary>
    public static ActionResult ToActionResult(this Result result, HttpStatusCode successStatusCode)
    {
        if (result.IsSuccess)
            return new ObjectResult(result) { StatusCode = (int)successStatusCode };

        return result.ToActionResult();
    }

    /// <summary>
    /// Result<T> ni ActionResult ga aylantiradi (custom success status code bilan)
    /// </summary>
    public static ActionResult<T> ToActionResult<T>(this Result<T> result, HttpStatusCode successStatusCode)
    {
        if (result.IsSuccess)
            return new ObjectResult(result) { StatusCode = (int)successStatusCode };

        return result.ToActionResult();
    }

    /// <summary>
    /// Result ni ActionResult ga aylantiradi (int status code bilan)
    /// </summary>
    public static ActionResult ToActionResult(this Result result, int successStatusCode)
    {
        return result.ToActionResult((HttpStatusCode)successStatusCode);
    }

    /// <summary>
    /// Result<T> ni ActionResult ga aylantiradi (int status code bilan)
    /// </summary>
    public static ActionResult<T> ToActionResult<T>(this Result<T> result, int successStatusCode)
    {
        return result.ToActionResult((HttpStatusCode)successStatusCode);
    }

    #endregion

    #region Specific HTTP Status Codes

    /// <summary>
    /// Created (201) response qaytaradi
    /// </summary>
    public static ActionResult<T> ToCreatedResult<T>(this Result<T> result, string? location = null)
    {
        if (result.IsSuccess)
        {
            if (!string.IsNullOrEmpty(location))
                return new CreatedResult(location, result);

            return new ObjectResult(result) { StatusCode = StatusCodes.Status201Created };
        }

        return result.ToActionResult();
    }

    /// <summary>
    /// Created (201) response qaytaradi - action name bilan
    /// </summary>
    public static ActionResult<T> ToCreatedAtActionResult<T>(
        this Result<T> result,
        string actionName,
        string? controllerName = null,
        object? routeValues = null)
    {
        if (result.IsSuccess)
        {
            return new CreatedAtActionResult(actionName, controllerName, routeValues, result);
        }

        return result.ToActionResult();
    }

    /// <summary>
    /// Created (201) response qaytaradi - route name bilan
    /// </summary>
    public static ActionResult<T> ToCreatedAtRouteResult<T>(
        this Result<T> result,
        string routeName,
        object? routeValues = null)
    {
        if (result.IsSuccess)
        {
            return new CreatedAtRouteResult(routeName, routeValues, result);
        }

        return result.ToActionResult();
    }

    /// <summary>
    /// Accepted (202) response qaytaradi
    /// </summary>
    public static ActionResult<T> ToAcceptedResult<T>(this Result<T> result, string? location = null)
    {
        if (result.IsSuccess)
        {
            if (!string.IsNullOrEmpty(location))
                return new AcceptedResult(location, result);

            return new ObjectResult(result) { StatusCode = StatusCodes.Status202Accepted };
        }

        return result.ToActionResult();
    }

    /// <summary>
    /// NoContent (204) response qaytaradi (muvaffaqiyatli bo'lsa)
    /// </summary>
    public static ActionResult ToNoContentResult(this Result result)
    {
        if (result.IsSuccess)
            return new NoContentResult();

        return result.ToActionResult();
    }

    /// <summary>
    /// NoContent (204) response qaytaradi - Result<T> uchun
    /// </summary>
    public static ActionResult ToNoContentResult<T>(this Result<T> result)
    {
        if (result.IsSuccess)
            return new NoContentResult();

        return CreateErrorResult(result.Error!, result);
    }

    #endregion

    #region Match Pattern

    /// <summary>
    /// Result ni match pattern bilan ActionResult ga aylantiradi
    /// </summary>
    public static ActionResult Match(
        this Result result,
        Func<ActionResult> onSuccess,
        Func<Error, ActionResult>? onFailure = null)
    {
        if (result.IsSuccess)
            return onSuccess();

        if (onFailure != null)
            return onFailure(result.Error!);

        return result.ToActionResult();
    }

    /// <summary>
    /// Result<T> ni match pattern bilan ActionResult ga aylantiradi
    /// </summary>
    public static ActionResult<TResult> Match<T, TResult>(
        this Result<T> result,
        Func<T, ActionResult<TResult>> onSuccess,
        Func<Error, ActionResult<TResult>>? onFailure = null)
    {
        if (result.IsSuccess)
            return onSuccess(result.Value);

        if (onFailure != null)
            return onFailure(result.Error!);

        return CreateErrorResult(result.Error!, result);
    }

    /// <summary>
    /// Result<T> ni match pattern bilan ActionResult ga aylantiradi (T qaytaradi)
    /// </summary>
    public static ActionResult<T> Match<T>(
        this Result<T> result,
        Func<T, ActionResult<T>> onSuccess,
        Func<Error, ActionResult<T>>? onFailure = null)
    {
        return result.Match<T, T>(onSuccess, onFailure);
    }

    #endregion

    #region Async Extensions

    /// <summary>
    /// Async Result ni ActionResult ga aylantiradi
    /// </summary>
    public static async Task<ActionResult> ToActionResultAsync(this Task<Result> resultTask)
    {
        var result = await resultTask;
        return result.ToActionResult();
    }

    /// <summary>
    /// Async Result<T> ni ActionResult ga aylantiradi
    /// </summary>
    public static async Task<ActionResult<T>> ToActionResultAsync<T>(this Task<Result<T>> resultTask)
    {
        var result = await resultTask;
        return result.ToActionResult();
    }

    /// <summary>
    /// Async Result ni NoContent ga aylantiradi
    /// </summary>
    public static async Task<ActionResult> ToNoContentResultAsync(this Task<Result> resultTask)
    {
        var result = await resultTask;
        return result.ToNoContentResult();
    }

    /// <summary>
    /// Async Result<T> ni Created ga aylantiradi
    /// </summary>
    public static async Task<ActionResult<T>> ToCreatedResultAsync<T>(
        this Task<Result<T>> resultTask,
        string? location = null)
    {
        var result = await resultTask;
        return result.ToCreatedResult(location);
    }

    #endregion

    #region Safe Execution (Exception Handling)

    /// <summary>
    /// Func ni xavfsiz bajaradi va Result qaytaradi (exception ni Error ga aylantiradi)
    /// </summary>
    public static async Task<ActionResult> ExecuteSafeAsync(
        Func<Task<Result>> action,
        ILogger? logger = null)
    {
        try
        {
            var result = await action();
            return result.ToActionResult();
        }
        catch (Exception ex)
        {
            return HandleException(ex, logger);
        }
    }

    /// <summary>
    /// Func<T> ni xavfsiz bajaradi va Result<T> qaytaradi (exception ni Error ga aylantiradi)
    /// </summary>
    public static async Task<ActionResult<T>> ExecuteSafeAsync<T>(
        Func<Task<Result<T>>> action,
        ILogger? logger = null)
    {
        try
        {
            var result = await action();
            return result.ToActionResult();
        }
        catch (Exception ex)
        {
            return HandleException<T>(ex, logger);
        }
    }

    /// <summary>
    /// Sync Func ni xavfsiz bajaradi
    /// </summary>
    public static ActionResult ExecuteSafe(
        Func<Result> action,
        ILogger? logger = null)
    {
        try
        {
            var result = action();
            return result.ToActionResult();
        }
        catch (Exception ex)
        {
            return HandleException(ex, logger);
        }
    }

    /// <summary>
    /// Sync Func<T> ni xavfsiz bajaradi
    /// </summary>
    public static ActionResult<T> ExecuteSafe<T>(
        Func<Result<T>> action,
        ILogger? logger = null)
    {
        try
        {
            var result = action();
            return result.ToActionResult();
        }
        catch (Exception ex)
        {
            return HandleException<T>(ex, logger);
        }
    }

    #endregion

    #region Safe Async Extensions with Try-Catch

    /// <summary>
    /// Task Result ni xavfsiz ActionResult ga aylantiradi (exception handling bilan)
    /// </summary>
    public static async Task<ActionResult> ToActionResultSafeAsync(
        this Task<Result> resultTask,
        ILogger? logger = null)
    {
        try
        {
            var result = await resultTask;
            return result.ToActionResult();
        }
        catch (Exception ex)
        {
            return HandleException(ex, logger);
        }
    }

    /// <summary>
    /// Task Result<T> ni xavfsiz ActionResult ga aylantiradi (exception handling bilan)
    /// </summary>
    public static async Task<ActionResult<T>> ToActionResultSafeAsync<T>(
        this Task<Result<T>> resultTask,
        ILogger? logger = null)
    {
        try
        {
            var result = await resultTask;
            return result.ToActionResult();
        }
        catch (Exception ex)
        {
            return HandleException<T>(ex, logger);
        }
    }

    /// <summary>
    /// Task Result<T> ni xavfsiz Created ga aylantiradi (exception handling bilan)
    /// </summary>
    public static async Task<ActionResult<T>> ToCreatedResultSafeAsync<T>(
        this Task<Result<T>> resultTask,
        ILogger? logger = null,
        string? location = null)
    {
        try
        {
            var result = await resultTask;
            return result.ToCreatedResult(location);
        }
        catch (Exception ex)
        {
            return HandleException<T>(ex, logger);
        }
    }

    /// <summary>
    /// Task Result ni xavfsiz NoContent ga aylantiradi (exception handling bilan)
    /// </summary>
    public static async Task<ActionResult> ToNoContentResultSafeAsync(
        this Task<Result> resultTask,
        ILogger? logger = null)
    {
        try
        {
            var result = await resultTask;
            return result.ToNoContentResult();
        }
        catch (Exception ex)
        {
            return HandleException(ex, logger);
        }
    }

    #endregion

    #region Private Helpers

    /// <summary>
    /// Error asosida mos ActionResult yaratadi
    /// </summary>
    private static ActionResult CreateErrorResult(Error error, object resultObject)
    {
        var statusCode = (int)error.StatusCode;

        return error.StatusCode switch
        {
            HttpStatusCode.NotFound => new NotFoundObjectResult(resultObject),
            HttpStatusCode.Unauthorized => new UnauthorizedObjectResult(resultObject),
            HttpStatusCode.Forbidden => new ObjectResult(resultObject) { StatusCode = StatusCodes.Status403Forbidden },
            HttpStatusCode.BadRequest => new BadRequestObjectResult(resultObject),
            HttpStatusCode.Conflict => new ConflictObjectResult(resultObject),
            HttpStatusCode.UnprocessableEntity => new UnprocessableEntityObjectResult(resultObject),
            HttpStatusCode.TooManyRequests => new ObjectResult(resultObject) { StatusCode = StatusCodes.Status429TooManyRequests },
            HttpStatusCode.ServiceUnavailable => new ObjectResult(resultObject) { StatusCode = StatusCodes.Status503ServiceUnavailable },
            HttpStatusCode.GatewayTimeout => new ObjectResult(resultObject) { StatusCode = StatusCodes.Status504GatewayTimeout },
            HttpStatusCode.InternalServerError => new ObjectResult(resultObject) { StatusCode = StatusCodes.Status500InternalServerError },
            _ => new ObjectResult(resultObject) { StatusCode = statusCode }
        };
    }

    /// <summary>
    /// Exception uchun xavfsiz error response yaratadi
    /// - To'liq exception logga yoziladi
    /// - Foydalanuvchiga faqat qisqa xabar qaytariladi
    /// </summary>
    private static ActionResult HandleException(Exception ex, ILogger? logger)
    {
        // Unique error ID yaratish (logda topish uchun)
        var errorId = Guid.NewGuid().ToString("N")[..8].ToUpper();

        // To'liq exception ni logga yozish
        logger?.LogError(ex,
            "Unhandled exception occurred. ErrorId: {ErrorId}, Type: {ExceptionType}, Message: {Message}, StackTrace: {StackTrace}",
            errorId,
            ex.GetType().FullName,
            ex.Message,
            ex.StackTrace);

        // Foydalanuvchiga faqat qisqa xabar qaytarish
        var safeError = Error.Custom(
            "INTERNAL_ERROR",
            $"Xatolik yuz berdi. Iltimos, keyinroq qayta urinib ko'ring. (Ref: {errorId})",
            HttpStatusCode.InternalServerError);

        var errorResult = Result.Failure(safeError);
        return new ObjectResult(errorResult) { StatusCode = StatusCodes.Status500InternalServerError };
    }

    /// <summary>
    /// Generic versiya - Result<T> uchun
    /// </summary>
    private static ActionResult HandleException<T>(Exception ex, ILogger? logger)
    {
        var errorId = Guid.NewGuid().ToString("N")[..8].ToUpper();

        logger?.LogError(ex,
            "Unhandled exception occurred. ErrorId: {ErrorId}, Type: {ExceptionType}, Message: {Message}, StackTrace: {StackTrace}",
            errorId,
            ex.GetType().FullName,
            ex.Message,
            ex.StackTrace);

        var safeError = Error.Custom(
            "INTERNAL_ERROR",
            $"Xatolik yuz berdi. Iltimos, keyinroq qayta urinib ko'ring. (Ref: {errorId})",
            HttpStatusCode.InternalServerError);

        var errorResult = Result<T>.Failure(safeError);
        return new ObjectResult(errorResult) { StatusCode = StatusCodes.Status500InternalServerError };
    }

    #endregion
}
