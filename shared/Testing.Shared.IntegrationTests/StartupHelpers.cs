using Microsoft.Extensions.Hosting;

namespace Testing.Shared.IntegrationTests;

public static class StartupHelpers
{
    public static void SetIntegrationTestEnvironmentVariables() =>
        // Act like we are in a production environment
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", Environments.Production);
}
