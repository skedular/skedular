namespace Enterprise.Shared.Exceptions;

public class Unauthorized() : Exception("Unauthorized");

public class CustomerNotFound() : Exception("Customer not found");

public class OrganizationNotFound() : Exception("Organization not found");

public class OrganizationNameIsInvalid() : Exception("Organization name is invalid");

public class OrganizationSsoIsNotYetSetup() : Exception("Organization SSO is not yet setup");

public class OrganizationMemberNotFound() : Exception("Organization member not found");

public class OrganizationJoinInvitationNotFound() : Exception("Organization join invitation not found");

public class LocationNotFound() : Exception("Location not found");

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

public class SamlMetadataException() : Exception("Signing certificate not found in IdP metadata");

public class TeamPrimaryLocationOrganizationDoesNotMatchTeamOrganization()
    : Exception("Team Primary location organization does not match team organization");

public class ProductNotFound() : Exception("Product not found");

public class OrganizationStripeConnectAccountNotFound() : Exception("Organization Stripe Connect Account not found");

public class OrganizationStripeConnectAccountRefreshCodeNotFound() : Exception("Organization Stripe Connect Account refresh code not found");

public class OrganizationStripeCustomerRelationshipIsNotSetYet() : Exception("Organization Stripe customer relationship is not set yet");
