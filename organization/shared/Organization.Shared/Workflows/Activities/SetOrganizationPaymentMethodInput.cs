namespace Organization.Shared.Workflows.Activities;

public record SetOrganizationPaymentMethodInput(string OrganizationId, string SetupIntentId, string RedirectStatus);
