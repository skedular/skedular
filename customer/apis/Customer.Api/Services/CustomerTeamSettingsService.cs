using Api.Shared.Services;
using Customer.Api.Mappers;
using Customer.Api.Services.Authorization;
using Customer.Shared.Repositories;

namespace Customer.Api.Services;

public interface ICustomerTeamSettingsService
{
    Task<Shared.Models.Customer> AddCustomerPreferredTeamAsync(
        string teamId,
        string? customerId,
        bool ignoreAuthorizationCheck,
        CancellationToken cancellationToken);

    Task<Shared.Models.Customer> RemoveCustomerPreferredTeamAsync(string teamId, string? customerId, CancellationToken cancellationToken);
}

public class CustomerTeamSettingsService(
    ICustomerHelperService customerHelperService,
    ITeamAuthorizationService teamAuthorizationService,
    IRepositoryFactory repositoryFactory,
    IMapper mapper)
    : ICustomerTeamSettingsService
{
    public async Task<Shared.Models.Customer> AddCustomerPreferredTeamAsync(
        string teamId,
        string? customerId,
        bool ignoreAuthorizationCheck,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(teamId);

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
            throw new UnauthorizedAccessException();
        }

        if (customer.PreferredTeams.Any(item => item.Id == teamId))
        {
            return mapper.MapTo(customer);
        }

        customer.PreferredTeams = customer.PreferredTeams.Concat([team]).ToList();

        return await customerHelperService.UpdateAndPublishEventAsync(customer, cancellationToken);
    }

    public async Task<Shared.Models.Customer> RemoveCustomerPreferredTeamAsync(string teamId, string? customerId, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(teamId);

        var customer = string.IsNullOrWhiteSpace(customerId)
            ? await customerHelperService.GetCustomerAsync(cancellationToken)
            : await customerHelperService.GetCustomerAsync(customerId, cancellationToken);
        customer.PreferredTeams = customer.PreferredTeams.Where(item => item.Id != teamId).ToList();
        return await customerHelperService.UpdateAndPublishEventAsync(customer, cancellationToken);
    }
}
