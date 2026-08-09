using Api.Shared.Services;
using Api.Shared.Services.Models;
using Booking.Api.GraphQL.Booking;
using Booking.Api.GraphQL.MarketplaceBookingSubscription;
using Booking.Api.GraphQL.MarketplacePurchaseHistory;
using Booking.Api.GraphQL.Payment;
using Booking.Api.GraphQL.RecurringBooking;
using Booking.Api.Services;
using Booking.Shared.Database.Entities;
using Booking.Shared.Mappers;
using Booking.Shared.Models;
using Enterprise.Shared;
using Enterprise.Shared.Sanitization;
using HotChocolate.Types.Pagination;
using BookingCategory = Api.Shared.Services.Models.BookingCategory;
using BookingEdge = Booking.Api.GraphQL.Booking.BookingEdge;
using BookingSchedule = Api.Shared.Services.Models.BookingSchedule;
using Customer = Booking.Shared.Models.Customer;
using Location = Booking.Shared.Database.Entities.Location;
using MarketplaceBooking = Booking.Shared.Models.MarketplaceBooking;
using MarketplaceBookingSubscription = Booking.Shared.Models.MarketplaceBookingSubscription;
using MarketplaceBookingFailureEntity = Booking.Shared.Database.Entities.MarketplaceBookingFailure;
using Organization = Booking.Shared.Models.Organization;
using OrganizationArrearsInvoice = Booking.Shared.Models.OrganizationArrearsInvoice;
using OrganizationTag = Booking.Shared.Models.OrganizationTag;
using ProductVersion = Booking.Shared.Models.ProductVersion;
using RecurringBooking = Booking.Shared.Models.RecurringBooking;
using Resource = Booking.Shared.Models.Resource;
using StripeCheckoutSession = Booking.Shared.Models.StripeCheckoutSession;
using Team = Booking.Shared.Models.Team;

namespace Booking.Api.Mappers;

public interface IGraphQlMapper
{
    BookingDetails MapTo(Shared.Models.Booking src);
    MarketplaceBookingFailureDetails MapTo(MarketplaceBookingFailureEntity src);
    MarketplaceBookingFailureDetails MapTo(MarketplaceBookingFailureSummary src);
    MarketplaceRefundDetails MapTo(MarketplaceRefund src);
    MarketplaceRefundEventDetails MapTo(MarketplaceRefundEvent src);
    MarketplaceRefundPreviewDetails MapTo(MarketplaceRefundPreviewModel src);
    MarketplaceRefundDetails MapTo(MarketplaceRefundReadModel src);
    MarketplaceRefundEventDetails MapTo(MarketplaceRefundEventModel src);
    OrganizationArrearsInvoiceDetails MapTo(OrganizationArrearsInvoice src);
    RecurringBookingDetails? MapTo(RecurringBooking? src);
    MarketplaceBookingSubscriptionDetails MapTo(MarketplaceBookingSubscription src);
    MarketplacePurchaseHistoryDetails MapTo(MarketplacePurchaseHistoryEntry src);
    Shared.Models.Booking MapTo(AddPrivateBookingInput src);
    RecurringBooking MapTo(AddPrivateRecurringBookingInput src);
    RecurringBooking MapTo(UpdatePrivateRecurringBookingInput src);
    MarketplaceBookingSubscription MapTo(AddMarketplaceBookingSubscriptionInput src);
    Shared.Models.Booking MapTo(UpdatePrivateBookingInput src);
    Shared.Models.Booking MapTo(AddMarketplaceBookingInput src);
    Shared.Models.Booking MapTo(UpdateMarketplaceBookingInput src);
    MarketplaceBookingModificationCommand MapTo(ModifyMarketplaceBookingInput src);
    MarketplaceBookingModificationDetails MapTo(MarketplaceBookingModificationSummary src);
    Shared.Models.Location? MapTo(Location? src);
    Edge<Shared.Models.Booking> MapTo(Edge<Shared.Database.Entities.Booking> src);
    Edge<RecurringBooking> MapTo(Edge<Shared.Database.Entities.RecurringBooking> src);
    Edge<MarketplaceBookingSubscription> MapTo(Edge<Shared.Database.Entities.MarketplaceBookingSubscription> src);
    BookingEdge MapTo(Edge<Shared.Models.Booking> src);
    RecurringBookingEdge MapTo(Edge<RecurringBooking> src);
    MarketplaceBookingSubscriptionEdge MapTo(Edge<MarketplaceBookingSubscription> src);
    IEnumerable<BookingResourceDetails> MapTo(IEnumerable<Resource> src);
    IEnumerable<BookingResourceDetails> MapTo(IEnumerable<Shared.Database.Entities.Resource> src);
}

