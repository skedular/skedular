namespace Enterprise.Shared.Metrics;

public static class MetricNames
{
    public const string GraphqlExceptionsCounter = "graphql.exceptions.total";
    public const string GraphqlDataLoaderBatchCounter = "graphql.dataloader.batch.total";
    public const string GraphqlDataLoaderKeyCounter = "graphql.dataloader.key.total";
    public const string GraphqlDataLoaderDuration = "graphql.dataloader.duration";
    public const string OperationSuccessCounter = "operation.success.total";
    public const string OperationFailureCounter = "operation.failure.total";
    public const string OperationDuration = "operation.duration";
    public const string OperationActivitySource = "skedular.operation";
}
