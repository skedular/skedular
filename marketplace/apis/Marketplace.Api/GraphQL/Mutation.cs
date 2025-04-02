using Enterprise.Shared.Sanitization;
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

    [UseResolverScope]
    public async Task<ProductsPayload?> ActivateProductsAsync(
        ActivateProductsInput input,
        [Service] IProductService productService,
        CancellationToken cancellationToken)
    {
        var products = await productService.ActivateAsync(input.Ids.RemoveInvalidIds()!.ToList(), cancellationToken);
        return new ProductsPayload { ClientMutationId = input.ClientMutationId, Products = products.Select(mapper.MapTo)! };
    }

    [UseResolverScope]
    public async Task<ProductsPayload?> DeactivateProductsAsync(
        DeactivateProductsInput input,
        [Service] IProductService productService,
        CancellationToken cancellationToken)
    {
        var products = await productService.DeactivateAsync(input.Ids.RemoveInvalidIds()!.ToList(), cancellationToken);
        return new ProductsPayload { ClientMutationId = input.ClientMutationId, Products = products.Select(mapper.MapTo)! };
    }
}
