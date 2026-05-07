using Api.Shared.Grpc.Skedular.Customer.Core.V1;
using Api.Shared.Services.Configurations.Grpc;
using Customer.Api.Mappers;
using Customer.Api.Services;
using Customer.Shared.Models;
using Enterprise.Shared;
using Enterprise.Shared.Grpc;
using Enterprise.Shared.Version;
using Grpc.Core;
using CustomerService = Api.Shared.Grpc.Skedular.Customer.Core.V1.CustomerService;
using FeedbackChannel = Api.Shared.Grpc.Skedular.Customer.Core.V1.FeedbackChannel;
using Version = Api.Shared.Grpc.Skedular.Customer.Core.V1.Version;

namespace Customer.Api.Grpc;

public class CustomerGrpcService(
    IVersionService versionService,
    CustomerConfiguration customerConfiguration,
    ICustomerService customerService,
    ICustomerLocationSettingsService customerLocationSettingsService,
    ICustomerFeedbackService customerFeedbackService,
    ICustomerOrganizationTagSettingsService customerOrganizationTagSettingsService,
    ICustomerResourceSettingsService customerResourceSettingsService,
    IGrpcMapper grpcMapper,
    IGrpcAuthenticator grpcAuthenticator) : CustomerService.CustomerServiceBase
{
    public override Task<Version> GetVersion(VersionInput request, ServerCallContext context)
    {
        var version = versionService.GetVersion();

        return Task.FromResult(new Version { Major = version.Major, Minor = version.Minor, Build = version.Build, Revision = version.Revision });
    }

    public override async Task<global::Api.Shared.Grpc.Skedular.Customer.Core.V1.Customer> Get(GetInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(customerConfiguration.ApiKey);

        return grpcMapper.MapToGrpcResponse(await customerService.GetMeAsync(false, context.CancellationToken));
    }

    public override async Task<global::Api.Shared.Grpc.Skedular.Customer.Core.V1.Customer> GetById(GetByIdInput request,
        ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(customerConfiguration.ApiKey);

        return grpcMapper.MapToGrpcResponse(await customerService.GetByIdAsync(request.CustomerId, false, context.CancellationToken));
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

    public override async Task<global::Api.Shared.Grpc.Skedular.Customer.Core.V1.Customer> AddPreferredLocation(
        AddPreferredLocationInput request,
        ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(customerConfiguration.ApiKey);

        return grpcMapper.MapToGrpcResponse(
            await customerLocationSettingsService.AddCustomerPreferredLocationAsync(request.LocationId, null, false, context.CancellationToken));
    }

    public override async Task<global::Api.Shared.Grpc.Skedular.Customer.Core.V1.Customer> RemovePreferredLocation(
        RemovePreferredLocationInput request,
        ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(customerConfiguration.ApiKey);

        return grpcMapper.MapToGrpcResponse(
            await customerLocationSettingsService.RemoveCustomerPreferredLocationAsync(request.LocationId, null, context.CancellationToken));
    }

    public override async Task<global::Api.Shared.Grpc.Skedular.Customer.Core.V1.Customer> AddPreferredOrganizationTag(
        AddPreferredOrganizationTagInput request,
        ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(customerConfiguration.ApiKey);

        return grpcMapper.MapToGrpcResponse(
            await customerOrganizationTagSettingsService.AddCustomerPreferredOrganizationTagAsync(
                request.OrganizationTagId,
                null,
                context.CancellationToken));
    }

    public override async Task<global::Api.Shared.Grpc.Skedular.Customer.Core.V1.Customer> RemovePreferredOrganizationTag(
        RemovePreferredOrganizationTagInput request,
        ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(customerConfiguration.ApiKey);

        return grpcMapper.MapToGrpcResponse(
            await customerOrganizationTagSettingsService.RemoveCustomerPreferredOrganizationTagAsync(
                request.OrganizationTagId,
                null,
                context.CancellationToken));
    }

    public override async Task<global::Api.Shared.Grpc.Skedular.Customer.Core.V1.Customer> AddPreferredResource(
        AddPreferredResourceInput request,
        ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(customerConfiguration.ApiKey);

        return grpcMapper.MapToGrpcResponse(
            await customerResourceSettingsService.AddCustomerPreferredResourceAsync(request.ResourceId, null, context.CancellationToken));
    }

    public override async Task<global::Api.Shared.Grpc.Skedular.Customer.Core.V1.Customer> RemovePreferredResource(
        RemovePreferredResourceInput request,
        ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(customerConfiguration.ApiKey);

        return grpcMapper.MapToGrpcResponse(
            await customerResourceSettingsService.RemoveCustomerPreferredResourceAsync(request.ResourceId, null, context.CancellationToken));
    }
}
