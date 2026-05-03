using Api.Shared.Services.Models;
using HotChocolate;
using HotChocolate.Types.Composite;

namespace Marketplace.Api.GraphQL.Product;

[GraphQLName("PaymentMethodTypeDetails")]
[Shareable]
public class PaymentMethodTypeDetails
{
    [GraphQLName("type")] public PaymentMethod Type { get; set; }
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
}
