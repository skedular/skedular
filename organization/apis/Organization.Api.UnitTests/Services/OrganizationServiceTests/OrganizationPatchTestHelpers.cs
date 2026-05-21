using Api.Shared.Services.Models;
using Organization.Api.Models;
using Organization.Shared.Database.Entities;
using Organization.Shared.Models;
using OrganizationEntity = Organization.Shared.Database.Entities.Organization;

namespace Organization.Api.UnitTests.Services.OrganizationServiceTests;

internal static class OrganizationPatchTestHelpers
{
    public static OrganizationEntity CreateOrganization(string id, string customDomain, string name) =>
        new()
        {
            Id = id,
            CustomDomain = customDomain,
            Name = name,
            Type = OrganizationTypeConstants.Private,
            ListingMetadata = new ListingMetadata("Description", "Title", "Sub title", [])
        };

    public static OrganizationPatchRequest CreateNameRequest(OrganizationEntity organization, string name) =>
        new(
            organization.Id,
            null,
            new HashSet<OrganizationPatchField> { OrganizationPatchField.Name },
            name,
            null);

    public static Organization.Shared.Models.Customer CreateCustomer(string id) => new() { Id = id };
}
