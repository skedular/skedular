using Api.Shared.Clients.Events.UnityHub.Organization.V1.Value;
using Api.Shared.Models;
using Api.Shared.Services.Offering;
using Enterprise.Shared;
using Google.Protobuf.WellKnownTypes;
using Organization.Shared.Models;
using Offering = Api.Shared.Clients.Events.UnityHub.Organization.V1.Value.Offering;
using Tag = Api.Shared.Clients.Events.UnityHub.Organization.V1.Value.Tag;

namespace Organization.Shared.Mappers;

public interface IMapper
{
    Api.Shared.Clients.Events.UnityHub.Organization.V1.Value.Organization MapTo(Models.Organization src);
    public InvitationToJoinOrganization MapTo(JoinInvitation src, string? inviteeIdToOverride);
}

public class Mapper : IMapper
{
    public Api.Shared.Clients.Events.UnityHub.Organization.V1.Value.Organization MapTo(Models.Organization src)
    {
        var organizationOffering = src.OrganizationOfferings.Where(item => !item.DeletedAt.HasValue)
            .OrderByDescending(item => item.End).First();
        var organization = new Api.Shared.Clients.Events.UnityHub.Organization.V1.Value.Organization
        {
            Id = src.Id,
            Name = src.Name.ToSafeString(),
            About = src.About.ToSafeString(),
            Website = src.Website.ToSafeString(),
            LogoUrl = src.LogoUrl.ToSafeString(),
            Offering = new Offering
            {
                Id = organizationOffering.Id,
                OrganizationId = src.Id,
                Code = organizationOffering.Code.ToOfferingCode(),
                Start = organizationOffering.Start.ToTimestamp(),
                End = organizationOffering.End.ToTimestamp(),
                AutoRenew = organizationOffering.AutoRenew,
                UnitPrice = organizationOffering.UnitPrice
            }
        };

        organization.AzureTenantIds.AddRange(src.AzureTenants.Select(item => item.Id));

        organization.Tags.AddRange(src.Tags.Select(item => new Tag
        {
            Id = item.Id,
            Name = item.Name.ToSafeString(),
            Description = item.Description.ToSafeString(),
            TagType = item.Type.ToSafeString()
        }));

        organization.Offering.ActiveCustomerIds.AddRange(
            organizationOffering.OrganizationOfferingActiveMembers.Select(item => item.OrganizationMember.Customer.Id));

        organization.Members.AddRange(src.OrganizationMembers.Select(item =>
        {
            var membershipType =
                item.MembershipType switch
                {
                    OldOrganizationMembershipType.Owner => MembershipType.Owner,
                    OldOrganizationMembershipType.Administrator => MembershipType.Administrator,
                    OldOrganizationMembershipType.Member => MembershipType.Member,
                    _ => throw new ArgumentOutOfRangeException()
                };

            return new Member { Id = item.Id, CustomerId = item.Customer.Id, MembershipType = membershipType };
        }));

        return organization;
    }

    public InvitationToJoinOrganization MapTo(JoinInvitation src, string? inviteeIdToOverride) =>
        new()
        {
            Id = src.Id,
            OrganizationId = src.Organization.Id,
            InvitedById = src.CreatedBy.Id,
            InviteeId = inviteeIdToOverride ?? (src.Invitee is null ? string.Empty : src.Invitee.Id)
        };
}
