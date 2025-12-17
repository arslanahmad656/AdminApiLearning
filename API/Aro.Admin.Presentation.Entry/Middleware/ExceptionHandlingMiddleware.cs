namespace Aro.Admin.Presentation.Entry.Middleware;

using Aro.Common.Application.Services.LogManager;
using Aro.Common.Domain.Shared;
using Aro.Common.Domain.Shared.Exceptions;
using System.Net;
using System.Text.Json;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogManager logger, ErrorCodes errorCodes)
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unhandled exception occurred while processing the request at path {path}.", context.Request.Path);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)(ex switch
            {
                AroAccountLockedException => HttpStatusCode.TooManyRequests, // 429
                AroUnauthorizedException => HttpStatusCode.Unauthorized,
                AroUserPermissionException => HttpStatusCode.Forbidden,
                AroNotFoundException => HttpStatusCode.NotFound,
                _ => HttpStatusCode.InternalServerError,
            });

            var errorCode = ex is AroException aroEx ? aroEx.ErrorCode
                : ex is OperationCanceledException ? errorCodes.OPERATION_CANCELLED_ERROR
                : errorCodes.UNKNOWN_ERROR;

            string json;
            if (ex is AroAccountLockedException lockedException)
            {
                var lockoutResponse = new LockoutError(errorCode, ex.Message, lockedException.LockoutEnd);
                json = JsonSerializer.Serialize(lockoutResponse, JsonOptions);
            }
            else
            {
                var response = new Error(errorCode, ex.Message);
                json = JsonSerializer.Serialize(response, JsonOptions);
            }

            await context.Response.WriteAsync(json);
        }
    }
}

file record Error(string ErrorCode, string ErrorMessage);
file record LockoutError(string ErrorCode, string ErrorMessage, DateTime LockoutEnd);