using Api.Shared.Services.Models;
using Booking.Shared.Models;
using Enterprise.Shared;
using Enterprise.Shared.Time;
using Google.Protobuf.WellKnownTypes;
using Stripe;
using BookingSchedule = Api.Shared.Clients.Events.Skedular.Booking.V1.Value.BookingSchedule;
using Customer = Booking.Shared.Database.Entities.Customer;
using Location = Booking.Shared.Database.Entities.Location;
using MarketplaceBooking = Booking.Shared.Database.Entities.MarketplaceBooking;
using Organization = Booking.Shared.Database.Entities.Organization;
using ProductVersion = Booking.Shared.Database.Entities.ProductVersion;
using Resource = Api.Shared.Clients.Events.Skedular.Booking.V1.Value.Resource;
using StripeCheckoutSession = Booking.Shared.Database.Entities.StripeCheckoutSession;
using StripeProduct = Booking.Shared.Database.Entities.StripeProduct;
using Team = Booking.Shared.Database.Entities.Team;

namespace Booking.Shared.Mappers;

public interface IMapper
{
    Api.Shared.Clients.Events.Skedular.Booking.V1.Value.Booking MapTo(Models.Booking src);
    ProductCreateOptions MapTo(ProductPricing pricing, ProductVersion productVersion);
    PriceCreateOptions MapTo(ProductPricing pricing, StripeProduct stripeProduct);
    CustomerCreateOptions MapToCustomerCreateOption(Organization src);
    CustomerCreateOptions MapToCustomerCreateOption(Customer src);
    Models.Booking MapTo(Database.Entities.Booking src);
    OrganizationArrearsInvoice MapTo(Database.Entities.OrganizationArrearsInvoice src);
    RecurringBooking MapTo(Database.Entities.RecurringBooking src);
    MarketplaceBookingSubscription MapTo(Database.Entities.MarketplaceBookingSubscription src);
    Models.Booking MapTo(Database.Entities.RecurringBooking src, DateOnly date);
    Models.Booking MapTo(Database.Entities.RecurringBooking src, Models.Booking booking, MarketplaceBooking? marketplaceBooking, DateOnly? date);

    Database.Entities.Booking MapTo(
        Models.Booking src,
        ICollection<Customer> involvedCustomers,
        ICollection<Organization> involvedOrganizations,
        ICollection<Location> involvedLocations,
        ICollection<Team> involvedTeams,
        ICollection<Database.Entities.Resource> resources,
        Customer? createdByCustomer,
        Customer? lastModifiedByCustomer,
        Customer? deletedByCustomer,
        MarketplaceBooking? marketplaceBooking,
        Database.Entities.RecurringBooking? recurringBooking);

    Database.Entities.Booking MergeTo(
        Models.Booking src,
        Database.Entities.Booking dest,
        ICollection<Customer> involvedCustomers,
        ICollection<Organization> involvedOrganizations,
        ICollection<Location> involvedLocations,
        ICollection<Team> involvedTeams,
        ICollection<Database.Entities.Resource> resources,
        Customer? createdByCustomer,
        Customer? lastModifiedByCustomer,
        Customer? deletedByCustomer,
        MarketplaceBooking? marketplaceBooking,
        Database.Entities.RecurringBooking? recurringBooking);

    Database.Entities.RecurringBooking MapTo(
        RecurringBooking src,
        ICollection<Customer> involvedCustomers,
        ICollection<Organization> involvedOrganizations,
        ICollection<Team> involvedTeams,
        ICollection<Database.Entities.Resource> requestedResources,
        Customer? createdByCustomer,
        Customer? lastModifiedByCustomer,
        Customer? deletedByCustomer,
        MarketplaceBooking? marketplaceBooking);

    Database.Entities.RecurringBooking MergeTo(
        RecurringBooking src,
        Database.Entities.RecurringBooking dest,
        ICollection<Customer> involvedCustomers,
        ICollection<Organization> involvedOrganizations,
        ICollection<Team> involvedTeams,
        ICollection<Database.Entities.Resource> requestedResources,
        Customer? createdByCustomer,
        Customer? lastModifiedByCustomer,
        Customer? deletedByCustomer,
        MarketplaceBooking? marketplaceBooking);

    MarketplaceBooking MapTo(
        Models.MarketplaceBooking src,
        Customer? paidByCustomer,
        Organization? paidByOrganization,
        ProductVersion productVersion,
        StripeCheckoutSession? stripeCheckoutSession);

    Database.Entities.MarketplaceBookingSubscription MapTo(
        MarketplaceBookingSubscription src,
        ICollection<Customer> involvedCustomers,
        ICollection<Organization> involvedOrganizations,
        ICollection<Team> involvedTeams,
        ICollection<Database.Entities.Resource> requestedResources,
        Customer? createdByCustomer,
        Customer? lastModifiedByCustomer,
        Customer? deletedByCustomer,
        MarketplaceBooking marketplaceBooking,
        ProductVersion productVersion);

    Models.MarketplaceBooking? MapTo(MarketplaceBooking? src);
}

