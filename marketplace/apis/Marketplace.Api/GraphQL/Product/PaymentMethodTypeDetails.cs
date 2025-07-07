using Api.Shared.Services.Models;
using HotChocolate;

namespace Marketplace.Api.GraphQL.Product;

[GraphQLName("Marketplace_PaymentMethodTypeDetails")]
public class PaymentMethodTypeDetails
{
    [GraphQLName("type")] public PaymentMethod Type { get; set; }
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
}
