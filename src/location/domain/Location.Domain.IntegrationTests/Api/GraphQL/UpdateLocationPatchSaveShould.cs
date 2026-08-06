using Api.Shared.Services.Models;
using Api.Shared.Services.Offering;
using Location.Domain.IntegrationTests.Skedular.GraphQL.V1;
using Location.Shared.Database.Entities;
using Location.Shared.Repositories;
using LocationEntity = Location.Shared.Database.Entities.Location;
using Offering = Api.Shared.Services.Models.Offering;

namespace Location.Domain.IntegrationTests.Api.GraphQL;

[Trait(CategoryNames.Key, CategoryNames.Integration)]
[Collection("Location.Api")]
public class UpdateLocationPatchSaveShould(
    IUpdateLocationPatchSaveMutation updateLocationPatchSaveMutation,
    IRepositoryFactory repositoryFactory,
    TimeProvider timeProvider)
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Preserve_Omitted_Details_For_Single_And_Grouped_Saves(
        string locationId,
        string organizationId,
        string customerId,
        string identityId,
        string memberId,
        string originalName,
        string updatedName,
        string originalTimezone,
        string updatedTimezone,
        string listingTitle,
        CancellationToken cancellationToken)
    {
        await SeedOwnedLocationAsync(
            locationId,
            organizationId,
            customerId,
            identityId,
            memberId,
            originalName,
            originalTimezone,
            listingTitle,
            cancellationToken);

        TestBearerTokenHandler.SetToken(identityId);
        try
        {
            var nameResult = await updateLocationPatchSaveMutation.ExecuteAsync(
                locationId,
                [LocationPatchField.Name],
                updatedName,
                null,
                cancellationToken);

            nameResult.Errors.Select(error => error.Message).ShouldBeEmpty();
            nameResult.Data.ShouldNotBeNull();
            nameResult.Data.UpdateLocation.Location.Name.ShouldBe(updatedName);
            nameResult.Data.UpdateLocation.Location.Timezone.ShouldBe(originalTimezone);

            var groupedResult = await updateLocationPatchSaveMutation.ExecuteAsync(
                locationId,
                [LocationPatchField.Name, LocationPatchField.Timezone],
                updatedName,
                updatedTimezone,
                cancellationToken);

            groupedResult.Errors.Select(error => error.Message).ShouldBeEmpty();
            groupedResult.Data.ShouldNotBeNull();
            groupedResult.Data.UpdateLocation.Location.Name.ShouldBe(updatedName);
            groupedResult.Data.UpdateLocation.Location.Timezone.ShouldBe(updatedTimezone);
            groupedResult.Data.UpdateLocation.Location.ListingMetadata.Title.ShouldBe(listingTitle);

            var location = await repositoryFactory.LocationRepository.GetByIdUntrackedAsync(locationId, cancellationToken);
            location.ShouldNotBeNull();
            location.Name.ShouldBe(updatedName);
            location.Timezone.ShouldBe(updatedTimezone);
            location.ListingMetadata.ShouldNotBeNull();
            location.ListingMetadata.Title.ShouldBe(listingTitle);
        }
        finally
        {
            TestBearerTokenHandler.ClearToken();
        }
    }

    private async Task SeedOwnedLocationAsync(
        string locationId,
        string organizationId,
        string customerId,
        string identityId,
        string memberId,
        string originalName,
        string originalTimezone,
        string listingTitle,
        CancellationToken cancellationToken)
    {
        var now = timeProvider.GetUtcNow();
        var organization = await repositoryFactory.OrganizationRepository.UpsertNakedAsync(organizationId, cancellationToken);
        organization.Offering = new Offering
        {
            Id = organizationId,
            Code = OfferingCode.EnterpriseCustomV1,
            Start = now.AddDays(-1),
            End = now.AddDays(1),
        };
        var customer = await repositoryFactory.CustomerRepository.UpsertNakedAsync(customerId, cancellationToken);

        repositoryFactory.IdentityRepository.Add(new Identity
        {
            Id = identityId,
            Customer = customer,
        });
        repositoryFactory.OrganizationMemberRepository.Add(new OrganizationMember
        {
            Id = memberId,
            Organization = organization,
            Customer = customer,
            Role = OrganizationMemberRoleConstants.Owner,
            Status = OrganizationMemberStatusConstants.Active,
        });
        repositoryFactory.LocationRepository.Add(new LocationEntity
        {
            Id = locationId,
            Organization = organization,
            Name = originalName,
            Timezone = originalTimezone,
            Type = LocationTypeConstants.Private,
            ListingMetadata = new ListingMetadata(null, listingTitle, null, []),
        });

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
