using Api.Shared.Services.Models;
using Enterprise.Shared.GraphQL.Types;
using HotChocolate;
using HotChocolate.Types;
using HotChocolate.Types.Relay;

namespace Organization.Api.GraphQL.Member;

[GraphQLName("OrganizationMemberDetails")]
public class OrganizationMemberDetails : Node
{
    [GraphQLName("role")] public OrganizationMemberRole? Role { get; set; }
    [GraphQLName("status")] public OrganizationMemberStatus Status { get; set; }

    [GraphQLName("isOrganizationOnboardingDone")]
    public bool IsOrganizationOnboardingDone { get; set; }

    [GraphQLName("customerId")] public string CustomerId { get; set; } = string.Empty;
    [GraphQLName("id")] [ID] public string Id { get; set; } = string.Empty;
}

[ObjectType<OrganizationMemberDetails>]
public static partial class OrganizationMemberDetailsType
{
    static partial void Configure(IObjectTypeDescriptor<OrganizationMemberDetails> descriptor) => descriptor.Ignore(item => item.CustomerId);

    public static CustomerDetails GetCustomer([Parent] OrganizationMemberDetails item)
        => new(item.CustomerId);
}
