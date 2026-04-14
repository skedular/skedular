using Enterprise.Shared.Configurations;
using Microsoft.Extensions.Logging;
using Temporalio.Api.Enums.V1;
using Temporalio.Client;
using Temporalio.Exceptions;

namespace Enterprise.Shared.Temporal;

public interface ITemporalHelperService
{
    string ToId(string id);
    Task<bool> IsRunningAsync<TWorkflow>(string workflowId, CancellationToken cancellationToken);
    Task<bool> DoesWorkflowExistAsync<TWorkflow>(string workflowId, CancellationToken cancellationToken);
}

public class TemporalHelperService(
    ApplicationConfiguration applicationConfiguration,
    ITemporalClient temporalClient,
    ILogger<TemporalHelperService> logger)
    : ITemporalHelperService
{
    public string ToId(string id)
    {
        var resolvedId = string.IsNullOrWhiteSpace(applicationConfiguration.Environment) ? id : $"{applicationConfiguration.Environment}.{id}";
        logger.LogDebug("Resolved Temporal workflow id. EnvironmentConfigured={EnvironmentConfigured}",
            !string.IsNullOrWhiteSpace(applicationConfiguration.Environment));
        return resolvedId;
    }

    public async Task<bool> IsRunningAsync<TWorkflow>(string workflowId, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogDebug("Checking whether Temporal workflow is running. WorkflowType={WorkflowType}", typeof(TWorkflow).FullName);
            var description = await temporalClient
                .GetWorkflowHandle<TWorkflow>(workflowId)
                .DescribeAsync(new WorkflowDescribeOptions { Rpc = new RpcOptions { CancellationToken = cancellationToken } });

            logger.LogInformation("Temporal workflow status retrieved. WorkflowType={WorkflowType}, IsRunning={IsRunning}",
                typeof(TWorkflow).FullName,
                description.Status == WorkflowExecutionStatus.Running);
            return description.Status == WorkflowExecutionStatus.Running;
        }
        catch (RpcException ex)
        {
            if (ex.Code == RpcException.StatusCode.NotFound)
            {
                logger.LogDebug("Temporal workflow was not found while checking running status. WorkflowType={WorkflowType}",
                    typeof(TWorkflow).FullName);
                return false;
            }

            logger.LogWarning("Temporal running-status lookup failed. WorkflowType={WorkflowType}, ExceptionType={ExceptionType}",
                typeof(TWorkflow).FullName,
                ex.GetType().Name);
            throw;
        }
    }

    public async Task<bool> DoesWorkflowExistAsync<TWorkflow>(string workflowId, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogDebug("Checking whether Temporal workflow exists. WorkflowType={WorkflowType}", typeof(TWorkflow).FullName);
            _ = await temporalClient
                .GetWorkflowHandle<TWorkflow>(workflowId)
                .DescribeAsync(new WorkflowDescribeOptions { Rpc = new RpcOptions { CancellationToken = cancellationToken } });

            logger.LogInformation("Temporal workflow exists. WorkflowType={WorkflowType}", typeof(TWorkflow).FullName);
            return true;
        }
        catch (RpcException ex)
        {
            if (ex.Code == RpcException.StatusCode.NotFound)
            {
                logger.LogDebug("Temporal workflow does not exist. WorkflowType={WorkflowType}", typeof(TWorkflow).FullName);
                return false;
            }

            logger.LogWarning(
                "Temporal existence lookup failed. WorkflowType={WorkflowType}, ExceptionType={ExceptionType}",
                typeof(TWorkflow).FullName,
                ex.GetType().Name);
            throw;
        }
    }
}
