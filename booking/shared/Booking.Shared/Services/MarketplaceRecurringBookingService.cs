using Api.Shared.Services;
using Api.Shared.Services.Models;
using Booking.Shared.Database.Entities;
using Booking.Shared.Mappers;
using Booking.Shared.Repositories;
using Booking.Shared.Workflows;
using Enterprise.Shared;
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
    IProductService productService,
    IRandomHelper randomHelper,
    ITemporalOutboxService temporalOutboxService) : IMarketplaceRecurringBookingService
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

        var productVersions = await productService.GetProductVersionsAsync(
            marketplaceBooking.LineItems.Select(item => item.ProductVersionId).ToList(),
            cancellationToken);

        if (productVersions.Any(item => item.ProductTags.Count == 0))
        {
            throw new ProductMissingProductTag();
        }

        var organizationIds = productVersions.Select(item => item.Product.Organization.Id).Distinct().ToList();
        if (organizationIds.Count > 1)
        {
            throw new CrossOrganizationProductBookingNotAllowed();
        }

        if (!productVersions.All(item => item.IsPriceTaxInclusive is null) &&
            !productVersions.All(item => item.IsPriceTaxInclusive!.Value) &&
            productVersions.Any(item => item.IsPriceTaxInclusive!.Value))
        {
            throw new BookingProductWithMixedTaxSetupNotAllowed();
        }

        marketplaceBooking.Id = randomHelper.Generate();
        marketplaceBooking.IsPaymentRequired = true;
        marketplaceBooking.PaymentStatus = PaymentStatus.Template;

        if (productVersions.Any(item =>
                !item.AcceptedBookingPaymentMethods.ToSafeCollection().Contains(marketplaceBooking.PaymentMethod.ToPaymentMethod())))
        {
            throw new BookingPaymentMethodNotAccepted();
        }

        var currencies = productVersions.Select(item => item.Currency).Distinct().ToList();
        if (currencies.Count > 1)
        {
            throw new BookingsProductsWithMultipleCurrenciesAreNotSupported();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        var marketplaceBookingEntity = mapper.MapTo(
            marketplaceBooking,
            customer,
            null,
            productVersions,
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
}
