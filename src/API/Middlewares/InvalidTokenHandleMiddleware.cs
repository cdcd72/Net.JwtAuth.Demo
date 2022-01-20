using API.Common;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Caching.Memory;

namespace API.Middlewares;

public class InvalidTokenHandleMiddleware
{
    private readonly RequestDelegate next;
    private readonly IMemoryCache cache;

    public InvalidTokenHandleMiddleware(RequestDelegate next, IMemoryCache cache)
    {
        this.next = next;
        this.cache = cache;
    }

    public async Task Invoke(HttpContext httpContext)
    {
        cache.TryGetValue(CacheKeys.TokenBlackList, out List<string> tokenBlackList);

        if (tokenBlackList is not null)
        {
            var accessToken =
                await httpContext.GetTokenAsync(JwtBearerDefaults.AuthenticationScheme, "access_token");

            if (accessToken is not null)
            {
                if (tokenBlackList.Any(token => token == accessToken))
                {
                    httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;

                    await httpContext.Response.CompleteAsync();
                }
                else
                {
                    await next(httpContext);
                }
            }
            else
            {
                await next(httpContext);
            }
        }
        else
        {
            await next(httpContext);
        }
    }
}
