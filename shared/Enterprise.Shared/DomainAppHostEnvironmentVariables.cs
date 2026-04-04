namespace Enterprise.Shared;

public static class DomainAppHostEnvironmentVariables
{
    public const string UseSharedInfrastructureGrpc = "DOMAIN_USE_SHARED_INFRA_GRPC";

    public static bool IsSharedInfrastructureGrpcEnabled() =>
        bool.TryParse(Environment.GetEnvironmentVariable(UseSharedInfrastructureGrpc), out var enabled) && enabled;

    public static void SetSharedInfrastructureGrpc(bool enabled) =>
        Environment.SetEnvironmentVariable(UseSharedInfrastructureGrpc, enabled.ToString());
}
