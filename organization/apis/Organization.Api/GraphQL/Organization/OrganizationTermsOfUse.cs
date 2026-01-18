using Enterprise.Shared.GraphQL.Types;
using HotChocolate;

namespace Organization.Api.GraphQL.Organization;

[GraphQLName("OrganizationTermsOfUse")]
public class OrganizationTermsOfUse : Node
{
    [GraphQLName("terms")] public string Terms { get; set; } = string.Empty;
}
