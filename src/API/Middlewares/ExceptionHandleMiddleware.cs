using API.Extensions;

namespace API.Middlewares;

public class ExceptionHandleMiddleware
{
    private readonly RequestDelegate next;
    private readonly ILogger<ExceptionHandleMiddleware> logger;

    public ExceptionHandleMiddleware(RequestDelegate next, ILogger<ExceptionHandleMiddleware> logger)
    {
        this.next = next;
        this.logger = logger;
    }

    public async Task Invoke(HttpContext httpContext)
    {
        try
        {
            await next(httpContext);
        }
        catch (Exception ex)
        {
            await ProcessExceptionAsync(httpContext, ex);
        }
    }

    private async Task ProcessExceptionAsync(HttpContext context, Exception exception)
    {
        logger.Error($"Api unexpected error occurred! message:{exception.Message}");

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        await context.Response.CompleteAsync();
    }
}
