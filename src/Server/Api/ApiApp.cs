using Gyro.Api.GraphQL;
using Gyro.Api.Routes;

namespace Gyro.Api;

public static class ApiApp
{
    public static void AddServices(IServiceCollection services) => services.AddGraphQLSchema();

    public static void Configure(WebApplication app)
    {
        app.Use(async (context, next) =>
        {
            try
            {
                await next(context);
            }
            catch (Exception ex) when (!context.Response.HasStarted)
            {
                Console.Error.WriteLine(ex);
                context.Response.StatusCode = ex is BadHttpRequestException bad ? bad.StatusCode : 500;
                await context.Response.WriteAsJsonAsync(new { error = ex.Message });
            }
        });

        app.MapGraphQL("/graphql");
        app.MapIndexRoutes();

        app.MapFallback(() => Results.Json(new { error = "Not Found" }, statusCode: StatusCodes.Status404NotFound));
    }
}
