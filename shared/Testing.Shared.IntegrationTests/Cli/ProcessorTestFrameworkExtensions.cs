using Microsoft.Extensions.DependencyInjection;

namespace Testing.Shared.IntegrationTests.Cli;

public static class CliTestFrameworkExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection UseCliTestFramework() => services.AddSingleton(typeof(CliApplicationFactory<>));
    }
}
