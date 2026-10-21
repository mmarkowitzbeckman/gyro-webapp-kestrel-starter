using Gyro.Api.GraphQL.Queries;

namespace Gyro.Tests.Api.GraphQL;

public class HealthQueriesTests
{
    [Fact]
    public void HealthResolverReportsOk()
    {
        Assert.Equal("ok", new HealthQueries().GetHealth());
    }
}
