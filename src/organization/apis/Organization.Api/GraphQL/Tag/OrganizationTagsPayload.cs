using HotChocolate;

namespace Organization.Api.GraphQL.Tag;

[GraphQLName("OrganizationTagsPayload")]
public class OrganizationTagsPayload
{
    [GraphQLName("clientMutationId")]
    public string? ClientMutationId { get; set; }

    [GraphQLName("organizationTags")]
    public IEnumerable<OrganizationTagDetails> OrganizationTags { get; set; } = [];
}
