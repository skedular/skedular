namespace Api.Shared.Services;

public class CustomerNotFound() : Exception("Customer not found");

public class OrganizationNotFound() : Exception("Organization not found");

public class OrganizationBillingDetailsNotFound() : Exception("Organization billing details not found");

public class CustomerBillingDetailsNotFound() : Exception("Customer billing details not found");

public class OrganizationSsoIsNotYetSetup() : Exception("Organization SSO is not yet setup");

public class OrganizationMemberNotFound() : Exception("Organization member not found");

public class OrganizationJoinInvitationNotFound() : Exception("Organization join invitation not found");

public class LocationNotFound() : Exception("Location not found");

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

public class NoMoreInteractionAllowed()
    : Exception("You have exceeded your free tier limit, please upgrade to 'Pay as you go' tier to have full access to all features.");

public class ResourceNotAvailable() : Exception("Resource not available");

public class SlackWorkspaceNotFound() : Exception("Slack workspace not found");

public class SlackWorkspaceMemberTypeNotSupported() : Exception("Slack workspace member type not supported");

public class OrganizationTagNotFound() : Exception("Organization tag not found");

public class CustomTagWithSameNameExist() : Exception("Tag with same name exist");

public class ZoneWithSameNameExist() : Exception("Zone with same name exist");

public class OrganizationTagWithSameNameExist() : Exception("Organization tag with same name exist");

public class TeamPrimaryLocationOrganizationDoesNotMatchTeamOrganization()
    : Exception("Team Primary location organization does not match team organization");

public class ProductNotFound() : Exception("Product not found");

public class OrganizationStripeConnectAccountNotFound() : Exception("Organization Stripe Connect Account not found");

public class NoStripeConnectAccountFoundForOrganization() : Exception("No Stripe Connect Account found for Organization");

public class OrganizationStripeConnectAccountRefreshCodeNotFound() : Exception("Organization Stripe Connect Account refresh code not found");

public class OrganizationStripeCustomerRelationshipIsNotSetYet() : Exception("Organization Stripe customer relationship is not set yet");

public class CrossOrganizationProductBookingNotAllowed() : Exception("Cross organization product booking not allowed");

public class InvalidSsoConfiguration() : Exception("Invalid SSO configuration");

public class OrganizationPaymentMethodNotFound() : Exception("Organization payment method not found");

public class ResourceAndFloorPlanLocationMismatch() : Exception("Resource and floor plan must belong to the same location");

public class ResourceIsPlacedOnDifferentFloorPlan() : Exception("Resource is placed on different floor plan");

public class StripeCustomerNotFound() : Exception("Stripe Customer not found");

public class OrganizationBankAccountNotFound() : Exception("Organization Bank Account not found");

public class BookingPaymentMethodNotAccepted() : Exception("Booking payment method not accepted");

public class BookingsProductsWithMultipleCurrenciesAreNotSupported() : Exception("Bookings products with multiple currencies are not supported");

public class BookingIsNotMarketplaceType() : Exception("Booking is not marketplace type");

public class OrganizationPhysicalAddressNotFound() : Exception("Organization physical address not found");
