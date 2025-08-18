using Enterprise.Shared.Configurations;
using Temporalio.Api.Enums.V1;
using Temporalio.Client;

namespace Enterprise.Shared.Temporal;

public interface ITemporalHelperService
{
    string ToId(string id);
    Task<bool> IsRunningAsync<TWorkflow>(string workflowId, CancellationToken cancellationToken);
}

public class TemporalHelperService(ApplicationConfiguration applicationConfiguration, ITemporalClient temporalClient) : ITemporalHelperService
{
    public string ToId(string id) =>
        string.IsNullOrWhiteSpace(applicationConfiguration.Environment) ? id : $"{applicationConfiguration.Environment}.{id}";

    public async Task<bool> IsRunningAsync<TWorkflow>(string workflowId, CancellationToken cancellationToken)
    {
        var handle = temporalClient.GetWorkflowHandle<TWorkflow>(workflowId);

        var description = await handle.DescribeAsync(new WorkflowDescribeOptions { Rpc = new RpcOptions { CancellationToken = cancellationToken } });

        return description.Status == WorkflowExecutionStatus.Running;
    }
}
