using Enterprise.Shared.Pagination;
using HotChocolate;
using Organization.Shared.Models;

namespace Organization.Api.GraphQL.BankAccount;

[GraphQLName("OrganizationBankAccountOrderInput")]
public class OrganizationBankAccountOrderInput
{
    [GraphQLName("direction")] public OrderDirection Direction { get; set; }
    [GraphQLName("field")] public OrganizationBankAccountOrderField Field { get; set; }
}
