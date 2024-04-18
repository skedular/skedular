using Api.Shared.Services.GraphQL.UnityHub.V1.Billing;
using Billing.Shared.Models;
using Enterprise.Shared;

namespace Billing.Api.Mappers;

public interface IMapper
{
    OrganizationBillingInfo MapTo(Organization src);
    OrganizationBillingInfoPayload MapTo(Organization src, string? clientMutationId);
    Customer MapTo(Shared.Database.Entities.Customer src);
    Organization MapTo(Shared.Database.Entities.Organization src);
    global::Api.Shared.Services.Grpc.UnityHub.Billing.V1.OrganizationBillingInfo MapToGrpcResponse(Organization src);
}

public class Mapper : IMapper
{
    public OrganizationBillingInfo MapTo(Organization src) =>
        new()
        {
            OrganizationId = src.Id,
            Email = src.BillingContactEmail,
            AddressLine1 = src.BillingContactAddressLine1,
            AddressLine2 = src.BillingContactAddressLine2,
            Suburb = src.BillingContactSuburb,
            City = src.BillingContactCity,
            Province = src.BillingContactProvince,
            Zipcode = src.BillingContactZipcode,
            Country = src.BillingContactCountry
        };

    public OrganizationBillingInfoPayload MapTo(Organization src, string? clientMutationId) =>
        new() { ClientMutationId = clientMutationId, OrganizationBillingInfo = MapTo(src) };

    public Customer MapTo(Shared.Database.Entities.Customer src) =>
        new()
        {
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            Id = src.Id,
            Identities = MapTo(src.Identities).ToList()
        };

    public Organization MapTo(Shared.Database.Entities.Organization src)
    {
        var organization = new Organization
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            EventRaisedAt = src.EventRaisedAt,
            Name = src.Name,
            BillingContactEmail = src.BillingContactEmail,
            BillingContactAddressLine1 = src.BillingContactAddressLine1,
            BillingContactAddressLine2 = src.BillingContactAddressLine2,
            BillingContactSuburb = src.BillingContactSuburb,
            BillingContactCity = src.BillingContactCity,
            BillingContactProvince = src.BillingContactProvince,
            BillingContactZipcode = src.BillingContactZipcode,
            BillingContactCountry = src.BillingContactCountry
        };

        organization.OrganizationMembers = MapTo(src.OrganizationMembers, organization).ToList();
        organization.OrganizationOfferings = MapTo(src.OrganizationOfferings, organization).ToList();

        return organization;
    }

    public global::Api.Shared.Services.Grpc.UnityHub.Billing.V1.OrganizationBillingInfo MapToGrpcResponse(
        Organization src) =>
        new()
        {
            Email = src.BillingContactEmail.ToSafeString(),
            AddressLine1 = src.BillingContactAddressLine1.ToSafeString(),
            AddressLine2 = src.BillingContactAddressLine2.ToSafeString(),
            Suburb = src.BillingContactSuburb.ToSafeString(),
            City = src.BillingContactCity.ToSafeString(),
            Province = src.BillingContactProvince.ToSafeString(),
            Zipcode = src.BillingContactZipcode.ToSafeString(),
            Country = src.BillingContactCountry.ToSafeString()
        };

    private OrganizationMember MapTo(Shared.Database.Entities.OrganizationMember src, Organization organization) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            EventRaisedAt = src.EventRaisedAt,
            Customer = MapTo(src.Customer),
            Organization = organization
        };

    private IEnumerable<OrganizationMember> MapTo(
        IEnumerable<Shared.Database.Entities.OrganizationMember> src,
        Organization organization) => src.Select(item => MapTo(item, organization));

    private static IEnumerable<Identity> MapTo(IEnumerable<Shared.Database.Entities.Identity?>? src) =>
        (src is null ? [] : src.Where(item => item is not null).Select(MapTo))!;

    private static Identity? MapTo(Shared.Database.Entities.Identity? src) =>
        src is null
            ? null
            : new Identity { Id = src.Id, CreatedAt = src.CreatedAt, ModifiedAt = src.ModifiedAt };

    private static IEnumerable<OrganizationOffering> MapTo(
        IEnumerable<Shared.Database.Entities.OrganizationOffering> src,
        Organization organization)
        => src.Select(item => MapTo(item, organization));

    private static OrganizationOffering MapTo(Shared.Database.Entities.OrganizationOffering src,
        Organization organization) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            ModifiedAt = src.ModifiedAt,
            DeletedAt = src.DeletedAt,
            EventRaisedAt = src.EventRaisedAt,
            Organization = organization,
            Code = src.Code,
            Start = src.Start,
            End = src.End,
            UnitPrice = src.UnitPrice,
            TotalNumberOfActiveCustomers = src.TotalNumberOfActiveCustomers,
            TotalCost = src.TotalCost,
            InvoiceDate = src.InvoiceDate
        };
}
