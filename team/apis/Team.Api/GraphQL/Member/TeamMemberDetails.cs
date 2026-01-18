using Api.Shared.Services.Models;
using Enterprise.Shared.GraphQL.Types;
using HotChocolate;
using HotChocolate.Types;

namespace Team.Api.GraphQL.Member;

[GraphQLName("TeamMemberDetails")]
public class TeamMemberDetails : Node
{
    [GraphQLName("role")] public TeamMemberRole? Role { get; set; }
    [GraphQLName("status")] public TeamMemberStatus Status { get; set; }
    [GraphQLName("customerId")] public string CustomerId { get; set; } = string.Empty;
    [GraphQLName("organizationMember")] public TeamOrganizationMemberDetails? OrganizationMember { get; set; }
}

[ObjectType<TeamMemberDetails>]
public static partial class TeamMemberDetailsType
{
    static partial void Configure(IObjectTypeDescriptor<TeamMemberDetails> descriptor) => descriptor.Ignore(item => item.CustomerId);

    public static CustomerDetails GetCustomer([Parent] TeamMemberDetails item) => new(item.CustomerId);
}
