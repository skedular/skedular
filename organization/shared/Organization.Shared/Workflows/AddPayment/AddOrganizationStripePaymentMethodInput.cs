namespace Organization.Shared.Workflows.AddPayment;

public record AddOrganizationStripePaymentMethodInput(string OrganizationId, string ClientSecret, string SetupIntentId);
