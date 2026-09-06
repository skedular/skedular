using Api.Shared.Services.Models;
using HotChocolate;

namespace Marketplace.Api.GraphQL.Product;

[GraphQLName("MembershipTermDetails")]
public class MembershipTermDetails
{
    [GraphQLName("type")]
    public MembershipTerm Type { get; set; }

    [GraphQLName("name")]
    public string Name { get; set; } = string.Empty;
}
