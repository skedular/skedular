using Microsoft.Extensions.DependencyInjection;

namespace Testing.Shared.IntegrationTests.Cli;

public static class CliTestFrameworkExtensions
{
    public static IServiceCollection UseCliTestFramework(this IServiceCollection services) =>
        services.AddSingleton(typeof(CliApplicationFactory<>));
}
