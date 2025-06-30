using Enterprise.Shared.GraphQL.Types;
using HotChocolate;
using HotChocolate.Types.Relay;

namespace Organization.Api.GraphQL;

[GraphQLName("OrganizationIndustryMainCategoryReferenceDetails")]
public class OrganizationIndustryMainCategoryReferenceDetails : Node
{
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("subCategories")] public IEnumerable<OrganizationIndustrySubCategoryReferenceDetails> SubCategories { get; set; } = [];
    [GraphQLName("id")] [ID] public string Id { get; set; } = string.Empty;
}
