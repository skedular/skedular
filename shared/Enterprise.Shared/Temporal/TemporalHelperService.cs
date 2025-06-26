using Temporalio.Api.Enums.V1;
using Temporalio.Client;

namespace Enterprise.Shared.Temporal;

public interface ITemporalHelperService
{
    Task<bool> IsRunningAsync<TWorkflow>(string workflowId, CancellationToken cancellationToken);
}

public class TemporalHelperService(ITemporalClient temporalClient) : ITemporalHelperService
{
    public async Task<bool> IsRunningAsync<TWorkflow>(string workflowId, CancellationToken cancellationToken)
    {
        var handle = temporalClient.GetWorkflowHandle<TWorkflow>(workflowId);

        var description = await handle.DescribeAsync(new WorkflowDescribeOptions { Rpc = new RpcOptions { CancellationToken = cancellationToken } });

        return description.Status == WorkflowExecutionStatus.Running;
    }
}
