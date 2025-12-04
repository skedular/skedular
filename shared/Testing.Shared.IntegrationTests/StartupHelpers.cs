using Microsoft.Extensions.Hosting;

namespace Testing.Shared.IntegrationTests;

public static class StartupHelpers
{
    public static void SetIntegrationTestEnvironmentVariables() =>
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", Environments.Production);
}
