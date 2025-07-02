using HotChocolate;

namespace Organization.Api.GraphQL.BankAccount;

[GraphQLName("OrganizationBankAccountWhereInput")]
public class OrganizationBankAccountWhereInput
{
    [GraphQLName("organizationId")] public string OrganizationId { get; set; } = string.Empty;
    [GraphQLName("nameContains")] public string? NameContains { get; set; }
    [GraphQLName("onboardingCompleted")] public bool? OnboardingCompleted { get; set; }
}
