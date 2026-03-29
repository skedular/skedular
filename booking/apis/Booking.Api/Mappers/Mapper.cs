using Api.Shared.Services;
using Api.Shared.Services.Grpc.Skedular.Booking.V1;
using Api.Shared.Services.Models;
using Booking.Api.GraphQL.Booking;
using Booking.Api.GraphQL.MarketplaceBookingSubscription;
using Booking.Api.GraphQL.Payment;
using Booking.Api.GraphQL.RecurringBooking;
using Booking.Shared.Models;
using Enterprise.Shared;
using Enterprise.Shared.Sanitization;
using Google.Protobuf.WellKnownTypes;
using HotChocolate.Types.Pagination;
using BookingCategory = Api.Shared.Services.Models.BookingCategory;
using BookingChannel = Api.Shared.Services.Models.BookingChannel;
using BookingEdge = Booking.Api.GraphQL.Booking.BookingEdge;
using BookingSchedule = Api.Shared.Services.Models.BookingSchedule;
using Customer = Booking.Shared.Models.Customer;
using Location = Booking.Shared.Database.Entities.Location;
using MarketplaceBooking = Booking.Shared.Models.MarketplaceBooking;
using OrganizationTag = Booking.Shared.Models.OrganizationTag;
using PaymentMethod = Api.Shared.Services.Models.PaymentMethod;
using PaymentStatus = Api.Shared.Services.Models.PaymentStatus;
using Resource = Booking.Shared.Models.Resource;

namespace Booking.Api.Mappers;

public interface IMapper
{
    BookingDetails MapTo(Shared.Models.Booking src);
    OrganizationArrearsInvoiceDetails MapTo(OrganizationArrearsInvoice src);
    RecurringBookingDetails? MapTo(RecurringBooking? src);
    MarketplaceBookingSubscriptionDetails MapTo(MarketplaceBookingSubscription src);
    Shared.Models.Booking MapTo(AddPrivateBookingInput src);
    RecurringBooking MapTo(AddPrivateRecurringBookingInput src);
    RecurringBooking MapTo(UpdatePrivateRecurringBookingInput src);
    MarketplaceBookingSubscription MapTo(AddMarketplaceBookingSubscriptionInput src);
    Shared.Models.Booking MapTo(UpdatePrivateBookingInput src);
    Shared.Models.Booking MapTo(AddMarketplaceBookingInput src);
    Shared.Models.Booking MapTo(UpdateMarketplaceBookingInput src);
    Shared.Models.Location? MapTo(Location? src);
    global::Api.Shared.Services.Grpc.Skedular.Booking.V1.Booking MapToGrpcResponse(Shared.Models.Booking src);
    Shared.Models.Booking MapTo(AddPrivateInput src);
    Shared.Models.Booking MapTo(UpdatePrivateInput src);
    Edge<Shared.Models.Booking> MapTo(Edge<Shared.Database.Entities.Booking> src);
    Edge<RecurringBooking> MapTo(Edge<Shared.Database.Entities.RecurringBooking> src);
    Edge<MarketplaceBookingSubscription> MapTo(Edge<Shared.Database.Entities.MarketplaceBookingSubscription> src);
    BookingEdge MapTo(Edge<Shared.Models.Booking> src);
    RecurringBookingEdge MapTo(Edge<RecurringBooking> src);
    MarketplaceBookingSubscriptionEdge MapTo(Edge<MarketplaceBookingSubscription> src);
    global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingEdge MapToGrpcResponse(Edge<Shared.Models.Booking> src);
    IEnumerable<Resource> MapTo(IEnumerable<Shared.Database.Entities.Resource> src);
    IEnumerable<BookingResourceDetails> MapTo(IEnumerable<Resource> src);
}

public class Mapper(Shared.Mappers.IMapper sharedMapper) : IMapper
{
    public BookingDetails MapTo(Shared.Models.Booking src) =>
        new()
        {
            Id = src.Id,
            From = src.From,
            Until = src.Until,
            Notes = src.Notes,
            Category = new BookingCategoryDetails { Category = src.Category, Name = src.Category.ToBookingCategoryName() },
            Channel = new BookingChannelDetails { Channel = src.Channel, Name = src.Channel.ToBookingChannelName() },
            BookingResources = MapTo(src.Resources, src.InvolvedResources),
            InvolvedCustomerIds = src.InvolvedCustomers.Select(item => item.Id),
            InvolvedOrganizationIds = src.InvolvedOrganizations.Select(item => (item.Id, item.CustomDomain.ToSafeString())),
            InvolvedLocations = MapTo(src.InvolvedLocations),
            InvolvedTeamIds = src.InvolvedTeams.Select(item => item.Id),
            CreatedByCustomerId = src.CreatedByCustomer?.Id,
            LastModifiedByCustomerId = src.LastModifiedByCustomer?.Id,
            DeletedByCustomerId = src.DeletedByCustomer?.Id,
            RecurringBooking = MapTo(src.RecurringBooking),
            MarketplaceBooking = MapTo(src.MarketplaceBooking),
            HasRecurringInstanceOverrides = src.HasRecurringInstanceOverrides
        };

