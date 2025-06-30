using Enterprise.Shared.GraphQL.Types;
using HotChocolate;
using HotChocolate.Types.Relay;

namespace Organization.Api.GraphQL;

[GraphQLName("OrganizationIndustrySubCategoryReferenceDetails")]
public class OrganizationIndustrySubCategoryReferenceDetails : Node
{
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("mainCategoryName")] public string MainCategoryName { get; set; } = string.Empty;
    [GraphQLName("id")] [ID] public string Id { get; set; } = string.Empty;
}
