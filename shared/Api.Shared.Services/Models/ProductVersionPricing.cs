namespace Api.Shared.Services.Models;

public record ProductVersionOneTimePricingV1(
    int Index,
    string Name,
    string Description,
    decimal Price,
    bool IsTaxInclusive,
    Currency Currency,
    ICollection<PaymentMethod> AcceptedPaymentMethods,
    int? MinDurationMinutes,
    int? MaxDurationMinutes,
    int MaxAllowedResourcesLockTimePaidViaCard,
    int MaxAllowedResourcesLockTimePaidViaBankTransfer);

public record ProductVersionPerMinutePricingV1(
    int Index,
    string Name,
    string Description,
    decimal Price,
    bool IsTaxInclusive,
    Currency Currency,
    ICollection<PaymentMethod> AcceptedPaymentMethods,
    int? MinDurationMinutes,
    int? MaxDurationMinutes,
    int MaxAllowedResourcesLockTimePaidViaCard,
    int MaxAllowedResourcesLockTimePaidViaBankTransfer);

public record ProductVersionDailyPricingV1(
    int Index,
    string Name,
    string Description,
    decimal Price,
    bool IsTaxInclusive,
    Currency Currency,
    ICollection<PaymentMethod> AcceptedPaymentMethods,
    int? MinDurationMinutes,
    int? MaxDurationMinutes,
    int MaxAllowedResourcesLockTimePaidViaCard,
    int MaxAllowedResourcesLockTimePaidViaBankTransfer);

public record ProductVersionWeeklyPricingV1(
    int Index,
    string Name,
    string Description,
    decimal Price,
    bool IsTaxInclusive,
    Currency Currency,
    ICollection<PaymentMethod> AcceptedPaymentMethods,
    int? MinDurationMinutes,
    int? MaxDurationMinutes,
    int MaxAllowedResourcesLockTimePaidViaCard,
    int MaxAllowedResourcesLockTimePaidViaBankTransfer);

public record ProductVersionMonthlyPricingV1(
    int Index,
    string Name,
    string Description,
    decimal Price,
    bool IsTaxInclusive,
    Currency Currency,
    ICollection<PaymentMethod> AcceptedPaymentMethods,
    int? MinDurationMinutes,
    int? MaxDurationMinutes,
    int MaxAllowedResourcesLockTimePaidViaCard,
    int MaxAllowedResourcesLockTimePaidViaBankTransfer);

public record ProductVersionPricingOptions(
    ProductVersionPricingCadence Cadence,
    ProductVersionOneTimePricingV1? OneTimeV1,
    ProductVersionPerMinutePricingV1? PerMinuteV1,
    ProductVersionDailyPricingV1? DailyV1,
    ProductVersionWeeklyPricingV1? WeeklyV1,
    ProductVersionMonthlyPricingV1? MonthlyV1);
