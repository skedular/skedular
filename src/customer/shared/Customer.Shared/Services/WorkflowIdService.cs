using Customer.Shared.Workflows;
using Enterprise.Shared.Temporal;

namespace Customer.Shared.Services;

public interface IWorkflowIdService
{
    string AddCustomerStripePaymentMethod(string clientSecret);
    string SubmitCustomerFeedback(string customerFeedbackId);
    string NewCustomerJoined(string customerId);
}

public class WorkflowIdService(ITemporalHelperService temporalHelperService) : IWorkflowIdService
{
    public string AddCustomerStripePaymentMethod(string clientSecret) =>
        temporalHelperService.ToId(clientSecret);

    public string SubmitCustomerFeedback(string customerFeedbackId) =>
        temporalHelperService.ToId(customerFeedbackId);

    public string NewCustomerJoined(string customerId) =>
        temporalHelperService.ToId($"{Constants.NewCustomerJoinedPrefix}-{customerId}");
}