public class Mapper : IMapper
{
    public Api.Shared.Clients.Events.Skedular.Booking.V1.Value.Booking MapTo(Models.Booking src)
    {
        var booking = new Api.Shared.Clients.Events.Skedular.Booking.V1.Value.Booking
        {
            Id = src.Id,
            DeletedAt = src.DeletedAt?.ToTimestamp(),
            From = src.From.ToTimestamp(),
            Until = src.Until.ToTimestamp(),
            Notes = src.Notes.ToSafeString(),
            Category = src.Category switch
            {
                BookingCategory.WorkingFromHome => Api.Shared.Clients.Events.Skedular.Booking.V1.Value.BookingCategory.WorkingFromHome,
                BookingCategory.WorkingFromOffice => Api.Shared.Clients.Events.Skedular.Booking.V1.Value.BookingCategory.WorkingFromOffice,
                BookingCategory.WorkingFromCoworkingSpace =>
                    Api.Shared.Clients.Events.Skedular.Booking.V1.Value.BookingCategory.WorkingFromCoworkingSpace,
                BookingCategory.SickLeave => Api.Shared.Clients.Events.Skedular.Booking.V1.Value.BookingCategory.SickLeave,
                BookingCategory.AnnualLeave => Api.Shared.Clients.Events.Skedular.Booking.V1.Value.BookingCategory.AnnualLeave,
                BookingCategory.WellbeingLeave => Api.Shared.Clients.Events.Skedular.Booking.V1.Value.BookingCategory.WellbeingLeave,
                BookingCategory.ClientOffice => Api.Shared.Clients.Events.Skedular.Booking.V1.Value.BookingCategory.ClientOffice,
                BookingCategory.Vacation => Api.Shared.Clients.Events.Skedular.Booking.V1.Value.BookingCategory.Vacation,
                BookingCategory.TravelingForWork => Api.Shared.Clients.Events.Skedular.Booking.V1.Value.BookingCategory.TravelingForWork,
                BookingCategory.NonWorkingDay => Api.Shared.Clients.Events.Skedular.Booking.V1.Value.BookingCategory.NonWorkingDay,
                _ => throw new ArgumentOutOfRangeException()
            },
            Channel = src.Channel switch
            {
                BookingChannel.Private => Api.Shared.Clients.Events.Skedular.Booking.V1.Value.BookingChannel.Private,
                BookingChannel.Marketplace => Api.Shared.Clients.Events.Skedular.Booking.V1.Value.BookingChannel.Marketplace,
                _ => throw new ArgumentOutOfRangeException()
            }
        };

        if (src.CreatedByCustomer is not null)
        {
            booking.CreatedByCustomerId = src.CreatedByCustomer.Id;
        }

        if (src.LastModifiedByCustomer is not null)
        {
            booking.LastModifiedByCustomerId = src.LastModifiedByCustomer.Id;
        }

        if (src.DeletedByCustomer is not null)
        {
            booking.DeletedByCustomerId = src.DeletedByCustomer.Id;
        }

        if (src.HasRecurringInstanceOverrides.HasValue)
        {
            booking.HasRecurringInstanceOverrides = src.HasRecurringInstanceOverrides.Value;
        }

        booking.Resources.AddRange(MapTo(src.Resources));
        booking.Schedules.AddRange(MapTo(src.Schedules));
        booking.InvolvedCustomerIds.AddRange(src.InvolvedCustomers.Select(item => item.Id));
        booking.InvolvedOrganizationIds.AddRange(src.InvolvedOrganizations.Select(item => item.Id));
        booking.InvolvedLocationIds.AddRange(src.InvolvedLocations.Select(item => item.Id));
        booking.InvolvedTeamIds.AddRange(src.InvolvedTeams.Select(item => item.Id));

        return booking;
    }

    public ProductCreateOptions MapTo(ProductPricing pricing, ProductVersion productVersion) =>
        new()
        {
            Name = productVersion.ListingMetadata?.Title ?? "Name not set",
            UnitLabel = pricing.PurchaseCadence.ToStripePriceUnitName(),
            TaxCode = "txcd_10103001",
            Metadata = new Dictionary<string, string>
            {
                { "productId", productVersion.Product.Id },
                { "productVersionId", productVersion.Id },
                { "organizationId", productVersion.Product.Organization.Id }
            }
        };

    public PriceCreateOptions MapTo(ProductPricing pricing, StripeProduct stripeProduct) =>
        new()
        {
            Currency = stripeProduct.ProductVersion.Currency,
            BillingScheme = "per_unit",
            UnitAmountDecimal = pricing.Price * 100,
            Product = stripeProduct.StripeProductId,
            TaxBehavior = pricing.IsTaxInclusive ? "inclusive" : "exclusive",
            Metadata = new Dictionary<string, string>
            {
                { "productId", stripeProduct.ProductVersion.Product.Id },
                { "productVersionId", stripeProduct.ProductVersion.Id },
                { "organizationId", stripeProduct.ProductVersion.Product.Organization.Id }
            }
        };

