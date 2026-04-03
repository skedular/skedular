namespace Api.Shared.Services;

public class CustomerNotFound() : Exception("Customer not found");

public class OrganizationNotFound() : Exception("Organization not found");

public class OrganizationBillingDetailsNotFound() : Exception("Organization billing details not found");

public class CustomerBillingDetailsNotFound() : Exception("Customer billing details not found");

public class OrganizationSsoIsNotYetSetup() : Exception("Organization SSO is not yet setup");

public class OrganizationMemberNotFound() : Exception("Organization member not found");

public class OrganizationJoinInvitationNotFound() : Exception("Organization join invitation not found");

public class LocationNotFound() : Exception("Location not found");

public class LocationUniqueClaimCodeNotFound() : Exception("Location unique claim code not found");

public class FloorPlanNotFound() : Exception("Floor plan not found");

public class TeamNotFound() : Exception("Team not found");

public class TeamMemberNotFound() : Exception("Team member ot found");

public class TeamJoinInvitationNotFound() : Exception("Team join invitation not found");

public class ResourceNotFound() : Exception("Resource not found");

public class ResourceWithSameNameExist() : Exception("Resource with same name exist");

public class ResourceTypeRequired() : Exception("Resource type required");

public class OnlySingleResourceTypeAllowed() : Exception("Only single resource type allowed");

public class OrganizationTermsOfUseAgreementMissing() : Exception("Organization terms of use agreement missing");

public class PaymentMethodRequired() : Exception("Payment method required");

public class BookingNotFound() : Exception("Booking not found");

public class RecurringBookingNotFound() : Exception("Recurring booking not found");

public class NoMoreInteractionAllowed()
    : Exception("You have exceeded your free tier limit, please upgrade to 'Pay as you go' tier to have full access to all features.");

public class ResourceNotAvailable() : Exception("Resource not available");

public class NoResourceAvailable() : Exception("No resource available");

public class ProductMissingProductTag() : Exception("Product missing product tag");

public class SlackWorkspaceNotFound() : Exception("Slack workspace not found");

public class SlackWorkspaceMemberTypeNotSupported() : Exception("Slack workspace member type not supported");

public class OrganizationTagNotFound() : Exception("Organization tag not found");

public class CustomTagWithSameNameExist() : Exception("Tag with same name exist");

public class ZoneWithSameNameExist() : Exception("Zone with same name exist");

public class OrganizationTagWithSameNameExist() : Exception("Organization tag with same name exist");

public class TeamPrimaryLocationOrganizationDoesNotMatchTeamOrganization()
    : Exception("Team Primary location organization does not match team organization");

public class ProductNotFound() : Exception("Product not found");

public class ProductPricingNotFound() : Exception("Product pricing not found");

public class ProductVersionNotFound() : Exception("Product version not found");

public class OrganizationStripeConnectAccountNotFound() : Exception("Organization Stripe Connect Account not found");

public class OrganizationStripeConnectAccountRefreshCodeNotFound() : Exception("Organization Stripe Connect Account refresh code not found");

public class OrganizationStripeCustomerRelationshipIsNotSetYet() : Exception("Organization Stripe customer relationship is not set yet");

public class InvalidSsoConfiguration() : Exception("Invalid SSO configuration");

public class OrganizationPaymentMethodNotFound() : Exception("Organization payment method not found");

public class ResourceAndFloorPlanLocationMismatch() : Exception("Resource and floor plan must belong to the same location");

public class ResourceIsPlacedOnDifferentFloorPlan() : Exception("Resource is placed on different floor plan");

public class StripeCustomerNotFound() : Exception("Stripe Customer not found");

public class OrganizationBankAccountNotFound() : Exception("Organization Bank Account not found");

public class BookingPaymentMethodNotAccepted() : Exception("Booking payment method not accepted");

public class MarketplaceBookingCheckoutReturnUrlInvalid() : Exception("Marketplace booking checkout return url is invalid");

public class BookingIsNotMarketplaceType() : Exception("Booking is not marketplace type");

public class BookingMustStartAndEndWithinSameDay() : Exception("Booking must start and end within the same day");

public class MarketplaceBookingCadenceRequiresRecurringFlow()
    : Exception("This marketplace booking cadence must be booked through the recurring marketplace flow");

public class MarketplaceBookingSubscriptionAutoRenewalNotSupported()
    : Exception("The selected product pricing does not support subscription auto renewal");

public class ProductPricingBillingModeRequired()
    : Exception("A billing mode must be selected for the product pricing option");

public class OrganizationPhysicalAddressNotFound() : Exception("Organization physical address not found");

public class LocationPhysicalAddressNotFound() : Exception("Location physical address not found");