    public OrganizationArrearsInvoiceDetails MapTo(OrganizationArrearsInvoice src) =>
        new()
        {
            InvoiceNumber = src.InvoiceNumber,
            InvoiceUrl = src.InvoiceUrl,
            BillingPeriodStartInclusive = src.BillingPeriodStartInclusive,
            BillingPeriodEndExclusive = src.BillingPeriodEndExclusive,
            Currency = src.Currency.ToCurrency(),
            TotalAmount = src.TotalAmount,
            TotalAmountToDisplay = src.TotalAmount.ToRoundedPrice().ToPriceToDisplay(src.Currency),
            CreatedAt = src.CreatedAt
        };

    public RecurringBookingDetails? MapTo(RecurringBooking? src) =>
        src is null
            ? null
            : new RecurringBookingDetails
            {
                Id = src.Id,
                From = src.From,
                Until = src.Until,
                Category = new BookingCategoryDetails { Category = src.Category, Name = src.Category.ToBookingCategoryName() },
                Channel = new BookingChannelDetails { Channel = src.Channel, Name = src.Channel.ToBookingChannelName() },
                Frequency = new BookingFrequencyDetails { Frequency = src.Frequency, Name = src.Frequency.ToBookingFrequencyName() },
                Interval = src.Interval,
                ByMonthDay = src.ByMonthDay,
                BySetPosition = src.BySetPosition,
                ByWeekDays = src.ByWeekDays.Select(item => new DayOfWeekDetails { DayOfWeek = item, Name = item.ToDayOfWeekName() }),
                EndType = new BookingRecurrenceEndTypeDetails { EndType = src.EndType, Name = src.EndType.ToRecurringBookingEndTypeName() },
                StartDate = src.StartDate,
                EndDate = src.EndDate,
                OccurrenceCount = src.OccurrenceCount,
                SkippedDates = src.SkippedDates,
                InvolvedCustomerIds = src.InvolvedCustomers.Select(item => item.Id),
                InvolvedOrganizationIds = src.InvolvedOrganizations.Select(item => (item.Id, item.CustomDomain.ToSafeString())),
                InvolvedTeamIds = src.InvolvedTeams.Select(item => item.Id),
                CreatedByCustomerId = src.CreatedByCustomer?.Id,
                LastModifiedByCustomerId = src.LastModifiedByCustomer?.Id,
                DeletedByCustomerId = src.DeletedByCustomer?.Id,
                MarketplaceBooking = src.MarketplaceBooking is null ? null : MapTo(src.MarketplaceBooking)
            };

    public MarketplaceBookingSubscriptionDetails MapTo(MarketplaceBookingSubscription src) =>
        new()
        {
            Id = src.Id,
            StartedAt = src.StartedAt,
            CancelledAt = src.CancelledAt,
            NextRenewalAt = src.NextRenewalAt,
            Status = new MarketplaceBookingSubscriptionStatusDetails
                {
                    Type = src.Status, Name = src.Status.ToMarketplaceBookingSubscriptionStatus()
                },
            AutoRenew = src.AutoRenew,
            CancelAtPeriodEnd = src.CancelAtPeriodEnd,
            MarketplaceBooking = MapTo(src.MarketplaceBooking)!,
            InvolvedCustomerIds = src.InvolvedCustomers.Select(item => item.Id),
            InvolvedOrganizationIds = src.InvolvedOrganizations.Select(item => (item.Id, item.CustomDomain.ToSafeString())),
            InvolvedTeamIds = src.InvolvedTeams.Select(item => item.Id),
            CreatedByCustomerId = src.CreatedByCustomer?.Id,
            LastModifiedByCustomerId = src.LastModifiedByCustomer?.Id,
            DeletedByCustomerId = src.DeletedByCustomer?.Id,
            RecurringBookings = MapTo(src.RecurringBookings).ToList()
        };

