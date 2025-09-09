using HotChocolate;
using HotChocolate.Types;
using HotChocolate.Types.Relay;

namespace Team.Api.GraphQL.Member;

[GraphQLName("TeamOrganizationMemberDetails")]
public class TeamOrganizationMemberDetails
{
    [GraphQLName("uniqueId")] [ID] public string UniqueId { get; set; } = string.Empty;
    [GraphQLName("customerId")] public string CustomerId { get; set; } = string.Empty;
}

[ObjectType<TeamOrganizationMemberDetails>]
public static partial class TeamOrganizationMemberDetailsType
{
    static partial void Configure(IObjectTypeDescriptor<TeamOrganizationMemberDetails> descriptor) => descriptor.Ignore(item => item.CustomerId);

    public static CustomerDetails GetCustomer([Parent] TeamOrganizationMemberDetails item) => new(item.CustomerId);
}
