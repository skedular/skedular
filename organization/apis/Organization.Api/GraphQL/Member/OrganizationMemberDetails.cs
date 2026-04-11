using Enterprise.Shared.GraphQL.Types;
using HotChocolate;
using HotChocolate.Types;

namespace Organization.Api.GraphQL.Member;

[GraphQLName("OrganizationMemberDetails")]
public class OrganizationMemberDetails : Node
{
    [GraphQLName("role")] public OrganizationMemberRoleDetails Role { get; set; } = new();
    [GraphQLName("status")] public OrganizationMemberStatusDetails Status { get; set; } = new();

    [GraphQLName("isOrganizationOnboardingDone")]
    public bool IsOrganizationOnboardingDone { get; set; }

    [GraphQLName("customerId")] public string CustomerId { get; set; } = string.Empty;
}

[ObjectType<OrganizationMemberDetails>]
public static partial class OrganizationMemberDetailsType
{
    static partial void Configure(IObjectTypeDescriptor<OrganizationMemberDetails> descriptor) => descriptor.Ignore(item => item.CustomerId);

    public static CustomerDetails GetCustomer([Parent] OrganizationMemberDetails item)
        => new(item.CustomerId);
}