public class GraphQlMapper(IEntityMapper sharedEntityMapper) : IGraphQlMapper
{
    public MarketplacePurchaseHistoryDetails MapTo(MarketplacePurchaseHistoryEntry src) => new()
    {
        Id = $"marketplace-purchase-history:{src.SourceType}:{src.Id}",
        SourceId = src.Id,
        SourceType = src.SourceType,
        SourceTypeName = src.SourceTypeName,
        LifecycleState = src.LifecycleState,
        LifecycleStateName = src.LifecycleStateName,
        RenewalState = src.RenewalState,
        RenewalStateName = src.RenewalStateName,
        PurchasedAt = src.PurchasedAt,
        ActivityAt = src.ActivityAt,
        BookingFrom = src.BookingFrom,
        BookingUntil = src.BookingUntil,
        PaymentStatus = src.PaymentStatus,
        ProductVersionId = src.ProductVersionId,
        ProductTitle = src.ProductTitle,
        TotalAmount = src.TotalAmount,
        Currency = src.Currency,
        CustomerId = src.CustomerId,
        DeletedByCustomerId = src.DeletedByCustomerId,
        CancellationReason = src.CancellationReason,
        RefundId = src.RefundId,
        BookingId = src.BookingId,
        IsDeleted = src.IsDeleted,
    };

    public MarketplaceRefundPreviewDetails MapTo(MarketplaceRefundPreviewModel src) => new()
    {
        LocalEntityType = src.LocalEntityType.ToMarketplaceRefundEntityTypeValue(),
        LocalEntityId = src.LocalEntityId,
        RequestedAt = src.RequestedAt,
        ReferenceTime = src.ReferenceTime,
        IsRefundable = src.IsRefundable,
        RefundPercentage = src.RefundPercentage,
        AppliedRuleMinutesBefore = src.AppliedRuleMinutesBefore,
        BaseAmount = src.BaseAmount,
        RefundAmount = src.RefundAmount,
        Currency =
            src.Currency is { } currency
                ? new CurrencyDetails
                {
                    Type = currency,
                    Name = currency.ToCurrencyName(),
                }
                : null,
        CurrencyToDisplay = src.Currency is { } displayCurrency ? displayCurrency.ToCurrencyName() : "N/A",
    };

    public MarketplaceRefundDetails MapTo(MarketplaceRefundReadModel src) => new()
    {
        Id = src.Id,
        LocalEntityType = src.LocalEntityType.ToMarketplaceRefundEntityTypeValue(),
        LocalEntityId = src.LocalEntityId,
        Status = new MarketplaceRefundStatusDetails
        {
            Type = src.Status,
            Name = src.Status.ToMarketplaceRefundStatusName(),
        },
        RequestedAt = src.RequestedAt,
        ReferenceTime = src.ReferenceTime,
        RefundPercentage = src.RefundPercentage,
        AppliedRuleMinutesBefore = src.AppliedRuleMinutesBefore,
        BaseAmount = src.BaseAmount,
        RefundAmount = src.RefundAmount,
        Currency =
            src.Currency is { } currency
                ? new CurrencyDetails
                {
                    Type = currency,
                    Name = currency.ToCurrencyName(),
                }
                : null,
        CurrencyToDisplay = src.Currency is { } displayCurrency ? displayCurrency.ToCurrencyName() : "N/A",
        Reason = src.Reason,
        AccountingProvider = src.AccountingProvider,
        ExternalRefundId = src.ExternalRefundId,
        ExternalRefundNumber = src.ExternalRefundNumber,
        LastProcessedAt = src.LastProcessedAt,
        LastError = src.LastError,
        PaymentProvider = src.PaymentProvider,
        ExternalPaymentRefundId = src.ExternalPaymentRefundId,
        PaymentRefundStatus = src.PaymentRefundStatus,
        PaymentRefundLastProcessedAt = src.PaymentRefundLastProcessedAt,
        PaymentRefundLastError = src.PaymentRefundLastError,
        CanProcessInXero = src.CanProcessInXero,
        RequestedByCustomerId = src.RequestedByCustomerId,
        RequestedByCustomerName = src.RequestedByCustomerName,
        XeroProcessingBlockedReason = src.XeroProcessingBlockedReason,
        RefundKind = src.RefundKind.ToString(),
        IdempotencyKey = src.IdempotencyKey,
        PolicySnapshotJson = src.PolicySnapshotJson,
        CalculationResultJson = src.CalculationResultJson,
        TimezoneId = src.TimezoneId,
        RetryCount = src.RetryCount,
        ApprovedAt = src.ApprovedAt,
        ApprovedByCustomerId = src.ApprovedByCustomerId,
        RejectedAt = src.RejectedAt,
        RejectedByCustomerId = src.RejectedByCustomerId,
        RejectionReason = src.RejectionReason,
        CancelledAt = src.CancelledAt,
        CancellationReason = src.CancellationReason,
        BankTransferReference = src.BankTransferReference,
        BankTransferSentAt = src.BankTransferSentAt,
        ReconciledAt = src.ReconciledAt,
        ReconciliationStatus = src.ReconciliationStatus?.ToString(),
        Events = src.Events.Select(MapTo),
        PaymentAllocations =
        [
            .. src.PaymentAllocations.Select(item => new MarketplaceRefundPaymentAllocationDetails
            {
                SourcePaymentProvider = item.SourcePaymentProvider,
                SourcePaymentReference = item.SourcePaymentReference,
                SourcePaymentAmount = item.SourcePaymentAmount,
                AllocatedRefundAmount = item.AllocatedRefundAmount,
                Currency = item.Currency.ToCurrency(),
            }),
        ],
    };

