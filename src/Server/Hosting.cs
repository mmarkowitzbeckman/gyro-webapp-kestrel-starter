using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace Gyro;

public static class Hosting
{
    // One Kestrel app per server, listening on its HTTPS port plus an HTTP port that only redirects.
    public static WebApplication Create(
        string name,
        X509Certificate2 certificate,
        int httpsPort,
        int httpPort,
        Action<IServiceCollection> addServices,
        Action<WebApplication> configure
    )
    {
        var builder = WebApplication.CreateBuilder();

        // Keep the console as quiet as the Node server: just the startup lines below, warnings, and errors.
        builder.Logging.SetMinimumLevel(LogLevel.Warning);
        builder.WebHost.ConfigureKestrel(kestrel =>
        {
            kestrel.AddServerHeader = false;
            kestrel.ListenAnyIP(
                httpsPort,
                listen =>
                {
                    // Node's https server only speaks HTTP/1.1; don't negotiate HTTP/2.
                    listen.Protocols = HttpProtocols.Http1;
                    listen.UseHttps(certificate);
                }
            );
            kestrel.ListenAnyIP(httpPort);
        });
        addServices(builder.Services);

        var app = builder.Build();
        app.UseHttpsRedirect(httpsPort);
        configure(app);

        app.Lifetime.ApplicationStarted.Register(() =>
        {
            Console.WriteLine($"{name} server (TLS) running on port {httpsPort}");
            Console.WriteLine($"{name} redirect server running on port {httpPort} -> {httpsPort}");
        });

        return app;
    }
}
