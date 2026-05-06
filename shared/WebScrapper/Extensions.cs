using Api.Shared.Clients.Configurations.Grpc;
using Api.Shared.Clients.Grpc;
using Api.Shared.Grpc.Skedular.Location.Core.V1;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrganizationService = Api.Shared.Grpc.Skedular.Organization.Core.V1.OrganizationService;
using OrganizationTagsService = Api.Shared.Grpc.Skedular.Organization.Tags.V1.OrganizationTagsService;

namespace WebScrapper;

public static class Extensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddSharedCrossDomainClients(IConfiguration configuration)
        {
            var locationConfiguration = configuration.GetSection(LocationConfiguration.Key).Get<LocationConfiguration>();
            ArgumentNullException.ThrowIfNull(locationConfiguration);
            ArgumentException.ThrowIfNullOrWhiteSpace(locationConfiguration.ApiKey);
            ArgumentNullException.ThrowIfNull(locationConfiguration.GrpcUrl);

            var organizationConfiguration = configuration.GetSection(OrganizationConfiguration.Key).Get<OrganizationConfiguration>();
            ArgumentNullException.ThrowIfNull(organizationConfiguration);
            ArgumentException.ThrowIfNullOrWhiteSpace(organizationConfiguration.ApiKey);
            ArgumentNullException.ThrowIfNull(organizationConfiguration.GrpcUrl);

            services.AddGrpcClient<LocationService.LocationServiceClient>(GrpcClients.ConfigureLocation);
            services.AddGrpcClient<OrganizationService.OrganizationServiceClient>(GrpcClients.ConfigureOrganization);
            services.AddGrpcClient<OrganizationTagsService.OrganizationTagsServiceClient>(GrpcClients.ConfigureOrganization);

            return services
                .AddSingleton(locationConfiguration)
                .AddSingleton(organizationConfiguration);
        }
    }
}
