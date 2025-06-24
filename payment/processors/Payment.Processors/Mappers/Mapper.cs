using Api.Shared.Clients.Events.Skedular.Organization.V1.Value;
using Api.Shared.Services.Models;
using Api.Shared.Services.Offering;
using Enterprise.Shared;
using Payment.Shared.Database.Entities;
using Stripe;
using Stripe.Checkout;
using Address = Payment.Shared.Models.Address;
using Booking = Payment.Shared.Models.Booking;
using Customer = Payment.Shared.Models.Customer;
using Event = Api.Shared.Clients.Events.Skedular.Customer.V1.Value.Event;
using Identity = Payment.Shared.Models.Identity;
using Organization = Payment.Shared.Models.Organization;
using OrganizationMember = Payment.Shared.Database.Entities.OrganizationMember;
using OrganizationOffering = Payment.Shared.Database.Entities.OrganizationOffering;
using OrganizationSsoSetting = Payment.Shared.Database.Entities.OrganizationSsoSetting;
using Product = Payment.Shared.Models.Product;

namespace Payment.Processors.Mappers;

public interface IMapper
{
    Customer MapTo(Event src);
    Organization MapTo(Api.Shared.Clients.Events.Skedular.Organization.V1.Value.Event src);
    Booking MapTo(Api.Shared.Clients.Events.Skedular.Booking.V1.Value.Event src);

    Shared.Database.Entities.Customer MergeToEntity(
        Customer src,
        Shared.Database.Entities.Customer dest,
        ICollection<Shared.Database.Entities.Identity> identities);

    Shared.Database.Entities.Identity MapToEntity(Identity src, Shared.Database.Entities.Customer? customer);

    Shared.Database.Entities.Identity MergeToEntity(
        Identity src,
        Shared.Database.Entities.Identity dest,
        Shared.Database.Entities.Customer? customer);

    Shared.Database.Entities.Customer MapToEntity(Customer src);
    Shared.Database.Entities.Customer MergeToEntity(Customer src, Shared.Database.Entities.Customer dest);
    Shared.Database.Entities.Organization MapToEntity(Organization src);
    Shared.Database.Entities.Organization MergeToEntity(Organization src, Shared.Database.Entities.Organization dest);

    OrganizationMember MapToEntity(
        Shared.Models.OrganizationMember src,
        Shared.Database.Entities.Organization organization,
        Shared.Database.Entities.Customer customer);

    OrganizationMember MergeToEntity(
        Shared.Models.OrganizationMember src,
        OrganizationMember dest,
        Shared.Database.Entities.Organization organization,
        Shared.Database.Entities.Customer customer);

    OrganizationOffering MapToEntity(Shared.Models.OrganizationOffering src, Shared.Database.Entities.Organization organization);

    OrganizationOffering MergeToEntity(
        Shared.Models.OrganizationOffering src,
        OrganizationOffering dest,
        Shared.Database.Entities.Organization organization);

    OrganizationSsoSetting MapTo(Shared.Models.OrganizationSsoSetting src, Shared.Database.Entities.Organization organization);

    OrganizationSsoSetting MergeTo(
        Shared.Models.OrganizationSsoSetting src,
        OrganizationSsoSetting dest,
        Shared.Database.Entities.Organization organization);

    Shared.Database.Entities.Address MapTo(Address src, Shared.Database.Entities.Organization organization);

    Shared.Database.Entities.Address MergeTo(
        Address src,
        Shared.Database.Entities.Address dest,
        Shared.Database.Entities.Organization organization);

    Shared.Database.Entities.StripeConnectAccount MergeTo(Account src, Shared.Database.Entities.StripeConnectAccount dest);
    Shared.Models.StripeConnectAccount MapTo(Shared.Database.Entities.StripeConnectAccount src);
    Product MapTo(Api.Shared.Clients.Events.Skedular.Marketplace.V1.Value.Event src);

    ProductVersion MapToEntity(
        Shared.Models.ProductVersion src,
        Shared.Database.Entities.Product product);

