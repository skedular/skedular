using Api.Shared.Services.Models;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Database;
using Enterprise.Shared.Exceptions;
using Enterprise.Shared.Random;
using Flurl;
using Microsoft.EntityFrameworkCore;
using Payment.Api.Mappers;
using Payment.Api.Services.Authorization;
using Payment.Shared.Database.Entities;
using Payment.Shared.Publishers;
using Payment.Shared.Repositories;
using Stripe;

namespace Payment.Api.Services;

public interface IOrganizationPaymentService
{
    Task<string> AddPaymentMethodIntentAsync(string id, CancellationToken cancellationToken);
    Task RemovePaymentMethodAsync(string paymentMethodId, CancellationToken cancellationToken);

    Task<string> HandleStripePaymentMethodEventAsync(
        string setupIntentId,
        string clientSecret,
        string redirectStatus,
        CancellationToken cancellationToken);
}

public class OrganizationPaymentService(
    ApplicationConfiguration applicationConfiguration,
    IDbTransactionBuilder transactionBuilder,
    IOrganizationAuthorizationService organizationAuthorizationService,
    IRepositoryFactory repositoryFactory,
    ICustomerService customerService,
    ICreatable<SetupIntent, SetupIntentCreateOptions> setupIntentCreateService,
    IRetrievable<SetupIntent, SetupIntentGetOptions> setupIntentRetrievableService,
    IRetrievable<PaymentMethod, PaymentMethodGetOptions> paymentMethodRetrievableService,
    PaymentMethodService paymentMethodService,
    IRandomHelper randomHelper,
    IPaymentOutboxPublisher paymentOutboxPublisher,
    IMapper mapper) : IOrganizationPaymentService
{
    public async Task<string> AddPaymentMethodIntentAsync(string id, CancellationToken cancellationToken)
    {
        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var organization = await repositoryFactory.OrganizationRepository.GetByIdAsync(id, false, false, cancellationToken);
        if (organization is null)
        {
            throw new OrganizationNotFound();
        }

        if (!organizationAuthorizationService.CanManagePaymentMethod(organization, customer))
        {
            throw new Unauthorized();
        }

        if (organization.StripeCustomer is null)
        {
            throw new OrganizationStripeCustomerRelationshipIsNotSetYet();
        }

        var setupIntent = await setupIntentCreateService.CreateAsync(
            new SetupIntentCreateOptions { Customer = organization.StripeCustomer.StripeCustomerId, PaymentMethodTypes = ["card"] },
            new RequestOptions(),
            cancellationToken);

        repositoryFactory.StripePaymentMethodRepository.Add(
            new StripePaymentMethod
            {
                Id = randomHelper.Generate(),
                SetupIntentId = setupIntent.Id,
                ClientSecret = setupIntent.ClientSecret,
                Status = StripePaymentMethodStatusConstants.Pending,
                Organization = organization
            });

        _ = await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        return setupIntent.ClientSecret;
    }

    public async Task RemovePaymentMethodAsync(string paymentMethodId, CancellationToken cancellationToken)
    {
        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var stripePaymentMethod = await repositoryFactory.StripePaymentMethodRepository.Query(
                new Specification<StripePaymentMethod> { Criteria = query => query.Id == paymentMethodId && query.Organization != null }
                    .AddInclude(query => query.Organization!))
            .FirstAsync(cancellationToken);

        var organization = stripePaymentMethod.Organization;
        organization = await repositoryFactory.OrganizationRepository.GetByIdAsync(organization!.Id, false, false, cancellationToken);
        if (organization is null)
        {
            throw new OrganizationNotFound();
        }

        if (!organizationAuthorizationService.CanManagePaymentMethod(organization, customer))
        {
            throw new Unauthorized();
        }

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
        await PublishOrganizationPaymentMethodStateAsync(organization.Id, cancellationToken);

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
                    Criteria = query => query.SetupIntentId == setupIntentId && query.ClientSecret == clientSecret && query.Organization != null
                }
                .AddInclude(query => query.Organization!))
            .FirstAsync(cancellationToken);

        var organization = organizationStripePaymentMethod.Organization;
        var redirectUrl = Url.Combine(applicationConfiguration.WebAppBaseDomain, "organizations", organization!.Id, "admin");

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        redirectUrl = redirectUrl.SetQueryParam("section", "billing-payment-setup");

        if (redirectStatus != "succeeded")
        {
            redirectUrl = redirectUrl.SetQueryParam("add-payment-method-status", "failed");
            organizationStripePaymentMethod.Status = StripePaymentMethodStatusConstants.Failed;
            repositoryFactory.StripePaymentMethodRepository.Update(organizationStripePaymentMethod);

            await PublishOrganizationPaymentMethodStateAsync(organization.Id, cancellationToken);

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
                        Criteria = query => !query.DeletedAt.HasValue && query.Organization != null && query.Organization.Id == organization.Id &&
                                            query.Status != StripePaymentMethodStatusConstants.Confirmed
                    })
                .ToListAsync(cancellationToken))
            .Except([organizationStripePaymentMethod]).ToList();
        paymentMethodsToRemove.ForEach(item => item.ClientSecret = null);
        repositoryFactory.StripePaymentMethodRepository.PurgeRange(paymentMethodsToRemove);

        repositoryFactory.StripePaymentMethodRepository.Update(organizationStripePaymentMethod);
        await PublishOrganizationPaymentMethodStateAsync(organization.Id, cancellationToken);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return redirectUrl;
    }

    private async Task PublishOrganizationPaymentMethodStateAsync(string organizationId, CancellationToken cancellationToken)
    {
        var organization = await repositoryFactory.OrganizationRepository.GetByIdAsync(organizationId, false, false, cancellationToken);
        if (organization is null)
        {
            throw new OrganizationNotFound();
        }

        var hasAttachedPaymentMethod =
            organization.StripePaymentMethods.Any(item => !item.DeletedAt.HasValue && item.Status == StripePaymentMethodStatusConstants.Confirmed);

        paymentOutboxPublisher.PublishOrganizationPaymentMethodState(organizationId, hasAttachedPaymentMethod, repositoryFactory.UnitOfWork);
    }
}
