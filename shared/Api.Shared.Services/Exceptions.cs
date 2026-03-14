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

public class BookingIsNotMarketplaceType() : Exception("Booking is not marketplace type");

public class BookingMustStartAndEndWithinSameDay() : Exception("Booking must start and end within the same day");

public class MarketplaceBookingCadenceRequiresRecurringFlow()
    : Exception("This marketplace booking cadence must be booked through the recurring marketplace flow");

public class MarketplaceRecurringBookingCadenceMustBeRecurring()
    : Exception("Marketplace recurring booking must use a cadence greater than a single-day booking");

public class MarketplaceBookingBillingScheduleNotAccepted()
    : Exception("The selected marketplace booking billing schedule is not accepted by the product pricing option");

public class ProductPricingAcceptedBillingSchedulesRequired()
    : Exception("At least one accepted billing schedule must be selected");

public class ProductPricingAcceptedBillingSchedulesCannotContainNotSet()
    : Exception("Accepted billing schedules cannot contain a not-set billing mode or interval");

public class ProductPricingAcceptedBillingSchedulesCannotContainDuplicates()
    : Exception("Accepted billing schedules cannot contain duplicate billing schedules");

public class OrganizationPhysicalAddressNotFound() : Exception("Organization physical address not found");

public class LocationPhysicalAddressNotFound() : Exception("Location physical address not found");

public class MoreResourcesHaveBeenSelectedThanAreAllowedForThisBooking()
    : Exception("More resources have been selected than are allowed for this booking");

public class BookingIsNotPrivate() : Exception("Booking is not private");

public class BookingIsNotMarketplace() : Exception("Booking is not marketplace");

public class RecurringBookingIsNotPrivate() : Exception("Recurring booking is not private");

public class RecurringBookingIsNotMarketplace() : Exception("Recurring booking is not marketplace");

public class MarketplaceRecurringBookingCannotBeUpdated() : Exception("Marketplace recurring booking cannot be updated");
