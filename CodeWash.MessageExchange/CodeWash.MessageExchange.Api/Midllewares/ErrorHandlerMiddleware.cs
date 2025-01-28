namespace CodeWash.MessageExchange.Api.Midllewares;

public class ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("Request was canceled by the client.");
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new
            {
                message = "The request was canceled by the client."
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception occurred.");
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new
            {
                message = "An unexpected error occurred. Please try again later."
            });
        }
    }
}