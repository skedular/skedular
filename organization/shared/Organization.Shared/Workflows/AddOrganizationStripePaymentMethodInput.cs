namespace Organization.Shared.Workflows;

public record AddOrganizationStripePaymentMethodInput(string OrganizationId, string ClientSecret, string SetupIntentId);
