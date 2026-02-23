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
using Enterprise.Shared.Grpc;
using Temporalio.Activities;

namespace Booking.Shared.Activities;

public record ReleaseBookingResourcesInput(string BookingId);

public record CalculateBookingDifferentAmountsInput(string BookingId);

public class BookingIntegrations(
    OrganizationConfiguration organizationConfiguration,
    OrganizationService.OrganizationServiceClient organizationServiceClient,
    IRepositoryFactory repositoryFactory,
    IDbTransactionBuilder transactionBuilder,
    IBookingResourceSlotsHelperService bookingResourceSlotsHelperService,
    IMapper mapper,
    IBookingOutboxPublisher bookingOutboxPublisher,
    ICachedBookingService cachedBookingService)
{
    [Activity]
    public async Task CalculateBookingDifferentAmountsAsync(CalculateBookingDifferentAmountsInput args)
    {
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;
        var booking = await repositoryFactory.BookingRepository.GetByIdAsync(args.BookingId, cancellationToken);
        if (booking is null || booking.IsDeleted() || booking.MarketplaceBooking is null)
        {
            return;
        }

        var marketplaceBooking = booking.MarketplaceBooking;
        var productVersionIds = marketplaceBooking.LineItems.Select(item => item.ProductVersionId).Distinct().ToList();
        var productVersions = await repositoryFactory.ProductVersionRepository.GetByIdsAsync(productVersionIds, cancellationToken);
        if (productVersions.Count != productVersionIds.Count)
        {
            throw new InvalidOperationException();
        }

        var currencies = productVersions.Select(item => item.Currency).Distinct().ToList();
        var organizationId = productVersions.First().Product.Organization.Id;
        var organization = await organizationServiceClient.Admin_GetAsync(
            new Admin_GetInput { Id = organizationId },
            organizationConfiguration.ApiKey.CreateMetadata(),
            cancellationToken: cancellationToken);
        ArgumentNullException.ThrowIfNull(organization);

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        marketplaceBooking.Currency = currencies.First();
        var totalPrice = marketplaceBooking.LineItems.Aggregate(0.00m, (acc, lineItem) =>
        {
            var productVersion = productVersions.Single(item => item.Id == lineItem.ProductVersionId);
            if (!productVersion.Price.HasValue)
            {
                throw new ArgumentNullException(nameof(productVersion.Price));
            }

            ArgumentException.ThrowIfNullOrWhiteSpace(productVersion.PriceUnit);
            var totalMinutes = (int)(booking.Until - booking.From).TotalMinutes;
            var price = productVersion.PriceUnit.ToPriceUnit() switch
            {
                PriceUnit.PerMinute => productVersion.Price.Value * lineItem.Quantity * totalMinutes,
                PriceUnit.PerHour => productVersion.Price.Value / 60 * lineItem.Quantity * totalMinutes,
                PriceUnit.PerUse => productVersion.Price.Value * lineItem.Quantity,
                _ => throw new ArgumentOutOfRangeException()
            };

            return acc + price;
        });

        if (organization.TaxDetails is null)
        {
            marketplaceBooking.TotalAmountExcludeTax = totalPrice;
            marketplaceBooking.TaxAmount = 0.00m;
            marketplaceBooking.TaxRatePercentage = 0.00m;
            marketplaceBooking.TaxAmount = marketplaceBooking.TotalAmountExcludeTax;
        }
        else
        {
            var isPriceTaxInclusive = productVersions.First().IsPriceTaxInclusive;
            ArgumentNullException.ThrowIfNull(isPriceTaxInclusive);

            var taxRatePercentage = Convert.ToDecimal(organization.TaxDetails.TaxRatePercentage);
            marketplaceBooking.TaxRatePercentage = taxRatePercentage.RoundedDecimal();

            if (isPriceTaxInclusive.Value)
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

        bookingResourceSlotsHelperService.RemoveAllSlotsFromBooking(booking);

        bookingOutboxPublisher.PublishBookings([mapper.MapTo(booking)], repositoryFactory.UnitOfWork);

        repositoryFactory.MarketplaceBookingRepository.Update(marketplaceBooking);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        await cachedBookingService.UpdateByIdAsync(booking.Id, cancellationToken);
    }
}
