using HotChocolate;
using Location.Shared.Models;

namespace Location.Api.GraphQL.Location;

[GraphQLName("UpdateLocationRestrictedInformationInput")]
public class UpdateLocationRestrictedInformationInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string Id { get; set; } = string.Empty;
    [GraphQLName("title")] public string Title { get; set; } = string.Empty;
    [GraphQLName("category")] public LocationRestrictedInformationCategory Category { get; set; }
    [GraphQLName("content")] public string Content { get; set; } = string.Empty;
    [GraphQLName("active")] public bool Active { get; set; } = true;
    [GraphQLName("sortOrder")] public int SortOrder { get; set; }
}
