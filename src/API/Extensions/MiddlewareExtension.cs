using API.Middlewares;

namespace API.Extensions;

public static class MiddlewareExtension
{
    public static IApplicationBuilder UseInvalidTokenHandleMiddleware(this IApplicationBuilder builder)
        => builder.UseMiddleware<InvalidTokenHandleMiddleware>();

    public static IApplicationBuilder UseExceptionHandleMiddleware(this IApplicationBuilder builder)
        => builder.UseMiddleware<ExceptionHandleMiddleware>();
}
