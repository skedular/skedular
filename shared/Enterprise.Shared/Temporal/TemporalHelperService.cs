using Enterprise.Shared.Configurations;
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

public class TemporalHelperService(ApplicationConfiguration applicationConfiguration, ITemporalClient temporalClient) : ITemporalHelperService
{
    public string ToId(string id) =>
        string.IsNullOrWhiteSpace(applicationConfiguration.Environment) ? id : $"{applicationConfiguration.Environment}.{id}";

    public async Task<bool> IsRunningAsync<TWorkflow>(string workflowId, CancellationToken cancellationToken)
    {
        try
        {
            var description = await temporalClient
                .GetWorkflowHandle<TWorkflow>(workflowId)
                .DescribeAsync(new WorkflowDescribeOptions { Rpc = new RpcOptions { CancellationToken = cancellationToken } });

            return description.Status == WorkflowExecutionStatus.Running;
        }
        catch (RpcException ex)
        {
            if (ex.Code == RpcException.StatusCode.NotFound)
            {
                return false;
            }

            throw;
        }
    }

    public async Task<bool> DoesWorkflowExistAsync<TWorkflow>(string workflowId, CancellationToken cancellationToken)
    {
        try
        {
            _ = await temporalClient
                .GetWorkflowHandle<TWorkflow>(workflowId)
                .DescribeAsync(new WorkflowDescribeOptions { Rpc = new RpcOptions { CancellationToken = cancellationToken } });

            return true;
        }
        catch (RpcException ex)
        {
            if (ex.Code == RpcException.StatusCode.NotFound)
            {
                return false;
            }

            throw;
        }
    }
}
