using Api.Shared.Grpc.Skedular.Organization.Core.V1;
using Api.Shared.Services;
using Api.Shared.Services.Configurations.Grpc;
using Enterprise.Shared;
using Enterprise.Shared.Grpc;
using Enterprise.Shared.Pagination;
using Enterprise.Shared.Version;
using Grpc.Core;
using Organization.Api.Mappers;
using Organization.Api.Services;
using Organization.Api.Services.Authorization;
using Organization.Shared.Models;
using OrderDirection = Enterprise.Shared.Pagination.OrderDirection;
using OrganizationService = Api.Shared.Grpc.Skedular.Organization.Core.V1.OrganizationService;
using TermsOfUse = Api.Shared.Grpc.Skedular.Organization.Core.V1.TermsOfUse;
using Version = Api.Shared.Grpc.Skedular.Organization.Core.V1.Version;
using Permissions = Api.Shared.Grpc.Skedular.Organization.Core.V1.Permissions;

namespace Organization.Api.Grpc;

public class OrganizationGrpcService(
    IVersionService versionService,
    OrganizationConfiguration organizationConfiguration,
    IGrpcAuthenticator grpcAuthenticator,
    IOrganizationTermsOfUseService organizationTermsOfUseService,
    IOrganizationService organizationService,
    IOrganizationMemberService organizationMemberService,
    IOrganizationAuthorizationService organizationAuthorizationService,
    IGrpcMapper grpcMapper) : OrganizationService.OrganizationServiceBase
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

    public override async Task<TermsOfUse> GetActiveOrganizationTermsOfUse(GetActiveOrganizationTermsOfUseInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);

        return grpcMapper.MapToGrpcResponse(await organizationTermsOfUseService.GetActiveTermsOfUseAsync(context.CancellationToken));
    }

    public override async Task<global::Api.Shared.Grpc.Skedular.Organization.Core.V1.Organization> Admin_Get(
        Admin_GetInput request,
        ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);

        var organization = await organizationService.GetByIdOrCustomDomainAsync(
                               request.Id,
                               request.CustomDomain,
                               true,
                               context.CancellationToken) ??
                           throw new OrganizationNotFound();

        return grpcMapper.MapToGrpcResponse(organization);
    }

    public override async Task<global::Api.Shared.Grpc.Skedular.Organization.Core.V1.Organization> Admin_Add(
        Admin_AddInput request,
        ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);

        return grpcMapper.MapToGrpcResponse(
            await organizationService.AddAsync(
                grpcMapper.MapTo(request),
                request.OfferingCode,
                true,
                context.CancellationToken));
    }

    public override async Task<global::Api.Shared.Grpc.Skedular.Organization.Core.V1.Organization> Get(
        GetInput request,
        ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);

        var organization = await organizationService.GetByIdOrCustomDomainAsync(
                               request.Id,
                               request.CustomDomain,
                               false,
                               context.CancellationToken) ??
                           throw new OrganizationNotFound();

        return grpcMapper.MapToGrpcResponse(organization);
    }

    public override async Task<global::Api.Shared.Grpc.Skedular.Organization.Core.V1.Organization> Admin_AddMember(
        Admin_AddMemberInput request,
        ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);

        return grpcMapper.MapToGrpcResponse(
            await organizationMemberService.AdminAddMemberAsync(request.Id, grpcMapper.MapTo(request), context.CancellationToken));
    }

    public override async Task<MemberConnection> GetPaginatedMembers(GetPaginatedMembersInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);

        var (paginatedInfo, edges, totalCount) = await organizationMemberService.GetPaginatedOrganizationMembersAsync(
            new PaginationInputParam(request.After, request.First.FromNullInt(), request.Before, request.Last.FromNullInt()),
            new OrganizationMemberSearchCriteria(request.Where.OrganizationId, null, request.Where.NameContains, request.Where.CustomerId),
            [
                .. request.OrderBy.Select(item =>
                {
                    var direction = item.Direction == global::Api.Shared.Grpc.Skedular.Organization.Core.V1.OrderDirection.Ascending
                        ? OrderDirection.Ascending
                        : OrderDirection.Descending;
                    var field = item.Field switch
                    {
                        MemberOrderField.RoleType => OrganizationMemberOrderField.Role,
                        MemberOrderField.Status => OrganizationMemberOrderField.Status,
                        MemberOrderField.Name => OrganizationMemberOrderField.Name,
                        MemberOrderField.GivenName => OrganizationMemberOrderField.GivenName,
                        MemberOrderField.MiddleName => OrganizationMemberOrderField.MiddleName,
                        MemberOrderField.FamilyName => OrganizationMemberOrderField.FamilyName,
                        MemberOrderField.PhoneNumber => OrganizationMemberOrderField.PhoneNumber,
                        _ => throw new ArgumentOutOfRangeException(nameof(item.Field), item.Field,
                            $"Unexpected value for {nameof(item.Field)}: {item.Field}. Update enum mapping or caller input."),
                    };

                    return new OrganizationMemberOrder(direction, field);
                }),
            ],
            context.CancellationToken);

        var connection = new MemberConnection
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

    public override async Task<Permissions> GetPermissions(GetPermissionsInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);

        var permissions = await organizationAuthorizationService.GetPermissionsAsync(request.Id, context.CancellationToken);
        return new Permissions
        {
            CanView = permissions.CanView,
            CanModify = permissions.CanModify,
            CanDelete = permissions.CanDelete,
            CanInvitePeople = permissions.CanInvitePeople,
            CanCancelPeopleExistingInvitations = permissions.CanCancelPeopleExistingInvitations,
            CanViewAnalytics = permissions.CanViewAnalytics,
        };
    }
}
