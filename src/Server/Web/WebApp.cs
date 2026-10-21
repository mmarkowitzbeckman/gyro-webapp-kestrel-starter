using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using Yarp.ReverseProxy.Forwarder;

namespace Gyro.Web;

public static class WebApp
{
    // Vite build output. Run `npm run client:build` to generate it.
    private static readonly string ClientDist = Path.Combine(
        Directory.GetCurrentDirectory(),
        "src",
        "web",
        "client",
        "dist"
    );

    public static void AddServices(IServiceCollection services) => services.AddHttpForwarder();

    public static void Configure(WebApplication app, int apiHttpsPort)
    {
        // The client is built with Vite `base: "/kestrel-example/"` (see vite.config.js) so a
        // gateway in front of this app can route the whole prefix here. Stripping it up front
        // means everything below keeps matching unprefixed paths (/graphql, /assets/...).
        app.UsePathBase("/kestrel-example");

        // Created up front so a later `npm run client:watch` build is served without a restart.
        Directory.CreateDirectory(ClientDist);

        var contentTypes = new FileExtensionContentTypeProvider();
        contentTypes.Mappings.TryAdd(".webmanifest", "application/manifest+json");

        // Before routing, so the SPA fallback endpoint doesn't shadow real assets.
        app.UseStaticFiles(
            new StaticFileOptions { FileProvider = new PhysicalFileProvider(ClientDist), ContentTypeProvider = contentTypes }
        );
        app.UseRouting();

        // Same-origin proxy to the GraphQL API so the browser only ever talks to this
        // origin (mirrors the Vite dev-server proxy in src/web/client/vite.config.js).
        var apiClient = new HttpMessageInvoker(
            new SocketsHttpHandler
            {
                UseProxy = false,
                AllowAutoRedirect = false,
                UseCookies = false,
                // The API uses the same self-signed cert and is only reached over localhost.
                SslOptions = { RemoteCertificateValidationCallback = (_, _, _, _) => true },
            }
        );
        app.MapForwarder(
            "/graphql/{**rest}",
            $"https://localhost:{apiHttpsPort}",
            ForwarderRequestConfig.Empty,
            HttpTransformer.Default,
            apiClient
        );

        // SPA fallback: let the client-side app handle any non-asset route.
        app.MapFallback(async context =>
        {
            var indexHtml = Path.Combine(ClientDist, "index.html");
            context.Response.ContentType = "text/html; charset=utf-8";

            if (File.Exists(indexHtml))
            {
                await context.Response.SendFileAsync(indexHtml);
                return;
            }

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsync("Client build not found. Run `npm run client:build`.");
        });
    }
}
