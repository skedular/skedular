using HotChocolate;
using HotChocolate.Types.Pagination;

namespace Organization.Api.GraphQL.BankAccount;

[GraphQLName("OrganizationBankAccountEdge")]
public class OrganizationBankAccountEdge(OrganizationBankAccountDetails node, string cursor) : Edge<OrganizationBankAccountDetails>(node, cursor);
