using Api.Shared.Services.Offering;
using Enterprise.Shared.Temporal.Configurations;
using Organization.Shared.Repositories;
using Organization.Shared.Workflows;
using Temporalio.Api.Enums.V1;
using Temporalio.Client;
using Temporalio.Exceptions;

namespace Organization.Jobs.Jobs;

public class MigrateOfferingToTemporalWorkflowJob(
    IServiceProvider serviceProvider,
    TemporalConfiguration temporalConfiguration,
    ILogger<MigrateOfferingToTemporalWorkflowJob> logger)
    : BackgroundService
{
    private readonly string _jobName = typeof(MigrateOfferingToTemporalWorkflowJob).FullName!;

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        do
        {
            try
            {
                await using var scope = serviceProvider.CreateAsyncScope();
                var repositoryFactory = scope.ServiceProvider.GetRequiredService<IRepositoryFactory>();
                var temporalClient = scope.ServiceProvider.GetRequiredService<ITemporalClient>();
                var organizationOfferings = await repositoryFactory.OrganizationOfferingRepository.GetActiveOfferingsAsync(cancellationToken);

                foreach (var organizationOffering in organizationOfferings)
                {
                    try
                    {
                        _ = await temporalClient.StartWorkflowAsync(
                            (ScheduleRenewOrganizationOffering workflow) =>
                                workflow.ExecuteAsync(new ScheduleRenewOrganizationOfferingInput(organizationOffering.Organization.Id,
                                    organizationOffering.Id, organizationOffering.End.GetNextOfferingPeriodStart())),
                            new WorkflowOptions
                            {
                                Id = organizationOffering.Id,
                                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                                RetryPolicy = null,
                                IdReusePolicy = WorkflowIdReusePolicy.AllowDuplicateFailedOnly,
                                Rpc = new RpcOptions { CancellationToken = cancellationToken }
                            });
                    }
                    catch (WorkflowAlreadyStartedException)
                    {
                    }
                }

                await Task.Delay(TimeSpan.FromMinutes(15), cancellationToken);
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
