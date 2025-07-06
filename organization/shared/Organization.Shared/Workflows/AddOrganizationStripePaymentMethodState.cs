namespace Organization.Shared.Workflows;

public record AddOrganizationStripePaymentMethodState(
    AddOrganizationStripePaymentMethodInput Args,
    StripePaymentMethodEventState? StripePaymentMethodEventState);
