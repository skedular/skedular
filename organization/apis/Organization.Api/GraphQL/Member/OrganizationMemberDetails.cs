using Api.Shared.Services.Models;
using Enterprise.Shared.GraphQL.Types;
using HotChocolate;
using HotChocolate.Types.Relay;

namespace Organization.Api.GraphQL.Member;

[GraphQLName("OrganizationMemberDetails")]
public class OrganizationMemberDetails : Node
{
    [GraphQLName("role")] public OrganizationMemberRole? Role { get; set; }
    [GraphQLName("status")] public OrganizationMemberStatus Status { get; set; }

    [GraphQLName("isOrganizationOnboardingDone")]
    public bool IsOrganizationOnboardingDone { get; set; }

    [GraphQLName("customer")] public CustomerDetails Customer { get; set; } = new();
    [GraphQLName("id")] [ID] public string Id { get; set; } = string.Empty;
}