    public MarketplaceRefundEventDetails MapTo(MarketplaceRefundEventModel src) => new()
    {
        Id = src.Id,
        EventType = new MarketplaceRefundEventTypeDetails
        {
            Type = src.EventType,
            Name = src.EventType.ToMarketplaceRefundEventTypeName(),
        },
        OccurredAt = src.OccurredAt,
        RefundAmount = src.RefundAmount,
        CurrencyToDisplay = src.Currency is { } currency ? currency.ToCurrencyName() : "N/A",
        Reason = src.Reason,
        AccountingProvider = src.AccountingProvider,
        ExternalRefundId = src.ExternalRefundId,
        ExternalRefundNumber = src.ExternalRefundNumber,
        LastError = src.LastError,
        ActorCustomerId = src.ActorCustomerId,
        ActorName = src.ActorName,
        PreviousStatus = src.PreviousStatus?.ToString(),
        NewStatus = src.NewStatus?.ToString(),
        CorrelationId = src.CorrelationId,
    };

    public MarketplaceBookingFailureDetails MapTo(MarketplaceBookingFailureEntity src) =>
        new()
        {
            Id = src.Id,
            Category =
                new MarketplaceBookingFailureChoiceDetails
                {
                    Type = src.Category,
                    Name = src.Category.ToMarketplaceBookingFailureCategoryName(),
                },
            Scope = new MarketplaceBookingFailureChoiceDetails
            {
                Type = src.Scope,
                Name = src.Scope.ToMarketplaceBookingFailureScopeName(),
            },
            FinalizedAt = src.FinalizedAt,
            RequestedFrom = src.RequestedFrom,
            RequestedUntil = src.RequestedUntil,
            CustomerAction =
                new MarketplaceBookingFailureChoiceDetails
                {
                    Type = src.CustomerAction.ToSafeString(),
                    Name = src.CustomerAction.ToSafeString().ToMarketplaceBookingFailureCustomerActionName(),
                },
            ResolutionDeadlineAt = src.ResolutionDeadlineAt,
            ResolutionDecidedAt = src.ResolutionDecidedAt,
            ResolutionDecision = src.ResolutionDecision,
            AllocatedRefundAmount = src.AllocatedRefundAmount,
        };

    public MarketplaceBookingFailureDetails MapTo(MarketplaceBookingFailureSummary src) =>
        new()
        {
            Id = src.Id,
            Category =
                new MarketplaceBookingFailureChoiceDetails
                {
                    Type = src.Category,
                    Name = src.Category.ToMarketplaceBookingFailureCategoryName(),
                },
            Scope = new MarketplaceBookingFailureChoiceDetails
            {
                Type = src.Scope,
                Name = src.Scope.ToMarketplaceBookingFailureScopeName(),
            },
            FinalizedAt = src.FinalizedAt,
            RequestedFrom = src.RequestedFrom,
            RequestedUntil = src.RequestedUntil,
            CustomerAction = new MarketplaceBookingFailureChoiceDetails
            {
                Type = src.CustomerAction,
                Name = src.CustomerAction.ToMarketplaceBookingFailureCustomerActionName(),
            },
        };

    public BookingDetails MapTo(Shared.Models.Booking src) =>
        new()
        {
            Id = src.Id,
            EntityFrameworkVersion = src.EntityFrameworkVersion,
            From = src.From,
            Until = src.Until,
            Notes = src.Notes,
            Category = new BookingCategoryDetails
            {
                Category = src.Category,
                Name = src.Category.ToBookingCategoryName(),
            },
            Channel = new BookingChannelDetails
            {
                Channel = src.Channel,
                Name = src.Channel.ToBookingChannelName(),
            },
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
            CancellationPolicyOverridden = src.CancellationPolicyOverridden,
            CancellationOverrideReason = src.CancellationOverrideReason,
            HasRecurringInstanceOverrides = src.HasRecurringInstanceOverrides,
        };

    public OrganizationArrearsInvoiceDetails MapTo(OrganizationArrearsInvoice src) =>
        new()
        {
            InvoiceNumber = src.InvoiceNumber,
            InvoiceUrl = src.InvoiceUrl,
            BillingPeriodStartInclusive = src.BillingPeriodStartInclusive,
            BillingPeriodEndExclusive = src.BillingPeriodEndExclusive,
            Currency = new CurrencyDetails
            {
                Type = src.Currency,
                Name = src.Currency.ToCurrencyName(),
            },
            TotalAmount = src.TotalAmount,
            TotalAmountToDisplay = src.TotalAmount.ToRoundedPrice().ToPriceToDisplay(src.Currency),
            CreatedAt = src.CreatedAt,
        };

