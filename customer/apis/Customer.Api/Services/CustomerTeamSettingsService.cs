using Customer.Api.Mappers;
using Customer.Api.Services.Authorization;
using Customer.Shared.Repositories;
using Enterprise.Shared.Exceptions;

namespace Customer.Api.Services;

public interface ICustomerTeamSettingsService
{
    Task<Shared.Models.Customer> AddCustomerDefaultTeamAsync(
        string teamId,
        string? customerId,
        bool ignoreAuthorizationCheck,
        CancellationToken cancellationToken);

    Task<Shared.Models.Customer> RemoveCustomerDefaultTeamAsync(
        string teamId,
        string? customerId,
        CancellationToken cancellationToken);
}

public class CustomerTeamSettingsService(
    ICustomerHelperService customerHelperService,
    ITeamAuthorizationService teamAuthorizationService,
    IRepositoryFactory repositoryFactory,
    IMapper mapper)
    : ICustomerTeamSettingsService
{
    public async Task<Shared.Models.Customer> AddCustomerDefaultTeamAsync(
        string teamId,
        string? customerId,
        bool ignoreAuthorizationCheck,
        CancellationToken cancellationToken)
    {
        var customer = string.IsNullOrWhiteSpace(customerId)
            ? await customerHelperService.GetCustomerAsync(cancellationToken)
            : await customerHelperService.GetCustomerAsync(customerId, cancellationToken);
        var team = await repositoryFactory.TeamRepository.UpsertNakedAsync(teamId, null, cancellationToken);
        if (team is null)
        {
            throw new TeamNotFound();
        }

        if (!ignoreAuthorizationCheck && !teamAuthorizationService.CanAddTeamAsDefault(team, customer))
        {
            throw new Unauthorized();
        }

        if (customer.DefaultTeams.Any(item => item.Id == teamId))
        {
            return mapper.MapTo(customer);
        }

        customer.DefaultTeams = customer.DefaultTeams.Concat([team]).ToList();

        return await customerHelperService.UpdateAndPublishEventAsync(customer, cancellationToken);
    }

    public async Task<Shared.Models.Customer> RemoveCustomerDefaultTeamAsync(
        string teamId,
        string? customerId,
        CancellationToken cancellationToken)
    {
        var customer = string.IsNullOrWhiteSpace(customerId)
            ? await customerHelperService.GetCustomerAsync(cancellationToken)
            : await customerHelperService.GetCustomerAsync(customerId, cancellationToken);
        customer.DefaultTeams = customer.DefaultTeams.Where(item => item.Id != teamId).ToList();
        return await customerHelperService.UpdateAndPublishEventAsync(customer, cancellationToken);
    }
}
