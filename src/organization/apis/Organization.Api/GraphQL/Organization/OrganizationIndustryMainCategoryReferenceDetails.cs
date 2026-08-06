using Enterprise.Shared.GraphQL.Types;
using HotChocolate;

namespace Organization.Api.GraphQL.Organization;

[GraphQLName("OrganizationIndustryMainCategoryReferenceDetails")]
public class OrganizationIndustryMainCategoryReferenceDetails : Node
{
    [GraphQLName("name")]
    public string Name { get; set; } = string.Empty;

    [GraphQLName("subCategories")]
    public IEnumerable<OrganizationIndustrySubCategoryReferenceDetails> SubCategories { get; set; } = [];
}