    public MarketplaceRefundDetails MapTo(MarketplaceRefund src)
    {
        var currency = src.Currency.ToNullableCurrency();

        return new MarketplaceRefundDetails
        {
            Id = src.Id,
            LocalEntityType = src.LocalEntityType,
            LocalEntityId = src.LocalEntityId,
            Status =
                new MarketplaceRefundStatusDetails
                {
                    Type = src.Status.ToMarketplaceRefundStatus(),
                    Name = src.Status.ToMarketplaceRefundStatus().ToMarketplaceRefundStatusName(),
                },
            RequestedAt = src.RequestedAt,
            ReferenceTime = src.ReferenceTime,
            RefundPercentage = src.RefundPercentage,
            AppliedRuleMinutesBefore = src.AppliedRuleMinutesBefore,
            BaseAmount = src.BaseAmount,
            RefundAmount = src.RefundAmount,
            Currency = currency is null
                ? null
                : new CurrencyDetails
                {
                    Type = currency.Value,
                    Name = currency.Value.ToCurrencyName(),
                },
            CurrencyToDisplay = currency is null ? "N/A" : currency.Value.ToCurrencyName(),
            Reason = src.Reason,
            AccountingProvider = src.AccountingProvider,
            ExternalRefundId = src.ExternalRefundId,
            ExternalRefundNumber = src.ExternalRefundNumber,
            LastProcessedAt = src.LastProcessedAt,
            LastError = src.LastError,
            PaymentProvider = src.PaymentProvider,
            ExternalPaymentRefundId = src.ExternalPaymentRefundId,
            PaymentRefundStatus = src.PaymentRefundStatus,
            PaymentRefundLastProcessedAt = src.PaymentRefundLastProcessedAt,
            PaymentRefundLastError = src.PaymentRefundLastError,
            RequestedByCustomerId = src.RequestedByCustomerId,
            RefundKind = src.RefundKind,
            IdempotencyKey = src.IdempotencyKey,
            PolicySnapshotJson = src.PolicySnapshotJson,
            CalculationResultJson = src.CalculationResultJson,
            TimezoneId = src.TimezoneId,
            RetryCount = src.RetryCount,
            ApprovedAt = src.ApprovedAt,
            ApprovedByCustomerId = src.ApprovedByCustomerId,
            RejectedAt = src.RejectedAt,
            RejectedByCustomerId = src.RejectedByCustomerId,
            RejectionReason = src.RejectionReason,
            CancelledAt = src.CancelledAt,
            CancellationReason = src.CancellationReason,
            BankTransferReference = src.BankTransferReference,
            BankTransferSentAt = src.BankTransferSentAt,
            ReconciledAt = src.ReconciledAt,
            ReconciliationStatus = src.ReconciliationStatus,
            PaymentAllocations =
            [
                .. src.PaymentAllocations.Select(item => new MarketplaceRefundPaymentAllocationDetails
                {
                    SourcePaymentProvider = item.SourcePaymentProvider,
                    SourcePaymentReference = item.SourcePaymentReference,
                    SourcePaymentAmount = item.SourceCapturedAmount,
                    AllocatedRefundAmount = item.AllocatedRefundAmount,
                    Currency = item.Currency,
                }),
            ],
        };
    }

    public MarketplaceRefundEventDetails MapTo(MarketplaceRefundEvent src)
    {
        var currency = src.MarketplaceRefund.Currency.ToNullableCurrency();

        return new MarketplaceRefundEventDetails
        {
            Id = src.Id,
            EventType =
                new MarketplaceRefundEventTypeDetails
                {
                    Type = src.EventType.ToMarketplaceRefundEventType(),
                    Name = src.EventType.ToMarketplaceRefundEventType().ToMarketplaceRefundEventTypeName(),
                },
            OccurredAt = src.OccurredAt,
            RefundAmount = src.RefundAmount,
            CurrencyToDisplay = currency is null ? "N/A" : currency.Value.ToCurrencyName(),
            Reason = src.Reason,
            AccountingProvider = src.AccountingProvider,
            ExternalRefundId = src.ExternalRefundId,
            ExternalRefundNumber = src.ExternalRefundNumber,
            LastError = src.LastError,
            ActorCustomerId = src.ActorCustomerId,
            PreviousStatus = src.PreviousStatus,
            NewStatus = src.NewStatus,
            CorrelationId = src.CorrelationId,
        };
    }