    ProductVersion MergeToEntity(
        Shared.Models.ProductVersion src,
        ProductVersion dest,
        Shared.Database.Entities.Product product);

    Shared.Database.Entities.Product MergeToEntity(
        Product src,
        Shared.Database.Entities.Product dest,
        Shared.Database.Entities.Organization organization,
        ICollection<ProductVersion> productVersions);

    Shared.Database.Entities.Booking MergeToEntity(Booking src, Shared.Database.Entities.Booking dest, StripeCheckoutSession stripeCheckoutSession);

    Customer MapTo(Shared.Database.Entities.Customer src);
    Organization? MapTo(Shared.Database.Entities.Organization? src);
    StripeCheckoutSession MergeTo(Session src, StripeCheckoutSession dest);
    Shared.Models.StripeCheckoutSession MapTo(StripeCheckoutSession src);
    Shared.Models.ProductVersion MapTo(ProductVersion src);
}

public class Mapper : IMapper
{
    public Customer MapTo(Event src)
    {
        var customer = src.Data.Customer;
        var deletedAt = customer.DeletedAt?.ToDateTimeOffset();
        var eventRaisedAt = src.Metadata.Time?.ToDateTimeOffset() ?? DateTimeOffset.MinValue;

        return new Customer
        {
            Id = customer.Id,
            DeletedAt = deletedAt,
            EventRaisedAt = eventRaisedAt,
            Identities = customer.Identities
                .Select(item => new Identity { Id = item.Id, Email = item.Email.ToSafeString(), EmailVerified = item.EmailVerified })
                .ToList(),
            Designation = customer.Designation,
            Title = customer.Title,
            Timezone = customer.Timezone,
            Locale = customer.Locale,
            Name = customer.Name,
            GivenName = customer.GivenName,
            MiddleName = customer.MiddleName,
            FamilyName = customer.FamilyName,
            PhotoUrl = customer.PhotoUrl,
            PhotoUrl24 = customer.PhotoUrl24,
            PhotoUrl32 = customer.PhotoUrl32,
            PhotoUrl48 = customer.PhotoUrl48,
            PhotoUrl72 = customer.PhotoUrl72,
            PhotoUrl192 = customer.PhotoUrl192,
            PhotoUrl512 = customer.PhotoUrl512,
            PhoneNumber = customer.PhoneNumber
        };
    }

    public Organization MapTo(Api.Shared.Clients.Events.Skedular.Organization.V1.Value.Event src)
    {
        var organizationAfterState = src.Data.Organization;
        var deletedAt = organizationAfterState.DeletedAt?.ToDateTimeOffset();
        var eventRaisedAt = src.Metadata.Time?.ToDateTimeOffset() ?? DateTimeOffset.MinValue;

        var organization = new Organization
        {
            Id = organizationAfterState.Id,
            DeletedAt = deletedAt,
            EventRaisedAt = eventRaisedAt,
            Name = organizationAfterState.Name,
            Website = organizationAfterState.Website,
            Type = organizationAfterState.Type.ToOrganizationType(),
            MemberVisibilityPolicy = organizationAfterState.MemberVisibilityPolicy.ToOrganizationMemberVisibilityPolicy(),
            ContactEmail = organizationAfterState.ContactEmail,
            ContactPhone = organizationAfterState.ContactPhone,
            PhysicalAddress = MapTo(organizationAfterState.PhysicalAddress)
        };

        organization.OrganizationMembers = organizationAfterState.Members.Select(item =>
        {
            return new Shared.Models.OrganizationMember
            {
                Id = item.Id,
                Role = item.Role switch
                {
                    Role.Owner => OrganizationMemberRole.Owner,
                    Role.Administrator => OrganizationMemberRole.Administrator,
                    Role.Member => OrganizationMemberRole.Member,
                    _ => throw new ArgumentOutOfRangeException()
                },
                Status = item.Status switch
                {
                    Status.Active => OrganizationMemberStatus.Active,
                    Status.Inactive => OrganizationMemberStatus.Inactive,
                    _ => throw new ArgumentOutOfRangeException()
                },
                Customer = new Customer { Id = item.CustomerId },
                Organization = organization
            };
        }).ToList();

        organization.OrganizationOfferings =
        [
            new Shared.Models.OrganizationOffering
            {
                Id = organizationAfterState.Offering.Id,
                EventRaisedAt = eventRaisedAt,
                Code = organizationAfterState.Offering.Code.ToOfferingCode(),
                Start = organizationAfterState.Offering.Start.ToDateTimeOffset(),
                End = organizationAfterState.Offering.End.ToDateTimeOffset(),
                Organization = organization
            }
        ];

        organization.OrganizationSsoSettings = organizationAfterState.SsoSettings is null
            ? null
            : new Shared.Models.OrganizationSsoSetting
            {
                Id = organizationAfterState.SsoSettings.Id,
                EventRaisedAt = eventRaisedAt,
                EntityId = organizationAfterState.SsoSettings.EntityId,
                LoginUrl = organizationAfterState.SsoSettings.LoginUrl,
                AppFederationMetadataUrl = organizationAfterState.SsoSettings.AppFederationMetadataUrl,
                IsActive = organizationAfterState.SsoSettings.IsActive,
                Organization = organization
            };

        return organization;
    }

