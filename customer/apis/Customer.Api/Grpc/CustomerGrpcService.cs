using System.Reflection;
using Api.Shared.Services.Grpc.Skedular.Customer.V1;
using Customer.Api.Mappers;
using Customer.Api.Services;
using Customer.Shared.Configurations;
using Customer.Shared.Models;
using Enterprise.Shared;
using Enterprise.Shared.Grpc;
using Grpc.Core;
using CustomerService = Api.Shared.Services.Grpc.Skedular.Customer.V1.CustomerService;
using FeedbackChannel = Api.Shared.Services.Grpc.Skedular.Customer.V1.FeedbackChannel;
using Version = Api.Shared.Services.Grpc.Skedular.Customer.V1.Version;

namespace Customer.Api.Grpc;

public class CustomerGrpcService(
    CustomerConfiguration customerConfiguration,
    ICustomerService customerService,
    ICustomerOrganizationSettingsService customerOrganizationSettingsService,
    ICustomerLocationSettingsService customerLocationSettingsService,
    ICustomerTeamSettingsService customerTeamSettingsService,
    ICustomerSettingsService customerSettingsService,
    ICustomerFeedbackService customerFeedbackService,
    ICustomerOrganizationTagSettingsService customerOrganizationTagSettingsService,
    ICustomerResourceSettingsService customerResourceSettingsService,
    IMapper mapper,
    IGrpcAuthenticator grpcAuthenticator) : CustomerService.CustomerServiceBase
{
    public override Task<Version> GetVersion(VersionInput request, ServerCallContext context)
    {
        var assembly = Assembly.GetEntryAssembly();
        ArgumentNullException.ThrowIfNull(assembly);
        var version = assembly.GetName().Version;
        ArgumentNullException.ThrowIfNull(version);

        return Task.FromResult(new Version { Major = version.Major, Minor = version.Minor, Build = version.Build, Revision = version.Revision });
    }

    public override async Task<global::Api.Shared.Services.Grpc.Skedular.Customer.V1.Customer>
        Admin_Get(Admin_GetInput request,
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

    public override async Task<global::Api.Shared.Services.Grpc.Skedular.Customer.V1.Customer> Admin_Add(
        Admin_AddInput request,
        ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(customerConfiguration.ApiKey);

        return mapper.MapToGrpcResponse(await customerService.AddAsync(mapper.MapTo(request), false, context.CancellationToken));
    }

    public override async Task<global::Api.Shared.Services.Grpc.Skedular.Customer.V1.Customer> Admin_AddIdentity(
        Admin_AddIdentityInput request,
        ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(customerConfiguration.ApiKey);

        return mapper.MapToGrpcResponse(await customerService.AddIdentityAsync(mapper.MapTo(request), context.CancellationToken));
    }

    public override async Task<global::Api.Shared.Services.Grpc.Skedular.Customer.V1.Customer> Admin_UpdateIdentity(
        Admin_UpdateIdentityInput request,
        ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(customerConfiguration.ApiKey);

        return mapper.MapToGrpcResponse(await customerService.UpdateIdentityAsync(mapper.MapTo(request), context.CancellationToken));
    }

    public override async Task<global::Api.Shared.Services.Grpc.Skedular.Customer.V1.Customer> Admin_SetDefaultOrganization(
        Admin_SetDefaultOrganizationInput request,
        ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(customerConfiguration.ApiKey);

        return mapper.MapToGrpcResponse(
            await customerOrganizationSettingsService.SetCustomerDefaultOrganizationAsync(
                request.OrganizationId,
                request.CustomerId,
                true,
                context.CancellationToken));
    }

    public override async Task<global::Api.Shared.Services.Grpc.Skedular.Customer.V1.Customer> Admin_AddPreferredLocation(
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

    public override async Task<global::Api.Shared.Services.Grpc.Skedular.Customer.V1.Customer> Admin_AddPreferredTeam(
        Admin_AddPreferredTeamInput request,
        ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(customerConfiguration.ApiKey);

        return mapper.MapToGrpcResponse(
            await customerTeamSettingsService.AddCustomerPreferredTeamAsync(request.TeamId, request.CustomerId, true, context.CancellationToken));
    }

    public override async Task<global::Api.Shared.Services.Grpc.Skedular.Customer.V1.Customer> Get(GetInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(customerConfiguration.ApiKey);

        return mapper.MapToGrpcResponse(await customerService.GetMeAsync(false, context.CancellationToken));
    }

    public override async Task<global::Api.Shared.Services.Grpc.Skedular.Customer.V1.Customer> DismissPreferredLocationOnboardingSetup(
        DismissPreferredLocationOnboardingSetupInput request,
        ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(customerConfiguration.ApiKey);

        return mapper.MapToGrpcResponse(await customerSettingsService.CompletePreferredLocationOnboardingAsync(context.CancellationToken));
    }

    public override async Task<global::Api.Shared.Services.Grpc.Skedular.Customer.V1.Customer> DismissSetupPreferredZones(
        DismissSetupPreferredZonesInput request,
        ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(customerConfiguration.ApiKey);

        return mapper.MapToGrpcResponse(await customerSettingsService.CompletePreferredZoneOnboardingAsync(context.CancellationToken));
    }

    public override async Task<Feedback> SubmitFeedback(SubmitFeedbackInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(customerConfiguration.ApiKey);

        return new Feedback
        {
            Id = (await customerFeedbackService.SubmitFeedbackAsync(
                new CustomerFeedback
                {
                    Id = request.Id,
                    Content = request.Feedback.ToSafeString(),
                    Channel = request.Channel switch
                    {
                        FeedbackChannel.Web => FeedbackChannelType.Web,
                        FeedbackChannel.Slack => FeedbackChannelType.Slack,
                        FeedbackChannel.MsTeams => FeedbackChannelType.MsTeams,
                        _ => throw new ArgumentOutOfRangeException()
                    }
                },
                context.CancellationToken)).Id
        };
    }

    public override async Task<global::Api.Shared.Services.Grpc.Skedular.Customer.V1.Customer> SetDefaultOrganization(
        SetDefaultOrganizationInput request,
        ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(customerConfiguration.ApiKey);

        return mapper.MapToGrpcResponse(
            await customerOrganizationSettingsService.SetCustomerDefaultOrganizationAsync(
                request.OrganizationId,
                null,
                false,
                context.CancellationToken));
    }

    public override async Task<global::Api.Shared.Services.Grpc.Skedular.Customer.V1.Customer> AddPreferredLocation(
        AddPreferredLocationInput request,
        ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(customerConfiguration.ApiKey);

        return mapper.MapToGrpcResponse(
            await customerLocationSettingsService.AddCustomerPreferredLocationAsync(request.LocationId, null, false, context.CancellationToken));
    }

    public override async Task<global::Api.Shared.Services.Grpc.Skedular.Customer.V1.Customer> AddPreferredTeam(
        AddPreferredTeamInput request,
        ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(customerConfiguration.ApiKey);

        return mapper.MapToGrpcResponse(
            await customerTeamSettingsService.AddCustomerPreferredTeamAsync(request.TeamId, null, false, context.CancellationToken));
    }

    public override async Task<global::Api.Shared.Services.Grpc.Skedular.Customer.V1.Customer> ClearDefaultOrganization(
        ClearDefaultOrganizationInput request,
        ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(customerConfiguration.ApiKey);

        return mapper.MapToGrpcResponse(
            await customerOrganizationSettingsService.ClearCustomerDefaultOrganizationAsync(null, context.CancellationToken));
    }

    public override async Task<global::Api.Shared.Services.Grpc.Skedular.Customer.V1.Customer> RemovePreferredLocation(
        RemovePreferredLocationInput request,
        ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(customerConfiguration.ApiKey);

        return mapper.MapToGrpcResponse(
            await customerLocationSettingsService.RemoveCustomerPreferredLocationAsync(request.LocationId, null, context.CancellationToken));
    }

    public override async Task<global::Api.Shared.Services.Grpc.Skedular.Customer.V1.Customer> RemovePreferredTeam(
        RemovePreferredTeamInput request,
        ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(customerConfiguration.ApiKey);

        return mapper.MapToGrpcResponse(
            await customerTeamSettingsService.RemoveCustomerPreferredTeamAsync(request.TeamId, null, context.CancellationToken));
    }

    public override async Task<global::Api.Shared.Services.Grpc.Skedular.Customer.V1.Customer> AddPreferredOrganizationTag(
        AddPreferredOrganizationTagInput request,
        ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(customerConfiguration.ApiKey);

        return mapper.MapToGrpcResponse(
            await customerOrganizationTagSettingsService.AddCustomerPreferredOrganizationTagAsync(
                request.OrganizationTagId,
                null,
                context.CancellationToken));
    }

    public override async Task<global::Api.Shared.Services.Grpc.Skedular.Customer.V1.Customer> RemovePreferredOrganizationTag(
        RemovePreferredOrganizationTagInput request,
        ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(customerConfiguration.ApiKey);

        return mapper.MapToGrpcResponse(
            await customerOrganizationTagSettingsService.RemoveCustomerPreferredOrganizationTagAsync(
                request.OrganizationTagId,
                null,
                context.CancellationToken));
    }

    public override async Task<global::Api.Shared.Services.Grpc.Skedular.Customer.V1.Customer> AddPreferredResource(
        AddPreferredResourceInput request,
        ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(customerConfiguration.ApiKey);

        return mapper.MapToGrpcResponse(
            await customerResourceSettingsService.AddCustomerPreferredResourceAsync(request.ResourceId, null, context.CancellationToken));
    }

    public override async Task<global::Api.Shared.Services.Grpc.Skedular.Customer.V1.Customer> RemovePreferredResource(
        RemovePreferredResourceInput request,
        ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(customerConfiguration.ApiKey);

        return mapper.MapToGrpcResponse(
            await customerResourceSettingsService.RemoveCustomerPreferredResourceAsync(request.ResourceId, null, context.CancellationToken));
    }
}
