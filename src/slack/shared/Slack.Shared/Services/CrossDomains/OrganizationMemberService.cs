using Api.Shared.Clients.Configurations.Grpc;
using Api.Shared.Grpc.Skedular.Organization.Core.V1;
using Enterprise.Shared;
using Enterprise.Shared.GraphQL.Types;
using Enterprise.Shared.Grpc;
using Slack.Shared.Mappers;
using Slack.Shared.Models;
using OrganizationMember = Slack.Shared.Models.OrganizationMember;
using OrganizationMemberRole = Api.Shared.Services.Models.OrganizationMemberRole;
using OrganizationMemberStatus = Api.Shared.Services.Models.OrganizationMemberStatus;
using PageInfo = Enterprise.Shared.GraphQL.Types.PageInfo;

namespace Slack.Shared.Services.CrossDomains;

public interface IOrganizationMemberService
{
    Task AdminAddAsync(OrganizationMember organizationMember, CancellationToken cancellationToken);

    Task<Connection<OrganizationMemberEdge>> GetPaginatedMembersAsync(
        string workspaceMemberId,
        string organizationId,
        string? nameContains,
        string? after,
        int? first,
        string? before,
        int? last,
        CancellationToken cancellationToken);
}

public class OrganizationMemberService(
    OrganizationConfiguration organizationConfiguration,
    Api.Shared.Grpc.Skedular.Organization.Core.V1.OrganizationService.OrganizationServiceClient organizationServiceClient,
    IGrpcMapper grpcMapper,
    ICustomerService customerService)
    : IOrganizationMemberService
{
    public async Task AdminAddAsync(OrganizationMember organizationMember, CancellationToken cancellationToken) =>
        await organizationServiceClient.Admin_AddMemberAsync(
            new Admin_AddMemberInput
            {
                Id = organizationMember.Organization.Id,
                Member = new Api.Shared.Grpc.Skedular.Organization.Core.V1.OrganizationMember
                {
                    Id = organizationMember.Id,
                    CustomerId = organizationMember.Customer.Id,
                    IsOrganizationOnboardingDone = true,
                    Role = organizationMember.Role switch
                    {
                        OrganizationMemberRole.Owner => Api.Shared.Grpc.Skedular.Organization.Core.V1.OrganizationMemberRole.Owner,
                        OrganizationMemberRole.Administrator => Api.Shared.Grpc.Skedular.Organization.Core.V1.OrganizationMemberRole
                            .Administrator,
                        OrganizationMemberRole.Member => Api.Shared.Grpc.Skedular.Organization.Core.V1.OrganizationMemberRole.Member,
                        _ => throw new ArgumentOutOfRangeException(nameof(organizationMember.Role), organizationMember.Role,
                            $"Unexpected value for {nameof(organizationMember.Role)}: {organizationMember.Role}. Update enum mapping or caller input."),
                    },
                    Status = organizationMember.Status switch
                    {
                        OrganizationMemberStatus.Active => Api.Shared.Grpc.Skedular.Organization.Core.V1.OrganizationMemberStatus.Active,
                        OrganizationMemberStatus.Inactive => Api.Shared.Grpc.Skedular.Organization.Core.V1.OrganizationMemberStatus.Inactive,
                        _ => throw new ArgumentOutOfRangeException(nameof(organizationMember.Status), organizationMember.Status,
                            $"Unexpected value for {nameof(organizationMember.Status)}: {organizationMember.Status}. Update enum mapping or caller input."),
                    },
                },
            },
            organizationConfiguration.ApiKey.CreateMetadata(),
            cancellationToken: cancellationToken);

    public async Task<Connection<OrganizationMemberEdge>> GetPaginatedMembersAsync(
        string workspaceMemberId,
        string organizationId,
        string? nameContains,
        string? after,
        int? first,
        string? before,
        int? last,
        CancellationToken cancellationToken)
    {
        var getPaginatedMembersInput = new GetPaginatedMembersInput
        {
            First = first.ToNullInt(),
            After = after.ToSafeString(),
            Before = before.ToSafeString(),
            Last = last.ToNullInt(),
            Where = new MemberWhereInput
            {
                OrganizationId = organizationId,
                NameContains = nameContains.ToSafeString(),
            },
        };

        getPaginatedMembersInput.OrderBy.Add(new MemberOrderInput
        {
            Direction = OrderDirection.Ascending,
            Field = MemberOrderField.Name,
        });

        var connection = await organizationServiceClient.GetPaginatedMembersAsync(
            getPaginatedMembersInput,
            organizationConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
            cancellationToken: cancellationToken);

        var customers = await Task.WhenAll(
            connection.Edges.Select(item => customerService.GetByIdAsync(workspaceMemberId, item.Node.CustomerId, cancellationToken)));

        return new Connection<OrganizationMemberEdge>
        {
            PageInfo = new PageInfo
            {
                StartCursor = connection.PageInfo.StartCursor,
                EndCursor = connection.PageInfo.EndCursor,
                HasNextPage = connection.PageInfo.HasNextPage,
                HasPreviousPage = connection.PageInfo.HasPreviousPage,
            },
            TotalCount = connection.TotalCount,
            Edges =
            [
                .. connection.Edges
                    .Select(item =>
                    {
                        var member = grpcMapper.MapTo(item.Node);
                        var matchingCustomer = customers.FirstOrDefault(customer => customer.Id == member.Customer.Id);
                        if (matchingCustomer is not null)
                        {
                            member.Customer = matchingCustomer;
                        }

                        return new OrganizationMemberEdge(member, item.Cursor);
                    }),
            ],
        };
    }
}
