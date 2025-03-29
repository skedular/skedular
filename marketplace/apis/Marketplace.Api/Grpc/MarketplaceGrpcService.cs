using System.Reflection;
using Api.Shared.Services.Grpc.Skedular.Marketplace.V1;
using Grpc.Core;
using Version = Api.Shared.Services.Grpc.Skedular.Marketplace.V1.Version;

namespace Marketplace.Api.Grpc;

public class MarketplaceGrpcService : MarketplaceService.MarketplaceServiceBase
{
    public override Task<Version> GetVersion(VersionInput request, ServerCallContext context)
    {
        var assembly = Assembly.GetEntryAssembly();
        ArgumentNullException.ThrowIfNull(assembly);
        var version = assembly.GetName().Version;
        ArgumentNullException.ThrowIfNull(version);

        return Task.FromResult(new Version { Major = version.Major, Minor = version.Minor, Build = version.Build, Revision = version.Revision });
    }
}
