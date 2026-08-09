using Api.Shared.Grpc.Skedular.Organization.Billing.V1;
using Api.Shared.Services;
using Api.Shared.Services.Configurations.Grpc;
using Enterprise.Shared;
using Enterprise.Shared.Grpc;
using Enterprise.Shared.Pagination;
using Grpc.Core;
using Organization.Api.Mappers;
using Organization.Api.Services;
using Organization.Shared.Models;
using BillingDetails = Api.Shared.Grpc.Skedular.Organization.Core.V1.BillingDetails;
using GrpcOrganizationBillingService = Api.Shared.Grpc.Skedular.Organization.Billing.V1.OrganizationBillingService;
using OrderDirection = Enterprise.Shared.Pagination.OrderDirection;
using PageInfo = Api.Shared.Grpc.Skedular.Organization.Core.V1.PageInfo;

namespace Organization.Api.Grpc;

public class OrganizationBillingGrpcService(
    OrganizationConfiguration organizationConfiguration,
    IGrpcAuthenticator grpcAuthenticator,
    IOrganizationService organizationService,
    IOrganizationStripeConnectAccountService organizationStripeConnectAccountService,
    IOrganizationBankAccountService organizationBankAccountService,
    IOrganizationBillingService organizationBillingService,
    IOrganizationXeroConnectionService organizationXeroConnectionService,
    IGrpcMapper grpcMapper) : GrpcOrganizationBillingService.OrganizationBillingServiceBase
{
    public override async Task<StripeConnectAccountConnection> Admin_GetStripeConnectAccounts(
        Admin_GetStripeConnectAccountsInput request,
        ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);

        var (paginatedInfo, edges, totalCount) = await organizationStripeConnectAccountService.GetPaginatedAccountsAsync(
            new PaginationInputParam(request.After, request.First.FromNullInt(), request.Before, request.Last.FromNullInt()),
            new OrganizationStripeConnectAccountSearchCriteria(
                request.Where.OrganizationId,
                null,
                request.Where.NameContains,
                request.Where.OnboardingCompleted),
            [
                .. request.OrderBy.Select(item =>
                {
                    var direction = item.Direction == global::Api.Shared.Grpc.Skedular.Organization.Core.V1.OrderDirection.Ascending
                        ? OrderDirection.Ascending
                        : OrderDirection.Descending;
                    var field = item.Field switch
                    {
                        StripeConnectAccountOrderField.Name => OrganizationStripeConnectAccountOrderField.Name,
                        _ => throw new ArgumentOutOfRangeException(nameof(item.Field), item.Field,
                            $"Unexpected value for {nameof(item.Field)}: {item.Field}. Update enum mapping or caller input."),
                    };

                    return new OrganizationStripeConnectAccountOrder(direction, field);
                }),
            ],
            true,
            context.CancellationToken);

        var connection = new StripeConnectAccountConnection
        {
            PageInfo = new PageInfo
            {
                HasNextPage = paginatedInfo.HasNextPage,
                HasPreviousPage = paginatedInfo.HasPreviousPage,
                StartCursor = paginatedInfo.StartCursor.ToSafeString(),
                EndCursor = paginatedInfo.EndCursor.ToSafeString(),
            },
            TotalCount = totalCount,
        };

        connection.Edges.AddRange(edges.Select(grpcMapper.MapToGrpcResponse));
        return connection;
    }

    public override async Task<BankAccountConnection> Admin_GetBankAccounts(Admin_GetBankAccountsInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);

        var (paginatedInfo, edges, totalCount) = await organizationBankAccountService.GetPaginatedAccountsAsync(
            new PaginationInputParam(request.After, request.First.FromNullInt(), request.Before, request.Last.FromNullInt()),
            new OrganizationBankAccountSearchCriteria(request.Where.OrganizationId, null, request.Where.NameContains),
            [
                .. request.OrderBy.Select(item =>
                {
                    var direction = item.Direction == global::Api.Shared.Grpc.Skedular.Organization.Core.V1.OrderDirection.Ascending
                        ? OrderDirection.Ascending
                        : OrderDirection.Descending;
                    var field = item.Field switch
                    {
                        BankAccountOrderField.Name => OrganizationBankAccountOrderField.Name,
                        _ => throw new ArgumentOutOfRangeException(nameof(item.Field), item.Field,
                            $"Unexpected value for {nameof(item.Field)}: {item.Field}. Update enum mapping or caller input."),
                    };

                    return new OrganizationBankAccountOrder(direction, field);
                }),
            ],
            true,
            context.CancellationToken);

        var connection = new BankAccountConnection
        {
            PageInfo = new PageInfo
            {
                HasNextPage = paginatedInfo.HasNextPage,
                HasPreviousPage = paginatedInfo.HasPreviousPage,
                StartCursor = paginatedInfo.StartCursor.ToSafeString(),
                EndCursor = paginatedInfo.EndCursor.ToSafeString(),
            },
            TotalCount = totalCount,
        };

        connection.Edges.AddRange(edges.Select(grpcMapper.MapToGrpcResponse));
        return connection;
    }

    public override async Task<XeroConnection> Admin_GetXeroConnection(Admin_GetXeroConnectionInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);

        var organization = await organizationService.GetByIdOrCustomDomainAsync(
                               request.OrganizationId,
                               request.OrganizationCustomDomain,
                               true,
                               context.CancellationToken) ??
                           throw new OrganizationNotFound();

        return grpcMapper.MapToGrpcResponse(organization.OrganizationXeroConnection) ?? new XeroConnection();
    }

    public override async Task<global::Api.Shared.Grpc.Skedular.Organization.Core.V1.Organization> Admin_GetByXeroTenantId(
        Admin_GetByXeroTenantIdInput request,
        ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);

        var organization = await organizationService.GetByXeroTenantIdAsync(request.TenantId, context.CancellationToken);
        return grpcMapper.MapToGrpcResponse(organization ?? new Shared.Models.Organization());
    }

    public override async Task<XeroConnection> Admin_RefreshXeroConnectionTokens(
        Admin_RefreshXeroConnectionTokensInput request,
        ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);

        var xeroConnection = await organizationXeroConnectionService.RefreshTokensAsync(
            request.OrganizationId,
            request.AccessTokenEncrypted,
            request.RefreshTokenEncrypted,
            request.AccessTokenExpiresAt.ToDateTimeOffset(),
            request.RefreshTokenExpiresAt.ToDateTimeOffset(),
            context.CancellationToken);

        return grpcMapper.MapToGrpcResponse(xeroConnection) ?? new XeroConnection();
    }

    public override async Task<BillingDetails> GetBillingDetails(GetBillingDetailsInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);

        return grpcMapper.MapToGrpcResponse(await organizationBillingService.GetAsync(
            request.OrganizationId,
            null,
            context.CancellationToken));
    }

    public override async Task<BillingDetails> AddBillingDetails(AddBillingDetailsInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);
        var organization = await organizationBillingService.AddAsync(grpcMapper.MapTo(request), context.CancellationToken);

        return grpcMapper.MapToGrpcResponse(organization.BillingDetails);
    }

    public override async Task<BillingDetails> UpdateBillingDetails(UpdateBillingDetailsInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);
        var organization = await organizationBillingService.UpdatePatchAsync(grpcMapper.MapTo(request), context.CancellationToken);

        return grpcMapper.MapToGrpcResponse(organization.BillingDetails);
    }
}
