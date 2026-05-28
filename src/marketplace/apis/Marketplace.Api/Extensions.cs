using Api.Shared.Services.Configurations.Grpc;
using Marketplace.Api.Mappers;
using Marketplace.Api.Services;
using Marketplace.Api.Services.Authorization;

namespace Marketplace.Api;

public static class Extensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddMappers() =>
            services.AddSingleton<IGraphQlMapper, GraphQlMapper>();

        public IServiceCollection AddServices() =>
            services
                .AddScoped<IOrganizationAuthorizationService, OrganizationAuthorizationService>()
                .AddScoped<IOrganizationSsoAuthorizationService, OrganizationSsoAuthorizationService>()
                .AddScoped<ICustomerService, CustomerService>()
                .AddScoped<IProductService, ProductService>()
                .AddScoped<IProductVersionService, ProductVersionService>()
                .AddScoped<IWorkaroundService, WorkaroundService>();

        public IServiceCollection AddJobs() =>
            services;

        public IServiceCollection AddGrpcServices(IConfiguration configuration)
        {
            var msTeamsConfiguration = configuration.GetSection(MarketplaceConfiguration.Key).Get<MarketplaceConfiguration>();
            ArgumentNullException.ThrowIfNull(msTeamsConfiguration);
            ArgumentException.ThrowIfNullOrWhiteSpace(msTeamsConfiguration.ApiKey);

            return services
                .AddSingleton(msTeamsConfiguration);
        }
    }
}
