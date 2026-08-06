using Api.Shared.Services.Models;
using HotChocolate;

namespace Customer.Api.GraphQL.Customer;

[GraphQLName("PersonalInformationVisibilityDetails")]
public class PersonalInformationVisibilityDetails
{
    [GraphQLName("type")]
    public PersonalInformationVisibility Type { get; set; }

    [GraphQLName("name")]
    public string Name { get; set; } = string.Empty;
}
