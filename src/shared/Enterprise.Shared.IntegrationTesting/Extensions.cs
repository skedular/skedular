using Microsoft.Extensions.DependencyInjection;

namespace Enterprise.Shared.IntegrationTesting;

public static class Extensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddTestingSharedIntegrationTests() =>
            services
                .AddSingleton<IEventually, Eventually>();
    }
}
