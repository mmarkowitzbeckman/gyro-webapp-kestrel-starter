namespace Gyro.Api.GraphQL.Queries;

[ExtendObjectType(OperationTypeNames.Query)]
public class HealthQueries
{
    public string GetHealth() => "ok";
}
