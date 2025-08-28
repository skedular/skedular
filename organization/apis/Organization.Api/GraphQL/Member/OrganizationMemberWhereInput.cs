using HotChocolate;

namespace Organization.Api.GraphQL.Member;

[GraphQLName("OrganizationMemberWhereInput")]
public class OrganizationMemberWhereInput
{
    [GraphQLName("organizationUniqueAlphanumericName")]
    public string OrganizationUniqueAlphanumericName { get; set; } = string.Empty;

    [GraphQLName("nameContains")] public string? NameContains { get; set; }
    [GraphQLName("customerId")] public string? CustomerId { get; set; }
}
