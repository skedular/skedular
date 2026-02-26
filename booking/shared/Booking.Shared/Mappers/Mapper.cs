using Api.Shared.Services.Models;
using Booking.Shared.Models;
using Enterprise.Shared;
using Enterprise.Shared.Time;
using Google.Protobuf.WellKnownTypes;
using Stripe;
using BookingCheckoutSession = Api.Shared.Clients.Events.Skedular.Booking.V1.Value.BookingCheckoutSession;
using BookingSchedule = Api.Shared.Clients.Events.Skedular.Booking.V1.Value.BookingSchedule;
using Customer = Booking.Shared.Database.Entities.Customer;
using LineItem = Api.Shared.Clients.Events.Skedular.Booking.V1.Value.LineItem;
using Location = Booking.Shared.Database.Entities.Location;
using MarketplaceBooking = Booking.Shared.Database.Entities.MarketplaceBooking;
using Organization = Booking.Shared.Database.Entities.Organization;
using PaymentMethod = Api.Shared.Clients.Events.Skedular.Booking.V1.Value.PaymentMethod;
using Product = Booking.Shared.Models.Product;
using Resource = Api.Shared.Clients.Events.Skedular.Booking.V1.Value.Resource;
using StripeCheckoutSession = Booking.Shared.Database.Entities.StripeCheckoutSession;
using Team = Booking.Shared.Database.Entities.Team;

namespace Booking.Shared.Mappers;

