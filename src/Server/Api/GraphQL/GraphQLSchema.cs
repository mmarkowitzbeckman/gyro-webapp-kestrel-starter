using Gyro.Api.GraphQL.Queries;
using HotChocolate.AspNetCore;
using HotChocolate.AspNetCore.Formatters;

namespace Gyro.Api.GraphQL;

public static class GraphQLSchema
{
    // Each feature adds a [ExtendObjectType] class under Queries/ (or Mutations/) and registers it here.
    // IncludeExceptionDetails matches Yoga's maskedErrors: false — clients see real error messages.
    // The legacy transport answers with application/json, as Yoga does for fetch()'s default Accept.
    public static void AddGraphQLSchema(this IServiceCollection services) =>
        services
            .AddGraphQLServer()
            .AddQueryType(descriptor => descriptor.Name(OperationTypeNames.Query))
            .AddTypeExtension<HealthQueries>()
            .ModifyRequestOptions(options => options.IncludeExceptionDetails = true)
            .AddHttpResponseFormatter(
                new HttpResponseFormatterOptions { HttpTransportVersion = HttpTransportVersion.Legacy }
            );
}
