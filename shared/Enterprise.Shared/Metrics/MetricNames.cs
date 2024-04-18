namespace Enterprise.Shared.Metrics;

public static class MetricNames
{
    public const string HttpTotalRequestsCounter = "http.server.requests.total";
    public const string HttpRequestsDurationGauge = "http.server.requests.duration";
    public const string GraphqlExceptionsCounter = "graphql.exceptions.total";
}