    public CustomerCreateOptions MapToCustomerCreateOption(Organization src) =>
        new()
        {
            Name = src.Name,
            Email = string.IsNullOrWhiteSpace(src.ContactEmail) ? null : src.ContactEmail,
            Phone = string.IsNullOrWhiteSpace(src.ContactPhone) ? null : src.ContactPhone,
            Metadata = new Dictionary<string, string> { { "type", "organization" }, { "organizationId", src.Id } }
        };

    public CustomerCreateOptions MapToCustomerCreateOption(Customer src) =>
        new()
        {
            Name = src.ToDisplayableName(),
            Email = src.Identities.ToSingleEmail(),
            Phone = src.PhoneNumber.ToSafeString(),
            PreferredLocales = string.IsNullOrWhiteSpace(src.Locale) ? [] : [src.Locale],
            Metadata = new Dictionary<string, string> { { "type", "customer" }, { "customerId", src.Id } }
        };

    public Models.Booking MapTo(Database.Entities.Booking src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            From = src.From,
            Until = src.Until,
            Notes = src.Notes,
            Category = src.Category.ToBookingCategory(),
            Channel = src.Channel.ToBookingChannel(),
            Schedules = src.Schedules,
            ResourceBookingSlots = MapTo(src.ResourceBookingSlots).ToList(),
            InvolvedCustomers = MapTo(src.InvolvedCustomers).ToList(),
            InvolvedOrganizations = MapTo(src.InvolvedOrganizations).ToList(),
            InvolvedLocations = MapTo(src.InvolvedLocations).ToList(),
            InvolvedTeams = MapTo(src.InvolvedTeams).ToList(),
            InvolvedResources = MapTo(src.InvolvedResources).ToList(),
            CreatedByCustomer = MapTo(src.CreatedByCustomer),
            LastModifiedByCustomer = MapTo(src.LastModifiedByCustomer),
            DeletedByCustomer = MapTo(src.DeletedByCustomer),
            RecurringBooking = src.RecurringBooking is null ? null : MapToRecurringBookingWithoutSubscription(src.RecurringBooking),
            MarketplaceBooking = MapTo(ResolveBookingMarketplaceBooking(src)),
            HasRecurringInstanceOverrides = src.HasRecurringInstanceOverrides
        };

    public OrganizationArrearsInvoice MapTo(Database.Entities.OrganizationArrearsInvoice src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            ModifiedAt = src.ModifiedAt,
            Organization = MapTo(src.Organization)!,
            Customer = MapTo(src.Customer)!,
            InvoiceNumber = src.InvoiceNumber,
            InvoiceUrl = src.InvoiceUrl,
            BillingPeriodStartInclusive = src.BillingPeriodStartInclusive,
            BillingPeriodEndExclusive = src.BillingPeriodEndExclusive,
            Currency = src.Currency.ToCurrency(),
            TotalAmount = src.TotalAmount
        };

    public RecurringBooking MapTo(Database.Entities.RecurringBooking src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            From = src.From,
            Until = src.Until,
            Category = src.Category.ToBookingCategory(),
            Channel = src.Channel.ToBookingChannel(),
            Frequency = src.Frequency.ToBookingFrequency(),
            Interval = src.Interval,
            ByMonthDay = src.ByMonthDay,
            BySetPosition = src.BySetPosition,
            ByWeekDays = src.ByWeekDays.Select(item => item.ToDayOfWeek()).ToList(),
            EndType = src.EndType.ToRecurringBookingEndType(),
            StartDate = src.StartDate,
            EndDate = src.EndDate,
            OccurrenceCount = src.OccurrenceCount,
            SkippedDates = src.SkippedDates,
            InvolvedCustomers = MapTo(src.InvolvedCustomers).ToList(),
            InvolvedOrganizations = MapTo(src.InvolvedOrganizations).ToList(),
            InvolvedTeams = MapTo(src.InvolvedTeams).ToList(),
            RequestedResources = MapTo(src.RequestedResources).ToList(),
            CreatedByCustomer = MapTo(src.CreatedByCustomer),
            LastModifiedByCustomer = MapTo(src.LastModifiedByCustomer),
            DeletedByCustomer = MapTo(src.DeletedByCustomer),
            MarketplaceBooking = MapTo(src.MarketplaceBooking),
            MarketplaceBookingSubscription = src.MarketplaceBookingSubscription is null
                ? null
                : MapToSubscriptionShallow(src.MarketplaceBookingSubscription)
        };