public class MoreResourcesHaveBeenSelectedThanAreAllowedForThisBooking()
    : Exception("More resources have been selected than are allowed for this booking");

public class BookingIsNotPrivate() : Exception("Booking is not private");

public class BookingIsNotMarketplace() : Exception("Booking is not marketplace");

public class RecurringBookingIsNotPrivate() : Exception("Recurring booking is not private");

public class RecurringBookingIsNotMarketplace() : Exception("Recurring booking is not marketplace");

public class MarketplaceBookingSubscriptionNotFound() : Exception("Marketplace booking subscription not found");

public class MarketplaceBookingSubscriptionCannotBeUpdated() : Exception("Marketplace booking subscription cannot be updated");

public class ProductOrganizationDidNotMatch() : Exception("Product organization did not match");

public class ProductPricingCancellationPolicyInvalid() : Exception("Product pricing cancellation policy is invalid");

public class MarketplaceBookingCancellationNotAllowed() : Exception("This marketplace booking can no longer be cancelled");

public class MarketplaceBookingSubscriptionCancellationNotAllowed()
    : Exception("This marketplace booking subscription can no longer be cancelled");

public class MarketplaceEventResourceSelectionRequiresEventProduct()
    : Exception("Event resource selection can only be used for event products.");

public class MarketplaceEventProductRecurringBookingNotSupported()
    : Exception("Event products do not support recurring or subscription booking materialization.");

public class ProductPricingEventRequiresExplicitTimeBooking()
    : Exception("Event products only support explicit-time booking pricing options.");

public class ProductPricingEventAutoRenewalNotSupported()
    : Exception("Event products cannot enable subscription auto renewal.");

public class ProductPricingAcceptedPaymentMethodsRequired()
    : Exception("At least one accepted booking payment method must be selected");

public class ProductPricingMinDurationMustBePositive()
    : Exception("MinDurationMinutes must be greater than 0");

public class ProductPricingMaxDurationMustBePositive()
    : Exception("MaxDurationMinutes must be greater than 0");

public class ProductPricingMinDurationIncrementInvalid(string durationStepLabel)
    : Exception($"MinDurationMinutes must be in {durationStepLabel} increments");

public class ProductPricingMaxDurationIncrementInvalid(string durationStepLabel)
    : Exception($"MaxDurationMinutes must be in {durationStepLabel} increments");

public class ProductPricingMaxDurationMustNotBeLessThanMinDuration()
    : Exception("MaxDurationMinutes must be greater or equal than productVersion.MinDurationMinutes");

public class OrganizationXeroConnectionUnauthorizedException()
    : UnauthorizedAccessException("You do not have permission to modify this organization's Xero connection.");

public class InvalidXeroAuthorizeStateException() : InvalidOperationException("Invalid Xero state.");

public class EmptyXeroTokenResponseException() : InvalidOperationException("Xero token response was empty.");

public class XeroTenantReconnectRequiredException() : InvalidOperationException("Disconnect Xero before changing the connected tenant.");

public class XeroActivationRequiresConnectionException() : InvalidOperationException("Connect Xero first before activating Xero-managed billing.");

public class XeroActivationRequiresTenantSelectionException()
    : InvalidOperationException("Select a Xero tenant before activating Xero-managed billing.");

public class UnsupportedXeroBillingModeException(string billingMode)
    : ArgumentOutOfRangeException(nameof(billingMode), billingMode, "Unsupported Xero billing mode.");

public class UnavailableXeroTenantSelectionException()
    : InvalidOperationException("Selected Xero tenant is not available for the current Xero connection.");

public class XeroTokenRefreshFailedException(string message) : InvalidOperationException(message);

public class NoXeroOrganizationTenantConnectionsException() : InvalidOperationException("No Xero organization tenant connections were returned.");

public class MissingXeroRefreshTokenException() : InvalidOperationException("Xero refresh token is missing. Reconnect required.");

public class XeroContactExportFailedException() : InvalidOperationException("Xero contact export did not return a contact.");

public class XeroInvoiceExportFailedException() : InvalidOperationException("Xero invoice export did not return an invoice.");

public class MixedXeroInvoiceTaxInclusivityException()
    : InvalidOperationException("Xero invoice export does not support mixing tax-inclusive and tax-exclusive line pricing on the same invoice.");

public class AzureTenantOnboardingFailedException(string error, string? errorDescription)
    : InvalidOperationException($"Azure tenant onboarding went wrong with error {error} and message {errorDescription}.");

public class OrganizationLookupRequiresIdOrCustomDomainException()
    : InvalidOperationException("Either id or customDomain must be provided.");

public class OrganizationLookupRequiresIdsOrCustomDomainsException()
    : InvalidOperationException("Either ids or customDomains must be provided.");