    public Shared.Models.Booking MapTo(AddPrivateBookingInput src)
    {
        var customers = src.CustomerIds.RemoveInvalidIds().Select(item => new Customer { Id = item }).ToList();

        return new Shared.Models.Booking
        {
            Id = src.Id.ToSafeString(),
            From = src.From,
            Until = src.Until,
            Notes = src.Notes,
            Category = src.Category ?? BookingCategory.WorkingFromOffice,
            Schedules = new List<BookingSchedule> { new(src.From, src.Until) },
            InvolvedCustomers = customers,
            InvolvedLocations = [],
            InvolvedOrganizations = src.OrganizationIds.ToSafeCollection().RemoveInvalidIds().Select(item => new Organization { Id = item })
                .Concat(src.OrganizationCustomDomains.ToSafeCollection().RemoveInvalidIds().Select(item =>
                    new Organization { CustomDomain = item }))
                .ToList(),
            InvolvedTeams = src.TeamIds.RemoveInvalidIds().Select(item => new Team { Id = item }).ToList(),
            Resources = src.ResourceIds.ToSafeCollection().Select(item => new ResourceCustomersPair(new Resource { Id = item }, customers)).ToList()
        };
    }

    public RecurringBooking MapTo(AddPrivateRecurringBookingInput src)
    {
        var customers = src.CustomerIds.RemoveInvalidIds().Select(item => new Customer { Id = item }).ToList();

        return new RecurringBooking
        {
            Id = src.Id.ToSafeString(),
            From = src.From,
            Until = src.Until,
            Category = src.Category ?? BookingCategory.WorkingFromOffice,
            Frequency = src.Frequency,
            Interval = src.Interval,
            ByMonthDay = src.ByMonthDay,
            BySetPosition = src.BySetPosition,
            ByWeekDays = src.ByWeekDays.ToSafeCollection(),
            EndType = src.EndType,
            StartDate = src.StartDate,
            EndDate = src.EndDate,
            OccurrenceCount = src.OccurrenceCount,
            SkippedDates = src.SkippedDates.ToSafeCollection(),
            InvolvedCustomers = customers,
            InvolvedOrganizations = src.OrganizationIds.ToSafeCollection().RemoveInvalidIds().Select(item => new Organization { Id = item })
                .Concat(src.OrganizationCustomDomains.ToSafeCollection().RemoveInvalidIds().Select(item =>
                    new Organization { CustomDomain = item }))
                .ToList(),
            InvolvedTeams = src.TeamIds.RemoveInvalidIds().Select(item => new Team { Id = item }).ToList(),
            RequestedResources = src.RequestedResourceIds.ToSafeCollection().RemoveInvalidIds().Select(item => new Resource { Id = item }).ToList()
        };
    }

    public RecurringBooking MapTo(UpdatePrivateRecurringBookingInput src)
    {
        var customers = src.CustomerIds.RemoveInvalidIds().Select(item => new Customer { Id = item }).ToList();

        return new RecurringBooking
        {
            Id = src.Id,
            From = src.From,
            Until = src.Until,
            Category = src.Category ?? BookingCategory.WorkingFromOffice,
            Frequency = src.Frequency,
            Interval = src.Interval,
            ByMonthDay = src.ByMonthDay,
            BySetPosition = src.BySetPosition,
            ByWeekDays = src.ByWeekDays.ToSafeCollection(),
            EndType = src.EndType,
            StartDate = src.StartDate,
            EndDate = src.EndDate,
            OccurrenceCount = src.OccurrenceCount,
            SkippedDates = src.SkippedDates.ToSafeCollection(),
            InvolvedCustomers = customers,
            InvolvedOrganizations = src.OrganizationIds.ToSafeCollection().RemoveInvalidIds().Select(item => new Organization { Id = item })
                .Concat(src.OrganizationCustomDomains.ToSafeCollection().RemoveInvalidIds().Select(item =>
                    new Organization { CustomDomain = item }))
                .ToList(),
            InvolvedTeams = src.TeamIds.RemoveInvalidIds().Select(item => new Team { Id = item }).ToList(),
            RequestedResources = src.RequestedResourceIds.ToSafeCollection().RemoveInvalidIds().Select(item => new Resource { Id = item }).ToList()
        };
    }

