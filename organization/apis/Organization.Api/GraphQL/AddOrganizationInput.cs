using Api.Shared.Services.Models;
using HotChocolate;

// ReSharper disable ClassNeverInstantiated.Global

namespace Organization.Api.GraphQL;

[GraphQLName("AddOrganizationInput")]
public class AddOrganizationInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string? Id { get; set; }
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("about")] public string? About { get; set; }
    [GraphQLName("website")] public string? Website { get; set; }
    [GraphQLName("type")] public OrganizationType Type { get; set; }
    [GraphQLName("contactEmail")] public string? ContactEmail { get; set; }
    [GraphQLName("contactPhone")] public string? ContactPhone { get; set; }

    [GraphQLName("memberVisibilityPolicy")]
    public OrganizationMemberVisibilityPolicy MemberVisibilityPolicy { get; set; }

    [GraphQLName("agreedToTermsOfUse")] public bool AgreedToTermsOfUse { get; set; }
    [GraphQLName("termsOfUseId")] public string TermsOfUseId { get; set; } = string.Empty;

    [GraphQLName("industrySubCategoryIds")]
    public IEnumerable<string> IndustrySubCategoryIds { get; set; } = [];

    [GraphQLName("physicalAddress")] public AddressDetailsInput PhysicalAddress { get; set; } = new();
}
