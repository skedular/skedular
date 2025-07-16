namespace Organization.Shared.Workflows.AddPayment;

public record AddOrganizationStripePaymentMethodState(
    AddOrganizationStripePaymentMethodInput Args,
    StripePaymentMethodEventState? StripePaymentMethodEventState);
