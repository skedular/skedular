using Payment.Shared.Repositories;

namespace Payment.Jobs.Jobs;

public class StripeCustomerMigrationJob(IServiceProvider serviceProvider, ILogger<StripeCustomerMigrationJob> logger) : BackgroundService
{
    private readonly string _jobName = typeof(StripeCustomerMigrationJob).FullName!;

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        do
        {
            try
            {
                await using var scope = serviceProvider.CreateAsyncScope();
                var repositoryFactory = scope.ServiceProvider.GetRequiredService<IRepositoryFactory>();
                var stripeCustomers = await repositoryFactory.StripeCustomerRepository.GetAllAsync(cancellationToken);
                foreach  (var stripeCustomer in stripeCustomers)
                {
                    if (stripeCustomer.Customer is not null)
                    {
                        stripeCustomer.CustomerId = stripeCustomer.Customer.Id;
                    }

                    if (stripeCustomer.Organization is not null)
                    {
                        stripeCustomer.OrganizationId = stripeCustomer.Organization.Id;
                    }

                    repositoryFactory.StripeCustomerRepository.Update(stripeCustomer);
                }

                await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
                
                await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to run job: {job}", _jobName);
            }
        } while (true);
    }
}
