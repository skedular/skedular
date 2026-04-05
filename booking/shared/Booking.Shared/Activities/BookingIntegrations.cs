using Api.Shared.Clients.Configurations.Grpc;
using Api.Shared.Services.Grpc.Skedular.Organization.V1;
using Api.Shared.Services.Models;
using Booking.Shared.Mappers;
using Booking.Shared.Publishers;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Booking.Shared.Services.Cache;
using Enterprise.Shared;
using Enterprise.Shared.Database;
using Enterprise.Shared.GraphQL;
using Enterprise.Shared.Grpc;
using Temporalio.Activities;
using Constants = Booking.Shared.GraphQL.Constants;

namespace Booking.Shared.Activities;

public record ReleaseBookingResourcesInput(string BookingId);

public record CalculateBookingDifferentAmountsInput(string BookingId);

public record CalculateRecurringBookingDifferentAmountsInput(string RecurringBookingId);

public class BookingIntegrations(
    OrganizationConfiguration organizationConfiguration,
    OrganizationService.OrganizationServiceClient organizationServiceClient,
    IRepositoryFactory repositoryFactory,
    IDbTransactionBuilder transactionBuilder,
    IBookingResourceSlotsHelperService bookingResourceSlotsHelperService,
    IMapper mapper,
    IOrganizationArrearsBillingPlannerService organizationArrearsBillingPlannerService,
    IBookingOutboxPublisher bookingOutboxPublisher,
    ICachedBookingService cachedBookingService,
    IGraphQlTopicEventSender graphQlTopicEventSender,
    IAccountingInvoiceCancellationService accountingInvoiceCancellationService)
{
    [Activity]
    public async Task CalculateBookingDifferentAmountsAsync(CalculateBookingDifferentAmountsInput args)
    {
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;
        var booking = await repositoryFactory.BookingRepository.GetByIdAsync(args.BookingId, cancellationToken);
        if (booking is null || booking.IsDeleted())
        {
            return;
        }

        ArgumentNullException.ThrowIfNull(booking.MarketplaceBooking);

        var marketplaceBooking = await repositoryFactory.MarketplaceBookingRepository.GetByIdAsync(booking.MarketplaceBooking.Id, cancellationToken);
        if (marketplaceBooking is null || booking.IsDeleted())
        {
            return;
        }

        var organization = await organizationServiceClient.Admin_GetAsync(
            new Admin_GetInput { Id = marketplaceBooking.ProductVersion.Product.Organization.Id },
            organizationConfiguration.ApiKey.CreateMetadata(),
            cancellationToken: cancellationToken);
        ArgumentNullException.ThrowIfNull(organization);

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        var totalMinutes = (decimal)(booking.Until - booking.From).TotalMinutes;
        var totalPrice = marketplaceBooking.ProductPricing.BookingCadence switch
        {
            ProductPricingCadence.OneTime => marketplaceBooking.ProductPricing.Price * marketplaceBooking.Quantity,
            ProductPricingCadence.HalfDay => marketplaceBooking.ProductPricing.Price * marketplaceBooking.Quantity,
            ProductPricingCadence.Daily => marketplaceBooking.ProductPricing.Price * marketplaceBooking.Quantity,
            ProductPricingCadence.PerMinute => marketplaceBooking.ProductPricing.Price * marketplaceBooking.Quantity * totalMinutes,
            ProductPricingCadence.Per15Minutes =>
                marketplaceBooking.ProductPricing.Price * marketplaceBooking.Quantity * (totalMinutes / 15m),
            ProductPricingCadence.Per30Minutes =>
                marketplaceBooking.ProductPricing.Price * marketplaceBooking.Quantity * (totalMinutes / 30m),
            ProductPricingCadence.PerHour =>
                marketplaceBooking.ProductPricing.Price * marketplaceBooking.Quantity * (totalMinutes / 60m),
            _ => throw new ArgumentOutOfRangeException()
        };

        marketplaceBooking.Currency = marketplaceBooking.ProductVersion.Currency;

        if (organization.TaxDetails is null)
        {
            marketplaceBooking.TotalAmountExcludeTax = totalPrice;
            marketplaceBooking.TaxAmount = 0.00m;
            marketplaceBooking.TaxRatePercentage = 0.00m;
            marketplaceBooking.TaxAmount = marketplaceBooking.TotalAmountExcludeTax;
        }
        else
        {
            var isPriceTaxInclusive = marketplaceBooking.ProductPricing.IsTaxInclusive;
            var taxRatePercentage = Convert.ToDecimal(organization.TaxDetails.TaxRatePercentage);
            marketplaceBooking.TaxRatePercentage = taxRatePercentage.RoundedDecimal();

            if (isPriceTaxInclusive)
            {
                marketplaceBooking.TotalAmount = totalPrice.RoundedDecimal();
                marketplaceBooking.TotalAmountExcludeTax = (marketplaceBooking.TotalAmount.Value * 100 / (100 + taxRatePercentage)).RoundedDecimal();
                marketplaceBooking.TaxAmount =
                    (marketplaceBooking.TotalAmount.Value - marketplaceBooking.TotalAmountExcludeTax.Value).RoundedDecimal();
            }
            else
            {
                marketplaceBooking.TotalAmountExcludeTax = totalPrice.RoundedDecimal();
                marketplaceBooking.TaxAmount = (marketplaceBooking.TotalAmountExcludeTax.Value * taxRatePercentage / 100).RoundedDecimal();
                marketplaceBooking.TotalAmount =
                    (marketplaceBooking.TotalAmountExcludeTax.Value + marketplaceBooking.TaxAmount.Value).RoundedDecimal();
            }
        }

        repositoryFactory.MarketplaceBookingRepository.Update(marketplaceBooking);
        bookingOutboxPublisher.PublishBookings([mapper.MapTo(booking)], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        await cachedBookingService.UpdateByIdAsync(booking.Id, cancellationToken);
        await graphQlTopicEventSender.RaiseGraphqlChangeAsync(Constants.BookingTopicName, booking.Id, cancellationToken);
    }

    [Activity]
    public async Task CalculateRecurringBookingDifferentAmountsAsync(CalculateRecurringBookingDifferentAmountsInput args)
    {
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;
        var recurringBooking = await repositoryFactory.RecurringBookingRepository.GetByIdAsync(args.RecurringBookingId, cancellationToken);
        if (recurringBooking is null || recurringBooking.IsDeleted() || recurringBooking.MarketplaceBooking is null)
        {
            return;
        }

        var marketplaceBooking =
            await repositoryFactory.MarketplaceBookingRepository.GetByIdAsync(recurringBooking.MarketplaceBooking.Id, cancellationToken);
        if (marketplaceBooking is null)
        {
            return;
        }

        var organization = await organizationServiceClient.Admin_GetAsync(
            new Admin_GetInput { Id = marketplaceBooking.ProductVersion.Product.Organization.Id },
            organizationConfiguration.ApiKey.CreateMetadata(),
            cancellationToken: cancellationToken);
        ArgumentNullException.ThrowIfNull(organization);

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        var totalPrice = (marketplaceBooking.ProductPricing.Price * marketplaceBooking.Quantity).RoundedDecimal();
        marketplaceBooking.Currency = marketplaceBooking.ProductVersion.Currency;

        if (marketplaceBooking.BillingMode.ToProductPricingBillingMode() == ProductPricingBillingMode.InArrears)
        {
            var recurringBookingModel = mapper.MapTo(recurringBooking);
            var draft = organizationArrearsBillingPlannerService.BuildInitialRecurringInvoiceDraft(
                recurringBookingModel,
                marketplaceBooking.ProductVersion.Product.Organization.BillingCycle.ToOrganizationBillingCycle());
            if (draft is not null)
            {
                totalPrice = draft.TotalAmount.RoundedDecimal();
            }
        }

        if (organization.TaxDetails is null)
        {
            marketplaceBooking.TotalAmountExcludeTax = totalPrice;
            marketplaceBooking.TaxAmount = 0.00m;
            marketplaceBooking.TaxRatePercentage = 0.00m;
            marketplaceBooking.TotalAmount = totalPrice;
        }
        else
        {
            var isPriceTaxInclusive = marketplaceBooking.ProductPricing.IsTaxInclusive;
            var taxRatePercentage = Convert.ToDecimal(organization.TaxDetails.TaxRatePercentage);
            marketplaceBooking.TaxRatePercentage = taxRatePercentage.RoundedDecimal();

            if (isPriceTaxInclusive)
            {
                marketplaceBooking.TotalAmount = totalPrice;
                marketplaceBooking.TotalAmountExcludeTax = (marketplaceBooking.TotalAmount.Value * 100 / (100 + taxRatePercentage)).RoundedDecimal();
                marketplaceBooking.TaxAmount =
                    (marketplaceBooking.TotalAmount.Value - marketplaceBooking.TotalAmountExcludeTax.Value).RoundedDecimal();
            }
            else
            {
                marketplaceBooking.TotalAmountExcludeTax = totalPrice;
                marketplaceBooking.TaxAmount = (marketplaceBooking.TotalAmountExcludeTax.Value * taxRatePercentage / 100).RoundedDecimal();
                marketplaceBooking.TotalAmount =
                    (marketplaceBooking.TotalAmountExcludeTax.Value + marketplaceBooking.TaxAmount.Value).RoundedDecimal();
            }
        }

        repositoryFactory.MarketplaceBookingRepository.Update(marketplaceBooking);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        if (recurringBooking.MarketplaceBookingSubscription is not null)
        {
            await graphQlTopicEventSender.RaiseGraphqlChangeAsync(
                Constants.MarketplaceBookingSubscriptionTopicName,
                recurringBooking.MarketplaceBookingSubscription.Id,
                cancellationToken);
        }
    }

    [Activity]
    public async Task ReleaseBookingResourcesAsync(ReleaseBookingResourcesInput args)
    {
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;
        var booking = await repositoryFactory.BookingRepository.GetByIdAsync(args.BookingId, cancellationToken);
        if (booking is null || booking.IsDeleted() || booking.MarketplaceBooking is null)
        {
            return;
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        var marketplaceBooking = booking.MarketplaceBooking;
        marketplaceBooking.PaymentStatus = marketplaceBooking.StripeCheckoutSession is null
            ? PaymentStatusConstants.RecordNeverCreated
            : PaymentStatusConstants.Expired;

        booking.InvolvedResources.Clear();
        repositoryFactory.BookingRepository.Update(booking);
        repositoryFactory.MarketplaceBookingRepository.Update(marketplaceBooking);

        bookingResourceSlotsHelperService.RemoveAllSlotsFromBooking(booking);

        bookingOutboxPublisher.PublishBookings([mapper.MapTo(booking)], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        await accountingInvoiceCancellationService.CancelBookingAsync(booking, cancellationToken);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        await cachedBookingService.UpdateByIdAsync(booking.Id, cancellationToken);
    }
}
