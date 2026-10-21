using DotNetEnv;
using Gyro;
using Gyro.Api;
using Gyro.Web;

Env.NoClobber().Load();

var webHttpPort = Config.Port("WEB_HTTP_PORT", 4004);
var webHttpsPort = Config.Port("WEB_HTTPS_PORT", 3003);
var apiHttpPort = Config.Port("API_HTTP_PORT", 6006);
var apiHttpsPort = Config.Port("API_HTTPS_PORT", 5005);

var certificate = Tls.LoadCertificate();

var web = Hosting.Create(
    "Web",
    certificate,
    webHttpsPort,
    webHttpPort,
    WebApp.AddServices,
    app => WebApp.Configure(app, apiHttpsPort)
);
var api = Hosting.Create("API", certificate, apiHttpsPort, apiHttpPort, ApiApp.AddServices, ApiApp.Configure);

await Task.WhenAll(web.RunAsync(), api.RunAsync());
