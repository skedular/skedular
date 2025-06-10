using HotChocolate;

// ReSharper disable ClassNeverInstantiated.Global

namespace Customer.Api.GraphQL.Payment;

[GraphQLName("AddCustomerPaymentMethodIntentInput")]
public class AddCustomerPaymentMethodIntentInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
}

[GraphQLName("AddCustomerPaymentMethodIntentPayload")]
public class AddCustomerPaymentMethodIntentPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("publishedKeys")] public string PublishedKeys { get; set; } = string.Empty;
    [GraphQLName("clientSecret")] public string ClientSecret { get; set; } = string.Empty;
}

[GraphQLName("RemoveCustomerPaymentMethodInput")]
public class RemoveCustomerPaymentMethodInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string Id { get; set; } = string.Empty;
}

[GraphQLName("RemoveCustomerPaymentMethodPayload")]
public class RemoveCustomerPaymentMethodPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
}

[GraphQLName("AddMyPaymentMethodIntentInput")]
public class AddMyPaymentMethodIntentInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
}
