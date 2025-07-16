namespace Organization.Shared.Activities;

public record SetOrganizationPaymentMethodInput(string OrganizationId, string SetupIntentId, string RedirectStatus);
