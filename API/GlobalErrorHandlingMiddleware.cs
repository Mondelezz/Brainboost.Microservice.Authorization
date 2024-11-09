using Application.Common.Exceptions;
using System.Net;
using System.Text.Json;

namespace API;

public class GlobalErrorHandlingMiddleware(RequestDelegate next, ILogger<GlobalErrorHandlingMiddleware> logger)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (EntityNotFoundException ex)
        {
            await HandleExceptionAsync(HttpStatusCode.InternalServerError, context, ex);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(HttpStatusCode.InternalServerError, context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpStatusCode status, HttpContext context, Exception ex)
    {
        logger.LogError("Exception occupied during executing request: {Path}\n{Ex}", context.Request.Path, ex);

        Exception? exception = ex;
        List<string> exeptionMessageList = [];

        while (exception is not null)
        {
            exeptionMessageList.Add(exception.Message);
            exception = exception.InnerException;
        }

        string exceptionResult = JsonSerializer.Serialize(new
        {
            path = new { context.Request.Path.Value, context.Request.Method },
            errorlist = exeptionMessageList
        });

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)status;

        return context.Response.WriteAsync(exceptionResult);
    }
}