    public RecurringBookingDetails? MapTo(RecurringBooking? src) =>
        src is null
            ? null
            : new RecurringBookingDetails
            {
                Id = src.Id,
                From = src.From,
                Until = src.Until,
                Category = new BookingCategoryDetails
                {
                    Category = src.Category,
                    Name = src.Category.ToBookingCategoryName(),
                },
                Channel = new BookingChannelDetails
                {
                    Channel = src.Channel,
                    Name = src.Channel.ToBookingChannelName(),
                },
                Frequency = new BookingFrequencyDetails
                {
                    Frequency = src.Frequency,
                    Name = src.Frequency.ToBookingFrequencyName(),
                },
                Interval = src.Interval,
                ByMonthDay = src.ByMonthDay,
                BySetPosition = src.BySetPosition,
                ByWeekDays = src.ByWeekDays.Select(item => new DayOfWeekDetails
                {
                    DayOfWeek = item,
                    Name = item.ToDayOfWeekName(),
                }),
                EndType = new BookingRecurrenceEndTypeDetails
                {
                    EndType = src.EndType,
                    Name = src.EndType.ToRecurringBookingEndTypeName(),
                },
                StartDate = src.StartDate,
                EndDate = src.EndDate,
                OccurrenceCount = src.OccurrenceCount,
                SkippedDates = src.SkippedDates,
                RequestedResources = [.. src.RequestedResources.Select(MapToResourceDetails)],
                InvolvedCustomerIds = src.InvolvedCustomers.Select(item => item.Id),
                InvolvedOrganizationIds = src.InvolvedOrganizations.Select(item => (item.Id, item.CustomDomain.ToSafeString())),
                InvolvedTeamIds = src.InvolvedTeams.Select(item => item.Id),
                CreatedByCustomerId = src.CreatedByCustomer?.Id,
                LastModifiedByCustomerId = src.LastModifiedByCustomer?.Id,
                DeletedByCustomerId = src.DeletedByCustomer?.Id,
                MarketplaceBookingSubscriptionId = src.MarketplaceBookingSubscription?.Id,
                MarketplaceBooking = src.MarketplaceBooking is null ? null : MapTo(src.MarketplaceBooking),
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
                Type = src.Status,
                Name = src.Status.ToMarketplaceBookingSubscriptionStatus(),
            },
            AutoRenew = src.AutoRenew,
            CancelAtPeriodEnd = src.CancelAtPeriodEnd,
            CancellationPolicyOverridden = src.CancellationPolicyOverridden,
            CancellationOverrideReason = src.CancellationOverrideReason,
            WeeklySelectedDays = [.. src.WeeklySelectedDays],
            MarketplaceBooking = MapTo(src.MarketplaceBooking)!,
            InvolvedCustomerIds = src.InvolvedCustomers.Select(item => item.Id),
            InvolvedOrganizationIds = src.InvolvedOrganizations.Select(item => (item.Id, item.CustomDomain.ToSafeString())),
            InvolvedTeamIds = src.InvolvedTeams.Select(item => item.Id),
            CreatedByCustomerId = src.CreatedByCustomer?.Id,
            LastModifiedByCustomerId = src.LastModifiedByCustomer?.Id,
            DeletedByCustomerId = src.DeletedByCustomer?.Id,
            RecurringBookings = [.. MapTo(src.RecurringBookings)],
        };

    public Shared.Models.Booking MapTo(AddPrivateBookingInput src)
    {
        var customers = src.CustomerIds.RemoveInvalidIds().Select(item => new Customer
        {
            Id = item,
        }).ToList();

        return new Shared.Models.Booking
        {
            Id = src.Id.ToSafeString(),
            From = src.From,
            Until = src.Until,
            Notes = src.Notes,
            Category = src.Category ?? BookingCategory.WorkingFromOffice,
            Schedules = new List<BookingSchedule>
            {
                new(src.From, src.Until),
            },
            InvolvedCustomers = customers,
            InvolvedLocations = [],
            InvolvedOrganizations =
            [
                .. src.OrganizationIds.ToSafeCollection().RemoveInvalidIds().Select(item => new Organization
                {
                    Id = item,
                }),

                .. src.OrganizationCustomDomains.ToSafeCollection().RemoveInvalidIds().Select(item =>
                    new Organization
                    {
                        CustomDomain = item,
                    }),
            ],
            InvolvedTeams =
            [
                .. src.TeamIds.RemoveInvalidIds().Select(item => new Team
                {
                    Id = item,
                }),
            ],
            Resources =
            [
                .. src.ResourceIds.ToSafeCollection().Select(item => new ResourceCustomersPair(new Resource
                {
                    Id = item,
                }, customers)),
            ],
        };
    }

    public RecurringBooking MapTo(AddPrivateRecurringBookingInput src)
    {
        var customers = src.CustomerIds.RemoveInvalidIds().Select(item => new Customer
        {
            Id = item,
        }).ToList();

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
            InvolvedOrganizations =
            [
                .. src.OrganizationIds.ToSafeCollection().RemoveInvalidIds().Select(item => new Organization
                {
                    Id = item,
                }),

                .. src.OrganizationCustomDomains.ToSafeCollection().RemoveInvalidIds().Select(item =>
                    new Organization
                    {
                        CustomDomain = item,
                    }),
            ],
            InvolvedTeams =
            [
                .. src.TeamIds.RemoveInvalidIds().Select(item => new Team
                {
                    Id = item,
                }),
            ],
            RequestedResources =
            [
                .. src.RequestedResourceIds.ToSafeCollection().RemoveInvalidIds().Select(item => new Resource
                {
                    Id = item,
                }),
            ],
        };
    }

    public RecurringBooking MapTo(UpdatePrivateRecurringBookingInput src)
    {
        var customers = src.CustomerIds.RemoveInvalidIds().Select(item => new Customer
        {
            Id = item,
        }).ToList();

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
            InvolvedOrganizations =
            [
                .. src.OrganizationIds.ToSafeCollection().RemoveInvalidIds().Select(item => new Organization
                {
                    Id = item,
                }),

                .. src.OrganizationCustomDomains.ToSafeCollection().RemoveInvalidIds().Select(item =>
                    new Organization
                    {
                        CustomDomain = item,
                    }),
            ],
            InvolvedTeams =
            [
                .. src.TeamIds.RemoveInvalidIds().Select(item => new Team
                {
                    Id = item,
                }),
            ],
            RequestedResources =
            [
                .. src.RequestedResourceIds.ToSafeCollection().RemoveInvalidIds().Select(item => new Resource
                {
                    Id = item,
                }),
            ],
        };
    }

    public MarketplaceBookingSubscription MapTo(AddMarketplaceBookingSubscriptionInput src)
    {
        var customers = src.CustomerIds.RemoveInvalidIds().Select(item => new Customer
        {
            Id = item,
        }).ToList();

        return new MarketplaceBookingSubscription
        {
            Id = src.Id.ToSafeString(),
            StartedAt = src.StartedAt,
            Status = MarketplaceBookingSubscriptionStatus.Active,
            AutoRenew = src.AutoRenew,
            CancelAtPeriodEnd = src.CancelAtPeriodEnd,
            WeeklySelectedDays = [.. src.WeeklySelectedDays.ToSafeCollection()],
            InvolvedCustomers = customers,
            InvolvedOrganizations =
            [
                .. src.OrganizationIds.ToSafeCollection().RemoveInvalidIds().Select(item => new Organization
                {
                    Id = item,
                }),

                .. src.OrganizationCustomDomains.ToSafeCollection().RemoveInvalidIds().Select(item =>
                    new Organization
                    {
                        CustomDomain = item,
                    }),
            ],
            InvolvedTeams =
            [
                .. src.TeamIds.RemoveInvalidIds().Select(item => new Team
                {
                    Id = item,
                }),
            ],
            RequestedResources =
            [
                .. src.RequestedResourceIds.ToSafeCollection().RemoveInvalidIds().Select(item => new Resource
                {
                    Id = item,
                }),
            ],
            MarketplaceBooking = new MarketplaceBooking
            {
                Quantity = src.Quantity,
                ProductVersion = new ProductVersion
                {
                    Id = src.ProductVersionId,
                },
                PaymentMethod = src.PaymentMethod,
                InvoiceEmailList = src.InvoiceEmailList.ToSafeCollection(),
                ProductPricing = ProductPricing.Empty(src.PricingId),
                CheckoutReturnUrl = src.CheckoutReturnUrl,
            },
        };
    }

    public Shared.Models.Booking MapTo(UpdatePrivateBookingInput src)
    {
        var customers = src.CustomerIds.RemoveInvalidIds().Select(item => new Customer
        {
            Id = item,
        }).ToList();

        return new Shared.Models.Booking
        {
            Id = src.Id,
            From = src.From,
            Until = src.Until,
            Notes = src.Notes,
            Category = src.Category ?? BookingCategory.WorkingFromOffice,
            Schedules = new List<BookingSchedule>
            {
                new(src.From, src.Until),
            },
            InvolvedCustomers = customers,
            InvolvedLocations = [],
            InvolvedOrganizations =
            [
                .. src.OrganizationIds.ToSafeCollection().RemoveInvalidIds().Select(item => new Organization
                {
                    Id = item,
                }),

                .. src.OrganizationCustomDomains.ToSafeCollection().RemoveInvalidIds().Select(item =>
                    new Organization
                    {
                        CustomDomain = item,
                    }),
            ],
            InvolvedTeams =
            [
                .. src.TeamIds.RemoveInvalidIds().Select(item => new Team
                {
                    Id = item,
                }),
            ],
            Resources =
            [
                .. src.ResourceIds.RemoveInvalidIds().Select(item => new ResourceCustomersPair(new Resource
                {
                    Id = item,
                }, customers)),
            ],
        };
    }

    public Shared.Models.Booking MapTo(AddMarketplaceBookingInput src)
    {
        var customers = src.CustomerIds.RemoveInvalidIds().Select(item => new Customer
        {
            Id = item,
        }).ToList();

        return new Shared.Models.Booking
        {
            Id = src.Id.ToSafeString(),
            From = src.From,
            Until = src.Until,
            Notes = src.Notes,
            Category = src.Category ?? BookingCategory.WorkingFromCoworkingSpace,
            Schedules = new List<BookingSchedule>
            {
                new(src.From, src.Until),
            },
            InvolvedCustomers = customers,
            InvolvedLocations = [],
            InvolvedOrganizations =
            [
                .. src.OrganizationIds
                    .ToSafeCollection().RemoveInvalidIds().Select(item => new Organization
                    {
                        Id = item,
                    }),

                .. src.OrganizationCustomDomains.ToSafeCollection().RemoveInvalidIds().Select(item =>
                    new Organization
                    {
                        CustomDomain = item,
                    }),
            ],
            InvolvedTeams =
            [
                .. src.TeamIds.RemoveInvalidIds().Select(item => new Team
                {
                    Id = item,
                }),
            ],
            Resources =
            [
                .. src.ResourceIds.ToSafeCollection().Select(item => new ResourceCustomersPair(new Resource
                {
                    Id = item,
                }, customers)),
            ],
            MarketplaceBooking = new MarketplaceBooking
            {
                Quantity = src.Quantity,
                ProductVersion = new ProductVersion
                {
                    Id = src.ProductVersionId,
                },
                PaymentMethod = src.PaymentMethod,
                InvoiceEmailList = src.InvoiceEmailList.ToSafeCollection(),
                ProductPricing = ProductPricing.Empty(src.PricingId),
                CheckoutReturnUrl = src.CheckoutReturnUrl,
            },
        };
    }

    public Shared.Models.Booking MapTo(UpdateMarketplaceBookingInput src)
    {
        var customers = src.CustomerIds.RemoveInvalidIds().Select(item => new Customer
        {
            Id = item,
        }).ToList();

        return new Shared.Models.Booking
        {
            Id = src.Id,
            Notes = src.Notes,
            Category = src.Category ?? BookingCategory.WorkingFromCoworkingSpace,
            InvolvedCustomers = customers,
            InvolvedLocations = [],
            InvolvedOrganizations =
            [
                .. src.OrganizationIds.ToSafeCollection().RemoveInvalidIds().Select(item => new Organization
                {
                    Id = item,
                }),

                .. src.OrganizationCustomDomains.ToSafeCollection().RemoveInvalidIds().Select(item =>
                    new Organization
                    {
                        CustomDomain = item,
                    }),
            ],
            InvolvedTeams =
            [
                .. src.TeamIds.RemoveInvalidIds().Select(item => new Team
                {
                    Id = item,
                }),
            ],
        };
    }

    public MarketplaceBookingModificationCommand MapTo(ModifyMarketplaceBookingInput src) =>
        new(src.BookingId, checked((uint)src.ExpectedVersion), src.From, src.Until, src.ResourceIds, src.Reason, src.ActorKind);

    public MarketplaceBookingModificationDetails MapTo(MarketplaceBookingModificationSummary src) =>
        new()
        {
            Id = src.Id,
            BookingId = src.BookingId,
            OccurredAt = src.OccurredAt,
            ActorKind = src.ActorKind,
            Reason = src.Reason,
            OriginalFrom = src.OriginalFrom,
            OriginalUntil = src.OriginalUntil,
            ResultFrom = src.ResultFrom,
            ResultUntil = src.ResultUntil,
            OriginalResourceIds = src.OriginalResourceIds,
            ResultResourceIds = src.ResultResourceIds,
            OriginalResourceNames = src.OriginalResourceNames,
            ResultResourceNames = src.ResultResourceNames,
            SubscriptionOccurrenceOverride = src.SubscriptionOccurrenceOverride,
        };

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
                OrganizationTags = [.. MapTo(src.OrganizationTags)],
            };

    public Edge<Shared.Models.Booking> MapTo(Edge<Shared.Database.Entities.Booking> src) => new(sharedEntityMapper.MapTo(src.Node), src.Cursor);
    public Edge<RecurringBooking> MapTo(Edge<Shared.Database.Entities.RecurringBooking> src) => new(sharedEntityMapper.MapTo(src.Node), src.Cursor);

    public Edge<MarketplaceBookingSubscription> MapTo(Edge<Shared.Database.Entities.MarketplaceBookingSubscription> src) =>
        new(sharedEntityMapper.MapTo(src.Node), src.Cursor);

    public BookingEdge MapTo(Edge<Shared.Models.Booking> src) => new(MapTo(src.Node), src.Cursor);
    public RecurringBookingEdge MapTo(Edge<RecurringBooking> src) => new(MapTo(src.Node)!, src.Cursor);
    public MarketplaceBookingSubscriptionEdge MapTo(Edge<MarketplaceBookingSubscription> src) => new(MapTo(src.Node), src.Cursor);

    public IEnumerable<BookingResourceDetails> MapTo(IEnumerable<Resource> src) => src.Select(item => MapTo(item, []));

    public IEnumerable<BookingResourceDetails> MapTo(IEnumerable<Shared.Database.Entities.Resource> src) => src.Select(item =>
        new BookingResourceDetails
        {
            Resource = new ResourceDetails
            {
                Id = item.Id,
                Name = item.Name.ToSafeString(),
                Inactive = item.Inactive,
                RequireBookingApproval = item.RequireBookingApproval,
                Color = item.Color,
                Capacity = item.Capacity,
                IsAvailableHoursOverridden = item.IsAvailableHoursOverridden ?? false,
                CustomTags =
                [
                    .. item.OrganizationTags.Where(tag => tag.Type == OrganizationTagTypeConstants.Custom).Select(MapToOrganizationTagDetails),
                ],
                Zones =
                    [.. item.OrganizationTags.Where(tag => tag.Type == OrganizationTagTypeConstants.Zone).Select(MapToOrganizationTagDetails)],
                ProductTags =
                [
                    .. item.OrganizationTags.Where(tag => tag.Type == OrganizationTagTypeConstants.Product).Select(MapToOrganizationTagDetails),
                ],
                ResourceType = item.OrganizationTags
                    .Where(tag => tag.Type.ToNullableOrganizationTagType() is { } type && OrganizationTagTypeConstants.ResourceTypes.Contains(type))
                    .Select(MapToOrganizationTagDetails)
                    .FirstOrDefault() ?? new OrganizationTagDetails(),
            },
            Location = item.Location is null
                ? null
                : new LocationDetails
                {
                    Id = item.Location.Id,
                    Name = item.Location.Name.ToSafeString(),
                },
        });

    private static OrganizationTagDetails MapToOrganizationTagDetails(Shared.Database.Entities.OrganizationTag src) => new()
    {
        Id = src.Id,
        Name = src.Name.ToSafeString(),
        Type = src.Type.ToNullableOrganizationTagType(),
        Color = src.Color,
    };

    private static IEnumerable<OrganizationTag> MapTo(IEnumerable<Shared.Database.Entities.OrganizationTag> src) => src.Select(MapTo);

    private static OrganizationTag MapTo(Shared.Database.Entities.OrganizationTag src) =>
        new()
        {
            Id = src.Id,
            Name = src.Name.ToSafeString(),
            Type = src.Type.ToNullableOrganizationTagType(),
            Color = src.Color,
        };

    private static BookingResourceDetails MapTo(Resource src, IEnumerable<Customer> customers) =>
        new()
        {
            Resource = MapToResourceDetails(src),
            Location = MapTo(src.Location),
            CustomerIds = customers.Select(item => item.Id),
        };

    private static BookingResourceDetails MapTo(Resource src) => new()
    {
        Resource = MapToResourceDetails(src),
        Location = MapTo(src.Location),
    };

    private static IEnumerable<LocationDetails> MapTo(IEnumerable<Shared.Models.Location> src) => src.Select(MapTo)!;

    private static LocationDetails? MapTo(Shared.Models.Location? src) => src is null
        ? null
        : new LocationDetails
        {
            Id = src.Id,
            Name = src.Name,
        };

    private static ResourceDetails MapToResourceDetails(Resource src)
    {
        var result = new ResourceDetails
        {
            Id = src.Id,
            Name = src.Name,
            Inactive = src.Inactive,
            RequireBookingApproval = src.RequireBookingApproval,
            Color = src.Color,
            Capacity = src.Capacity,
            IsAvailableHoursOverridden = src.IsAvailableHoursOverridden,
            CustomTags = [.. src.OrganizationTags.Where(item => item.Type == OrganizationTagType.Custom).Select(MapTo)],
            Zones = [.. src.OrganizationTags.Where(item => item.Type == OrganizationTagType.Zone).Select(MapTo)],
            ProductTags = [.. src.OrganizationTags.Where(item => item.Type == OrganizationTagType.Product).Select(MapTo)],
        };

        var organizationTag =
            src.OrganizationTags.FirstOrDefault(item => OrganizationTagTypeConstants.ResourceTypes.Any(tagType => tagType == item.Type));
        if (organizationTag is not null)
        {
            result.ResourceType = MapTo(organizationTag);
        }

        return result;
    }

    private static OrganizationTagDetails MapTo(OrganizationTag src) => new()
    {
        Id = src.Id,
        Name = src.Name,
        Type = src.Type,
        Color = src.Color,
    };

    private static IEnumerable<BookingResourceDetails> MapTo(IReadOnlyList<ResourceCustomersPair> src, IReadOnlyList<Resource> involvedResources) =>
        src.Count == 0 ? involvedResources.Select(MapTo) : src.Select(item => MapTo(item.Resource, item.Customers));

    private static BookingCheckoutSessionDetails? MapTo(StripeCheckoutSession? src) =>
        src is null
            ? null
            : new BookingCheckoutSessionDetails
            {
                UniqueId = src.Id,
                CheckoutUrl = src.CheckoutUrl,
            };

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
                PaymentStatus = new PaymentStatusDetails
                {
                    Type = src.PaymentStatus,
                    Name = src.PaymentStatus.ToPaymentStatusName(),
                },
                PaymentMethod = new PaymentMethodTypeDetails
                {
                    Type = src.PaymentMethod,
                    Name = src.PaymentMethod.ToPaymentMethodName(),
                },
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
                HostCommissionRatePercentage = src.HostCommissionRatePercentage,
                HostCommissionAmount = src.HostCommissionAmount,
                HostPayoutAmount = src.HostPayoutAmount,
                HostGrossProceedsAmount = src.HostPayoutAmount,
                TotalAmountToDisplay = src.TotalAmount is null || src.Currency is null
                    ? "N/A"
                    : src.TotalAmount.Value.ToRoundedPrice().ToPriceToDisplay(src.Currency.Value),
                Currency =
                    src.Currency is null
                        ? null
                        : new CurrencyDetails
                        {
                            Type = src.Currency.Value,
                            Name = src.Currency.Value.ToCurrencyName(),
                        },
                CurrencyToDisplay = src.Currency is null ? "N/A" : src.Currency.Value.ToCurrencyName(),
            };

    private IEnumerable<RecurringBookingDetails> MapTo(IEnumerable<RecurringBooking> src) => src.Select(MapTo)!;
}
