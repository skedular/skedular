using Api.Shared.Services.Offering;

namespace Api.Shared.Services;

public class CustomerNotFound() : Exception("We couldn't find that customer.");

public class OrganizationNotFound() : Exception("We couldn't find that organization.");

public class OrganizationBillingDetailsNotFound() : Exception("We couldn't find the billing details for this organization.");

public class CustomerBillingDetailsNotFound() : Exception("We couldn't find the billing details for this customer.");

public class OrganizationSsoIsNotYetSetup() : Exception("Single sign-on has not been set up for this organization yet.");

public class OrganizationMemberNotFound() : Exception("We couldn't find that organization member.");

public class OrganizationJoinInvitationNotFound() : Exception("We couldn't find that organization invitation.");

public class LocationNotFound() : Exception("We couldn't find that location.");

public class LocationUniqueClaimCodeNotFound() : Exception("We couldn't find that location claim code.");

public class FloorPlanNotFound() : Exception("We couldn't find that floor plan.");

public class TeamNotFound() : Exception("We couldn't find that team.");

public class TeamMemberNotFound() : Exception("We couldn't find that team member.");

public class TeamJoinInvitationNotFound() : Exception("We couldn't find that team invitation.");

public class ResourceNotFound() : Exception("We couldn't find that resource.");

public class ResourceWithSameNameExist() : Exception("A resource with this name already exists.");

public class ResourceTypeRequired() : Exception("Please choose a resource type.");

public class OnlySingleResourceTypeAllowed() : Exception("Please choose only one resource type.");

public class OrganizationTermsOfUseAgreementMissing() : Exception("You need to accept the organization's terms before continuing.");

public class ActiveTermsOfUseNotFoundException() : InvalidOperationException("No active terms of use record was found.");

public class MultipleActiveTermsOfUseFoundException() : InvalidOperationException("Multiple active terms of use records were found.");

public class PaymentMethodRequired() : Exception("Please choose a payment method.");

public class BookingNotFound() : Exception("We couldn't find that booking.");

public class RecurringBookingNotFound() : Exception("We couldn't find that recurring booking.");

public class NoMoreInteractionAllowed()
    : Exception("You've reached the limit of the free plan. Upgrade to Pay as you go to keep using all features.");

public class SpacesBookingQuotaExceeded(
    SpacesQuotaReasonCode reasonCode,
    int currentUsage,
    int quotaLimit,
    int attemptedCurrentPeriodCount,
    int excludedOutOfPeriodCount,
    int remainingQuota,
    IReadOnlyList<SpacesQuotaUpgradePlan> upgradePlans)
    : Exception("You've reached the Spaces booking quota for the current billing period.")
{
    public const string Code = "SPACES_BOOKING_QUOTA_EXCEEDED";
    public string ErrorCode { get; } = Code;
    public SpacesQuotaReasonCode ReasonCode { get; } = reasonCode;
    public int CurrentUsage { get; } = currentUsage;
    public int QuotaLimit { get; } = quotaLimit;
    public int AttemptedCurrentPeriodCount { get; } = attemptedCurrentPeriodCount;
    public int ExcludedOutOfPeriodCount { get; } = excludedOutOfPeriodCount;
    public int TotalAttemptedInstanceCount => AttemptedCurrentPeriodCount + ExcludedOutOfPeriodCount;
    public int RemainingQuota { get; } = remainingQuota;
    public IReadOnlyList<SpacesQuotaUpgradePlan> UpgradePlans { get; } = upgradePlans;
}

public class SpacesOfferingStateMissing()
    : Exception("This organization does not have Spaces offering state. Run the default Free assignment before creating bookings.");

public class SpacesAccessDenied(SpacesAccessDecision decision)
    : Exception(GetMessage(decision))
{
    public const string Code = "SPACES_ACCESS_DENIED";
    public string ErrorCode { get; } = Code;
    public SpacesSubscriptionStatus Status { get; } = decision.Status;
    public SpacesAccessReasonCode ReasonCode { get; } = decision.ReasonCode;
    public bool UpgradeRequired { get; } = decision.UpgradeRequired;

    private static string GetMessage(SpacesAccessDecision decision) => decision.ReasonCode switch
    {
        SpacesAccessReasonCode.TrialExpired => "This organization's Spaces trial has ended. Upgrade to a paid plan to continue.",
        SpacesAccessReasonCode.PaidInactive => "This organization's Spaces subscription is inactive. Update the subscription to continue.",
        SpacesAccessReasonCode.MissingOfferingState or SpacesAccessReasonCode.MissingTrialState =>
            "This organization's Spaces subscription state is incomplete. Update the subscription to continue.",
        _ => "This organization cannot perform this Spaces action with its current subscription.",
    };
}

public class ResourceNotAvailable() : Exception("That resource is no longer available for the selected time.");

public class MarketplaceBookingDateUnavailable() : Exception("This price is not available on the selected date.");