    public MarketplaceBookingSubscription MapTo(AddMarketplaceBookingSubscriptionInput src)
    {
        var customers = src.CustomerIds.RemoveInvalidIds().Select(item => new Customer { Id = item }).ToList();

        return new MarketplaceBookingSubscription
        {
            Id = src.Id.ToSafeString(),
            StartedAt = src.StartedAt,
            Status = MarketplaceBookingSubscriptionStatus.Active,
            AutoRenew = src.AutoRenew,
            CancelAtPeriodEnd = src.CancelAtPeriodEnd,
            InvolvedCustomers = customers,
            InvolvedOrganizations = src.OrganizationIds.ToSafeCollection().RemoveInvalidIds().Select(item => new Organization { Id = item })
                .Concat(src.OrganizationCustomDomains.ToSafeCollection().RemoveInvalidIds().Select(item =>
                    new Organization { CustomDomain = item }))
                .ToList(),
            InvolvedTeams = src.TeamIds.RemoveInvalidIds().Select(item => new Team { Id = item }).ToList(),
            RequestedResources = src.RequestedResourceIds.ToSafeCollection().RemoveInvalidIds().Select(item => new Resource { Id = item }).ToList(),
            MarketplaceBooking = new MarketplaceBooking
            {
                Quantity = src.Quantity,
                ProductVersion = new ProductVersion { Id = src.ProductVersionId },
                PaymentMethod = src.PaymentMethod,
                InvoiceEmailList = src.InvoiceEmailList.ToSafeCollection(),
                ProductPricing = ProductPricing.Empty(src.PricingId),
                CheckoutReturnUrl = src.CheckoutReturnUrl
            }
        };
    }

    public Shared.Models.Booking MapTo(UpdatePrivateBookingInput src)
    {
        var customers = src.CustomerIds.RemoveInvalidIds().Select(item => new Customer { Id = item }).ToList();

        return new Shared.Models.Booking
        {
            Id = src.Id,
            From = src.From,
            Until = src.Until,
            Notes = src.Notes,
            Category = src.Category ?? BookingCategory.WorkingFromOffice,
            Schedules = new List<BookingSchedule> { new(src.From, src.Until) },
            InvolvedCustomers = customers,
            InvolvedLocations = [],
            InvolvedOrganizations =
                src.OrganizationIds.ToSafeCollection().RemoveInvalidIds().Select(item => new Organization { Id = item })
                    .Concat(src.OrganizationCustomDomains.ToSafeCollection().RemoveInvalidIds().Select(item =>
                        new Organization { CustomDomain = item }))
                    .ToList(),
            InvolvedTeams = src.TeamIds.RemoveInvalidIds().Select(item => new Team { Id = item }).ToList(),
            Resources = src.ResourceIds.RemoveInvalidIds().Select(item => new ResourceCustomersPair(new Resource { Id = item }, customers)).ToList()
        };
    }

    public Shared.Models.Booking MapTo(AddMarketplaceBookingInput src)
    {
        var customers = src.CustomerIds.RemoveInvalidIds().Select(item => new Customer { Id = item }).ToList();

        return new Shared.Models.Booking
        {
            Id = src.Id.ToSafeString(),
            From = src.From,
            Until = src.Until,
            Notes = src.Notes,
            Category = src.Category ?? BookingCategory.WorkingFromCoworkingSpace,
            Schedules = new List<BookingSchedule> { new(src.From, src.Until) },
            InvolvedCustomers = customers,
            InvolvedLocations = [],
            InvolvedOrganizations = src.OrganizationIds
                .ToSafeCollection().RemoveInvalidIds().Select(item => new Organization { Id = item })
                .Concat(src.OrganizationCustomDomains.ToSafeCollection().RemoveInvalidIds().Select(item =>
                    new Organization { CustomDomain = item }))
                .ToList(),
            InvolvedTeams = src.TeamIds.RemoveInvalidIds().Select(item => new Team { Id = item }).ToList(),
            Resources = src.ResourceIds.ToSafeCollection().Select(item => new ResourceCustomersPair(new Resource { Id = item }, customers)).ToList(),
            MarketplaceBooking = new MarketplaceBooking
            {
                Quantity = src.Quantity,
                ProductVersion = new ProductVersion { Id = src.ProductVersionId },
                PaymentMethod = src.PaymentMethod,
                InvoiceEmailList = src.InvoiceEmailList.ToSafeCollection(),
                ProductPricing = ProductPricing.Empty(src.PricingId),
                CheckoutReturnUrl = src.CheckoutReturnUrl
            }
        };
    }