    public Booking MapTo(Api.Shared.Clients.Events.Skedular.Booking.V1.Value.Event src)
    {
        var booking = src.Data.Booking;
        var deletedAt = booking.DeletedAt?.ToDateTimeOffset();
        var eventRaisedAt = src.Metadata.Time?.ToDateTimeOffset() ?? DateTimeOffset.MinValue;

        return new Booking
        {
            Id = booking.Id,
            DeletedAt = deletedAt,
            EventRaisedAt = eventRaisedAt,
            IsPaymentRequired = booking.IsPaymentRequired,
            Schedules = booking.Schedules.Select(item => new BookingSchedule(item.From.ToDateTimeOffset(), item.Until.ToDateTimeOffset())).ToList(),
            LineItems = booking.LineItems.Select(item => new ProductVersionLineItem(item.ProductVersionId, item.Quantity)).ToList(),
            PaidByCustomer = string.IsNullOrWhiteSpace(booking.PaidByCustomerId) ? null : new Customer { Id = booking.PaidByCustomerId },
            PaidByOrganization = string.IsNullOrWhiteSpace(booking.PaidByOrganizationId)
                ? null
                : new Organization { Id = booking.PaidByOrganizationId }
        };
    }

    public Shared.Database.Entities.Customer MergeToEntity(
        Customer src,
        Shared.Database.Entities.Customer dest,
        ICollection<Shared.Database.Entities.Identity> identities)
    {
        dest.Id = src.Id;
        dest.Identities = identities;
        return dest;
    }

    public Shared.Database.Entities.Identity MapToEntity(Identity src, Shared.Database.Entities.Customer? customer) =>
        MergeToEntity(src, new Shared.Database.Entities.Identity(), customer);

    public Shared.Database.Entities.Identity MergeToEntity(
        Identity src,
        Shared.Database.Entities.Identity dest,
        Shared.Database.Entities.Customer? customer)
    {
        dest.Id = src.Id;
        if (customer is not null)
        {
            dest.Customer = customer;
        }

        return dest;
    }

    public Shared.Database.Entities.Customer MapToEntity(Customer src) => MergeToEntity(src, new Shared.Database.Entities.Customer());

    public Shared.Database.Entities.Customer MergeToEntity(Customer src, Shared.Database.Entities.Customer dest)
    {
        dest.Id = src.Id;
        dest.EventRaisedAt = src.EventRaisedAt;
        dest.Locale = src.Locale;
        dest.Name = src.Name;
        dest.GivenName = src.GivenName;
        dest.MiddleName = src.MiddleName;
        dest.FamilyName = src.FamilyName;
        dest.PhoneNumber = src.PhoneNumber;
        return dest;
    }

    public Shared.Database.Entities.Organization MapToEntity(Organization src) => MergeToEntity(src, new Shared.Database.Entities.Organization());

