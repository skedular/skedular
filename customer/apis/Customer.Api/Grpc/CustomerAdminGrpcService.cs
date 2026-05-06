using Api.Shared.Grpc.Skedular.Customer.Admin.V1;
using Api.Shared.Services.Configurations.Grpc;
using Customer.Api.Mappers;
using Customer.Api.Services;
using Enterprise.Shared.Grpc;
using Grpc.Core;
using CustomerAdminService = Api.Shared.Grpc.Skedular.Customer.Admin.V1.CustomerAdminService;

namespace Customer.Api.Grpc;

public class CustomerAdminGrpcService(
    CustomerConfiguration customerConfiguration,
    IGrpcAuthenticator grpcAuthenticator,
    ICustomerService customerService,
    ICustomerOrganizationSettingsService customerOrganizationSettingsService,
    ICustomerLocationSettingsService customerLocationSettingsService,
    IMapper mapper) : CustomerAdminService.CustomerAdminServiceBase
{
    public override async Task<global::Api.Shared.Grpc.Skedular.Customer.Core.V1.Customer> Admin_Get(
        Admin_GetInput request,
        ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(customerConfiguration.ApiKey);

        return mapper.MapToGrpcResponse(await customerService.GetByIdAsync(request.CustomerId, true, context.CancellationToken));
    }

    public override async Task<AnyCustomerExistByVerifiableTokenResponse> Admin_AnyCustomerExistByVerifiableToken(
        Admin_AnyCustomerExistByVerifiableTokenInput request,
        ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(customerConfiguration.ApiKey);

        var (exist, customer) = await customerService.AnyCustomerExistByVerifiableTokenAsync(request.VerifiableToken, context.CancellationToken);

        return new AnyCustomerExistByVerifiableTokenResponse
        {
            Exist = exist, Customer = customer is null ? null : mapper.MapToGrpcResponse(customer)
        };
    }

    public override async Task<AnyCustomerExistByEmailResponse> Admin_AnyCustomerExistByEmail(
        Admin_AnyCustomerExistByEmailInput request,
        ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(customerConfiguration.ApiKey);

        var (exist, customer) = await customerService.AnyCustomerExistByEmailAsync(request.Email, context.CancellationToken);

        return new AnyCustomerExistByEmailResponse { Exist = exist, Customer = customer is null ? null : mapper.MapToGrpcResponse(customer) };
    }

    public override async Task<global::Api.Shared.Grpc.Skedular.Customer.Core.V1.Customer> Admin_Add(
        Admin_AddInput request,
        ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(customerConfiguration.ApiKey);

        return mapper.MapToGrpcResponse(await customerService.AddAsync(mapper.MapTo(request), false, context.CancellationToken));
    }

    public override async Task<global::Api.Shared.Grpc.Skedular.Customer.Core.V1.Customer> Admin_AddIdentity(
        Admin_AddIdentityInput request,
        ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(customerConfiguration.ApiKey);

        return mapper.MapToGrpcResponse(await customerService.AddIdentityAsync(mapper.MapTo(request), context.CancellationToken));
    }

    public override async Task<global::Api.Shared.Grpc.Skedular.Customer.Core.V1.Customer> Admin_UpdateIdentity(
        Admin_UpdateIdentityInput request,
        ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(customerConfiguration.ApiKey);

        return mapper.MapToGrpcResponse(await customerService.UpdateIdentityAsync(mapper.MapTo(request), context.CancellationToken));
    }

    public override async Task<global::Api.Shared.Grpc.Skedular.Customer.Core.V1.Customer> Admin_SetDefaultOrganization(
        Admin_SetDefaultOrganizationInput request,
        ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(customerConfiguration.ApiKey);

        return mapper.MapToGrpcResponse(
            await customerOrganizationSettingsService.SetCustomerDefaultOrganizationAsync(
                request.OrganizationId,
                null,
                request.CustomerId,
                true,
                context.CancellationToken));
    }

    public override async Task<global::Api.Shared.Grpc.Skedular.Customer.Core.V1.Customer> Admin_AddPreferredLocation(
        Admin_AddPreferredLocationInput request,
        ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(customerConfiguration.ApiKey);

        return mapper.MapToGrpcResponse(
            await customerLocationSettingsService.AddCustomerPreferredLocationAsync(
                request.LocationId,
                request.CustomerId,
                true,
                context.CancellationToken));
    }
}