    public Shared.Models.Booking MapTo(UpdateMarketplaceBookingInput src)
    {
        var customers = src.CustomerIds.RemoveInvalidIds().Select(item => new Customer { Id = item }).ToList();

        return new Shared.Models.Booking
        {
            Id = src.Id,
            Notes = src.Notes,
            Category = src.Category ?? BookingCategory.WorkingFromCoworkingSpace,
            InvolvedCustomers = customers,
            InvolvedLocations = [],
            InvolvedOrganizations =
                src.OrganizationIds.ToSafeCollection().RemoveInvalidIds().Select(item => new Organization { Id = item })
                    .Concat(src.OrganizationCustomDomains.ToSafeCollection().RemoveInvalidIds().Select(item =>
                        new Organization { CustomDomain = item }))
                    .ToList(),
            InvolvedTeams = src.TeamIds.RemoveInvalidIds().Select(item => new Team { Id = item }).ToList()
        };
    }

    public global::Api.Shared.Services.Grpc.Skedular.Booking.V1.Booking MapToGrpcResponse(Shared.Models.Booking src)
    {
        var booking = new global::Api.Shared.Services.Grpc.Skedular.Booking.V1.Booking
        {
            Id = src.Id,
            From = src.From.ToTimestamp(),
            To = src.Until.ToTimestamp(),
            Notes = src.Notes.ToSafeString(),
            Category = src.Category switch
            {
                BookingCategory.WorkingFromHome => global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingCategory.WorkingFromHome,
                BookingCategory.WorkingFromOffice => global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingCategory.WorkingFromOffice,
                BookingCategory.WorkingFromCoworkingSpace => global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingCategory
                    .WorkingFromCoworkingSpace,
                BookingCategory.SickLeave => global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingCategory.SickLeave,
                BookingCategory.AnnualLeave => global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingCategory.AnnualLeave,
                BookingCategory.WellbeingLeave => global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingCategory.WellbeingLeave,
                BookingCategory.ClientOffice => global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingCategory.ClientOffice,
                BookingCategory.Vacation => global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingCategory.Vacation,
                BookingCategory.TravelingForWork => global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingCategory.TravelingForWork,
                BookingCategory.NonWorkingDay => global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingCategory.NonWorkingDay,
                _ => throw new ArgumentOutOfRangeException()
            },
            Channel = src.Channel switch
            {
                BookingChannel.Private => global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingChannel.Private,
                BookingChannel.Marketplace => global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingChannel.Marketplace,
                _ => throw new ArgumentOutOfRangeException()
            },
            MarketplaceBooking = MapToGrpcResponse(src.MarketplaceBooking),
            CreatedByCustomerId = src.CreatedByCustomer is null ? string.Empty : src.CreatedByCustomer.Id.ToSafeString(),
            LastModifiedByCustomerId = src.LastModifiedByCustomer is null ? string.Empty : src.LastModifiedByCustomer.Id.ToSafeString(),
            DeletedByCustomerId = src.DeletedByCustomer is null ? string.Empty : src.DeletedByCustomer.Id.ToSafeString()
        };

        booking.InvolvedCustomerIds.AddRange(src.InvolvedCustomers.Select(item => item.Id));
        booking.InvolvedOrganizationIds.AddRange(src.InvolvedOrganizations.Select(item => item.Id));
        booking.InvolvedLocationIds.AddRange(src.InvolvedLocations.Select(item => item.Id));
        booking.InvolvedTeamIds.AddRange(src.InvolvedTeams.Select(item => item.Id));
        booking.Resources.AddRange(src.InvolvedResources.Select(item =>
            new global::Api.Shared.Services.Grpc.Skedular.Booking.V1.Resource { Id = item.Id }));

        booking.Schedules.AddRange(MapToGrpcResponse(src.Schedules));

        if (src.HasRecurringInstanceOverrides.HasValue)
        {
            booking.HasRecurringInstanceOverrides = src.HasRecurringInstanceOverrides.Value;
        }

        return booking;
    }

    public Shared.Models.Booking MapTo(AddPrivateInput src)
    {
        var customers = src.CustomerIds.RemoveInvalidIds().Select(item => new Customer { Id = item }).ToList();

        return new Shared.Models.Booking
        {
            Id = src.Id,
            From = src.From.ToDateTimeOffset(),
            Until = src.Until.ToDateTimeOffset(),
            Notes = src.Notes.ToSafeString(),
            Category = MapTo(src.Category),
            Schedules = new List<BookingSchedule> { new(src.From.ToDateTimeOffset(), src.Until.ToDateTimeOffset()) },
            InvolvedCustomers = customers,
            InvolvedOrganizations = src.OrganizationIds.RemoveInvalidIds().Select(item => new Organization { Id = item }).ToList(),
            InvolvedLocations = [],
            InvolvedTeams = src.TeamIds.RemoveInvalidIds().Select(item => new Team { Id = item }).ToList(),
            Resources = src.ResourceIds.Select(item => new ResourceCustomersPair(new Resource { Id = item }, customers)).ToList()
        };
    }