    public Shared.Database.Entities.Organization MergeToEntity(Organization src, Shared.Database.Entities.Organization dest)
    {
        dest.Id = src.Id;
        dest.EventRaisedAt = src.EventRaisedAt;
        dest.Name = src.Name;
        dest.Website = src.Website;
        dest.Type = src.Type.ToOrganizationType();
        dest.MemberVisibilityPolicy = src.MemberVisibilityPolicy.ToOrganizationMemberVisibilityPolicy();
        dest.ContactEmail = src.ContactEmail;
        dest.ContactPhone = src.ContactPhone;
        return dest;
    }

    public OrganizationMember MapToEntity(
        Shared.Models.OrganizationMember src,
        Shared.Database.Entities.Organization organization,
        Shared.Database.Entities.Customer customer) =>
        MergeToEntity(src, new OrganizationMember(), organization, customer);

    public OrganizationMember MergeToEntity(
        Shared.Models.OrganizationMember src,
        OrganizationMember dest,
        Shared.Database.Entities.Organization organization,
        Shared.Database.Entities.Customer customer)
    {
        dest.Id = src.Id;
        dest.EventRaisedAt = src.EventRaisedAt;
        dest.Role = src.Role.ToNullableOrganizationMemberRole();
        dest.Status = src.Status.ToOrganizationMemberStatus();
        dest.Organization = organization;
        dest.Customer = customer;
        return dest;
    }

    public OrganizationOffering MapToEntity(Shared.Models.OrganizationOffering src, Shared.Database.Entities.Organization organization) =>
        MergeToEntity(src, new OrganizationOffering(), organization);

    public OrganizationOffering MergeToEntity(
        Shared.Models.OrganizationOffering src,
        OrganizationOffering dest,
        Shared.Database.Entities.Organization organization)
    {
        dest.Id = src.Id;
        dest.EventRaisedAt = src.EventRaisedAt;
        dest.Code = src.Code;
        dest.Start = src.Start;
        dest.End = src.End;
        dest.Organization = organization;
        return dest;
    }

    public OrganizationSsoSetting MapTo(Shared.Models.OrganizationSsoSetting src, Shared.Database.Entities.Organization organization) =>
        MergeTo(src, new OrganizationSsoSetting(), organization);

    public OrganizationSsoSetting MergeTo(
        Shared.Models.OrganizationSsoSetting src,
        OrganizationSsoSetting dest,
        Shared.Database.Entities.Organization organization)
    {
        dest.Id = src.Id;
        dest.EventRaisedAt = src.EventRaisedAt;
        dest.EntityId = src.EntityId;
        dest.LoginUrl = src.LoginUrl;
        dest.AppFederationMetadataUrl = src.AppFederationMetadataUrl;
        dest.IsActive = src.IsActive;
        dest.Organization = organization;

        return dest;
    }

    public Shared.Database.Entities.Address MapTo(Address src, Shared.Database.Entities.Organization organization) =>
        MergeTo(src, new Shared.Database.Entities.Address(), organization);

    public Shared.Database.Entities.Address MergeTo(
        Address src,
        Shared.Database.Entities.Address dest,
        Shared.Database.Entities.Organization organization)
    {
        dest.Id = src.Id;
        dest.AddressLine1 = src.AddressLine1;
        dest.AddressLine2 = src.AddressLine2;
        dest.Suburb = src.Suburb;
        dest.City = src.City;
        dest.Province = src.Province;
        dest.Zipcode = src.Zipcode;
        dest.Country = src.Country;
        dest.Latitude = src.Latitude;
        dest.Longitude = src.Longitude;
        dest.Organization = organization;
        return dest;
    }