public class NoResourceAvailable() : Exception("No resources are available for the selected time.");

public class ProductMissingProductTag() : Exception("Please add at least one product tag.");

public class SlackWorkspaceNotFound() : Exception("We couldn't find that Slack workspace.");

public class SlackWorkspaceMemberTypeNotSupported() : Exception("This Slack member type isn't supported.");

public class OrganizationTagNotFound() : Exception("We couldn't find that organization tag.");

public class CustomTagWithSameNameExist() : Exception("A tag with this name already exists.");

public class ZoneWithSameNameExist() : Exception("A zone with this name already exists.");

public class OrganizationTagWithSameNameExist() : Exception("An organization tag with this name already exists.");

public class TeamPrimaryLocationOrganizationDoesNotMatchTeamOrganization()
    : Exception("The team's main location must belong to the same organization as the team.");

public class TeamNotAllowedForOrganizationType() : Exception("Teams can only be created for private organizations.");

public class ProductNotFound() : Exception("We couldn't find that product.");

public class ProductPricingNotFound() : Exception("We couldn't find that pricing option.");

public class ProductVersionNotFound() : Exception("We couldn't find that product version.");

public class OrganizationStripeConnectAccountNotFound() : Exception("We couldn't find that Stripe Connect account.");

public class OrganizationStripeConnectAccountRefreshCodeNotFound()
    : Exception("We couldn't find the Stripe Connect refresh code for this organization.");

public class OrganizationStripeCustomerRelationshipIsNotSetYet() : Exception("This organization's Stripe customer account has not been set up yet.");

public class InvalidSsoConfiguration() : Exception("The single sign-on setup is incomplete or invalid.");

public class OrganizationPaymentMethodNotFound() : Exception("We couldn't find that organization payment method.");

public class LocationTypeNotAllowedForOrganizationType() : Exception("This location type is not allowed for this organization type.");

public class ResourceAndFloorPlanLocationMismatch() : Exception("The resource and floor plan must belong to the same location.");

public class ResourceIsPlacedOnDifferentFloorPlan() : Exception("This resource is assigned to a different floor plan.");

public class StripeCustomerNotFound() : Exception("We couldn't find that Stripe customer.");

public class OrganizationBankAccountNotFound() : Exception("We couldn't find that bank account.");

public class BookingPaymentMethodNotAccepted() : Exception("This payment method isn't accepted for this booking.");

public class MarketplaceBookingCheckoutReturnUrlInvalid() : Exception("The checkout return link is invalid.");

public class BookingIsNotMarketplaceType() : Exception("This booking is not a marketplace booking.");

public class BookingMustStartAndEndWithinSameDay() : Exception("The booking must start and end on the same day.");

public class MarketplaceBookingSubscriptionAutoRenewalNotSupported()
    : Exception("This pricing option does not support auto-renewal.");

public class ProductPricingBillingModeRequired()
    : Exception("Please choose a billing option for this pricing plan.");

public class EntitlementPricingConfigurationInvalid()
    : Exception("Entitlement pricing must define a positive credit quantity, unit, and validity period.");

public class EntitlementCreditUnavailable()
    : Exception("No active booking credits are available for this booking.");

public class OrganizationPhysicalAddressNotFound() : Exception("We couldn't find this organization's address.");

public class LocationPhysicalAddressNotFound() : Exception("We couldn't find this location's address.");

public class MoreResourcesHaveBeenSelectedThanAreAllowedForThisBooking()
    : Exception("Too many resources were selected for this booking.");

public class BookingIsNotPrivate() : Exception("This booking is not a private booking.");

public class BookingIsNotMarketplace() : Exception("This booking is not a marketplace booking.");

public class RecurringBookingIsNotPrivate() : Exception("This recurring booking is not a private booking.");

public class RecurringBookingIsNotMarketplace() : Exception("This recurring booking is not a marketplace booking.");

public class MarketplaceBookingSubscriptionNotFound() : Exception("We couldn't find that subscription.");

public class MarketplaceBookingSubscriptionCannotBeUpdated() : Exception("This subscription can't be updated.");

public class ProductOrganizationDidNotMatch() : Exception("This product does not belong to this organization.");

public class ProductPricingCancellationPolicyInvalid() : Exception("The cancellation policy for this pricing option is incomplete or invalid.");

public class MarketplaceBookingCancellationNotAllowed() : Exception("This booking has passed its cancellation deadline.");

public class MarketplaceBookingCancellationOverrideReasonRequired()
    : Exception("A cancellation override reason is required when the cancellation policy blocks the cancellation.");

public class MarketplaceBookingSubscriptionCancellationNotAllowed()
    : Exception("This subscription has passed its cancellation deadline.");

public class MarketplaceBookingSubscriptionCancellationOverrideReasonRequired()
    : Exception("A cancellation override reason is required when the cancellation policy blocks the cancellation.");

