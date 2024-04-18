using Api.Shared.Clients.Events.UnityHub.MsTeamsInternal.V1.Key;
using Confluent.Kafka;
using Enterprise.Shared.Database;
using Enterprise.Shared.Kafka.Consume;
using Microsoft.EntityFrameworkCore;
using MsTeams.Shared.Database.Entities;
using MsTeams.Shared.Mappers;
using MsTeams.Shared.Repositories;
using MsTeams.Shared.Services;
using Event = Api.Shared.Clients.Events.UnityHub.MsTeamsInternal.V1.Value.Event;
using Type = Api.Shared.Clients.Events.UnityHub.MsTeamsInternal.V1.Value.Type;

namespace MsTeams.Processors.Subscribers;

public class MsTeamsInternalSubscriber(
    TimeProvider timeProvider,
    IMsGraphService msGraphService,
    IMapper sharedMapper,
    IRepositoryFactory repositoryFactory)
    : IEventSubscriber<Key, Event>
{
    public async Task HandleAsync(Headers headers, Key key, Event @event, CancellationToken cancellationToken)
    {
        switch (@event.Metadata.Type)
        {
            case Type.RefreshTenantMembers:
                await HandleRefreshTenantMembersAsync(@event.TenantId, cancellationToken);
                break;

            default:
                return;
        }
    }

    private async Task HandleRefreshTenantMembersAsync(
        string tenantId,
        CancellationToken cancellationToken)
    {
        var existingTenant = await repositoryFactory.TenantRepository
            .Query(
                new Specification<Tenant> { Criteria = query => query.Id == tenantId }
                    .AddInclude(query => query.TenantMembers)
                    .ApplyOrderBy(query => query.Id))
            .FirstOrDefaultAsync(cancellationToken);

        if (existingTenant is null)
        {
            return;
        }

        var users = await msGraphService.GetUsersAsync(tenantId, cancellationToken);

        var itemsToRemove = existingTenant.TenantMembers
            .Where(tenantMember => users.All(item => item.Id != tenantMember.Id))
            .ToList();

        var updatedItems = existingTenant.TenantMembers
            .Where(tenantMember => users.Any(item => item.Id == tenantMember.Id))
            .ToList();

        var addedItems = users
            .Where(tenantMember => existingTenant.TenantMembers.All(item => item.Id != tenantMember.Id))
            .Select(user => repositoryFactory.TenantMemberRepository.Add(sharedMapper.MapToEntity(user))).ToList();

        existingTenant.EntitiesLastRefreshedAt = timeProvider.GetUtcNow();
        repositoryFactory.TenantMemberRepository.RemoveRange(itemsToRemove);
        existingTenant.TenantMembers = addedItems.Concat(updatedItems).ToList();
        await repositoryFactory.TenantRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
