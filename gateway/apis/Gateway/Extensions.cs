using Gateway.Configurations;

namespace Gateway;

public static class Extensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddConfigurations(IConfiguration configuration)
        {
            var subgraphsConfigurations = configuration.GetSection(SubgraphsConfigurations.Key).Get<SubgraphsConfigurations>();
            ArgumentNullException.ThrowIfNull(subgraphsConfigurations);

            return services.AddSingleton(subgraphsConfigurations);
        }
    }
}
