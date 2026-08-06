using Customer.Shared.Models;
using HotChocolate;

namespace Customer.Api.GraphQL.Feedback;

[GraphQLName("SubmitCustomerFeedbackInput")]
public class SubmitCustomerFeedbackInput
{
    [GraphQLName("clientMutationId")]
    public string? ClientMutationId { get; set; }

    [GraphQLName("id")]
    public string? Id { get; set; }

    [GraphQLName("feedbackContent")]
    public string FeedbackContent { get; set; } = string.Empty;

    [GraphQLName("channel")]
    public FeedbackChannelType Channel { get; set; }
}
