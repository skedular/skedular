using Api.Shared.Services.Grpc.Skedular.Billing.V1;
using Enterprise.Shared.Version;
using Grpc.Core;
using Version = Api.Shared.Services.Grpc.Skedular.Billing.V1.Version;

namespace Billing.Api.Grpc;

public class BillingGrpcService(IVersionService versionService) : BillingService.BillingServiceBase
{
    public override Task<Version> GetVersion(VersionInput request, ServerCallContext context)
    {
        var version = versionService.GetVersion();

        return Task.FromResult(new Version { Major = version.Major, Minor = version.Minor, Build = version.Build, Revision = version.Revision });
    }
}
