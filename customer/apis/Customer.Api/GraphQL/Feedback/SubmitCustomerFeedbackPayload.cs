using HotChocolate;
using HotChocolate.Types.Relay;

namespace Customer.Api.GraphQL.Feedback;

[GraphQLName("SubmitCustomerFeedbackPayload")]
public class SubmitCustomerFeedbackPayload
{
    [GraphQLName("id")] [ID] public string Id { get; set; } = string.Empty;
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
}
