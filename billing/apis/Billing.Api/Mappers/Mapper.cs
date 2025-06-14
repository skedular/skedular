using Api.Shared.Services.Models;
using Billing.Shared.Models;

namespace Billing.Api.Mappers;

public interface IMapper
{
    Customer MapTo(Shared.Database.Entities.Customer src);
    Organization MapTo(Shared.Database.Entities.Organization src);
}

public class Mapper : IMapper
{
    public Customer MapTo(Shared.Database.Entities.Customer src) =>
        new()
        {
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            Id = src.Id,
            Title = src.Title,
            Name = src.Name,
            GivenName = src.GivenName,
            MiddleName = src.MiddleName,
            FamilyName = src.FamilyName,
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
            Type = src.Type.ToOrganizationType(),
            MemberVisibilityPolicy = src.MemberVisibilityPolicy.ToOrganizationMemberVisibilityPolicy()
        };

        organization.OrganizationMembers = MapTo(src.OrganizationMembers, organization).ToList();
        organization.OrganizationOfferings = MapTo(src.OrganizationOfferings, organization).ToList();

        return organization;
    }

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

    private IEnumerable<OrganizationMember> MapTo(IEnumerable<Shared.Database.Entities.OrganizationMember> src, Organization organization) =>
        src.Select(item => MapTo(item, organization));

    private static IEnumerable<Identity> MapTo(IEnumerable<Shared.Database.Entities.Identity?>? src) =>
        (src is null ? [] : src.Where(item => item is not null).Select(MapTo))!;

    private static Identity? MapTo(Shared.Database.Entities.Identity? src) =>
        src is null ? null : new Identity { Id = src.Id, CreatedAt = src.CreatedAt, ModifiedAt = src.ModifiedAt };

    private static IEnumerable<OrganizationOffering> MapTo(IEnumerable<Shared.Database.Entities.OrganizationOffering> src, Organization organization)
        => src.Select(item => MapTo(item, organization));

    private static OrganizationOffering MapTo(
        Shared.Database.Entities.OrganizationOffering src,
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
