using Slack.Shared.Models;
using Customer = Slack.Shared.Models.Customer;

namespace Slack.Shared.Services;

public interface IWorkspaceMemberService
{
    string GetMentionedCustomerNameInSlackFormat(
        Workspace workspace,
        ICollection<string> identities,
        Customer customer);
}

public class WorkspaceMemberService : IWorkspaceMemberService
{
    public string GetMentionedCustomerNameInSlackFormat(
        Workspace workspace,
        ICollection<string> identities,
        Customer customer)
    {
        var workspaceMember = workspace.WorkspaceMembers.FirstOrDefault(item => identities.Contains(item.Id));
        return workspaceMember is null ? customer.GetCustomerName() : $"<@{workspaceMember.Id}>";
    }
}