    public Shared.Database.Entities.StripeConnectAccount MergeTo(Account src, Shared.Database.Entities.StripeConnectAccount dest)
    {
        dest.StripeAccountId = src.Id;
        dest.ChargesEnabled = src.ChargesEnabled;
        dest.PayoutsEnabled = src.PayoutsEnabled;
        dest.Type = src.Type.ToSafeString();
        dest.Country = src.Country;
        dest.DefaultCurrency = src.DefaultCurrency;
        dest.BusinessType = src.BusinessType;
        dest.CompanyName = src.BusinessProfile?.Name;
        dest.Url = src.BusinessProfile?.Url;
        dest.SupportUrl = src.BusinessProfile?.SupportUrl;
        dest.ContactEmail = src.Email;
        dest.ContactPhone = src.BusinessProfile?.SupportPhone;
        dest.DetailsSubmitted = src.DetailsSubmitted;
        dest.CapabilitiesCardPayments = src.Capabilities.CardPayments.ToSafeString();
        dest.CapabilitiesTransfers = src.Capabilities.Transfers.ToSafeString();
        return dest;
    }

    public Shared.Models.StripeConnectAccount MapTo(Shared.Database.Entities.StripeConnectAccount src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            ModifiedAt = src.ModifiedAt,
            DeletedAt = src.DeletedAt,
            StripeAccountId = src.StripeAccountId,
            Name = src.Name,
            ChargesEnabled = src.ChargesEnabled,
            PayoutsEnabled = src.PayoutsEnabled,
            Type = src.Type,
            Country = src.Country,
            DefaultCurrency = src.DefaultCurrency,
            BusinessType = src.BusinessType,
            CompanyName = src.CompanyName,
            Url = src.Url,
            SupportUrl = src.SupportUrl,
            ContactEmail = src.ContactEmail,
            ContactPhone = src.ContactPhone,
            DetailsSubmitted = src.DetailsSubmitted,
            CapabilitiesCardPayments = src.CapabilitiesCardPayments,
            CapabilitiesTransfers = src.CapabilitiesTransfers,
            OnboardingUrl = src.OnboardingUrl,
            Organization = MapTo(src.Organization),
            StripeConnectAccountAuthorization = MapTo(src.StripeConnectAccountAuthorization)
        };

    public Product MapTo(Api.Shared.Clients.Events.Skedular.Marketplace.V1.Value.Event src)
    {
        var productAfterState = src.Data.Product;
        var deletedAt = productAfterState.DeletedAt?.ToDateTimeOffset();
        var eventRaisedAt = src.Metadata.Time?.ToDateTimeOffset() ?? DateTimeOffset.MinValue;

        var product = new Product
        {
            Id = productAfterState.Id,
            DeletedAt = deletedAt,
            EventRaisedAt = eventRaisedAt,
            Organization = new Organization { Id = productAfterState.OrganizationId }
        };

        product.ProductVersions = new List<Shared.Models.ProductVersion> { MapTo(productAfterState.LatestProductVersion, product) };

        return product;
    }

    public ProductVersion MapToEntity(Shared.Models.ProductVersion src, Shared.Database.Entities.Product product) =>
        MergeToEntity(src, new ProductVersion(), product);

    public ProductVersion MergeToEntity(
        Shared.Models.ProductVersion src,
        ProductVersion dest,
        Shared.Database.Entities.Product product)
    {
        dest.Id = src.Id;
        dest.Name = src.Name;
        dest.Price = src.Price;
        dest.PriceUnit = src.PriceUnit.ToPriceUnit();
        dest.Currency = src.Currency.ToCurrency();
        dest.Product = product;
        return dest;
    }

    public Shared.Database.Entities.Product MergeToEntity(
        Product src,
        Shared.Database.Entities.Product dest,
        Shared.Database.Entities.Organization organization,
        ICollection<ProductVersion> productVersions)
    {
        dest.Id = src.Id;
        dest.EventRaisedAt = src.EventRaisedAt;
        dest.Organization = organization;
        dest.ProductVersions = productVersions;
        return dest;
    }

    public Shared.Database.Entities.Booking MergeToEntity(
        Booking src,
        Shared.Database.Entities.Booking dest,
        StripeCheckoutSession stripeCheckoutSession)
    {
        dest.Id = src.Id;
        dest.EventRaisedAt = src.EventRaisedAt;
        dest.Schedules = src.Schedules;
        dest.LineItems = src.LineItems;
        dest.StripeCheckoutSession = stripeCheckoutSession;
        return dest;
    }