public class MarketplaceEventResourceSelectionRequiresEventProduct()
    : Exception("You can only choose event resources for event products.");

public class MarketplaceEventProductRecurringBookingNotSupported()
    : Exception("Events can only be booked once. Recurring bookings and subscriptions aren't available for this event.");

public class ProductPricingEventRequiresExplicitTimeBooking()
    : Exception("Event products must use pricing with a specific start and end time.");

public class ProductPricingEventAutoRenewalNotSupported()
    : Exception("Event products can't use auto-renewal.");

public class ProductPricingEntitlementAutoRenewalNotSupported()
    : Exception("Credit entitlement offers can't use auto-renewal.");

public class ProductPricingAcceptedPaymentMethodsRequired()
    : Exception("Please choose at least one accepted payment method.");

public class ProductPricingMinDurationMustBePositive()
    : Exception("Minimum booking length must be at least 1 minute.");

public class ProductPricingMaxDurationMustBePositive()
    : Exception("Maximum booking length must be at least 1 minute.");

public class ProductPricingMinDurationIncrementInvalid(string durationStepLabel)
    : Exception($"Minimum booking length must increase in {durationStepLabel} steps.");

public class ProductPricingMaxDurationIncrementInvalid(string durationStepLabel)
    : Exception($"Maximum booking length must increase in {durationStepLabel} steps.");

public class ProductPricingMaxDurationMustNotBeLessThanMinDuration()
    : Exception("Maximum booking length can't be shorter than the minimum booking length.");

public class MarketplaceBookingDurationMustBeAtLeastMinimum(int minimumMinutes)
    : Exception($"The booking must be at least {minimumMinutes} minutes long.");

public class MarketplaceBookingDurationMustNotExceedMaximum(int maximumMinutes)
    : Exception($"The booking cannot be longer than {maximumMinutes} minutes.");

public class ProductPricingAvailableDaysInvalid()
    : Exception("Available days must be unique calendar days from Monday through Sunday.");

public class ProductPricingWeeklyDaySelectionOnlySupportedForWeeklyPricing()
    : Exception("Selected days per week can only be configured for weekly pricing.");

public class ProductPricingWeeklyDaySelectionRangeInvalid()
    : Exception("Required selected days per week must be from 1 to 7 and fit within the available days.");

public class MarketplaceBookingWeeklyDaySelectionInvalid()
    : Exception("Choose the required number of available days per week.");

public class OrganizationXeroConnectionUnauthorizedException()
    : UnauthorizedAccessException("You don't have permission to change this organization's Xero connection.");

public class InvalidXeroAuthorizeStateException() : InvalidOperationException("The Xero sign-in session is invalid. Please try connecting again.");

public class EmptyXeroTokenResponseException() : InvalidOperationException("Xero did not return a sign-in token. Please try again.");

public class XeroTenantReconnectRequiredException() : InvalidOperationException("Disconnect Xero before choosing a different tenant.");

public class XeroActivationRequiresConnectionException() : InvalidOperationException("Connect Xero before turning on Xero-managed billing.");

public class XeroActivationRequiresTenantSelectionException()
    : InvalidOperationException("Choose a Xero tenant before turning on Xero-managed billing.");

public class UnsupportedXeroBillingModeException(string billingMode)
    : ArgumentOutOfRangeException(nameof(billingMode), billingMode, "This Xero billing mode isn't supported.");

public class UnavailableXeroTenantSelectionException()
    : InvalidOperationException("The selected Xero tenant isn't available for the current connection.");

public class XeroTokenRefreshFailedException(string message) : InvalidOperationException(message);

public class NoXeroOrganizationTenantConnectionsException() : InvalidOperationException("No Xero tenants were returned for this connection.");

public class MissingXeroRefreshTokenException() : InvalidOperationException("The Xero connection has expired. Please reconnect.");

public class XeroContactExportFailedException() : InvalidOperationException("Xero did not return a contact for this export.");

public class XeroInvoiceExportFailedException() : InvalidOperationException("Xero did not return an invoice for this export.");

public class MixedXeroInvoiceTaxInclusivityException()
    : InvalidOperationException("Xero invoice export does not support mixing tax-inclusive and tax-exclusive line pricing on the same invoice.");

public class AzureTenantOnboardingFailedException(string error, string? errorDescription)
    : InvalidOperationException($"We couldn't finish setting up the Azure tenant. Error: {error}. {errorDescription}");

public class OrganizationLookupRequiresIdOrCustomDomainException()
    : InvalidOperationException("Please provide either an ID or a custom domain.");

public class OrganizationLookupRequiresIdsOrCustomDomainsException()
    : InvalidOperationException("Please provide either IDs or custom domains.");

public class InvoiceDueInDaysMustBeBetween1And999() : ArgumentException("Invoice due days must be between 1 and 999.");

public class LocationRestrictedInformationNotFound : Exception;
