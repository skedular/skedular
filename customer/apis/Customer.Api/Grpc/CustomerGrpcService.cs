using System.Reflection;
using Api.Shared.Services.Grpc.UnityHub.Customer.V1;
using Customer.Api.Mappers;
using Customer.Api.Services;
using Customer.Shared.Configurations;
using Customer.Shared.Models;
using Enterprise.Shared;
using Enterprise.Shared.Grpc;
using Grpc.Core;
using CustomerService = Api.Shared.Services.Grpc.UnityHub.Customer.V1.CustomerService;
using FeedbackChannel = Api.Shared.Services.Grpc.UnityHub.Customer.V1.FeedbackChannel;
using Version = Api.Shared.Services.Grpc.UnityHub.Customer.V1.Version;

namespace Customer.Api.Grpc;

public class CustomerGrpcService(
    CustomerConfiguration customerConfiguration,
    ICustomerService customerService,
    ICustomerOrganizationSettingsService customerOrganizationSettingsService,
    ICustomerLocationSettingsService customerLocationSettingsService,
    ICustomerTeamSettingsService customerTeamSettingsService,
    ICustomerSettingsService customerSettingsService,
    ICustomerFeedbackService customerFeedbackService,
    ICustomerLocationTagSettingsService customerLocationTagSettingsService,
    ICustomerDeskSettingsService customerDeskSettingsService,
    IMapper mapper,
    IGrpcAuthenticator grpcAuthenticator) : CustomerService.CustomerServiceBase
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

    public override async Task<global::Api.Shared.Services.Grpc.UnityHub.Customer.V1.Customer> Admin_Get(
        Admin_GetInput request,
        ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(customerConfiguration.ApiKey);

        return mapper.MapToGrpcResponse(
            await customerService.GetByIdAsync(request.CustomerId, context.CancellationToken));
    }

    public override async Task<AnyCustomerExistByVerifiableTokenResponse> Admin_AnyCustomerExistByVerifiableToken(
        Admin_AnyCustomerExistByVerifiableTokenInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(customerConfiguration.ApiKey);

        var (exist, customer) =
            await customerService.AnyCustomerExistByVerifiableTokenAsync(
                request.VerifiableToken,
                context.CancellationToken);

        return new AnyCustomerExistByVerifiableTokenResponse
        {
            Exist = exist, Customer = customer is null ? null : mapper.MapToGrpcResponse(customer)
        };
    }

    public override async Task<AnyCustomerExistByEmailResponse> Admin_AnyCustomerExistByEmail(
        Admin_AnyCustomerExistByEmailInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(customerConfiguration.ApiKey);

        var (exist, customer) =
            await customerService.AnyCustomerExistByEmailAsync(request.Email, context.CancellationToken);

        return new AnyCustomerExistByEmailResponse
        {
            Exist = exist, Customer = customer is null ? null : mapper.MapToGrpcResponse(customer)
        };
    }

    public override async Task<global::Api.Shared.Services.Grpc.UnityHub.Customer.V1.Customer> Admin_Add(
        Admin_AddInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(customerConfiguration.ApiKey);

        return mapper.MapToGrpcResponse(
            await customerService.AddAsync(mapper.MapTo(request), false, context.CancellationToken));
    }

    public override async Task<global::Api.Shared.Services.Grpc.UnityHub.Customer.V1.Customer> Admin_AddIdentity(
        Admin_AddIdentityInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(customerConfiguration.ApiKey);

        return mapper.MapToGrpcResponse(
            await customerService.AddIdentityAsync(mapper.MapTo(request), context.CancellationToken));
    }

    public override async Task<global::Api.Shared.Services.Grpc.UnityHub.Customer.V1.Customer> Admin_UpdateIdentity(
        Admin_UpdateIdentityInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(customerConfiguration.ApiKey);

        return mapper.MapToGrpcResponse(
            await customerService.UpdateIdentityAsync(mapper.MapTo(request), context.CancellationToken));
    }

    public override async Task<global::Api.Shared.Services.Grpc.UnityHub.Customer.V1.Customer>
        Admin_SetDefaultOrganization(
            Admin_SetDefaultOrganizationInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(customerConfiguration.ApiKey);

        return mapper.MapToGrpcResponse(
            await customerOrganizationSettingsService.SetCustomerDefaultOrganizationAsync(
                request.OrganizationId, request.CustomerId, true, context.CancellationToken));
    }

    public override async Task<global::Api.Shared.Services.Grpc.UnityHub.Customer.V1.Customer>
        Admin_AddDefaultLocation(
            Admin_AddDefaultLocationInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(customerConfiguration.ApiKey);

        return mapper.MapToGrpcResponse(
            await customerLocationSettingsService.AddCustomerDefaultLocationAsync(
                request.LocationId, request.CustomerId, true, context.CancellationToken));
    }

    public override async Task<global::Api.Shared.Services.Grpc.UnityHub.Customer.V1.Customer> Admin_AddDefaultTeam(
        Admin_AddDefaultTeamInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(customerConfiguration.ApiKey);

        return mapper.MapToGrpcResponse(
            await customerTeamSettingsService.AddCustomerDefaultTeamAsync(
                request.TeamId, request.CustomerId, true, context.CancellationToken));
    }

    public override async Task<global::Api.Shared.Services.Grpc.UnityHub.Customer.V1.Customer> Get(
        GetInput request,
        ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(customerConfiguration.ApiKey);

        return mapper.MapToGrpcResponse(await customerService.GetMeAsync(false, context.CancellationToken));
    }

    public override async Task<global::Api.Shared.Services.Grpc.UnityHub.Customer.V1.Customer>
        DismissDefaultLocationOnboardingSetup(
            DismissDefaultLocationOnboardingSetupInput request,
            ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(customerConfiguration.ApiKey);

        return mapper.MapToGrpcResponse(
            await customerSettingsService.CompleteDefaultLocationOnboardingAsync(context.CancellationToken));
    }

    public override async Task<global::Api.Shared.Services.Grpc.UnityHub.Customer.V1.Customer>
        DismissSetupPreferredZones(
            DismissSetupPreferredZonesInput request,
            ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(customerConfiguration.ApiKey);

        return mapper.MapToGrpcResponse(
            await customerSettingsService.CompletePreferredZoneOnboardingAsync(context.CancellationToken));
    }

    public override async Task<global::Api.Shared.Services.Grpc.UnityHub.Customer.V1.Customer>
        DismissSetupPreferredDesks(
            DismissSetupPreferredDesksInput request,
            ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(customerConfiguration.ApiKey);

        return mapper.MapToGrpcResponse(
            await customerSettingsService.CompletePreferredDeskOnboardingAsync(context.CancellationToken));
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

    public override async Task<global::Api.Shared.Services.Grpc.UnityHub.Customer.V1.Customer>
        SetDefaultOrganization(
            SetDefaultOrganizationInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(customerConfiguration.ApiKey);

        return mapper.MapToGrpcResponse(
            await customerOrganizationSettingsService.SetCustomerDefaultOrganizationAsync(
                request.OrganizationId, null, false, context.CancellationToken));
    }

    public override async Task<global::Api.Shared.Services.Grpc.UnityHub.Customer.V1.Customer>
        AddDefaultLocation(
            AddDefaultLocationInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(customerConfiguration.ApiKey);

        return mapper.MapToGrpcResponse(
            await customerLocationSettingsService.AddCustomerDefaultLocationAsync(
                request.LocationId, null, false, context.CancellationToken));
    }

    public override async Task<global::Api.Shared.Services.Grpc.UnityHub.Customer.V1.Customer> AddDefaultTeam(
        AddDefaultTeamInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(customerConfiguration.ApiKey);

        return mapper.MapToGrpcResponse(
            await customerTeamSettingsService.AddCustomerDefaultTeamAsync(
                request.TeamId, null, false, context.CancellationToken));
    }

    public override async Task<global::Api.Shared.Services.Grpc.UnityHub.Customer.V1.Customer>
        ClearDefaultOrganization(
            ClearDefaultOrganizationInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(customerConfiguration.ApiKey);

        return mapper.MapToGrpcResponse(
            await customerOrganizationSettingsService.ClearCustomerDefaultOrganizationAsync(null,
                context.CancellationToken));
    }

    public override async Task<global::Api.Shared.Services.Grpc.UnityHub.Customer.V1.Customer> RemoveDefaultLocation(
        RemoveDefaultLocationInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(customerConfiguration.ApiKey);

        return mapper.MapToGrpcResponse(
            await customerLocationSettingsService.RemoveCustomerDefaultLocationAsync(
                request.LocationId, null, context.CancellationToken));
    }

    public override async Task<global::Api.Shared.Services.Grpc.UnityHub.Customer.V1.Customer> RemoveDefaultTeam(
        RemoveDefaultTeamInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(customerConfiguration.ApiKey);

        return mapper.MapToGrpcResponse(
            await customerTeamSettingsService.RemoveCustomerDefaultTeamAsync(
                request.TeamId, null, context.CancellationToken));
    }

    public override async Task<global::Api.Shared.Services.Grpc.UnityHub.Customer.V1.Customer> AddPreferredLocationTag(
        AddPreferredLocationTagInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(customerConfiguration.ApiKey);

        return mapper.MapToGrpcResponse(
            await customerLocationTagSettingsService.AddCustomerDefaultLocationTagAsync(
                request.LocationTagId, null, context.CancellationToken));
    }

    public override async Task<global::Api.Shared.Services.Grpc.UnityHub.Customer.V1.Customer>
        RemovePreferredLocationTag(
            RemovePreferredLocationTagInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(customerConfiguration.ApiKey);

        return mapper.MapToGrpcResponse(
            await customerLocationTagSettingsService.RemoveCustomerDefaultLocationTagAsync(
                request.LocationTagId, null, context.CancellationToken));
    }

    public override async Task<global::Api.Shared.Services.Grpc.UnityHub.Customer.V1.Customer> AddPreferredDesk(
        AddPreferredDeskInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(customerConfiguration.ApiKey);

        return mapper.MapToGrpcResponse(
            await customerDeskSettingsService.AddCustomerDefaultDeskAsync(
                request.DeskId, null, context.CancellationToken));
    }

    public override async Task<global::Api.Shared.Services.Grpc.UnityHub.Customer.V1.Customer> RemovePreferredDesk(
        RemovePreferredDeskInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(customerConfiguration.ApiKey);

        return mapper.MapToGrpcResponse(
            await customerDeskSettingsService.RemoveCustomerDefaultDeskAsync(
                request.DeskId, null, context.CancellationToken));
    }
}
