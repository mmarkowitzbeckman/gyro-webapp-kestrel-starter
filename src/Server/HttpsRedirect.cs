using Microsoft.AspNetCore.Http.Extensions;

namespace Gyro;

public static class HttpsRedirect
{
    public static void UseHttpsRedirect(this IApplicationBuilder app, int httpsPort)
    {
        app.Use(async (context, next) =>
        {
            if (context.Request.IsHttps)
            {
                await next(context);
                return;
            }

            var host = context.Request.Host.HasValue ? context.Request.Host.Host : "localhost";
            context.Response.StatusCode = StatusCodes.Status308PermanentRedirect;
            context.Response.Headers.Location = $"https://{host}:{httpsPort}{context.Request.GetEncodedPathAndQuery()}";
        });
    }
}
