using Customer.Api.Mappers;
using Customer.Shared.Models;
using Customer.Shared.Repositories;
using Customer.Shared.Services;
using Customer.Shared.Workflows;
using Enterprise.Shared.Database;
using Enterprise.Shared.Random;

namespace Customer.Api.Services;

public interface ICustomerFeedbackService
{
    Task<CustomerFeedback> SubmitFeedbackAsync(CustomerFeedback feedback, CancellationToken cancellationToken);
}

public class CustomerFeedbackService(
    IDbTransactionBuilder transactionBuilder,
    ICustomerHelperService customerHelperService,
    IRepositoryFactory repositoryFactory,
    IMapper mapper,
    IRandomHelper randomHelper,
    ITemporalOutboxService temporalOutboxService) : ICustomerFeedbackService
{
    public async Task<CustomerFeedback> SubmitFeedbackAsync(CustomerFeedback feedback, CancellationToken cancellationToken)
    {
        var customer = await customerHelperService.GetCustomerAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(feedback.Id))
        {
            feedback.Id = randomHelper.Generate();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        var customerFeedback = mapper.MapTo(repositoryFactory.CustomerFeedbackRepository.Add(mapper.MapTo(feedback, customer)));
        temporalOutboxService.StartWorkflowSubmitCustomerFeedback(
            new SubmitCustomerFeedbackInput(customerFeedback.Id),
            repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return customerFeedback;
    }
}
