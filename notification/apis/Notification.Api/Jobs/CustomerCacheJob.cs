using Notification.Api.Services;
using Notification.Shared.Repositories;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;

namespace Notification.Api.Jobs;

public class CustomerCacheJob(IServiceProvider serviceProvider, ILogger<CustomerCacheJob> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        do
        {
            try
            {
                await using var scope = serviceProvider.CreateAsyncScope();
                var repositoryFactory = scope.ServiceProvider.GetRequiredService<IRepositoryFactory>();
                var cachedCustomerService = scope.ServiceProvider.GetRequiredService<ICachedCustomerService>();
                var customers = await repositoryFactory.CustomerRepository
                    .Query(new Specification<Shared.Database.Entities.Customer>
                    {
                        Criteria = query => !query.DeletedAt.HasValue
                    }.AddInclude(query => query.Identities)).ToListAsync(cancellationToken);

                foreach (var identity in customers.SelectMany(customer => customer.Identities))
                {
                    logger.LogTrace("Caching customer by token {id}", identity.Id);
                    _ = await cachedCustomerService.GetByVerifiableTokenAsync(identity.Id, cancellationToken);
                }

                foreach (var customer in customers)
                {
                    logger.LogTrace("Caching customer by id {id}", customer.Id);
                    _ = await cachedCustomerService.GetByIdAsync(customer.Id, cancellationToken);
                }

                await Task.Delay(TimeSpan.FromSeconds(15), cancellationToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to run job: {job}", nameof(CustomerCacheJob));
            }
        } while (true);
    }
}