public interface IMapper
{
    Api.Shared.Clients.Events.Skedular.Booking.V1.Value.Booking MapTo(Models.Booking src);
    ProductCreateOptions MapTo(ProductVersion src, Product product, string organizationId);
    PriceCreateOptions MapTo(ProductVersion src, Product product, string organizationId, string stripeProductId);
    ProductVersion MapTo(Database.Entities.ProductVersion src);
    CustomerCreateOptions MapToCustomerCreateOption(Organization src);
    CustomerCreateOptions MapToCustomerCreateOption(Customer src);
    Models.Booking MapTo(Database.Entities.Booking src);
    RecurringBooking MapTo(Database.Entities.RecurringBooking src);
    Models.Booking MapTo(Database.Entities.RecurringBooking src, DateOnly date);

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
        MarketplaceBooking? marketplaceBooking);

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
        MarketplaceBooking? marketplaceBooking);

    Database.Entities.RecurringBooking MapTo(
        RecurringBooking src,
        ICollection<Customer> involvedCustomers,
        ICollection<Organization> involvedOrganizations,
        ICollection<Team> involvedTeams,
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
        Customer? createdByCustomer,
        Customer? lastModifiedByCustomer,
        Customer? deletedByCustomer,
        MarketplaceBooking? marketplaceBooking);

    MarketplaceBooking MapTo(
        Models.MarketplaceBooking src,
        Customer? paidByCustomer,
        Organization? paidByOrganization,
        ICollection<Database.Entities.ProductVersion> productVersions,
        StripeCheckoutSession? stripeCheckoutSession);
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
            },
            MarketplaceBooking = MapTo(src.MarketplaceBooking)
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

        booking.Resources.AddRange(MapTo(src.Resources));
        booking.Schedules.AddRange(MapTo(src.Schedules));
        booking.InvolvedCustomerIds.AddRange(src.InvolvedCustomers.Select(item => item.Id));
        booking.InvolvedOrganizationIds.AddRange(src.InvolvedOrganizations.Select(item => item.Id));
        booking.InvolvedLocationIds.AddRange(src.InvolvedLocations.Select(item => item.Id));
        booking.InvolvedTeamIds.AddRange(src.InvolvedTeams.Select(item => item.Id));

        return booking;
    }

    public ProductCreateOptions MapTo(ProductVersion src, Product product, string organizationId) =>
        new()
        {
            Name = src.Name.ToSafeString(),
            UnitLabel = src.PriceUnit.ToStripePriceUnitName(),
            TaxCode = "txcd_10103001",
            Metadata = new Dictionary<string, string>
            {
                { "productId", product.Id }, { "productVersionId", src.Id }, { "organizationId", organizationId }
            }
        };

    public PriceCreateOptions MapTo(ProductVersion src, Product product, string organizationId, string stripeProductId) =>
        new()
        {
            Currency = src.Currency.ToCurrency(),
            BillingScheme = "per_unit",
            UnitAmountDecimal = src.Price * 100,
            Product = stripeProductId,
            TaxBehavior = src.IsPriceTaxInclusive ? "inclusive" : "exclusive",
            Metadata = new Dictionary<string, string> { { "productId", product.Id }, { "organizationId", organizationId } }
        };

    public ProductVersion MapTo(Database.Entities.ProductVersion src)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(src.PriceUnit);

        if (!src.PricePerMinute.HasValue)
        {
            throw new ArgumentNullException(nameof(src.PricePerMinute));
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(src.Currency);

        if (!src.BookAllLocationResources.HasValue)
        {
            throw new ArgumentNullException(nameof(src.BookAllLocationResources));
        }

        if (!src.RecurrenceWindowDays.HasValue)
        {
            throw new ArgumentNullException(nameof(src.RecurrenceWindowDays));
        }

        if (!src.RequireConsecutiveDays.HasValue)
        {
            throw new ArgumentNullException(nameof(src.RequireConsecutiveDays));
        }

        if (!src.NumberOfResourcesToBook.HasValue)
        {
            throw new ArgumentNullException(nameof(src.NumberOfResourcesToBook));
        }

        if (!src.IsPriceTaxInclusive.HasValue)
        {
            throw new ArgumentNullException(nameof(src.IsPriceTaxInclusive));
        }

        return new ProductVersion
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            ModifiedAt = src.ModifiedAt,
            Name = src.Name.ToSafeString(),
            Price = src.Price ?? 0,
            PriceUnit = src.PriceUnit.ToPriceUnit(),
            IsPriceTaxInclusive = src.IsPriceTaxInclusive.Value,
            PricePerMinute = src.PricePerMinute.Value,
            Currency = src.Currency.ToCurrency(),
            MinDurationMinutes = src.MinDurationMinutes,
            MaxDurationMinutes = src.MaxDurationMinutes,
            BookAllLocationResources = src.BookAllLocationResources.Value,
            RecurrenceWindowDays = src.RecurrenceWindowDays.Value,
            RequireConsecutiveDays = src.RequireConsecutiveDays.Value,
            MaxBookingSpreadDays = src.MaxBookingSpreadDays,
            NumberOfResourcesToBook = src.NumberOfResourcesToBook.Value
        };
    }

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
            MarketplaceBooking = MapTo(src.MarketplaceBooking)
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
            CreatedByCustomer = MapTo(src.CreatedByCustomer),
            LastModifiedByCustomer = MapTo(src.LastModifiedByCustomer),
            DeletedByCustomer = MapTo(src.DeletedByCustomer),
            MarketplaceBooking = MapTo(src.MarketplaceBooking)
        };

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
            CreatedByCustomer = MapTo(src.CreatedByCustomer)
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
        MarketplaceBooking? marketplaceBooking) =>
        MergeTo(
            src,
            new Database.Entities.Booking(),
            involvedCustomers,
            involvedOrganizations,
            involvedLocations,
            involvedTeams,
            resources,
            createdByCustomer,
            lastModifiedByCustomer,
            deletedByCustomer,
            marketplaceBooking);

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
        MarketplaceBooking? marketplaceBooking)
    {
        dest.Id = src.Id;
        dest.From = src.From;
        dest.Until = src.Until;
        dest.Notes = src.Notes;
        dest.Category = src.Category.ToBookingCategory();
        dest.Schedules = src.Schedules;
        dest.ResourceBookingSlots = resources.SelectMany(item => item.ResourceBookingSlots).ToList();
        dest.InvolvedCustomers = involvedCustomers;
        dest.InvolvedOrganizations = involvedOrganizations;
        dest.InvolvedLocations = involvedLocations;
        dest.InvolvedTeams = involvedTeams;
        dest.InvolvedResources = resources;
        dest.CreatedByCustomer = createdByCustomer;
        dest.LastModifiedByCustomer = lastModifiedByCustomer;
        dest.DeletedByCustomer = deletedByCustomer;
        dest.MarketplaceBooking = marketplaceBooking;
        return dest;
    }

    public Database.Entities.RecurringBooking MapTo(
        RecurringBooking src,
        ICollection<Customer> involvedCustomers,
        ICollection<Organization> involvedOrganizations,
        ICollection<Team> involvedTeams,
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
        ICollection<Database.Entities.ProductVersion> productVersions,
        StripeCheckoutSession? stripeCheckoutSession) =>
        MergeTo(
            src,
            new MarketplaceBooking(),
            paidByCustomer,
            paidByOrganization,
            productVersions,
            stripeCheckoutSession);

    private Models.MarketplaceBooking? MapTo(MarketplaceBooking? src) =>
        src is null
            ? null
            : new Models.MarketplaceBooking
            {
                Id = src.Id,
                CreatedAt = src.CreatedAt,
                ModifiedAt = src.ModifiedAt,
                PaymentStatus = src.PaymentStatus.ToPaymentStatus(),
                IsPaymentRequired = src.IsPaymentRequired,
                LineItems = src.LineItems,
                PaidByCustomer = MapTo(src.PaidByCustomer),
                PaidByOrganization = MapTo(src.PaidByOrganization),
                ProductVersions = MapTo(src.ProductVersions).ToList(),
                PaymentMethod = src.PaymentMethod.ToPaymentMethod(),
                TotalAmountExcludeTax = src.TotalAmountExcludeTax,
                TaxAmount = src.TaxAmount,
                TaxRatePercentage = src.TaxRatePercentage,
                TotalAmount = src.TotalAmount,
                Currency = src.Currency,
                InvoiceUrl = src.InvoiceUrl,
                InvoiceNumber = src.InvoiceNumber,
                InvoiceEmailList = src.InvoiceEmailList.ToSafeCollection(),
                StripeCheckoutSession = MapTo(src.StripeCheckoutSession),
                PaymentExpiry = src.PaymentExpiry
            };

    private static MarketplaceBooking MergeTo(
        Models.MarketplaceBooking src,
        MarketplaceBooking dest,
        Customer? paidByCustomer,
        Organization? paidByOrganization,
        ICollection<Database.Entities.ProductVersion> productVersions,
        StripeCheckoutSession? stripeCheckoutSession)
    {
        dest.Id = src.Id;
        dest.PaymentStatus = src.PaymentStatus.ToPaymentStatus();
        dest.IsPaymentRequired = src.IsPaymentRequired;
        dest.LineItems = src.LineItems;
        dest.PaidByCustomer = paidByCustomer;
        dest.PaidByOrganization = paidByOrganization;
        dest.ProductVersions = productVersions;
        dest.StripeCheckoutSession = stripeCheckoutSession;
        dest.PaymentMethod = src.PaymentMethod.ToPaymentMethod();
        dest.TotalAmountExcludeTax = src.TotalAmountExcludeTax;
        dest.TaxAmount = src.TaxAmount;
        dest.TaxRatePercentage = src.TaxRatePercentage;
        dest.TotalAmount = src.TotalAmount;
        dest.Currency = src.Currency;
        dest.InvoiceUrl = src.InvoiceUrl;
        dest.InvoiceNumber = src.InvoiceNumber;
        dest.InvoiceEmailList = src.InvoiceEmailList;
        dest.PaymentExpiry = src.PaymentExpiry;
        return dest;
    }

    private static Api.Shared.Clients.Events.Skedular.Booking.V1.Value.MarketplaceBooking? MapTo(Models.MarketplaceBooking? src)
    {
        if (src is null)
        {
            return null;
        }

        var marketplaceBooking = new Api.Shared.Clients.Events.Skedular.Booking.V1.Value.MarketplaceBooking
        {
            Id = src.Id,
            PaymentStatus = src.PaymentStatus switch
            {
                PaymentStatus.Pending => Api.Shared.Clients.Events.Skedular.Booking.V1.Value.PaymentStatus.Pending,
                PaymentStatus.Rejected => Api.Shared.Clients.Events.Skedular.Booking.V1.Value.PaymentStatus.Rejected,
                PaymentStatus.Confirmed => Api.Shared.Clients.Events.Skedular.Booking.V1.Value.PaymentStatus.Confirmed,
                PaymentStatus.Expired => Api.Shared.Clients.Events.Skedular.Booking.V1.Value.PaymentStatus.Expired,
                PaymentStatus.RecordNeverCreated => Api.Shared.Clients.Events.Skedular.Booking.V1.Value.PaymentStatus.RecordNeverCreated,
                PaymentStatus.NoPaymentRequired => Api.Shared.Clients.Events.Skedular.Booking.V1.Value.PaymentStatus.NoPaymentRequired,
                _ => throw new ArgumentOutOfRangeException()
            },
            PaymentMethod = src.PaymentMethod switch
            {
                Api.Shared.Services.Models.PaymentMethod.Card => PaymentMethod.Card,
                Api.Shared.Services.Models.PaymentMethod.BankTransfer => PaymentMethod.BankAccount,
                _ => throw new ArgumentOutOfRangeException()
            },
            IsPaymentRequired = src.IsPaymentRequired,
            BookingCheckoutSession = MapTo(src.StripeCheckoutSession),
            TotalAmountExcludeTax = src.TotalAmountExcludeTax.ToNullDouble(),
            TaxAmount = src.TaxAmount.ToNullDouble(),
            TaxRatePercentage = src.TaxRatePercentage.ToNullDouble(),
            TotalAmount = src.TotalAmount.ToNullDouble(),
            Currency = src.Currency.ToSafeString(),
            InvoiceUrl = src.InvoiceUrl.ToSafeString(),
            InvoiceNumber = src.InvoiceNumber.ToSafeString(),
            PaymentExpiry = src.PaymentExpiry.ToTimestamp()
        };

        if (src.PaidByCustomer is not null)
        {
            marketplaceBooking.PaidByCustomerId = src.PaidByCustomer.Id;
        }

        if (src.PaidByOrganization is not null)
        {
            marketplaceBooking.PaidByOrganizationId = src.PaidByOrganization.Id;
        }

        marketplaceBooking.LineItems.AddRange(MapTo(src.LineItems));
        marketplaceBooking.InvoiceEmailList.AddRange(src.InvoiceEmailList.ToSafeCollection());

        return marketplaceBooking;
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
        new() { Id = src.Id, Type = src.Type.ToNullableOrganizationTagType() };

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
                UniqueAlphanumericName = src.UniqueAlphanumericName,
                Name = src.Name,
                ContactEmail = src.ContactEmail,
                ContactPhone = src.ContactPhone,
                IsOwnershipVerified = src.IsOwnershipVerified,
                LogoUrl = src.LogoUrl,
                Offering = src.Offering,
                Type = src.Type.ToOrganizationType()
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
            Inactive = src.Inactive,
            RequireBookingApproval = src.RequireBookingApproval,
            OrganizationTags = MapTo(src.OrganizationTags).ToList()
        };

    private IEnumerable<ProductVersion> MapTo(IEnumerable<Database.Entities.ProductVersion> src) => src.Select(MapTo);

    private static BookingCheckoutSession? MapTo(Models.StripeCheckoutSession? src) =>
        src is null
            ? null
            : new BookingCheckoutSession { Id = src.Id, CheckoutUrl = src.CheckoutUrl.ToSafeString() };

    private static IEnumerable<Resource> MapTo(IEnumerable<ResourceCustomersPair> src) => src.Select(item => new Resource { Id = item.Resource.Id });

    private static IEnumerable<BookingSchedule> MapTo(IEnumerable<Api.Shared.Services.Models.BookingSchedule> src) => src.Select(MapTo);

    private static BookingSchedule MapTo(Api.Shared.Services.Models.BookingSchedule src) =>
        new() { From = src.From.ToTimestamp(), Until = src.Until.ToTimestamp() };

    private static IEnumerable<LineItem> MapTo(IEnumerable<ProductVersionLineItem> src) => src.Select(MapTo);

    private static LineItem MapTo(ProductVersionLineItem src) =>
        new() { ProductVersionId = src.ProductVersionId, Quantity = src.Quantity };
}
