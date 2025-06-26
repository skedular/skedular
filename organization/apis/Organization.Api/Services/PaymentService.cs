using Api.Shared.Services;
using Api.Shared.Services.Offering;
using Enterprise.Shared.Database;
using Enterprise.Shared.Random;
using Enterprise.Shared.Temporal.Configurations;
using Microsoft.EntityFrameworkCore;
using Organization.Api.Services.Authorization;
using Organization.Shared.Database.Entities;
using Organization.Shared.Publishers;
using Organization.Shared.Repositories;
using Organization.Shared.Workflows;
using Stripe;
using Temporalio.Api.Enums.V1;
using Temporalio.Client;

namespace Organization.Api.Services;

public interface IPaymentService
{
    Task<string> HandleStripePaymentMethodEventAsync(
        string setupIntentId,
        string clientSecret,
        string redirectStatus,
        CancellationToken cancellationToken);

    Task<string> AddPaymentMethodIntentAsync(string organizationId, CancellationToken cancellationToken);
    Task RemovePaymentMethodAsync(string paymentMethodId, CancellationToken cancellationToken);
}

public class PaymentService(
    IDbTransactionBuilder transactionBuilder,
    IOrganizationAuthorizationService organizationAuthorizationService,
    IRepositoryFactory repositoryFactory,
    ICustomerService customerService,
    ICreatable<SetupIntent, SetupIntentCreateOptions> setupIntentCreateService,
    IRetrievable<PaymentMethod, PaymentMethodGetOptions> paymentMethodRetrievableService,
    PaymentMethodService paymentMethodService,
    IStripeCustomerService stripeCustomerService,
    TimeProvider timeProvider,
    IRandomHelper randomHelper,
    IOrganizationOutboxPublisher organizationOutboxPublisher,
    TemporalConfiguration temporalConfiguration,
    ITemporalClient temporalClient) : IPaymentService
{
    public async Task<string> HandleStripePaymentMethodEventAsync(
        string setupIntentId,
        string clientSecret,
        string redirectStatus,
        CancellationToken cancellationToken)
    {
        var handle = temporalClient.GetWorkflowHandle<AddOrganizationStripePaymentMethod>(clientSecret);

        ArgumentNullException.ThrowIfNull(handle);

        await handle.SignalAsync(
            workflow => workflow.StripePaymentMethodEventReceivedAsync(new StripePaymentMethodEventState(redirectStatus)),
            new WorkflowSignalOptions { Rpc = new RpcOptions { CancellationToken = cancellationToken } }
        );

        return await handle.GetResultAsync<string>(rpcOptions: new RpcOptions { CancellationToken = cancellationToken });
    }

    public async Task<string> AddPaymentMethodIntentAsync(string organizationId, CancellationToken cancellationToken)
    {
        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var organization = await repositoryFactory.OrganizationRepository.GetByIdAsync(organizationId, cancellationToken) ??
                           throw new OrganizationNotFound();
        if (!organizationAuthorizationService.CanManagePaymentMethod(organization, customer))
        {
            throw new UnauthorizedAccessException();
        }

        organization.OrganizationStripeCustomer ??= await stripeCustomerService.AddAsync(organization.Id, cancellationToken);

        var setupIntent = await setupIntentCreateService.CreateAsync(
            new SetupIntentCreateOptions { Customer = organization.OrganizationStripeCustomer.StripeCustomerId, PaymentMethodTypes = ["card"] },
            new RequestOptions(),
            cancellationToken);

        _ = await temporalClient.StartWorkflowAsync(
            (AddOrganizationStripePaymentMethod workflow) =>
                workflow.ExecuteAsync(new AddOrganizationStripePaymentMethodInput(organization.Id, setupIntent.ClientSecret, setupIntent.Id)),
            new WorkflowOptions
            {
                Id = setupIntent.ClientSecret,
                TaskQueue = temporalConfiguration.Worker.TaskQueue,
                RetryPolicy = null,
                IdReusePolicy = WorkflowIdReusePolicy.AllowDuplicateFailedOnly,
                Rpc = new RpcOptions { CancellationToken = cancellationToken }
            });

        return setupIntent.ClientSecret;
    }

    public async Task RemovePaymentMethodAsync(string paymentMethodId, CancellationToken cancellationToken)
    {
        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var organizationStripePaymentMethod =
            await repositoryFactory.OrganizationStripePaymentMethodRepository.GetByIdAsync(paymentMethodId, cancellationToken) ??
            throw new OrganizationPaymentMethodNotFound();
        var organization = organizationStripePaymentMethod.Organization;
        organization = await repositoryFactory.OrganizationRepository.GetByIdAsync(organization.Id, cancellationToken) ??
                       throw new OrganizationNotFound();
        if (!organizationAuthorizationService.CanManagePaymentMethod(organization, customer))
        {
            throw new UnauthorizedAccessException();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        repositoryFactory.OrganizationStripePaymentMethodRepository.Remove(organizationStripePaymentMethod);
        repositoryFactory.OrganizationRepository.Update(organization);

        if (organization.OrganizationStripePaymentMethods.All(item => item.IsDeleted()))
        {
            var now = timeProvider.GetUtcNow();
            var organizationOffering = await repositoryFactory.OrganizationOfferingRepository
                .Query(new Specification<OrganizationOffering>
                    {
                        Criteria = query =>
                            !query.DeletedAt.HasValue && query.Organization.Id == organization.Id && query.Start <= now && query.End >= now
                    }
                    .ApplyOrderBy(query => query.Id))
                .FirstOrDefaultAsync(cancellationToken);
            if (organizationOffering is not null)
            {
                if (organizationOffering.Code.IsFreeOffering())
                {
                    await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

                    return;
                }

                // If current active offering is not free tier, it needs to be deleted
                repositoryFactory.OrganizationOfferingRepository.Remove(organizationOffering);
            }

            // Looking for an existing offering to avoid creating a duplicated offering as well as making sure we are not
            // losing track of active users against a free offering
            var existingFreeOffering = await repositoryFactory.OrganizationOfferingRepository
                .Query(new Specification<OrganizationOffering>
                    {
                        Criteria = query =>
                            query.Organization.Id == organization.Id && query.Start <= now && query.End >= now &&
                            query.Code == OfferingCode.FreeTierV1
                    }
                    .ApplyOrderBy(query => query.Id))
                .FirstOrDefaultAsync(cancellationToken);

            if (existingFreeOffering is null)
            {
                var newOrganizationOffering = new OrganizationOffering
                {
                    Id = randomHelper.Generate(),
                    Code = OfferingCode.FreeTierV1,
                    Organization = organization,
                    Start = now,
                    End = now.GetOfferingPeriodStart().GetOfferingPeriodEnd(),
                    AutoRenew = true,
                    UnitPrice = OfferingCode.FreeTierV1.GetOffering().UnitPrice
                };

                repositoryFactory.OrganizationOfferingRepository.Add(newOrganizationOffering);

                organizationOutboxPublisher.ExecuteWorkflowScheduleRenewOrganizationOffering(
                    new ScheduleRenewOrganizationOfferingInput(
                        organization.Id,
                        newOrganizationOffering.Id,
                        newOrganizationOffering.End.GetNextOfferingPeriodStart()),
                    repositoryFactory.UnitOfWork);
            }
            else
            {
                repositoryFactory.OrganizationOfferingRepository.Undelete(existingFreeOffering);

                organizationOutboxPublisher.ExecuteWorkflowScheduleRenewOrganizationOffering(
                    new ScheduleRenewOrganizationOfferingInput(
                        organization.Id,
                        existingFreeOffering.Id,
                        existingFreeOffering.End.GetNextOfferingPeriodStart()),
                    repositoryFactory.UnitOfWork);
            }
        }

        var paymentMethod =
            await paymentMethodRetrievableService.GetAsync(organizationStripePaymentMethod.PaymentMethodId, cancellationToken: cancellationToken);
        if (paymentMethod is not null)
        {
            await paymentMethodService.DetachAsync(
                organizationStripePaymentMethod.PaymentMethodId,
                new PaymentMethodDetachOptions(),
                new RequestOptions { IdempotencyKey = $"DetachPaymentMethod-{organizationStripePaymentMethod.Id}" },
                cancellationToken);
        }

        _ = await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }
}
