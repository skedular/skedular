using Api.Shared.Services.Grpc.Skedular.Payment.V1;
using Enterprise.Shared.Version;
using Grpc.Core;
using Version = Api.Shared.Services.Grpc.Skedular.Payment.V1.Version;

namespace Payment.Api.Grpc;

public class PaymentGrpcService(IVersionService versionService) : PaymentService.PaymentServiceBase
{
    public override Task<Version> GetVersion(VersionInput request, ServerCallContext context)
    {
        var version = versionService.GetVersion();

        return Task.FromResult(new Version { Major = version.Major, Minor = version.Minor, Build = version.Build, Revision = version.Revision });
    }
}
