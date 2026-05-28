using Api.Shared.Services.Models;
using Booking.Shared.Database.Entities;
using Booking.Shared.Repositories;
using Booking.Shared.Services;

namespace Booking.Shared.UnitTests.Services.PrivateBookingPreferenceServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class PickResourceBasedOnCustomerPreferencesAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Fall_Back_To_The_Next_Organization_Location_When_The_First_Default_Location_Has_No_Available_Desk(
        [Frozen] IRepositoryFactory repositoryFactory,
        PrivateBookingPreferenceService sut,
        IOrganizationRepository organizationRepository,
        IResourceRepository resourceRepository,
        CancellationToken cancellationToken)
    {
        var from = new DateTimeOffset(2026, 3, 28, 9, 0, 0, TimeSpan.Zero);
        var until = new DateTimeOffset(2026, 3, 28, 10, 0, 0, TimeSpan.Zero);
        var firstLocation = new Location { Id = "loc-1", Name = "First" };
        var secondLocation = new Location { Id = "loc-2", Name = "Second" };
        var organization = new Organization { Id = "org-1", Locations = [firstLocation, secondLocation] };
        var customer = new Customer();
        var resource = new Resource
        {
            Id = "res-2",
            Location = secondLocation,
            OrganizationTags = [new OrganizationTag { Id = "desk-tag", Type = OrganizationTagTypeConstants.ResourceDesk }]
        };

        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => repositoryFactory.ResourceRepository).Returns(resourceRepository);
        A.CallTo(() => organizationRepository.GetByIdOrCustomDomainAsync("org-1", null, false, false, cancellationToken)).Returns(organization);
        A.CallTo(() => resourceRepository.GetAvailableResourcesAsync(
                null,
                "loc-1",
                from,
                until,
                A<IReadOnlyList<string>>._,
                A<IReadOnlyList<string>>._,
                A<IReadOnlyList<string>>._,
                cancellationToken))
            .Returns([]);
        A.CallTo(() => resourceRepository.GetAvailableResourcesAsync(
                null,
                "loc-2",
                from,
                until,
                A<IReadOnlyList<string>>._,
                A<IReadOnlyList<string>>._,
                A<IReadOnlyList<string>>._,
                cancellationToken))
            .Returns([resource]);

        var result = await sut.PickResourceBasedOnCustomerPreferencesAsync(
            customer,
            from,
            until,
            ["org-1"],
            [],
            cancellationToken);

        result.Item1.Select(item => item.Id).ShouldBe(["org-1"]);
        result.Item2.Select(item => item.Id).ShouldBe(["res-2"]);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Fall_Back_To_The_Next_Organization_Location_When_The_Preferred_Location_Has_No_Available_Desk(
        [Frozen] IRepositoryFactory repositoryFactory,
        PrivateBookingPreferenceService sut,
        IOrganizationRepository organizationRepository,
        IResourceRepository resourceRepository,
        CancellationToken cancellationToken)
    {
        var from = new DateTimeOffset(2026, 3, 28, 9, 0, 0, TimeSpan.Zero);
        var until = new DateTimeOffset(2026, 3, 28, 10, 0, 0, TimeSpan.Zero);
        var firstLocation = new Location { Id = "loc-1", Name = "First" };
        var secondLocation = new Location { Id = "loc-2", Name = "Second" };
        var organization = new Organization { Id = "org-1", Locations = [firstLocation, secondLocation] };
        firstLocation.Organization = organization;
        secondLocation.Organization = organization;
        var customer = new Customer { PreferredLocations = [firstLocation] };
        var resource = new Resource
        {
            Id = "res-2",
            Location = secondLocation,
            OrganizationTags = [new OrganizationTag { Id = "desk-tag", Type = OrganizationTagTypeConstants.ResourceDesk }]
        };

        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => repositoryFactory.ResourceRepository).Returns(resourceRepository);
        A.CallTo(() => organizationRepository.GetByIdOrCustomDomainAsync("org-1", null, false, false, cancellationToken)).Returns(organization);
        A.CallTo(() => resourceRepository.GetAvailableResourcesAsync(
                null,
                "loc-1",
                from,
                until,
                A<IReadOnlyList<string>>._,
                A<IReadOnlyList<string>>._,
                A<IReadOnlyList<string>>._,
                cancellationToken))
            .Returns([]);
        A.CallTo(() => resourceRepository.GetAvailableResourcesAsync(
                null,
                "loc-2",
                from,
                until,
                A<IReadOnlyList<string>>._,
                A<IReadOnlyList<string>>._,
                A<IReadOnlyList<string>>._,
                cancellationToken))
            .Returns([resource]);

        var result = await sut.PickResourceBasedOnCustomerPreferencesAsync(
            customer,
            from,
            until,
            ["org-1"],
            [],
            cancellationToken);

        result.Item1.Select(item => item.Id).ShouldBe(["org-1"]);
        result.Item2.Select(item => item.Id).ShouldBe(["res-2"]);
    }
}