    public Shared.Models.Booking MapTo(UpdatePrivateInput src)
    {
        var customers = src.CustomerIds.RemoveInvalidIds().Select(item => new Customer { Id = item }).ToList();

        return new Shared.Models.Booking
        {
            Id = src.Id,
            From = src.From.ToDateTimeOffset(),
            Until = src.Until.ToDateTimeOffset(),
            Notes = src.Notes.ToSafeString(),
            Category = MapTo(src.Category),
            Schedules = new List<BookingSchedule> { new(src.From.ToDateTimeOffset(), src.Until.ToDateTimeOffset()) },
            InvolvedCustomers = customers,
            InvolvedOrganizations = src.OrganizationIds.RemoveInvalidIds().Select(item => new Organization { Id = item }).ToList(),
            InvolvedLocations = [],
            InvolvedTeams = src.TeamIds.RemoveInvalidIds().Select(item => new Team { Id = item }).ToList(),
            Resources = src.ResourceIds.Select(item => new ResourceCustomersPair(new Resource { Id = item }, customers)).ToList()
        };
    }

    public Shared.Models.Location? MapTo(Location? src) =>
        src is null
            ? null
            : new Shared.Models.Location
            {
                Id = src.Id,
                CreatedAt = src.CreatedAt,
                DeletedAt = src.DeletedAt,
                ModifiedAt = src.ModifiedAt,
                EventRaisedAt = src.EventRaisedAt,
                OrganizationTags = MapTo(src.OrganizationTags).ToList()
            };

    public Edge<Shared.Models.Booking> MapTo(Edge<Shared.Database.Entities.Booking> src) => new(sharedMapper.MapTo(src.Node), src.Cursor);
    public Edge<RecurringBooking> MapTo(Edge<Shared.Database.Entities.RecurringBooking> src) => new(sharedMapper.MapTo(src.Node), src.Cursor);

    public Edge<MarketplaceBookingSubscription> MapTo(Edge<Shared.Database.Entities.MarketplaceBookingSubscription> src) =>
        new(sharedMapper.MapTo(src.Node), src.Cursor);

    public BookingEdge MapTo(Edge<Shared.Models.Booking> src) => new(MapTo(src.Node), src.Cursor);
    public RecurringBookingEdge MapTo(Edge<RecurringBooking> src) => new(MapTo(src.Node)!, src.Cursor);
    public MarketplaceBookingSubscriptionEdge MapTo(Edge<MarketplaceBookingSubscription> src) => new(MapTo(src.Node), src.Cursor);

    public global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingEdge MapToGrpcResponse(Edge<Shared.Models.Booking> src) =>
        new() { Cursor = src.Cursor, Node = MapToGrpcResponse(src.Node) };

    public IEnumerable<Resource> MapTo(IEnumerable<Shared.Database.Entities.Resource> src) => src.Select(MapTo);
    public IEnumerable<BookingResourceDetails> MapTo(IEnumerable<Resource> src) => src.Select(item => MapTo(item, []));

    private static IEnumerable<OrganizationTag> MapTo(IEnumerable<Shared.Database.Entities.OrganizationTag> src) => src.Select(MapTo);

    private static OrganizationTag MapTo(Shared.Database.Entities.OrganizationTag src) =>
        new() { Id = src.Id, Name = src.Name.ToSafeString(), Type = src.Type.ToNullableOrganizationTagType(), Color = src.Color };

    private static Resource MapTo(Shared.Database.Entities.Resource src) =>
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

    private static BookingResourceDetails MapTo(Resource src, IEnumerable<Customer> customers) =>
        new() { Resource = MapToResourceDetails(src), Location = MapTo(src.Location), CustomerIds = customers.Select(item => item.Id) };

    private static BookingResourceDetails MapTo(Resource src) => new() { Resource = MapToResourceDetails(src), Location = MapTo(src.Location) };

    private static IEnumerable<LocationDetails> MapTo(IEnumerable<Shared.Models.Location> src) => src.Select(MapTo)!;

    private static LocationDetails? MapTo(Shared.Models.Location? src) => src is null ? null : new LocationDetails { Id = src.Id, Name = src.Name };

