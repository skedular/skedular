using System.Reflection;
using Api.Shared.Services.Grpc.Skedular.Billing.V1;
using Billing.Api.Mappers;
using Billing.Api.Services;
using Billing.Api.Services.Authorization;
using Billing.Shared.Configurations;
using Enterprise.Shared.Grpc;
using Grpc.Core;
using Version = Api.Shared.Services.Grpc.Skedular.Billing.V1.Version;

namespace Billing.Api.Grpc;

public class BillingGrpcService(
    BillingConfiguration billingConfiguration,
    IGrpcAuthenticator grpcAuthenticator,
    IMapper mapper,
    IOrganizationAuthorizationService organizationAuthorizationService,
    IOrganizationBillingService organizationBillingService) : BillingService.BillingServiceBase
{
    public override Task<Version> GetVersion(VersionInput request, ServerCallContext context)
    {
        var assembly = Assembly.GetEntryAssembly();
        ArgumentNullException.ThrowIfNull(assembly);
        var version = assembly.GetName().Version;
        ArgumentNullException.ThrowIfNull(version);

        return Task.FromResult(new Version
        {
            Major = version.Major, Minor = version.Minor, Build = version.Build, Revision = version.Revision
        });
    }

    public override async Task<OrganizationPermissions> GetOrganizationPermissions(
        GetOrganizationPermissionsInput request,
        ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(billingConfiguration.ApiKey);

        var permissions =
            await organizationAuthorizationService.GetPermissionsAsync(
                request.OrganizationId,
                context.CancellationToken);
        return new OrganizationPermissions
        {
            CanViewBillingInfo = permissions.CanViewBillingInfo,
            CanManageBillingInfo = permissions.CanManageBillingInfo
        };
    }

    public override async Task<OrganizationBillingInfo> GetOrganizationBillingInfo(
        GetOrganizationBillingInfoInput request,
        ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(billingConfiguration.ApiKey);

        return mapper.MapToGrpcResponse(
            await organizationBillingService.GetBillingInfoById(request.OrganizationId, context.CancellationToken));
    }

    public override async Task<OrganizationBillingInfo> SetOrganizationBillingInfo(
        SetOrganizationBillingInfoInput request,
        ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(billingConfiguration.ApiKey);

        return mapper.MapToGrpcResponse(
            await organizationBillingService.SetBillingInfoAsync(
                request.OrganizationId,
                request.Email,
                request.AddressLine1,
                request.AddressLine2,
                request.Suburb,
                request.Province,
                request.City,
                request.Zipcode,
                request.Country,
                context.CancellationToken));
    }
}
