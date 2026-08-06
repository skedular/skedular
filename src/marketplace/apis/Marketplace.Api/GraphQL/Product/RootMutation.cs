using Enterprise.Shared.Sanitization;
using HotChocolate;
using HotChocolate.Types;
using Marketplace.Api.Mappers;
using Marketplace.Api.Models;
using Marketplace.Api.Services;

namespace Marketplace.Api.GraphQL.Product;

[MutationType]
public class RootMutation(IGraphQlMapper graphQlMapper)
{
    [UseResolverScope]
    public async Task<ProductPayload> AddProductAsync(
        AddProductInput input,
        [Service]
        IProductService productService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            Product = graphQlMapper.MapTo(
                await productService.AddAsync(
                    input.Id,
                    input.OrganizationId,
                    input.OrganizationCustomDomain,
                    graphQlMapper.MapTo(input),
                    cancellationToken))!,
        };

    [UseResolverScope]
    public async Task<ProductPayload> UpdateProductAsync(
        UpdateProductInput input,
        [Service]
        IProductService productService,
        CancellationToken cancellationToken) =>
        new()
        {
            ClientMutationId = input.ClientMutationId,
            Product = graphQlMapper.MapTo(
                await productService.UpdateAsync(
                    new ProductPatchRequest(input.Id, input.FieldsToUpdate, graphQlMapper.MapTo(input)),
                    cancellationToken))!,
        };

    [UseResolverScope]
    public async Task<ProductsPayload> DeleteProductsAsync(
        DeleteProductsInput input,
        [Service]
        IProductService productService,
        CancellationToken cancellationToken)
    {
        var products = await productService.DeleteAsync(input.Ids.RemoveInvalidIds().ToList(), cancellationToken);
        return new ProductsPayload
        {
            ClientMutationId = input.ClientMutationId,
            Products = products.Select(graphQlMapper.MapTo)!,
        };
    }

    [UseResolverScope]
    public async Task<ProductsPayload> ActivateProductsAsync(
        ActivateProductsInput input,
        [Service]
        IProductService productService,
        CancellationToken cancellationToken)
    {
        var products = await productService.ActivateAsync(input.Ids.RemoveInvalidIds().ToList(), cancellationToken);
        return new ProductsPayload
        {
            ClientMutationId = input.ClientMutationId,
            Products = products.Select(graphQlMapper.MapTo)!,
        };
    }

    [UseResolverScope]
    public async Task<ProductsPayload> DeactivateProductsAsync(
        DeactivateProductsInput input,
        [Service]
        IProductService productService,
        CancellationToken cancellationToken)
    {
        var products = await productService.DeactivateAsync(input.Ids.RemoveInvalidIds().ToList(), cancellationToken);
        return new ProductsPayload
        {
            ClientMutationId = input.ClientMutationId,
            Products = products.Select(graphQlMapper.MapTo)!,
        };
    }
}
