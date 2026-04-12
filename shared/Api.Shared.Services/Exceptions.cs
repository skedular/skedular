namespace Api.Shared.Services;

public class CustomerNotFound() : Exception("We couldn't find that customer.");

public class OrganizationNotFound() : Exception("We couldn't find that organisation.");

public class OrganizationBillingDetailsNotFound() : Exception("We couldn't find the billing details for this organisation.");

public class CustomerBillingDetailsNotFound() : Exception("We couldn't find the billing details for this customer.");

public class OrganizationSsoIsNotYetSetup() : Exception("Single sign-on has not been set up for this organisation yet.");

public class OrganizationMemberNotFound() : Exception("We couldn't find that organisation member.");

public class OrganizationJoinInvitationNotFound() : Exception("We couldn't find that organisation invitation.");

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

public class OrganizationTermsOfUseAgreementMissing() : Exception("You need to accept the organisation's terms before continuing.");

public class PaymentMethodRequired() : Exception("Please choose a payment method.");

public class BookingNotFound() : Exception("We couldn't find that booking.");

public class RecurringBookingNotFound() : Exception("We couldn't find that recurring booking.");

public class NoMoreInteractionAllowed()
    : Exception("You've reached the limit of the free plan. Upgrade to Pay as you go to keep using all features.");

public class ResourceNotAvailable() : Exception("That resource is no longer available for the selected time.");

public class NoResourceAvailable() : Exception("No resources are available for the selected time.");

public class ProductMissingProductTag() : Exception("Please add at least one product tag.");

public class SlackWorkspaceNotFound() : Exception("We couldn't find that Slack workspace.");

public class SlackWorkspaceMemberTypeNotSupported() : Exception("This Slack member type isn't supported.");

public class OrganizationTagNotFound() : Exception("We couldn't find that organisation tag.");

public class CustomTagWithSameNameExist() : Exception("A tag with this name already exists.");

public class ZoneWithSameNameExist() : Exception("A zone with this name already exists.");

public class OrganizationTagWithSameNameExist() : Exception("An organisation tag with this name already exists.");

public class TeamPrimaryLocationOrganizationDoesNotMatchTeamOrganization()
    : Exception("The team's main location must belong to the same organisation as the team.");

public class ProductNotFound() : Exception("We couldn't find that product.");

public class ProductPricingNotFound() : Exception("We couldn't find that pricing option.");

public class ProductVersionNotFound() : Exception("We couldn't find that product version.");

public class OrganizationStripeConnectAccountNotFound() : Exception("We couldn't find that Stripe Connect account.");

public class OrganizationStripeConnectAccountRefreshCodeNotFound()
    : Exception("We couldn't find the Stripe Connect refresh code for this organisation.");

public class OrganizationStripeCustomerRelationshipIsNotSetYet() : Exception("This organisation's Stripe customer account has not been set up yet.");

public class InvalidSsoConfiguration() : Exception("The single sign-on setup is incomplete or invalid.");

public class OrganizationPaymentMethodNotFound() : Exception("We couldn't find that organisation payment method.");

public class ResourceAndFloorPlanLocationMismatch() : Exception("The resource and floor plan must belong to the same location.");

public class ResourceIsPlacedOnDifferentFloorPlan() : Exception("This resource is assigned to a different floor plan.");

public class StripeCustomerNotFound() : Exception("We couldn't find that Stripe customer.");

public class OrganizationBankAccountNotFound() : Exception("We couldn't find that bank account.");

public class BookingPaymentMethodNotAccepted() : Exception("This payment method isn't accepted for this booking.");

public class MarketplaceBookingCheckoutReturnUrlInvalid() : Exception("The checkout return link is invalid.");

public class BookingIsNotMarketplaceType() : Exception("This booking is not a marketplace booking.");

public class BookingMustStartAndEndWithinSameDay() : Exception("The booking must start and end on the same day.");

public class MarketplaceBookingCadenceRequiresRecurringFlow()
    : Exception("This booking schedule must be set up as a recurring booking.");

public class MarketplaceBookingSubscriptionAutoRenewalNotSupported()
    : Exception("This pricing option does not support auto-renewal.");

public class ProductPricingBillingModeRequired()
    : Exception("Please choose a billing option for this pricing plan.");

public class OrganizationPhysicalAddressNotFound() : Exception("We couldn't find this organisation's address.");

public class LocationPhysicalAddressNotFound() : Exception("We couldn't find this location's address.");

public class MoreResourcesHaveBeenSelectedThanAreAllowedForThisBooking()
    : Exception("Too many resources were selected for this booking.");

public class BookingIsNotPrivate() : Exception("This booking is not a private booking.");

public class BookingIsNotMarketplace() : Exception("This booking is not a marketplace booking.");

public class RecurringBookingIsNotPrivate() : Exception("This recurring booking is not a private booking.");

public class RecurringBookingIsNotMarketplace() : Exception("This recurring booking is not a marketplace booking.");

public class MarketplaceBookingSubscriptionNotFound() : Exception("We couldn't find that subscription.");

public class MarketplaceBookingSubscriptionCannotBeUpdated() : Exception("This subscription can't be updated.");

public class ProductOrganizationDidNotMatch() : Exception("This product does not belong to this organisation.");

public class ProductPricingCancellationPolicyInvalid() : Exception("The cancellation policy for this pricing option is incomplete or invalid.");

public class MarketplaceBookingCancellationNotAllowed() : Exception("This booking has passed its cancellation deadline.");

public class MarketplaceBookingSubscriptionCancellationNotAllowed()
    : Exception("This subscription has passed its cancellation deadline.");

public class MarketplaceEventResourceSelectionRequiresEventProduct()
    : Exception("You can only choose event resources for event products.");

public class MarketplaceEventProductRecurringBookingNotSupported()
    : Exception("Events can only be booked once. Recurring bookings and subscriptions aren't available for this event.");

public class ProductPricingEventRequiresExplicitTimeBooking()
    : Exception("Event products must use pricing with a specific start and end time.");

public class ProductPricingEventAutoRenewalNotSupported()
    : Exception("Event products can't use auto-renewal.");

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

public class OrganizationXeroConnectionUnauthorizedException()
    : UnauthorizedAccessException("You don't have permission to change this organisation's Xero connection.");

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
