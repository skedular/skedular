using Api.Shared.Services;
using Api.Shared.Services.Offering;
using Enterprise.Shared.Database;
using Enterprise.Shared.Random;
using Organization.Shared.Database.Entities;
using Organization.Shared.Mappers;
using Organization.Shared.Publishers;
using Organization.Shared.Repositories;
using Organization.Shared.Services;
using Organization.Shared.Workflows;
using Stripe;
using Temporalio.Activities;

namespace Organization.Shared.Activities;

public record PayForOrganizationOffering(string OrganizationOfferingId);

public record RenewAutoRenewableOrganizationOfferingAsyncInput(string OrganizationId, string OrganizationOfferingId);

public class OrganizationOfferings(
    IRepositoryFactory repositoryFactory,
    IDbTransactionBuilder transactionBuilder,
    IRandomHelper randomHelper,
    IMapper mapper,
    TimeProvider timeProvider,
    IOrganizationOutboxPublisher organizationOutboxPublisher,
    ITemporalOutboxService temporalOutboxService,
    ICreatable<PaymentIntent, PaymentIntentCreateOptions> paymentIntentCreateService)
{
    [Activity]
    public async Task PayForOrganizationOfferingAsync(PayForOrganizationOffering args)
    {
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;

        var organizationOffering =
            await repositoryFactory.OrganizationOfferingRepository.GetByIdAsync(args.OrganizationOfferingId, cancellationToken);
        if (organizationOffering is null || organizationOffering.IsDeleted())
        {
            return;
        }

        var totalCost = organizationOffering.OrganizationOfferingActiveMembers.Count * organizationOffering.UnitPrice;
        if (totalCost == 0)
        {
            // The total cost is Zero, no need to try charge customer 
            return;
        }

        if (organizationOffering.Organization.OrganizationStripeCustomer is null ||
            organizationOffering.Organization.OrganizationStripeCustomer.IsDeleted())
        {
            throw new OrganizationStripeCustomerRelationshipIsNotSetYet();
        }

        if (organizationOffering.Organization.OrganizationStripePaymentMethods.Count == 0)
        {
            throw new OrganizationPaymentMethodNotFound();
        }

        var organizationStripePaymentMethod = organizationOffering.Organization.OrganizationStripePaymentMethods.First();
        var paymentIntent = await paymentIntentCreateService.CreateAsync(
            new PaymentIntentCreateOptions
            {
                Customer = organizationOffering.Organization.OrganizationStripeCustomer.StripeCustomerId,
                PaymentMethod = organizationStripePaymentMethod.PaymentMethodId,
                Amount = totalCost,
                // TODO: 20240601 : Morteza: Currency should not be probably hard-coded
                Currency = "usd",
                Confirm = true,
                OffSession = true
            },
            new RequestOptions { IdempotencyKey = organizationOffering.Id },
            cancellationToken);

        repositoryFactory.OrganizationStripePaymentIntentRepository.Add(
            new OrganizationStripePaymentIntent
            {
                Id = paymentIntent.Id,
                OrganizationStripePaymentMethod = organizationStripePaymentMethod,
                Amount = paymentIntent.Amount,
                Currency = paymentIntent.Currency,
                OrganizationOffering = organizationOffering
            });

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    [Activity]
    public async Task RenewAutoRenewableOrganizationOfferingAsync(RenewAutoRenewableOrganizationOfferingAsyncInput args)
    {
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;

        var organization = await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(
            args.OrganizationId,
            null,
            cancellationToken);
        if (organization is null || organization.IsDeleted())
        {
            return;
        }

        var organizationOffering =
            await repositoryFactory.OrganizationOfferingRepository.GetByIdAsync(args.OrganizationOfferingId, cancellationToken);
        if (organizationOffering is null || organizationOffering.IsDeleted() || !organizationOffering.AutoRenew)
        {
            return;
        }

        var now = timeProvider.GetUtcNow();
        if (organizationOffering.End > now)
        {
            return;
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        var offering = organizationOffering.Code.GetOffering();
        var start = organizationOffering.End.GetNextOfferingPeriodStart();
        var newOrganizationOffering = new OrganizationOffering
        {
            Id = randomHelper.Generate(),
            Code = organizationOffering.Code,
            Start = start,
            End = start.GetOfferingPeriodStart().GetOfferingPeriodEnd(),
            AutoRenew = organizationOffering.AutoRenew,
            UnitPrice = offering.UnitPrice,
            Organization = organization
        };
        repositoryFactory.OrganizationOfferingRepository.Add(newOrganizationOffering);
        repositoryFactory.OrganizationOfferingRepository.Remove(organizationOffering);

        var mappedOrganization = mapper.MapTo(organization);
        mappedOrganization.OrganizationOfferings =
        [
            mappedOrganization.OrganizationOfferings.Where(item => !item.DeletedAt.HasValue).OrderByDescending(item => item.End).First()
        ];

        organizationOutboxPublisher.PublishOrganizations([mappedOrganization], repositoryFactory.UnitOfWork);
        temporalOutboxService.StartWorkflowScheduleRenewOrganizationOffering(
            new ScheduleRenewOrganizationOfferingInput(
                organization.Id,
                newOrganizationOffering.Id,
                newOrganizationOffering.End.GetNextOfferingPeriodStart()),
            repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }
}