    private static ResourceDetails MapToResourceDetails(Resource src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name,
            Inactive = src.Inactive,
            RequireBookingApproval = src.RequireBookingApproval,
            Color = src.Color,
            Capacity = src.Capacity,
            IsAvailableHoursOverridden = src.IsAvailableHoursOverridden,
            CustomTags = src.OrganizationTags.Where(item => item.Type == OrganizationTagType.Custom).Select(MapTo).ToList(),
            Zones = src.OrganizationTags.Where(item => item.Type == OrganizationTagType.Zone).Select(MapTo).ToList(),
            ProductTags = src.OrganizationTags.Where(item => item.Type == OrganizationTagType.Product).Select(MapTo).ToList(),
            ResourceType = MapTo(src.OrganizationTags.First(item => OrganizationTagTypeConstants.ResourceTypes.Any(tagType => tagType == item.Type)))
        };

    private static OrganizationTagDetails MapTo(OrganizationTag src) => new() { Id = src.Id, Name = src.Name, Type = src.Type, Color = src.Color };

    private static IEnumerable<BookingResourceDetails> MapTo(ICollection<ResourceCustomersPair> src, ICollection<Resource> involvedResources) =>
        src.Count == 0 ? involvedResources.Select(MapTo) : src.Select(item => MapTo(item.Resource, item.Customers));

    private static IEnumerable<global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingSchedule> MapToGrpcResponse(
        IEnumerable<BookingSchedule> src) =>
        src.Select(MapToGrpcResponse);

    private static global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingSchedule MapToGrpcResponse(BookingSchedule src) =>
        new() { From = src.From.ToTimestamp(), Until = src.Until.ToTimestamp() };

    private static BookingCheckoutSessionDetails? MapTo(StripeCheckoutSession? src) =>
        src is null
            ? null
            : new BookingCheckoutSessionDetails { UniqueId = src.Id, CheckoutUrl = src.CheckoutUrl };

    private static BookingCheckoutSession? MapToGrpcResponse(StripeCheckoutSession? src) =>
        src is null
            ? null
            : new BookingCheckoutSession { Id = src.Id, CheckoutUrl = src.CheckoutUrl };

    private static MarketplaceBookingDetails? MapTo(MarketplaceBooking? src) =>
        src is null
            ? null
            : new MarketplaceBookingDetails
            {
                Id = src.Id,
                IsPaymentRequired = src.IsPaymentRequired,
                PaidByCustomerId = src.PaidByCustomer?.Id,
                PaidByOrganizationId = src.PaidByOrganization?.Id,
                PaidByOrganizationUniqueCustomDomain = src.PaidByOrganization?.CustomDomain,
                Quantity = src.Quantity,
                ProductVersionId = src.ProductVersion.Id,
                ProductPricing = src.ProductPricing,
                BookingCheckoutSession = MapTo(src.StripeCheckoutSession),
                PaymentExpiry = src.PaymentExpiry,
                PaymentStatus = new PaymentStatusDetails { Type = src.PaymentStatus, Name = src.PaymentStatus.ToPaymentStatusName() },
                PaymentMethod = new PaymentMethodTypeDetails { Type = src.PaymentMethod, Name = src.PaymentMethod.ToPaymentMethodName() },
                InvoiceUrl = src.InvoiceUrl,
                InvoiceNumber = src.InvoiceNumber,
                InvoiceEmailList = src.InvoiceEmailList,
                BillingMode = src.BillingMode,
                TotalAmountExcludeTax = src.TotalAmountExcludeTax,
                TotalAmountExcludeTaxToDisplay = src.TotalAmountExcludeTax is null || src.Currency is null
                    ? "N/A"
                    : src.TotalAmountExcludeTax.Value.ToRoundedPrice().ToPriceToDisplay(src.Currency.Value),
                TaxAmount = src.TaxAmount,
                TaxAmountToDisplay = src.TaxAmount is null || src.Currency is null
                    ? "N/A"
                    : src.TaxAmount.Value.ToRoundedPrice().ToPriceToDisplay(src.Currency.Value),
                TaxRatePercentage = src.TaxRatePercentage,
                TaxRatePercentageToDisplay = src.TaxRatePercentage is null ? "N/A" : src.TaxRatePercentage.Value.ToRoundedDecimal(),
                TotalAmount = src.TotalAmount,
                TotalAmountToDisplay = src.TotalAmount is null || src.Currency is null
                    ? "N/A"
                    : src.TotalAmount.Value.ToRoundedPrice().ToPriceToDisplay(src.Currency.Value),
                Currency =
                    src.Currency is null ? null : new CurrencyDetails { Type = src.Currency.Value, Name = src.Currency.Value.ToCurrencyName() },
                CurrencyToDisplay = src.Currency is null ? "N/A" : src.Currency.Value.ToCurrencyName()
            };

