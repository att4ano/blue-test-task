namespace Presentation.Middlewares;

public class ExceptionFormatMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var statusCode = exception is ArgumentException
            ? StatusCodes.Status400BadRequest
            : StatusCodes.Status500InternalServerError;

        context.Response.StatusCode = statusCode;

        var errorResponse = CreateErrorResponse(statusCode, exception);

        return context.Response.WriteAsJsonAsync(errorResponse);
    }

    private static object CreateErrorResponse(int statusCode, Exception exception)
    {
        return new
        {
            Status = statusCode,
            Error = $"Exception: {exception.GetType().Name}, Message: {exception.Message}"
        };
    }
}