    public Customer MapTo(Shared.Database.Entities.Customer src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            ModifiedAt = src.ModifiedAt,
            DeletedAt = src.DeletedAt,
            Locale = src.Locale,
            Name = src.Name,
            GivenName = src.GivenName,
            MiddleName = src.MiddleName,
            FamilyName = src.FamilyName,
            PhoneNumber = src.PhoneNumber
        };

    public Organization? MapTo(Shared.Database.Entities.Organization? src) =>
        src is null
            ? null
            : new Organization
            {
                Id = src.Id,
                CreatedAt = src.CreatedAt,
                ModifiedAt = src.ModifiedAt,
                DeletedAt = src.DeletedAt,
                Type = src.Type.ToOrganizationType(),
                MemberVisibilityPolicy = src.MemberVisibilityPolicy.ToOrganizationMemberVisibilityPolicy(),
                ContactEmail = src.ContactEmail,
                ContactPhone = src.ContactPhone
            };

    public StripeCheckoutSession MergeTo(Session src, StripeCheckoutSession dest)
    {
        dest.AmountTotal = src.AmountTotal is null ? null : (decimal)src.AmountTotal / 100;
        dest.Currency = src.Currency;
        dest.PaymentStatus = src.PaymentStatus switch
        {
            "no_payment_required" => PaymentStatusConstants.NoPaymentRequired,
            "paid" => PaymentStatusConstants.Paid,
            "unpaid" => PaymentStatusConstants.Unpaid,
            _ => throw new ArgumentOutOfRangeException()
        };

        return dest;
    }

    public Shared.Models.StripeCheckoutSession MapTo(StripeCheckoutSession src) =>
        new()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            ModifiedAt = src.ModifiedAt,
            DeletedAt = src.DeletedAt,
            StripeCheckoutSessionId = src.StripeCheckoutSessionId,
            CheckoutUrl = src.CheckoutUrl,
            PaymentStatus = src.PaymentStatus.ToPaymentStatus(),
            AmountTotal = src.AmountTotal,
            Currency = src.Currency,
            Booking = MapTo(src.Booking)
        };

    public Shared.Models.ProductVersion MapTo(ProductVersion src) =>
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

    private static Booking? MapTo(Shared.Database.Entities.Booking? src) =>
        src is null
            ? null
            : new Booking
            {
                Id = src.Id,
                CreatedAt = src.CreatedAt,
                ModifiedAt = src.ModifiedAt,
                LineItems = src.LineItems ?? [],
                Schedules = src.Schedules ?? []
            };

    private static Address? MapTo(Api.Shared.Clients.Events.Skedular.Organization.V1.Value.Address? src) =>
        src is null
            ? null
            : new Address
            {
                Id = src.Id,
                AddressLine1 = src.AddressLine1,
                AddressLine2 = src.AddressLine2,
                Suburb = src.Suburb,
                City = src.City,
                Province = src.Province,
                Zipcode = src.Zipcode,
                Country = src.Country
            };

    private static Shared.Models.ProductVersion MapTo(Api.Shared.Clients.Events.Skedular.Marketplace.V1.Value.ProductVersion src, Product product) =>
        new()
        {
            Id = src.Id,
            Name = src.Name.ToSafeString(),
            Price = src.Price.FromRoundedPrice(),
            PriceUnit = src.PriceUnit.ToPriceUnit(),
            Currency = src.Currency.ToCurrency(),
            Product = product
        };

    private static Shared.Models.StripeConnectAccountAuthorization? MapTo(StripeConnectAccountAuthorization? src) =>
        src is null
            ? null
            : new Shared.Models.StripeConnectAccountAuthorization
            {
                Id = src.Id, CreatedAt = src.CreatedAt, ModifiedAt = src.ModifiedAt, IsAuthorized = src.IsAuthorized
            };
}
