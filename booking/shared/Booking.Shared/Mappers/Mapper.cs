using Api.Shared.Services.Models;
using Booking.Shared.Models;
using Enterprise.Shared;
using Google.Protobuf.WellKnownTypes;
using Stripe;
using BookingCheckoutSession = Api.Shared.Clients.Events.Skedular.Booking.V1.Value.BookingCheckoutSession;
using BookingSchedule = Api.Shared.Clients.Events.Skedular.Booking.V1.Value.BookingSchedule;
using Customer = Booking.Shared.Database.Entities.Customer;
using LineItem = Api.Shared.Clients.Events.Skedular.Booking.V1.Value.LineItem;
using Organization = Booking.Shared.Database.Entities.Organization;
using PaymentMethod = Api.Shared.Clients.Events.Skedular.Booking.V1.Value.PaymentMethod;
using Product = Booking.Shared.Models.Product;
using Resource = Api.Shared.Clients.Events.Skedular.Booking.V1.Value.Resource;

namespace Booking.Shared.Mappers;

public interface IMapper
{
    Api.Shared.Clients.Events.Skedular.Booking.V1.Value.Booking MapTo(Models.Booking src);
    ProductCreateOptions MapTo(ProductVersion src, Product product, string organizationId);
    PriceCreateOptions MapTo(ProductVersion src, Product product, string organizationId, string stripeProductId);
    ProductVersion MapTo(Database.Entities.ProductVersion src);
    CustomerCreateOptions MapToCustomerCreateOption(Organization src);
    CustomerUpdateOptions MergeToCustomerUpdateOption(Organization src);
    CustomerCreateOptions MapToCustomerCreateOption(Customer src);
    CustomerUpdateOptions MergeToCustomerUpdateOption(Customer src);
    Models.Booking MapTo(Database.Entities.Booking src);
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
            Type = src.Type switch
            {
                BookingType.WorkingFromHome => Api.Shared.Clients.Events.Skedular.Booking.V1.Value.BookingType.WorkingFromHome,
                BookingType.WorkingFromOffice => Api.Shared.Clients.Events.Skedular.Booking.V1.Value.BookingType.WorkingFromOffice,
                BookingType.WorkingFromCoworkingSpace => Api.Shared.Clients.Events.Skedular.Booking.V1.Value.BookingType.WorkingFromCoworkingSpace,
                BookingType.SickLeave => Api.Shared.Clients.Events.Skedular.Booking.V1.Value.BookingType.SickLeave,
                BookingType.AnnualLeave => Api.Shared.Clients.Events.Skedular.Booking.V1.Value.BookingType.AnnualLeave,
                BookingType.WellbeingLeave => Api.Shared.Clients.Events.Skedular.Booking.V1.Value.BookingType.WellbeingLeave,
                BookingType.ClientOffice => Api.Shared.Clients.Events.Skedular.Booking.V1.Value.BookingType.ClientOffice,
                BookingType.Vacation => Api.Shared.Clients.Events.Skedular.Booking.V1.Value.BookingType.Vacation,
                BookingType.TravelingForWork => Api.Shared.Clients.Events.Skedular.Booking.V1.Value.BookingType.TravelingForWork,
                BookingType.NonWorkingDay => Api.Shared.Clients.Events.Skedular.Booking.V1.Value.BookingType.NonWorkingDay,
                _ => throw new ArgumentOutOfRangeException()
            },
            PaymentStatus = src.PaymentStatus switch
            {
                BookingPaymentStatus.Pending => Api.Shared.Clients.Events.Skedular.Booking.V1.Value.BookingPaymentStatus.PaymentPending,
                BookingPaymentStatus.Rejected => Api.Shared.Clients.Events.Skedular.Booking.V1.Value.BookingPaymentStatus.PaymentRejected,
                BookingPaymentStatus.Confirmed => Api.Shared.Clients.Events.Skedular.Booking.V1.Value.BookingPaymentStatus.PaymentConfirmed,
                BookingPaymentStatus.Expired => Api.Shared.Clients.Events.Skedular.Booking.V1.Value.BookingPaymentStatus.PaymentExpired,
                BookingPaymentStatus.RecordNeverCreated =>
                    Api.Shared.Clients.Events.Skedular.Booking.V1.Value.BookingPaymentStatus.PaymentRecordNeverCreated,
                _ => throw new ArgumentOutOfRangeException()
            },
            IsPaymentRequired = src.IsPaymentRequired,
            BookedOnMarketplace = src.BookedOnMarketplace,
            BookingCheckoutSession = MapTo(src.StripeCheckoutSession),
            PaymentMethod = src.PaymentMethod switch
            {
                BookingPaymentMethod.Card => PaymentMethod.Card,
                BookingPaymentMethod.BankAccount => PaymentMethod.BankAccount,
                _ => throw new ArgumentOutOfRangeException()
            },
            SendInvoice = src.SendInvoice ?? false,
            InvoiceUrl = src.InvoiceUrl.ToSafeString()
        };

        if (src.PaidByCustomer is not null)
        {
            booking.PaidByCustomerId = src.PaidByCustomer.Id;
        }

        if (src.PaidByOrganization is not null)
        {
            booking.PaidByOrganizationId = src.PaidByOrganization.Id;
        }

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
        booking.LineItems.AddRange(MapTo(src.LineItems));
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
            Metadata = new Dictionary<string, string> { { "productId", product.Id }, { "organizationId", organizationId } }
        };

    public ProductVersion MapTo(Database.Entities.ProductVersion src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            ModifiedAt = src.ModifiedAt,
            Name = src.Name.ToSafeString(),
            Price = src.Price ?? 0,
            PriceUnit = src.PriceUnit!.ToPriceUnit(),
            Currency = src.Currency!.ToCurrency()
        };

    public CustomerCreateOptions MapToCustomerCreateOption(Organization src) =>
        new()
        {
            Name = src.Name,
            Email = string.IsNullOrWhiteSpace(src.ContactEmail) ? null : src.ContactEmail,
            Phone = string.IsNullOrWhiteSpace(src.ContactPhone) ? null : src.ContactPhone,
            Metadata = new Dictionary<string, string> { { "type", "organization" }, { "organizationId", src.Id } }
        };

    public CustomerUpdateOptions MergeToCustomerUpdateOption(Organization src) =>
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

    public CustomerUpdateOptions MergeToCustomerUpdateOption(Customer src) =>
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
            Type = src.Type.ToBookingType(),
            PaymentStatus = src.PaymentStatus.ToBookingPaymentStatus(),
            IsPaymentRequired = src.IsPaymentRequired,
            Schedules = src.Schedules,
            LineItems = src.LineItems,
            BookedOnMarketplace = src.BookedOnMarketplace,
            ResourceBookingSlots = MapTo(src.ResourceBookingSlots).ToList(),
            InvolvedCustomers = MapTo(src.InvolvedCustomers).ToList(),
            InvolvedOrganizations = MapTo(src.InvolvedOrganizations).ToList(),
            InvolvedLocations = MapTo(src.InvolvedLocations).ToList(),
            InvolvedTeams = MapTo(src.InvolvedTeams).ToList(),
            PaidByCustomer = MapTo(src.PaidByCustomer),
            PaidByOrganization = MapTo(src.PaidByOrganization),
            CreatedByCustomer = MapTo(src.CreatedByCustomer),
            LastModifiedByCustomer = MapTo(src.LastModifiedByCustomer),
            DeletedByCustomer = MapTo(src.DeletedByCustomer),
            ProductVersions = MapTo(src.ProductVersions).ToList(),
            PaymentMethod = src.PaymentMethod.ToBookingPaymentMethod(),
            SendInvoice = src.SendInvoice,
            InvoiceUrl = src.InvoiceUrl
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
                Identities = MapTo(src.Identities).ToList()
            };

    private static IEnumerable<Identity> MapTo(IEnumerable<Database.Entities.Identity> src) => src.Select(MapTo);

    private static Identity MapTo(Database.Entities.Identity src) =>
        new() { Id = src.Id, Email = src.Email, EmailVerified = src.EmailVerified };

    private static Models.Resource MapTo(Database.Entities.Resource src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            DeletedAt = src.DeletedAt,
            ModifiedAt = src.ModifiedAt,
            EventRaisedAt = src.EventRaisedAt,
            Name = src.Name,
            Capacity = src.Capacity,
            Inactive = src.Inactive,
            RequireBookingApproval = src.RequireBookingApproval,
            Color = src.Color,
            OrganizationTags = MapTo(src.OrganizationTags).ToList()
        };

    private static IEnumerable<OrganizationTag> MapTo(IEnumerable<Database.Entities.OrganizationTag> src) => src.Select(MapTo);

    private static OrganizationTag MapTo(Database.Entities.OrganizationTag src) =>
        new() { Id = src.Id, Name = src.Name, Type = src.Type.ToNullableOrganizationTagType(), Color = src.Color };

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
                Name = src.Name,
                ContactEmail = src.ContactEmail,
                ContactPhone = src.ContactPhone,
                LogoUrl = src.LogoUrl,
                Offering = src.Offering,
                Type = src.Type.ToOrganizationType(),
                MemberVisibilityPolicy = src.MemberVisibilityPolicy.ToOrganizationMemberVisibilityPolicy()
            };


    private IEnumerable<Location> MapTo(IEnumerable<Database.Entities.Location> src) => src.Select(MapTo)!;

    public Location? MapTo(Database.Entities.Location? src) =>
        src is null
            ? null
            : new Location
            {
                Id = src.Id,
                CreatedAt = src.CreatedAt,
                DeletedAt = src.DeletedAt,
                ModifiedAt = src.ModifiedAt,
                EventRaisedAt = src.EventRaisedAt,
                Name = src.Name,
                OrganizationTags = MapTo(src.OrganizationTags).ToList()
            };

    private static IEnumerable<Team> MapTo(IEnumerable<Database.Entities.Team> src) => src.Select(MapTo)!;

    private static Team? MapTo(Database.Entities.Team? src) =>
        src is null
            ? null
            : new Team
            {
                Id = src.Id,
                CreatedAt = src.CreatedAt,
                DeletedAt = src.DeletedAt,
                ModifiedAt = src.ModifiedAt,
                EventRaisedAt = src.EventRaisedAt,
                Name = src.Name
            };

    private IEnumerable<ProductVersion> MapTo(IEnumerable<Database.Entities.ProductVersion> src) =>
        src.Select(MapTo);

    private static BookingCheckoutSession? MapTo(StripeCheckoutSession? src) =>
        src is null
            ? null
            : new BookingCheckoutSession
            {
                Id = src.Id,
                CheckoutUrl = src.CheckoutUrl.ToSafeString(),
                PaymentStatus = src.PaymentStatus switch
                {
                    PaymentStatus.NoPaymentRequired => Api.Shared.Clients.Events.Skedular.Booking.V1.Value.PaymentStatus.NoPaymentRequired,
                    PaymentStatus.Pending => Api.Shared.Clients.Events.Skedular.Booking.V1.Value.PaymentStatus.Pending,
                    PaymentStatus.Paid => Api.Shared.Clients.Events.Skedular.Booking.V1.Value.PaymentStatus.Paid,
                    PaymentStatus.Unpaid => Api.Shared.Clients.Events.Skedular.Booking.V1.Value.PaymentStatus.Unpaid,
                    PaymentStatus.Expired => Api.Shared.Clients.Events.Skedular.Booking.V1.Value.PaymentStatus.Expired,
                    _ => throw new ArgumentOutOfRangeException()
                },
                AmountTotal = src.AmountTotal is null ? string.Empty : src.AmountTotal.Value.ToRoundedPrice(),
                Currency = src.Currency.ToSafeString()
            };

    private static IEnumerable<Resource> MapTo(IEnumerable<ResourceCustomersPair> src) =>
        src.Select(item =>
        {
            var resource = new Resource { Id = item.Resource.Id };

            resource.CustomerIds.AddRange(item.Customers.Select(customer => customer.Id));

            return resource;
        });

    private static IEnumerable<BookingSchedule> MapTo(IEnumerable<Api.Shared.Services.Models.BookingSchedule> src) => src.Select(MapTo);

    private static BookingSchedule MapTo(Api.Shared.Services.Models.BookingSchedule src) =>
        new() { From = src.From.ToTimestamp(), Until = src.Until.ToTimestamp() };

    private static IEnumerable<LineItem> MapTo(IEnumerable<ProductVersionLineItem> src) => src.Select(MapTo);

    private static LineItem MapTo(ProductVersionLineItem src) =>
        new() { ProductVersionId = src.ProductVersionId, Quantity = src.Quantity };
}
