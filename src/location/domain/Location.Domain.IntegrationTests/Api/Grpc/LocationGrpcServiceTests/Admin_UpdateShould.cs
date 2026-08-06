using Api.Shared.Grpc.Skedular.Location.Core.V1;
using Api.Shared.Services.Configurations.Grpc;
using Api.Shared.Services.Models;
using Enterprise.Shared.Grpc;
using Location.Shared.Repositories;
using LocationEntity = Location.Shared.Database.Entities.Location;

namespace Location.Domain.IntegrationTests.Api.Grpc.LocationGrpcServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Integration)]
[Collection("Location.Api")]
public class Admin_UpdateShould(
    LocationService.LocationServiceClient locationServiceClient,
    IRepositoryFactory repositoryFactory,
    LocationConfiguration locationConfiguration)
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Preserve_Omitted_Details(
        string locationId,
        string organizationId,
        string originalName,
        string updatedName,
        string originalTimezone,
        CancellationToken cancellationToken)
    {
        await SeedLocationAsync(locationId, organizationId, originalName, originalTimezone, cancellationToken);

        var result = await locationServiceClient.Admin_UpdateAsync(
            new Admin_UpdateInput
            {
                Id = locationId,
                OrganizationId = organizationId,
                Name = updatedName,
                FieldsToUpdate =
                {
                    LocationPatchField.Name,
                },
            },
            locationConfiguration.ApiKey.CreateMetadata(),
            cancellationToken: cancellationToken);

        result.ShouldNotBeNull();
        result.Name.ShouldBe(updatedName);
        result.Timezone.ShouldBe(originalTimezone);

        var location = await repositoryFactory.LocationRepository.GetByIdUntrackedAsync(locationId, cancellationToken);
        location.ShouldNotBeNull();
        location.Name.ShouldBe(updatedName);
        location.Timezone.ShouldBe(originalTimezone);
    }

    private async Task SeedLocationAsync(
        string locationId,
        string organizationId,
        string name,
        string timezone,
        CancellationToken cancellationToken)
    {
        var organization = await repositoryFactory.OrganizationRepository.UpsertNakedAsync(organizationId, cancellationToken);

        repositoryFactory.LocationRepository.Add(new LocationEntity
        {
            Id = locationId,
            Organization = organization,
            Name = name,
            Timezone = timezone,
            Type = LocationTypeConstants.Private,
        });

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
