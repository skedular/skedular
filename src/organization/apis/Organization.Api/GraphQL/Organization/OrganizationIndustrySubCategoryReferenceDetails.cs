using Enterprise.Shared.GraphQL.Types;
using HotChocolate;

namespace Organization.Api.GraphQL.Organization;

[GraphQLName("OrganizationIndustrySubCategoryReferenceDetails")]
public class OrganizationIndustrySubCategoryReferenceDetails : Node
{
    [GraphQLName("name")]
    public string Name { get; set; } = string.Empty;

    [GraphQLName("mainCategoryName")]
    public string MainCategoryName { get; set; } = string.Empty;
}
