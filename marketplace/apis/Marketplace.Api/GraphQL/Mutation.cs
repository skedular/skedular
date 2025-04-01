using HotChocolate.Types;
using Marketplace.Api.Mappers;

namespace Marketplace.Api.GraphQL;

[MutationType]
public class Mutation(IMapper mapper)
{
    [UseResolverScope]
    public async Task<ProductPayload?> AddProductAsync(
        AddProductInput input,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(input.OrganizationId);

        throw new NotImplementedException();
    }

    [UseResolverScope]
    public async Task<ProductPayload?> UpdateProductAsync(
        UpdateProductInput input,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(input.OrganizationId);

        throw new NotImplementedException();
    }

    [UseResolverScope]
    public async Task<ProductPayload?> DeleteProductAsync(
        DeleteProductInput input,
        CancellationToken cancellationToken) =>
        throw new NotImplementedException();
}
