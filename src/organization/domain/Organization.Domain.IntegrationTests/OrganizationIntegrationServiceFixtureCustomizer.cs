using Api.Shared.Grpc.Skedular.Organization.Billing.V1;
using Api.Shared.Grpc.Skedular.Organization.Tags.V1;
using Api.Shared.Grpc.Skedular.Organization.Zones.V1;
using Api.Shared.Services.Configurations.Grpc;
using AutoFixture;
using Microsoft.Extensions.DependencyInjection;
using Organization.Domain.IntegrationTests.Skedular.GraphQL.V1;
using Organization.Shared.Repositories;

namespace Organization.Domain.IntegrationTests;

public static class OrganizationIntegrationTestServices
{
    public static IServiceProvider? Provider { get; set; }
}

public class OrganizationIntegrationServiceFixtureCustomizer : IFixtureCustomizer
{
    public void Customize(IFixture fixture)
    {
        var serviceProvider = OrganizationIntegrationTestServices.Provider ??
                              throw new InvalidOperationException("Organization integration test services are not configured.");

        fixture.Register(() => serviceProvider.GetRequiredService<IUpdateOrganizationMutation>());
        fixture.Register(() => serviceProvider.GetRequiredService<IOrganizationMutationContractQuery>());
        fixture.Register(() => serviceProvider.GetRequiredService<OrganizationBillingService.OrganizationBillingServiceClient>());
        fixture.Register(() => serviceProvider.GetRequiredService<OrganizationTagsService.OrganizationTagsServiceClient>());
        fixture.Register(() => serviceProvider.GetRequiredService<OrganizationZonesService.OrganizationZonesServiceClient>());
        fixture.Register(() => serviceProvider.GetRequiredService<OrganizationConfiguration>());
        fixture.Register(() => serviceProvider.CreateScope().ServiceProvider.GetRequiredService<IRepositoryFactory>());
        fixture.Register(() => serviceProvider.GetRequiredService<TimeProvider>());
    }
}