    public MarketplaceBookingSubscription MapTo(Database.Entities.MarketplaceBookingSubscription src)
    {
        var marketplaceBooking = MapTo(src.MarketplaceBooking)!;
        var latestRecurringMarketplaceBooking = ResolveLatestRecurringMarketplaceBooking(src.RecurringBookings);
        if (latestRecurringMarketplaceBooking is not null)
        {
            // The subscription-level marketplace booking keeps the plan template details
            // such as purchase cadence, while its payment-facing fields mirror the latest
            // recurring cycle for this same subscription lineage.
            marketplaceBooking = MapTo(marketplaceBooking, latestRecurringMarketplaceBooking);
        }

        return new MarketplaceBookingSubscription
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            StartedAt = src.StartedAt,
            CancelledAt = src.CancelledAt,
            NextRenewalAt = src.NextRenewalAt,
            Status = src.Status.ToMarketplaceBookingSubscriptionStatus(),
            AutoRenew = src.AutoRenew,
            CancelAtPeriodEnd = src.CancelAtPeriodEnd,
            MarketplaceBooking = marketplaceBooking,
            InvolvedCustomers = MapTo(src.InvolvedCustomers).ToList(),
            InvolvedOrganizations = MapTo(src.InvolvedOrganizations).ToList(),
            InvolvedTeams = MapTo(src.InvolvedTeams).ToList(),
            RequestedResources = MapTo(src.RequestedResources).ToList(),
            CreatedByCustomer = MapTo(src.CreatedByCustomer),
            LastModifiedByCustomer = MapTo(src.LastModifiedByCustomer),
            DeletedByCustomer = MapTo(src.DeletedByCustomer),
            RecurringBookings = src.RecurringBookings.Select(MapToRecurringBookingWithoutSubscription).ToList()
        };
    }

    public Models.Booking MapTo(Database.Entities.RecurringBooking src, DateOnly date)
    {
        var from = date.ToDateTimeOffset(src.From.TimeOfDay);
        var until = date.ToDateTimeOffset(src.Until.TimeOfDay);

        return new Models.Booking
        {
            From = from,
            Until = until,
            Category = src.Category.ToBookingCategory(),
            Channel = src.Channel.ToBookingChannel(),
            Schedules = [new Api.Shared.Services.Models.BookingSchedule(from, until)],
            InvolvedCustomers = MapTo(src.InvolvedCustomers).ToList(),
            InvolvedOrganizations = MapTo(src.InvolvedOrganizations).ToList(),
            InvolvedTeams = MapTo(src.InvolvedTeams).ToList(),
            Resources = src.RequestedResources
                .Select(item => new ResourceCustomersPair(
                    new Models.Resource { Id = item.Id },
                    MapTo(src.InvolvedCustomers).ToList()))
                .ToList(),
            CreatedByCustomer = MapTo(src.CreatedByCustomer)
        };
    }

    public Models.Booking MapTo(
        Database.Entities.RecurringBooking src,
        Models.Booking booking,
        MarketplaceBooking? marketplaceBooking,
        DateOnly? date)
    {
        var from = date?.ToDateTimeOffset(src.From.TimeOfDay) ?? booking.From;
        var until = date?.ToDateTimeOffset(src.Until.TimeOfDay) ?? booking.Until;

        return new Models.Booking
        {
            Id = booking.Id,
            From = from,
            Until = until,
            Category = src.Category.ToBookingCategory(),
            Channel = src.Channel.ToBookingChannel(),
            Schedules = [new Api.Shared.Services.Models.BookingSchedule(from, until)],
            InvolvedCustomers = MapTo(src.InvolvedCustomers).ToList(),
            InvolvedOrganizations = MapTo(src.InvolvedOrganizations).ToList(),
            InvolvedTeams = MapTo(src.InvolvedTeams).ToList(),
            CreatedByCustomer = MapTo(src.CreatedByCustomer),
            Resources = src.RequestedResources.Count != 0
                ? src.RequestedResources
                    .Select(item => new ResourceCustomersPair(
                        new Models.Resource { Id = item.Id },
                        MapTo(src.InvolvedCustomers).ToList()))
                    .ToList()
                : booking.Resources,
            MarketplaceBooking = MapTo(marketplaceBooking)
        };
    }

    public Database.Entities.Booking MapTo(
        Models.Booking src,
        ICollection<Customer> involvedCustomers,
        ICollection<Organization> involvedOrganizations,
        ICollection<Location> involvedLocations,
        ICollection<Team> involvedTeams,
        ICollection<Database.Entities.Resource> resources,
        Customer? createdByCustomer,
        Customer? lastModifiedByCustomer,
        Customer? deletedByCustomer,
        MarketplaceBooking? marketplaceBooking,
        Database.Entities.RecurringBooking? recurringBooking)
    {
        var booking = MergeTo(
            src,
            new Database.Entities.Booking { Channel = src.Channel.ToBookingChannel() },
            involvedCustomers,
            involvedOrganizations,
            involvedLocations,
            involvedTeams,
            resources,
            createdByCustomer,
            lastModifiedByCustomer,
            deletedByCustomer,
            marketplaceBooking,
            recurringBooking);

        booking.From = src.From;
        booking.Until = src.Until;
        booking.Schedules = src.Schedules;

        return booking;
    }

    public Database.Entities.Booking MergeTo(
        Models.Booking src,
        Database.Entities.Booking dest,
        ICollection<Customer> involvedCustomers,
        ICollection<Organization> involvedOrganizations,
        ICollection<Location> involvedLocations,
        ICollection<Team> involvedTeams,
        ICollection<Database.Entities.Resource> resources,
        Customer? createdByCustomer,
        Customer? lastModifiedByCustomer,
        Customer? deletedByCustomer,
        MarketplaceBooking? marketplaceBooking,
        Database.Entities.RecurringBooking? recurringBooking)
    {
        dest.Id = src.Id;

        if (dest.Channel.ToBookingChannel() == BookingChannel.Private)
        {
            dest.From = src.From;
            dest.Until = src.Until;
            dest.Schedules = src.Schedules;
        }

        dest.Notes = src.Notes;
        dest.Category = src.Category.ToBookingCategory();
        dest.ResourceBookingSlots = resources.SelectMany(item => item.ResourceBookingSlots).ToList();
        dest.InvolvedCustomers = involvedCustomers;
        dest.InvolvedOrganizations = involvedOrganizations;
        dest.InvolvedLocations = involvedLocations;
        dest.InvolvedTeams = involvedTeams;
        dest.InvolvedResources = resources;

        if (createdByCustomer is not null)
        {
            dest.CreatedByCustomer = createdByCustomer;
        }

        if (lastModifiedByCustomer is not null)
        {
            dest.LastModifiedByCustomer = lastModifiedByCustomer;
        }

        if (deletedByCustomer is not null)
        {
            dest.DeletedByCustomer = deletedByCustomer;
        }

        dest.MarketplaceBooking = marketplaceBooking;
        dest.HasRecurringInstanceOverrides = src.HasRecurringInstanceOverrides;
        dest.RecurringBooking = recurringBooking;
        return dest;
    }

    public Database.Entities.RecurringBooking MapTo(
        RecurringBooking src,
        ICollection<Customer> involvedCustomers,
        ICollection<Organization> involvedOrganizations,
        ICollection<Team> involvedTeams,
        ICollection<Database.Entities.Resource> requestedResources,
        Customer? createdByCustomer,
        Customer? lastModifiedByCustomer,
        Customer? deletedByCustomer,
        MarketplaceBooking? marketplaceBooking) =>
        MergeTo(
            src,
            new Database.Entities.RecurringBooking(),
            involvedCustomers,
            involvedOrganizations,
            involvedTeams,
            requestedResources,
            createdByCustomer,
            lastModifiedByCustomer,
            deletedByCustomer,
            marketplaceBooking);

    public Database.Entities.RecurringBooking MergeTo(
        RecurringBooking src,
        Database.Entities.RecurringBooking dest,
        ICollection<Customer> involvedCustomers,
        ICollection<Organization> involvedOrganizations,
        ICollection<Team> involvedTeams,
        ICollection<Database.Entities.Resource> requestedResources,
        Customer? createdByCustomer,
        Customer? lastModifiedByCustomer,
        Customer? deletedByCustomer,
        MarketplaceBooking? marketplaceBooking)
    {
        dest.Id = src.Id;
        dest.From = src.From;
        dest.Until = src.Until;
        dest.Category = src.Category.ToBookingCategory();
        dest.Frequency = src.Frequency.ToBookingFrequency();
        dest.Interval = src.Interval;
        dest.ByMonthDay = src.ByMonthDay;
        dest.BySetPosition = src.BySetPosition;
        dest.ByWeekDays = src.ByWeekDays.Select(item => item.ToDayOfWeek()).ToList();
        dest.EndType = src.EndType.ToRecurringBookingEndType();
        dest.StartDate = src.StartDate;
        dest.EndDate = src.EndDate;
        dest.OccurrenceCount = src.OccurrenceCount;
        dest.SkippedDates = src.SkippedDates;
        dest.InvolvedCustomers = involvedCustomers;
        dest.InvolvedOrganizations = involvedOrganizations;
        dest.InvolvedTeams = involvedTeams;
        dest.RequestedResources = requestedResources;
        dest.CreatedByCustomer = createdByCustomer;
        dest.LastModifiedByCustomer = lastModifiedByCustomer;
        dest.DeletedByCustomer = deletedByCustomer;
        dest.MarketplaceBooking = marketplaceBooking;
        return dest;
    }

    public MarketplaceBooking MapTo(
        Models.MarketplaceBooking src,
        Customer? paidByCustomer,
        Organization? paidByOrganization,
        ProductVersion productVersion,
        StripeCheckoutSession? stripeCheckoutSession) =>
        MergeTo(
            src,
            new MarketplaceBooking(),
            paidByCustomer,
            paidByOrganization,
            productVersion,
            stripeCheckoutSession);

    public Database.Entities.MarketplaceBookingSubscription MapTo(
        MarketplaceBookingSubscription src,
        ICollection<Customer> involvedCustomers,
        ICollection<Organization> involvedOrganizations,
        ICollection<Team> involvedTeams,
        ICollection<Database.Entities.Resource> involvedResources,
        Customer? createdByCustomer,
        Customer? lastModifiedByCustomer,
        Customer? deletedByCustomer,
        MarketplaceBooking marketplaceBooking,
        ProductVersion productVersion) =>
        MergeTo(
            src,
            new Database.Entities.MarketplaceBookingSubscription(),
            involvedCustomers,
            involvedOrganizations,
            involvedTeams,
            involvedResources,
            createdByCustomer,
            lastModifiedByCustomer,
            deletedByCustomer,
            marketplaceBooking,
            productVersion);

    public Models.MarketplaceBooking? MapTo(MarketplaceBooking? src) =>
        src is null
            ? null
            : new Models.MarketplaceBooking
            {
                Id = src.Id,
                CreatedAt = src.CreatedAt,
                ModifiedAt = src.ModifiedAt,
                PaymentStatus = src.PaymentStatus.ToPaymentStatus(),
                IsPaymentRequired = src.IsPaymentRequired,
                Quantity = src.Quantity,
                ProductPricing = src.ProductPricing,
                PaidByCustomer = MapTo(src.PaidByCustomer),
                PaidByOrganization = MapTo(src.PaidByOrganization),
                ProductVersion = MapTo(src.ProductVersion),
                PaymentMethod = src.PaymentMethod.ToPaymentMethod(),
                TotalAmountExcludeTax = src.TotalAmountExcludeTax,
                TaxAmount = src.TaxAmount,
                TaxRatePercentage = src.TaxRatePercentage,
                TotalAmount = src.TotalAmount,
                Currency = src.Currency.ToNullableCurrency(),
                InvoiceUrl = src.InvoiceUrl,
                InvoiceNumber = src.InvoiceNumber,
                CheckoutReturnUrl = src.CheckoutReturnUrl,
                InvoiceEmailList = src.InvoiceEmailList.ToSafeCollection(),
                BillingMode = src.BillingMode.ToProductPricingBillingMode(),
                StripeCheckoutSession = MapTo(src.StripeCheckoutSession),
                PaymentExpiry = src.PaymentExpiry
            };

    private static Models.ProductVersion MapTo(ProductVersion src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            ModifiedAt = src.ModifiedAt,
            Type = src.Type.ToSafeString().ToProductType(),
            Currency = src.Currency.ToSafeString().ToCurrency(),
            ListingMetadata = src.ListingMetadata ?? ListingMetadata.Empty,
            PricingOptions = src.PricingOptions.ToSafeCollection()
        };

    private MarketplaceBookingSubscription MapToSubscriptionShallow(Database.Entities.MarketplaceBookingSubscription src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            StartedAt = src.StartedAt,
            CancelledAt = src.CancelledAt,
            NextRenewalAt = src.NextRenewalAt,
            Status = src.Status.ToMarketplaceBookingSubscriptionStatus(),
            AutoRenew = src.AutoRenew,
            CancelAtPeriodEnd = src.CancelAtPeriodEnd,
            MarketplaceBooking = MapTo(src.MarketplaceBooking)!,
            RequestedResources = MapTo(src.RequestedResources).ToList()
        };

    private RecurringBooking MapToRecurringBookingWithoutSubscription(Database.Entities.RecurringBooking src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            From = src.From,
            Until = src.Until,
            Category = src.Category.ToBookingCategory(),
            Channel = src.Channel.ToBookingChannel(),
            Frequency = src.Frequency.ToBookingFrequency(),
            Interval = src.Interval,
            ByMonthDay = src.ByMonthDay,
            BySetPosition = src.BySetPosition,
            ByWeekDays = src.ByWeekDays.Select(item => item.ToDayOfWeek()).ToList(),
            EndType = src.EndType.ToRecurringBookingEndType(),
            StartDate = src.StartDate,
            EndDate = src.EndDate,
            OccurrenceCount = src.OccurrenceCount,
            SkippedDates = src.SkippedDates,
            InvolvedCustomers = MapTo(src.InvolvedCustomers).ToList(),
            InvolvedOrganizations = MapTo(src.InvolvedOrganizations).ToList(),
            InvolvedTeams = MapTo(src.InvolvedTeams).ToList(),
            RequestedResources = MapTo(src.RequestedResources).ToList(),
            CreatedByCustomer = MapTo(src.CreatedByCustomer),
            LastModifiedByCustomer = MapTo(src.LastModifiedByCustomer),
            DeletedByCustomer = MapTo(src.DeletedByCustomer),
            MarketplaceBooking = MapTo(src.MarketplaceBooking),
            MarketplaceBookingSubscription = null
        };

    private static MarketplaceBooking? ResolveLatestRecurringMarketplaceBooking(ICollection<Database.Entities.RecurringBooking> recurringBookings) =>
        recurringBookings
            .Where(item => !item.DeletedAt.HasValue && item.MarketplaceBooking is not null)
            .OrderByDescending(item => item.StartDate)
            .Select(item => item.MarketplaceBooking)
            .FirstOrDefault();

    private static Models.MarketplaceBooking MapTo(Models.MarketplaceBooking src, MarketplaceBooking marketplaceBooking)
    {
        src.PaymentStatus = marketplaceBooking.PaymentStatus.ToPaymentStatus();
        src.IsPaymentRequired = marketplaceBooking.IsPaymentRequired;
        src.PaymentMethod = marketplaceBooking.PaymentMethod.ToPaymentMethod();
        src.PaymentExpiry = marketplaceBooking.PaymentExpiry;
        src.TotalAmountExcludeTax = marketplaceBooking.TotalAmountExcludeTax;
        src.TaxAmount = marketplaceBooking.TaxAmount;
        src.TaxRatePercentage = marketplaceBooking.TaxRatePercentage;
        src.TotalAmount = marketplaceBooking.TotalAmount;
        src.Currency = marketplaceBooking.Currency.ToNullableCurrency();
        src.InvoiceUrl = marketplaceBooking.InvoiceUrl;
        src.InvoiceNumber = marketplaceBooking.InvoiceNumber;
        src.CheckoutReturnUrl = marketplaceBooking.CheckoutReturnUrl;
        src.InvoiceEmailList = marketplaceBooking.InvoiceEmailList.ToSafeCollection();
        src.BillingMode = marketplaceBooking.BillingMode.ToProductPricingBillingMode();
        src.PaidByCustomer = MapTo(marketplaceBooking.PaidByCustomer);
        src.PaidByOrganization = MapTo(marketplaceBooking.PaidByOrganization);
        src.StripeCheckoutSession = MapTo(marketplaceBooking.StripeCheckoutSession);
        return src;
    }

    private static MarketplaceBooking MergeTo(
        Models.MarketplaceBooking src,
        MarketplaceBooking dest,
        Customer? paidByCustomer,
        Organization? paidByOrganization,
        ProductVersion productVersion,
        StripeCheckoutSession? stripeCheckoutSession)
    {
        dest.Id = src.Id;
        dest.PaymentStatus = src.PaymentStatus.ToPaymentStatus();
        dest.IsPaymentRequired = src.IsPaymentRequired;
        dest.Quantity = src.Quantity;
        dest.ProductPricing = src.ProductPricing;
        dest.PaidByCustomer = paidByCustomer;
        dest.PaidByOrganization = paidByOrganization;
        dest.ProductVersion = productVersion;
        dest.StripeCheckoutSession = stripeCheckoutSession;
        dest.PaymentMethod = src.PaymentMethod.ToPaymentMethod();
        dest.TotalAmountExcludeTax = src.TotalAmountExcludeTax;
        dest.TaxAmount = src.TaxAmount;
        dest.TaxRatePercentage = src.TaxRatePercentage;
        dest.TotalAmount = src.TotalAmount;
        dest.Currency = src.Currency.ToNullableCurrency();
        dest.InvoiceUrl = src.InvoiceUrl;
        dest.InvoiceNumber = src.InvoiceNumber;
        dest.CheckoutReturnUrl = src.CheckoutReturnUrl;
        dest.InvoiceEmailList = src.InvoiceEmailList;
        dest.BillingMode = src.BillingMode.ToProductPricingBillingMode();
        dest.PaymentExpiry = src.PaymentExpiry;
        return dest;
    }

    private static Database.Entities.MarketplaceBookingSubscription MergeTo(
        MarketplaceBookingSubscription src,
        Database.Entities.MarketplaceBookingSubscription dest,
        ICollection<Customer> involvedCustomers,
        ICollection<Organization> involvedOrganizations,
        ICollection<Team> involvedTeams,
        ICollection<Database.Entities.Resource> requestedResources,
        Customer? createdByCustomer,
        Customer? lastModifiedByCustomer,
        Customer? deletedByCustomer,
        MarketplaceBooking marketplaceBooking,
        ProductVersion productVersion)
    {
        dest.Id = src.Id;
        dest.StartedAt = src.StartedAt;
        dest.CancelledAt = src.CancelledAt;
        dest.NextRenewalAt = src.NextRenewalAt;
        dest.Status = src.Status.ToMarketplaceBookingSubscriptionStatus();
        dest.AutoRenew = src.AutoRenew;
        dest.CancelAtPeriodEnd = src.CancelAtPeriodEnd;
        dest.InvolvedCustomers = involvedCustomers;
        dest.InvolvedOrganizations = involvedOrganizations;
        dest.InvolvedTeams = involvedTeams;
        dest.RequestedResources = requestedResources;
        dest.CreatedByCustomer = createdByCustomer;
        dest.LastModifiedByCustomer = lastModifiedByCustomer;
        dest.DeletedByCustomer = deletedByCustomer;
        dest.MarketplaceBooking = marketplaceBooking;
        dest.ProductVersion = productVersion;
        return dest;
    }

    private static Models.StripeCheckoutSession? MapTo(StripeCheckoutSession? src) =>
        src is null
            ? null
            : new Models.StripeCheckoutSession
            {
                Id = src.Id,
                CreatedAt = src.CreatedAt,
                ModifiedAt = src.ModifiedAt,
                DeletedAt = src.DeletedAt,
                CheckoutUrl = src.CheckoutUrl.ToSafeString()
            };

    private static IEnumerable<ResourceBookingSlot> MapTo(IEnumerable<Database.Entities.ResourceBookingSlot> src) => src.Select(MapTo);

    private static ResourceBookingSlot MapTo(Database.Entities.ResourceBookingSlot src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            ModifiedAt = src.ModifiedAt,
            Available = src.Available,
            Start = src.Start,
            Customers = MapTo(src.Customers).ToList(),
            Resource = MapTo(src.Resource)
        };

    private static IEnumerable<Models.Customer> MapTo(IEnumerable<Customer> src) => src.Select(MapTo)!;

    private static Models.Customer? MapTo(Customer? src) =>
        src is null
            ? null
            : new Models.Customer
            {
                Id = src.Id,
                CreatedAt = src.CreatedAt,
                DeletedAt = src.DeletedAt,
                ModifiedAt = src.ModifiedAt,
                EventRaisedAt = src.EventRaisedAt,
                Designation = src.Designation,
                Title = src.Title,
                Timezone = src.Timezone,
                Locale = src.Locale,
                Name = src.Name,
                GivenName = src.GivenName,
                MiddleName = src.MiddleName,
                FamilyName = src.FamilyName,
                PhotoUrl = src.PhotoUrl,
                PhotoUrl24 = src.PhotoUrl24,
                PhotoUrl32 = src.PhotoUrl32,
                PhotoUrl48 = src.PhotoUrl48,
                PhotoUrl72 = src.PhotoUrl72,
                PhotoUrl192 = src.PhotoUrl192,
                PhotoUrl512 = src.PhotoUrl512,
                PhoneNumber = src.PhoneNumber,
                Type = src.Type.ToNullableCustomerType(),
                Identities = MapTo(src.Identities).ToList()
            };

    private static IEnumerable<Identity> MapTo(IEnumerable<Database.Entities.Identity> src) => src.Select(MapTo);

    private static Identity MapTo(Database.Entities.Identity src) =>
        new() { Id = src.Id, Email = src.Email, EmailVerified = src.EmailVerified };

    private static IEnumerable<OrganizationTag> MapTo(IEnumerable<Database.Entities.OrganizationTag> src) => src.Select(MapTo);

    private static OrganizationTag MapTo(Database.Entities.OrganizationTag src) =>
        new() { Id = src.Id, Name = src.Name.ToSafeString(), Type = src.Type.ToNullableOrganizationTagType(), Color = src.Color };

    private static IEnumerable<Models.Organization> MapTo(IEnumerable<Organization> src) => src.Select(MapTo)!;

    private static Models.Organization? MapTo(Organization? src) =>
        src is null
            ? null
            : new Models.Organization
            {
                Id = src.Id,
                CreatedAt = src.CreatedAt,
                DeletedAt = src.DeletedAt,
                ModifiedAt = src.ModifiedAt,
                EventRaisedAt = src.EventRaisedAt,
                CustomDomain = src.CustomDomain,
                Name = src.Name,
                ContactEmail = src.ContactEmail,
                ContactPhone = src.ContactPhone,
                IsOwnershipVerified = src.IsOwnershipVerified,
                LogoUrl = src.LogoUrl,
                Offering = src.Offering,
                Type = src.Type.ToOrganizationType(),
                BillingCycle = src.BillingCycle.ToOrganizationBillingCycle()
            };

    private static IEnumerable<Models.Location> MapTo(IEnumerable<Location> src) => src.Select(MapTo)!;

    private static Models.Location? MapTo(Location? src) =>
        src is null
            ? null
            : new Models.Location
            {
                Id = src.Id,
                CreatedAt = src.CreatedAt,
                DeletedAt = src.DeletedAt,
                ModifiedAt = src.ModifiedAt,
                EventRaisedAt = src.EventRaisedAt,
                Name = src.Name.ToSafeString(),
                Type = src.Type.ToLocationType(),
                OrganizationTags = MapTo(src.OrganizationTags).ToList()
            };

    private static IEnumerable<Models.Team> MapTo(IEnumerable<Team> src) => src.Select(MapTo)!;

    private static Models.Team? MapTo(Team? src) =>
        src is null
            ? null
            : new Models.Team
            {
                Id = src.Id,
                CreatedAt = src.CreatedAt,
                DeletedAt = src.DeletedAt,
                ModifiedAt = src.ModifiedAt,
                EventRaisedAt = src.EventRaisedAt
            };

    private static IEnumerable<Models.Resource> MapTo(IEnumerable<Database.Entities.Resource> src) => src.Select(MapTo);

    private static Models.Resource MapTo(Database.Entities.Resource src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            EventRaisedAt = src.EventRaisedAt,
            Capacity = src.Capacity,
            Name = src.Name.ToSafeString(),
            Color = src.Color,
            Inactive = src.Inactive,
            RequireBookingApproval = src.RequireBookingApproval,
            OrganizationTags = MapTo(src.OrganizationTags).ToList()
        };

    private static IEnumerable<Resource> MapTo(IEnumerable<ResourceCustomersPair> src) => src.Select(item => new Resource { Id = item.Resource.Id });

    private static IEnumerable<BookingSchedule> MapTo(IEnumerable<Api.Shared.Services.Models.BookingSchedule> src) => src.Select(MapTo);

    private static BookingSchedule MapTo(Api.Shared.Services.Models.BookingSchedule src) =>
        new() { From = src.From.ToTimestamp(), Until = src.Until.ToTimestamp() };

    private static MarketplaceBooking? ResolveBookingMarketplaceBooking(Database.Entities.Booking booking) =>
        booking.RecurringBooking?.MarketplaceBooking ?? booking.MarketplaceBooking;
}
