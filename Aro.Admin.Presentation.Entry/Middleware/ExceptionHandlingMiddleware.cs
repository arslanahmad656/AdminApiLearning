namespace Aro.Admin.Presentation.Entry.Middleware;

using Aro.Admin.Application.Services;
using Aro.Admin.Domain.Shared.Exceptions;
using System.Net;
using System.Text.Json;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogManager logger, ErrorCodes errorCodes)
{
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
                AroUnauthorizedException => HttpStatusCode.Unauthorized,
                AroUserPermissionException => HttpStatusCode.Forbidden,
                AroNotFoundException => HttpStatusCode.NotFound,
                _ => HttpStatusCode.InternalServerError,
            });

            var errorCode = ex is AroException aroEx ? aroEx.ErrorCode
                : ex is OperationCanceledException opEx ? errorCodes.OPERATION_CANCELLED_ERROR
                : errorCodes.UNKNOWN_ERROR;
            var response = new Error(errorCode, ex.Message);

            var json = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(json);
        }
    }
}

file record Error(string ErrorCode, string ErrorMessage);