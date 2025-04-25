using Api.Shared.Services.Models;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Database;
using Enterprise.Shared.Exceptions;
using Enterprise.Shared.Random;
using Flurl;
using Microsoft.EntityFrameworkCore;
using Payment.Api.Mappers;
using Payment.Shared.Database.Entities;
using Payment.Shared.Publishers;
using Payment.Shared.Repositories;
using Stripe;

namespace Payment.Api.Services;

public interface ICustomerPaymentService
{
    Task<string> AddPaymentMethodIntentAsync(CancellationToken cancellationToken);
    Task RemovePaymentMethodAsync(string paymentMethodId, CancellationToken cancellationToken);

    Task<string> HandleStripePaymentMethodEventAsync(
        string setupIntentId,
        string clientSecret,
        string redirectStatus,
        CancellationToken cancellationToken);
}

public class CustomerPaymentService(
    ApplicationConfiguration applicationConfiguration,
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    ICustomerService customerService,
    ICreatable<SetupIntent, SetupIntentCreateOptions> setupIntentCreateService,
    IRetrievable<SetupIntent, SetupIntentGetOptions> setupIntentRetrievableService,
    IRetrievable<PaymentMethod, PaymentMethodGetOptions> paymentMethodRetrievableService,
    PaymentMethodService paymentMethodService,
    IRandomHelper randomHelper,
    IPaymentOutboxPublisher paymentOutboxPublisher,
    IMapper mapper) : ICustomerPaymentService
{
    public async Task<string> AddPaymentMethodIntentAsync(CancellationToken cancellationToken)
    {
        var (customer, customerEntity) = await customerService.GetCustomerAsync(cancellationToken);
        if (customer.StripeCustomer is null)
        {
            throw new CustomerStripeCustomerRelationshipIsNotSetYet();
        }

        var setupIntent = await setupIntentCreateService.CreateAsync(
            new SetupIntentCreateOptions { Customer = customer.StripeCustomer.StripeCustomerId, PaymentMethodTypes = ["card"] },
            new RequestOptions(),
            cancellationToken);

        repositoryFactory.StripePaymentMethodRepository.Add(
            new StripePaymentMethod
            {
                Id = randomHelper.Generate(),
                SetupIntentId = setupIntent.Id,
                ClientSecret = setupIntent.ClientSecret,
                Status = StripePaymentMethodStatusConstants.Pending,
                Customer = customerEntity
            });

        _ = await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        return setupIntent.ClientSecret;
    }

    public async Task RemovePaymentMethodAsync(string paymentMethodId, CancellationToken cancellationToken)
    {
        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var stripePaymentMethod = await repositoryFactory.StripePaymentMethodRepository.Query(
                new Specification<StripePaymentMethod>
                    {
                        Criteria = query => query.Id == paymentMethodId && query.Customer != null && query.Customer.Id == customer.Id
                    }
                    .AddInclude(query => query.Customer!))
            .FirstAsync(cancellationToken);

        var paymentMethod = await paymentMethodRetrievableService.GetAsync(stripePaymentMethod.PaymentMethodId, cancellationToken: cancellationToken);
        if (paymentMethod is not null)
        {
            await paymentMethodService.DetachAsync(
                stripePaymentMethod.PaymentMethodId,
                new PaymentMethodDetachOptions(),
                new RequestOptions(),
                cancellationToken);
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        _ = repositoryFactory.StripePaymentMethodRepository.Remove(stripePaymentMethod);
        await PublishCustomerPaymentMethodStateAsync(customer.Id, cancellationToken);

        _ = await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    public async Task<string> HandleStripePaymentMethodEventAsync(
        string setupIntentId,
        string clientSecret,
        string redirectStatus,
        CancellationToken cancellationToken)
    {
        var organizationStripePaymentMethod = await repositoryFactory.StripePaymentMethodRepository
            .Query(new Specification<StripePaymentMethod>
                {
                    Criteria = query => query.SetupIntentId == setupIntentId && query.ClientSecret == clientSecret && query.Customer != null
                }
                .AddInclude(query => query.Customer!))
            .FirstAsync(cancellationToken);

        var customer = organizationStripePaymentMethod.Customer;
        var redirectUrl = Url.Combine(applicationConfiguration.WebAppBaseDomain, "me");

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        redirectUrl = redirectUrl.SetQueryParam("section", "billing-payment-setup");

        if (redirectStatus != "succeeded")
        {
            redirectUrl = redirectUrl.SetQueryParam("add-payment-method-status", "failed");
            organizationStripePaymentMethod.Status = StripePaymentMethodStatusConstants.Failed;
            repositoryFactory.StripePaymentMethodRepository.Update(organizationStripePaymentMethod);

            await PublishCustomerPaymentMethodStateAsync(customer!.Id, cancellationToken);

            await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return redirectUrl;
        }

        redirectUrl = redirectUrl.SetQueryParam("add-payment-method-status", "added");
        organizationStripePaymentMethod.Status = StripePaymentMethodStatusConstants.Confirmed;

        var setupIntent = await setupIntentRetrievableService.GetAsync(setupIntentId, cancellationToken: cancellationToken);
        ArgumentNullException.ThrowIfNull(setupIntent);
        ArgumentNullException.ThrowIfNull(setupIntent.PaymentMethodId);

        var paymentMethod = await paymentMethodRetrievableService.GetAsync(setupIntent.PaymentMethodId, cancellationToken: cancellationToken);
        ArgumentNullException.ThrowIfNull(paymentMethod);
        ArgumentNullException.ThrowIfNull(paymentMethod.Card);

        organizationStripePaymentMethod = mapper.MergeTo(paymentMethod, organizationStripePaymentMethod);

        var paymentMethodsToRemove = (await repositoryFactory.StripePaymentMethodRepository.Query(
                    new Specification<StripePaymentMethod>
                    {
                        Criteria = query => !query.DeletedAt.HasValue && query.Customer != null && query.Customer.Id == customer!.Id &&
                                            query.Status != StripePaymentMethodStatusConstants.Confirmed
                    })
                .ToListAsync(cancellationToken))
            .Except([organizationStripePaymentMethod]).ToList();
        paymentMethodsToRemove.ForEach(item => item.ClientSecret = null);
        repositoryFactory.StripePaymentMethodRepository.PurgeRange(paymentMethodsToRemove);

        repositoryFactory.StripePaymentMethodRepository.Update(organizationStripePaymentMethod);
        await PublishCustomerPaymentMethodStateAsync(customer!.Id, cancellationToken);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return redirectUrl;
    }

    private async Task PublishCustomerPaymentMethodStateAsync(string customerId, CancellationToken cancellationToken)
    {
        var customer = await repositoryFactory.CustomerRepository.GetByIdAsync(customerId, cancellationToken);
        if (customer is null)
        {
            throw new CustomerNotFound();
        }

        var hasAttachedPaymentMethod =
            customer.StripePaymentMethods.Any(item => !item.DeletedAt.HasValue && item.Status == StripePaymentMethodStatusConstants.Confirmed);

        paymentOutboxPublisher.PublishCustomerPaymentMethodState(customerId, hasAttachedPaymentMethod, repositoryFactory.UnitOfWork);
    }
}
