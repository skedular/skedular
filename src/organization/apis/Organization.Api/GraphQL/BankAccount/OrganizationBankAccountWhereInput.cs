using HotChocolate;

namespace Organization.Api.GraphQL.BankAccount;

[GraphQLName("OrganizationBankAccountWhereInput")]
public class OrganizationBankAccountWhereInput
{
    [GraphQLName("organizationCustomDomain")]
    public string OrganizationCustomDomain { get; set; } = string.Empty;

    [GraphQLName("nameContains")]
    public string? NameContains { get; set; }

    [GraphQLName("onboardingCompleted")]
    public bool? OnboardingCompleted { get; set; }
}
