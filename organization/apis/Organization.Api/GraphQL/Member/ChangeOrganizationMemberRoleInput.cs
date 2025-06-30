using Api.Shared.Services.Models;
using HotChocolate;

// ReSharper disable ClassNeverInstantiated.Global

namespace Organization.Api.GraphQL.Member;

[GraphQLName("ChangeOrganizationMemberRoleInput")]
public class ChangeOrganizationMemberRoleInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string Id { get; set; } = string.Empty;
    [GraphQLName("role")] public OrganizationMemberRole Role { get; set; }
}
