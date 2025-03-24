using Enterprise.Shared.Configurations;
using Enterprise.Shared.Database;
using Enterprise.Shared.Exceptions;
using Enterprise.Shared.Random;
using Flurl;
using Microsoft.EntityFrameworkCore;
using Payment.Api.Mappers;
using Payment.Api.Services.Authorization;
using Payment.Shared.Models;
using Payment.Shared.Publishers;
using Payment.Shared.Repositories;
using Stripe;
using OrganizationStripePaymentMethod = Payment.Shared.Database.Entities.OrganizationStripePaymentMethod;

namespace Payment.Api.Services;

public interface IPaymentService
{
    Task<string> AddOrganizationPaymentMethodIntentAsync(
        string organizationId,
        CancellationToken cancellationToken);

    Task<string> AddOrganizationPaymentMethodAsync(
        string setupIntentId,
        string clientSecret,
        string redirectStatus,
        CancellationToken cancellationToken);

    Task RemoveOrganizationPaymentMethodAsync(
        string paymentMethodId,
        CancellationToken cancellationToken);
}

public class PaymentService(
    ApplicationConfiguration applicationConfiguration,
    IDbTransactionBuilder transactionBuilder,
    IOrganizationAuthorizationService organizationAuthorizationService,
    IRepositoryFactory repositoryFactory,
    ICustomerService customerService,
    ICreatable<SetupIntent, SetupIntentCreateOptions> stripeSetupIntentCreateService,
    IRetrievable<SetupIntent, SetupIntentGetOptions> stripeSetupIntentRetrievableService,
    IRetrievable<PaymentMethod, PaymentMethodGetOptions> stripePaymentMethodRetrievableService,
    IRandomHelper randomHelper,
    IPaymentOutboxPublisher paymentOutboxPublisher,
    IMapper mapper) : IPaymentService
{
    public async Task<string> AddOrganizationPaymentMethodIntentAsync(string organizationId, CancellationToken cancellationToken)
    {
        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var organization =
            await repositoryFactory.OrganizationRepository.GetByIdAsync(organizationId, cancellationToken);
        if (organization is null)
        {
            throw new OrganizationNotFound();
        }

        if (!organizationAuthorizationService.CanManagePaymentMethod(organization, customer))
        {
            throw new Unauthorized();
        }

        var setupIntent = await stripeSetupIntentCreateService.CreateAsync(
            new SetupIntentCreateOptions { Customer = organization.StripeCustomerId, PaymentMethodTypes = ["card"] },
            new RequestOptions(), cancellationToken);

        repositoryFactory.OrganizationStripePaymentMethodRepository.Add(
            new OrganizationStripePaymentMethod
            {
                Id = randomHelper.Generate(),
                SetupIntentId = setupIntent.Id,
                ClientSecret = setupIntent.ClientSecret,
                Status = OrganizationStripePaymentMethodStatus.Pending,
                Organization = organization
            });

        _ = await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        return setupIntent.ClientSecret;
    }

    public async Task<string> AddOrganizationPaymentMethodAsync(
        string setupIntentId,
        string clientSecret,
        string redirectStatus,
        CancellationToken cancellationToken)
    {
        var organizationStripePaymentMethod = await repositoryFactory.OrganizationStripePaymentMethodRepository
            .Query(new Specification<OrganizationStripePaymentMethod>
            {
                Criteria = query => query.SetupIntentId == setupIntentId && query.ClientSecret == clientSecret
            }.AddInclude(query => query.Organization)).FirstAsync(cancellationToken);

        var redirectUrl = Url.Combine(
            applicationConfiguration.WebAppBaseDomain,
            "organizations",
            organizationStripePaymentMethod.Organization.Id,
            "admin");

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        redirectUrl = redirectUrl.SetQueryParam("section", "billing-payment-setup");

        if (redirectStatus != "succeeded")
        {
            redirectUrl = redirectUrl.SetQueryParam("add-payment-method-status", "failed");
            organizationStripePaymentMethod.Status = OrganizationStripePaymentMethodStatus.Failed;
            repositoryFactory.OrganizationStripePaymentMethodRepository.Update(organizationStripePaymentMethod);

            await PublishOrganizationPaymentMethodStateAsync(organizationStripePaymentMethod.Organization.Id, cancellationToken);
            await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return redirectUrl;
        }

        redirectUrl = redirectUrl.SetQueryParam("add-payment-method-status", "added");
        organizationStripePaymentMethod.Status = OrganizationStripePaymentMethodStatus.Confirmed;

        var setupIntent = await stripeSetupIntentRetrievableService.GetAsync(setupIntentId, cancellationToken: cancellationToken);
        ArgumentNullException.ThrowIfNull(setupIntent);
        ArgumentNullException.ThrowIfNull(setupIntent.PaymentMethodId);

        var paymentMethod = await stripePaymentMethodRetrievableService.GetAsync(setupIntent.PaymentMethodId, cancellationToken: cancellationToken);
        ArgumentNullException.ThrowIfNull(paymentMethod);
        ArgumentNullException.ThrowIfNull(paymentMethod.Card);

        organizationStripePaymentMethod = mapper.MergeTo(paymentMethod, organizationStripePaymentMethod);

        var paymentMethodsToRemove = (await repositoryFactory.OrganizationStripePaymentMethodRepository.Query(
                    new Specification<OrganizationStripePaymentMethod>
                    {
                        Criteria = query =>
                            !query.DeletedAt.HasValue &&
                            query.Organization.Id == organizationStripePaymentMethod.Organization.Id &&
                            query.Status != OrganizationStripePaymentMethodStatus.Confirmed
                    })
                .ToListAsync(cancellationToken))
            .Except([organizationStripePaymentMethod]).ToList();
        paymentMethodsToRemove.ForEach(item => item.ClientSecret = null);
        repositoryFactory.OrganizationStripePaymentMethodRepository.RemoveRange(paymentMethodsToRemove);

        repositoryFactory.OrganizationStripePaymentMethodRepository.Update(organizationStripePaymentMethod);
        await PublishOrganizationPaymentMethodStateAsync(organizationStripePaymentMethod.Organization.Id, cancellationToken);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return redirectUrl;
    }

    public async Task RemoveOrganizationPaymentMethodAsync(string paymentMethodId, CancellationToken cancellationToken)
    {
        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var organizationStripePaymentMethod = await repositoryFactory.OrganizationStripePaymentMethodRepository.Query(
            new Specification<OrganizationStripePaymentMethod> { Criteria = query => query.Id == paymentMethodId }
                .AddInclude(query =>
                    query.Organization)).FirstAsync(cancellationToken);
        var organization =
            await repositoryFactory.OrganizationRepository.GetByIdAsync(organizationStripePaymentMethod.Organization.Id, cancellationToken);
        if (organization is null)
        {
            throw new OrganizationNotFound();
        }

        if (!organizationAuthorizationService.CanManagePaymentMethod(organization, customer))
        {
            throw new Unauthorized();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        _ = repositoryFactory.OrganizationStripePaymentMethodRepository.Remove(organizationStripePaymentMethod);
        _ = await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await PublishOrganizationPaymentMethodStateAsync(organizationStripePaymentMethod.Organization.Id, cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    private async Task PublishOrganizationPaymentMethodStateAsync(string organizationId, CancellationToken cancellationToken)
    {
        var organization = await repositoryFactory.OrganizationRepository.GetByIdAsync(organizationId, cancellationToken);
        if (organization is null)
        {
            throw new OrganizationNotFound();
        }

        var hasAttachedPaymentMethod = organization.OrganizationStripePaymentMethods.Any(item =>
            !item.DeletedAt.HasValue && item.Status == OrganizationStripePaymentMethodStatus.Confirmed);

        await paymentOutboxPublisher.PublishOrganizationPaymentMethodStateAsync(
            organizationId,
            hasAttachedPaymentMethod,
            repositoryFactory.UnitOfWork,
            cancellationToken);
    }
}
