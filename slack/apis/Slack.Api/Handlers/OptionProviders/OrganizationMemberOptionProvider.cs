using Api.Shared.Clients.Configurations.Grpc;
using Api.Shared.Services;
using Api.Shared.Services.Grpc.Skedular.Organization.V1;
using Enterprise.Shared;
using Enterprise.Shared.Grpc;
using Slack.Api.Mappers;
using Slack.Shared.Constants;
using Slack.Shared.Repositories;
using Slack.Shared.Services.CrossDomains;
using SlackNet.Blocks;
using SlackNet.Interaction;

namespace Slack.Api.Handlers.OptionProviders;

public class OrganizationMemberOptionProvider(
    OrganizationConfiguration organizationConfiguration,
    IRepositoryFactory repositoryFactory,
    IMapper mapper,
    OrganizationService.OrganizationServiceClient organizationServiceClient,
    IAdminCustomerService customerAdminService)
    : IBlockOptionProvider
{
    public async Task<BlockOptionsResponse> GetOptions(BlockOptionsRequest request)
    {
        var cancellationToken = CancellationToken.None;
        var workspaceEntity = await repositoryFactory.WorkspaceRepository.GetByIdAsync(request.Team.Id, cancellationToken) ??
                              throw new SlackWorkspaceNotFound();
        var getPaginatedMembersInput = new GetPaginatedMembersInput
        {
            First = 100,
            After = string.Empty,
            Last = ((int?)null).ToNullInt(),
            Before = string.Empty,
            Where = new MemberWhereInput { OrganizationId = workspaceEntity.Organization.Id, NameContains = request.Value }
        };

        getPaginatedMembersInput.OrderBy.Add(new MemberOrderInput { Direction = OrderDirection.Ascending, Field = MemberOrderField.Name });

        var memberConnection = await organizationServiceClient.GetPaginatedMembersAsync(
            getPaginatedMembersInput,
            organizationConfiguration.ApiKey.CreateMetadata(request.User.Id),
            cancellationToken: cancellationToken);

        var customers =
            await Task.WhenAll(memberConnection.Edges.Select(item => customerAdminService.GetAsync(item.Node.CustomerId, cancellationToken)));

        return new BlockOptionsResponse
        {
            Options = memberConnection.Edges
                .Select(item => mapper.MapTo(item.Node))
                .Select(item =>
                {
                    var matchingCustomer = customers.FirstOrDefault(customer => customer.Id == item.Customer.Id);

                    return new Option
                    {
                        Text = matchingCustomer is null ? "???" : matchingCustomer.DisplayableName.ToOptionText(), Value = item.Customer.Id
                    };
                })
                .ToList()
        };
    }
}
