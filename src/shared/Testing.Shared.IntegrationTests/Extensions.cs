using Microsoft.Extensions.DependencyInjection;

namespace Testing.Shared.IntegrationTests;

public static class Extensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddTestingSharedIntegrationTests() =>
            services
                .AddSingleton<IEventually, Eventually>();
    }
}