    private static global::Api.Shared.Services.Grpc.Skedular.Booking.V1.MarketplaceBooking? MapToGrpcResponse(MarketplaceBooking? src)
    {
        if (src is null)
        {
            return null;
        }

        var marketplaceBooking = new global::Api.Shared.Services.Grpc.Skedular.Booking.V1.MarketplaceBooking
        {
            Id = src.Id,
            PaymentStatus = src.PaymentStatus switch
            {
                PaymentStatus.NotSet => global::Api.Shared.Services.Grpc.Skedular.Booking.V1.PaymentStatus.NotSet,
                PaymentStatus.Pending => global::Api.Shared.Services.Grpc.Skedular.Booking.V1.PaymentStatus.Pending,
                PaymentStatus.Rejected => global::Api.Shared.Services.Grpc.Skedular.Booking.V1.PaymentStatus.Rejected,
                PaymentStatus.Confirmed => global::Api.Shared.Services.Grpc.Skedular.Booking.V1.PaymentStatus.Confirmed,
                PaymentStatus.Expired => global::Api.Shared.Services.Grpc.Skedular.Booking.V1.PaymentStatus.Expired,
                PaymentStatus.RecordNeverCreated => global::Api.Shared.Services.Grpc.Skedular.Booking.V1.PaymentStatus.RecordNeverCreated,
                PaymentStatus.NoPaymentRequired => global::Api.Shared.Services.Grpc.Skedular.Booking.V1.PaymentStatus.NoPaymentRequired,
                _ => throw new ArgumentOutOfRangeException()
            },
            IsPaymentRequired = src.IsPaymentRequired,
            PaidByCustomerId = src.PaidByCustomer is null ? string.Empty : src.PaidByCustomer.Id.ToSafeString(),
            PaidByOrganizationId = src.PaidByOrganization is null ? string.Empty : src.PaidByOrganization.Id.ToSafeString(),
            BookingCheckoutSession = MapToGrpcResponse(src.StripeCheckoutSession),
            PaymentExpiry = src.PaymentExpiry.ToTimestamp(),
            TotalAmountExcludeTax = src.TotalAmountExcludeTax.ToNullDouble(),
            TaxAmount = src.TaxAmount.ToNullDouble(),
            TaxRatePercentage = src.TaxRatePercentage.ToNullDouble(),
            TotalAmount = src.TotalAmount.ToNullDouble(),
            Currency = src.Currency.ToNullableCurrency(),
            InvoiceUrl = src.InvoiceUrl.ToSafeString(),
            InvoiceNumber = src.InvoiceNumber.ToSafeString(),
            PaymentMethod = src.PaymentMethod switch
            {
                PaymentMethod.Card => global::Api.Shared.Services.Grpc.Skedular.Booking.V1.PaymentMethod.Card,
                PaymentMethod.BankTransfer => global::Api.Shared.Services.Grpc.Skedular.Booking.V1.PaymentMethod.BankAccount,
                _ => throw new ArgumentOutOfRangeException()
            },
            Quantity = src.Quantity,
            ProductVersionId = src.ProductVersion.Id.ToSafeString()
        };

        marketplaceBooking.InvoiceEmailList.AddRange(src.InvoiceEmailList.ToSafeCollection());

        return marketplaceBooking;
    }

    private static BookingCategory MapTo(global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingCategory src) =>
        src switch
        {
            global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingCategory.WorkingFromHome => BookingCategory.WorkingFromHome,
            global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingCategory.WorkingFromOffice => BookingCategory.WorkingFromOffice,
            global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingCategory.WorkingFromCoworkingSpace => BookingCategory
                .WorkingFromCoworkingSpace,
            global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingCategory.SickLeave => BookingCategory.SickLeave,
            global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingCategory.AnnualLeave => BookingCategory.AnnualLeave,
            global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingCategory.WellbeingLeave => BookingCategory.WellbeingLeave,
            global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingCategory.ClientOffice => BookingCategory.ClientOffice,
            global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingCategory.Vacation => BookingCategory.Vacation,
            global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingCategory.TravelingForWork => BookingCategory.TravelingForWork,
            global::Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingCategory.NonWorkingDay => BookingCategory.NonWorkingDay,
            _ => throw new ArgumentOutOfRangeException()
        };

    private IEnumerable<RecurringBookingDetails> MapTo(IEnumerable<RecurringBooking> src) => src.Select(MapTo)!;
}
