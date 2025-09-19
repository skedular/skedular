using Api.Shared.Clients.Configurations.Grpc;
using Api.Shared.Services.Grpc.Skedular.Organization.V1;
using Enterprise.Shared;
using Enterprise.Shared.Grpc;
using Slack.Shared.Mappers;
using OrganizationMember = Slack.Shared.Models.OrganizationMember;

namespace Slack.Shared.Services.CrossDomains;

public interface IOrganizationMemberService
{
    Task<(ICollection<OrganizationMember>, MemberConnection)> GetPaginatedMembersAsync(
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
    Api.Shared.Services.Grpc.Skedular.Organization.V1.OrganizationService.OrganizationServiceClient organizationServiceClient,
    IMapper mapper,
    ICustomerService customerService)
    : IOrganizationMemberService
{
    public async Task<(ICollection<OrganizationMember>, MemberConnection)> GetPaginatedMembersAsync(
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
            Where = new MemberWhereInput { OrganizationId = organizationId, NameContains = nameContains.ToSafeString() }
        };

        getPaginatedMembersInput.OrderBy.Add(new MemberOrderInput { Direction = OrderDirection.Ascending, Field = MemberOrderField.Name });

        var connection = await organizationServiceClient.GetPaginatedMembersAsync(
            getPaginatedMembersInput,
            organizationConfiguration.ApiKey.CreateMetadata(workspaceMemberId),
            cancellationToken: cancellationToken);

        var customers = await Task.WhenAll(
            connection.Edges.Select(item => customerService.GetByIdAsync(workspaceMemberId, item.Node.CustomerId, cancellationToken)));

        return (connection.Edges
            .Select(item => mapper.MapTo(item.Node))
            .Select(item =>
            {
                var matchingCustomer = customers.FirstOrDefault(customer => customer.Id == item.Customer.Id);
                if (matchingCustomer is not null)
                {
                    item.Customer = matchingCustomer;
                }

                return item;
            }).ToList(), connection);
    }
}
