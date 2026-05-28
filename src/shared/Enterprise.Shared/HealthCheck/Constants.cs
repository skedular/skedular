namespace Enterprise.Shared.HealthCheck;

public class Constants
{
    public const string LivenessTag = "liveness";
    public const string ReadinessTag = "readiness";
    public const string ReadinessPath = "/health/readiness";
    public const string LivenessPath = "/health/liveness";
}
