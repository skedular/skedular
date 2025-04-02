using HotChocolate;
using HotChocolate.Types;
using Marketplace.Api.Mappers;
using Marketplace.Api.Services;

namespace Marketplace.Api.GraphQL;

[MutationType]
public class Mutation(IMapper mapper)
{
    [UseResolverScope]
    public async Task<ProductPayload?> AddProductAsync(
        AddProductInput input,
        [Service] IProductService productService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            Product = mapper.MapTo(await productService.AddAsync(input.Id, input.OrganizationId, mapper.MapTo(input), cancellationToken))!
        };

    [UseResolverScope]
    public async Task<ProductPayload?> UpdateProductAsync(
        UpdateProductInput input,
        [Service] IProductService productService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            Product = mapper.MapTo(await productService.UpdateAsync(input.Id, mapper.MapTo(input), cancellationToken))!
        };

    [UseResolverScope]
    public async Task<ProductPayload?> DeleteProductAsync(
        DeleteProductInput input,
        [Service] IProductService productService,
        CancellationToken cancellationToken) =>
        new() { ClientMutationId = input.ClientMutationId, Product = mapper.MapTo(await productService.DeleteAsync(input.Id, cancellationToken))! };
}
