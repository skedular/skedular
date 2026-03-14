using Api.Shared.Services;
using Api.Shared.Services.Models;
using Booking.Shared.Database.Entities;
using Booking.Shared.Mappers;
using Booking.Shared.Repositories;
using Booking.Shared.Workflows;
using Enterprise.Shared.Database;
using Enterprise.Shared.Random;
using RecurringBooking = Booking.Shared.Models.RecurringBooking;

namespace Booking.Shared.Services;

public interface IMarketplaceRecurringBookingService
{
    Task<RecurringBooking> AddAsync(
        RecurringBooking recurringBooking,
        Customer customer,
        ICollection<Organization> organizations,
        ICollection<Team> teams,
        CancellationToken cancellationToken);

    Task<RecurringBooking> DeleteAsync(
        Database.Entities.RecurringBooking existingRecurringBooking,
        Customer? deletedByCustomer,
        CancellationToken cancellationToken);
}

public class MarketplaceRecurringBookingService(
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    IMapper mapper,
    IRandomHelper randomHelper,
    ITemporalOutboxService temporalOutboxService,
    IProductVersionHelperService productVersionHelperService) : IMarketplaceRecurringBookingService
{
    public async Task<RecurringBooking> AddAsync(
        RecurringBooking recurringBooking,
        Customer customer,
        ICollection<Organization> organizations,
        ICollection<Team> teams,
        CancellationToken cancellationToken)
    {
        var customerIds = recurringBooking.InvolvedCustomers.Select(item => item.Id).Distinct().ToList();
        var customerEntities = await repositoryFactory.CustomerRepository.GetByIdsAsync(customerIds, true, cancellationToken);
        if (customerEntities.Count != customerIds.Count)
        {
            throw new CustomerNotFound();
        }

        var marketplaceBooking = recurringBooking.MarketplaceBooking;
        ArgumentNullException.ThrowIfNull(marketplaceBooking);

        var productVersion = await repositoryFactory.ProductVersionRepository.GetByIdAsync(marketplaceBooking.ProductVersion.Id, cancellationToken) ??
                             throw new ProductVersionNotFound();
        if (productVersion.OrganizationTags.All(item => item.Type != OrganizationTagTypeConstants.Product))
        {
            throw new ProductMissingProductTag();
        }

        ArgumentNullException.ThrowIfNull(productVersion.PricingOptions);

        marketplaceBooking.ProductPricing =
            productVersionHelperService.FindMatchingPricing(productVersion.PricingOptions, marketplaceBooking.ProductPricing) ??
            throw new ProductPricingNotFound();
        marketplaceBooking.BillingSchedule = ResolveBillingSchedule(marketplaceBooking.ProductPricing, marketplaceBooking.BillingSchedule);
        if (!IsRecurringPurchaseCadence(marketplaceBooking.ProductPricing.Cadence))
        {
            throw new MarketplaceRecurringBookingCadenceMustBeRecurring();
        }

        marketplaceBooking.Id = randomHelper.Generate();
        marketplaceBooking.IsPaymentRequired = true;
        marketplaceBooking.PaymentStatus = PaymentStatus.NotSet;

        if (!marketplaceBooking.ProductPricing.AcceptedPaymentMethods.Contains(marketplaceBooking.PaymentMethod))
        {
            throw new BookingPaymentMethodNotAccepted();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        var marketplaceBookingEntity = mapper.MapTo(
            marketplaceBooking,
            customer,
            null,
            productVersion,
            null);

        marketplaceBookingEntity = repositoryFactory.MarketplaceBookingRepository.Add(marketplaceBookingEntity);

        var recurringBookingEntity = mapper.MapTo(
            recurringBooking,
            customerEntities,
            organizations,
            teams,
            customer,
            null,
            null,
            marketplaceBookingEntity);

        recurringBookingEntity.Channel = BookingChannelConstants.Marketplace;

        recurringBookingEntity = repositoryFactory.RecurringBookingRepository.Add(recurringBookingEntity);
        recurringBooking = mapper.MapTo(recurringBookingEntity);

        temporalOutboxService.StartBookMarketplaceRecurringResources(
            new BookMarketplaceRecurringResourcesInput(recurringBooking.Id),
            repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return recurringBooking;
    }

    public async Task<RecurringBooking> DeleteAsync(
        Database.Entities.RecurringBooking existingRecurringBooking,
        Customer? deletedByCustomer,
        CancellationToken cancellationToken)
    {
        if (existingRecurringBooking.Channel.ToBookingChannel() != BookingChannel.Marketplace)
        {
            throw new RecurringBookingIsNotMarketplace();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        existingRecurringBooking.DeletedByCustomer = deletedByCustomer;
        existingRecurringBooking = repositoryFactory.RecurringBookingRepository.Update(existingRecurringBooking);
        var deletedRecurringBooking = mapper.MapTo(repositoryFactory.RecurringBookingRepository.Remove(existingRecurringBooking));

        temporalOutboxService.SignalWorkflowBookMarketplaceRecurringResourcesDeleted(existingRecurringBooking.Id, repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return deletedRecurringBooking;
    }

    private static bool IsRecurringPurchaseCadence(ProductPricingCadence cadence) =>
        cadence is ProductPricingCadence.Weekly or
            ProductPricingCadence.Fortnightly or
            ProductPricingCadence.Monthly or
            ProductPricingCadence.TwoMonths or
            ProductPricingCadence.Quarterly or
            ProductPricingCadence.FourMonths or
            ProductPricingCadence.FiveMonths or
            ProductPricingCadence.SixMonths or
            ProductPricingCadence.Yearly;

    private static ProductPricingBillingSchedule ResolveBillingSchedule(
        ProductPricing pricing,
        ProductPricingBillingSchedule selectedBillingSchedule)
    {
        if (!pricing.AcceptedBillingSchedules.Contains(selectedBillingSchedule))
        {
            throw new MarketplaceBookingBillingScheduleNotAccepted();
        }

        return selectedBillingSchedule;
    }
}
