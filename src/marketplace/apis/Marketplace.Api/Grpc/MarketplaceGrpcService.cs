using Api.Shared.Grpc.Skedular.Marketplace.Core.V1;
using Api.Shared.Services.Configurations.Grpc;
using Enterprise.Shared.Grpc;
using Enterprise.Shared.Version;
using Grpc.Core;
using Marketplace.Api.Services;
using Version = Api.Shared.Grpc.Skedular.Marketplace.Core.V1.Version;

namespace Marketplace.Api.Grpc;

public class MarketplaceGrpcService(
    MarketplaceConfiguration marketplaceConfiguration,
    IGrpcAuthenticator grpcAuthenticator,
    IProductService productService,
    IVersionService versionService) : MarketplaceService.MarketplaceServiceBase
{
    public override Task<Version> GetVersion(VersionInput request, ServerCallContext context)
    {
        var version = versionService.GetVersion();

        return Task.FromResult(new Version
        {
            Major = version.Major,
            Minor = version.Minor,
            Build = version.Build,
            Revision = version.Revision,
        });
    }

    public override async Task<HostLocationProduct> EnsureHostLocationDraftProduct(
        EnsureHostLocationDraftProductInput request,
        ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(marketplaceConfiguration.ApiKey);
        var product = await productService.EnsureHostLocationDraftAsync(
            request.Id,
            request.OrganizationId,
            request.ProductTagId,
            request.LocationName,
            context.CancellationToken);
        return new HostLocationProduct
        {
            Id = product.Id,
            Inactive = product.Inactive,
        };
    }

    public override async Task<HostLocationProduct> RemoveHostLocationDraftProduct(
        RemoveHostLocationDraftProductInput request,
        ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(marketplaceConfiguration.ApiKey);
        await productService.RemoveHostLocationDraftAsync(request.Id, request.OrganizationId, context.CancellationToken);
        return new HostLocationProduct
        {
            Id = request.Id,
            Inactive = true,
        };
    }
}
