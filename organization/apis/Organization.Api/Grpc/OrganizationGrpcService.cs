using Api.Shared.Services;
using Api.Shared.Services.Configurations.Grpc;
using Api.Shared.Services.Grpc.Skedular.Organization.V1;
using Api.Shared.Services.Models;
using Enterprise.Shared;
using Enterprise.Shared.Grpc;
using Enterprise.Shared.Pagination;
using Enterprise.Shared.Version;
using Grpc.Core;
using HotChocolate.Subscriptions;
using Organization.Api.Mappers;
using Organization.Api.Services;
using Organization.Api.Services.Authorization;
using Organization.Shared.Models;
using OrderDirection = Enterprise.Shared.Pagination.OrderDirection;
using BillingDetails = Api.Shared.Services.Grpc.Skedular.Organization.V1.BillingDetails;
using OrganizationService = Api.Shared.Services.Grpc.Skedular.Organization.V1.OrganizationService;
using TermsOfUse = Api.Shared.Services.Grpc.Skedular.Organization.V1.TermsOfUse;
using Version = Api.Shared.Services.Grpc.Skedular.Organization.V1.Version;
using Permissions = Api.Shared.Services.Grpc.Skedular.Organization.V1.Permissions;
using Tag = Api.Shared.Services.Grpc.Skedular.Organization.V1.Tag;

namespace Organization.Api.Grpc;

public class OrganizationGrpcService(
    IVersionService versionService,
    OrganizationConfiguration organizationConfiguration,
    IGrpcAuthenticator grpcAuthenticator,
    IOrganizationTermsOfUseService organizationTermsOfUseService,
    IOrganizationService organizationService,
    IOrganizationMemberService organizationMemberService,
    IOrganizationAuthorizationService organizationAuthorizationService,
    IOrganizationStripeConnectAccountService organizationStripeConnectAccountService,
    IOrganizationBankAccountService organizationBankAccountService,
    ITagService tagService,
    IOrganizationBillingService organizationBillingService,
    IOrganizationXeroConnectionService organizationXeroConnectionService,
    IMapper mapper,
    ITopicEventSender topicEventSender) : OrganizationService.OrganizationServiceBase
{
    public override async Task<RaiseGraphqlChangeResponse> RaiseGraphqlChange(RaiseGraphqlChangeInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);

        await topicEventSender.SendAsync(request.TopicName, request.Id, context.CancellationToken);

        return new RaiseGraphqlChangeResponse();
    }

    public override Task<Version> GetVersion(VersionInput request, ServerCallContext context)
    {
        var version = versionService.GetVersion();

        return Task.FromResult(new Version { Major = version.Major, Minor = version.Minor, Build = version.Build, Revision = version.Revision });
    }

    public override async Task<TermsOfUse> GetActiveOrganizationTermsOfUse(GetActiveOrganizationTermsOfUseInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);

        return mapper.MapToGrpcResponse(await organizationTermsOfUseService.GetActiveTermsOfUseAsync(context.CancellationToken));
    }

    public override async Task<global::Api.Shared.Services.Grpc.Skedular.Organization.V1.Organization> Admin_Get(
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

        return mapper.MapToGrpcResponse(organization);
    }

    public override async Task<global::Api.Shared.Services.Grpc.Skedular.Organization.V1.Organization> Admin_Add(
        Admin_AddInput request,
        ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);

        return mapper.MapToGrpcResponse(
            await organizationService.AddAsync(
                mapper.MapTo(request),
                request.OfferingCode,
                true,
                context.CancellationToken));
    }

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
            request.OrderBy.Select(item =>
            {
                var direction = item.Direction == global::Api.Shared.Services.Grpc.Skedular.Organization.V1.OrderDirection.Ascending
                    ? OrderDirection.Ascending
                    : OrderDirection.Descending;
                var field = item.Field switch
                {
                    StripeConnectAccountOrderField.Name => OrganizationStripeConnectAccountOrderField.Name,
                    _ => throw new ArgumentOutOfRangeException()
                };

                return new OrganizationStripeConnectAccountOrder(direction, field);
            }).ToList(),
            true,
            context.CancellationToken);

        var connection = new StripeConnectAccountConnection
        {
            PageInfo = new PageInfo
            {
                HasNextPage = paginatedInfo.HasNextPage,
                HasPreviousPage = paginatedInfo.HasPreviousPage,
                StartCursor = paginatedInfo.StartCursor.ToSafeString(),
                EndCursor = paginatedInfo.EndCursor.ToSafeString()
            },
            TotalCount = totalCount
        };

        connection.Edges.AddRange(edges.Select(mapper.MapToGrpcResponse));
        return connection;
    }

    public override async Task<BankAccountConnection> Admin_GetBankAccounts(Admin_GetBankAccountsInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);

        var (paginatedInfo, edges, totalCount) = await organizationBankAccountService.GetPaginatedAccountsAsync(
            new PaginationInputParam(request.After, request.First.FromNullInt(), request.Before, request.Last.FromNullInt()),
            new OrganizationBankAccountSearchCriteria(request.Where.OrganizationId, null, request.Where.NameContains),
            request.OrderBy.Select(item =>
            {
                var direction = item.Direction == global::Api.Shared.Services.Grpc.Skedular.Organization.V1.OrderDirection.Ascending
                    ? OrderDirection.Ascending
                    : OrderDirection.Descending;
                var field = item.Field switch
                {
                    BankAccountOrderField.Name => OrganizationBankAccountOrderField.Name,
                    _ => throw new ArgumentOutOfRangeException()
                };

                return new OrganizationBankAccountOrder(direction, field);
            }).ToList(),
            true,
            context.CancellationToken);

        var connection = new BankAccountConnection
        {
            PageInfo = new PageInfo
            {
                HasNextPage = paginatedInfo.HasNextPage,
                HasPreviousPage = paginatedInfo.HasPreviousPage,
                StartCursor = paginatedInfo.StartCursor.ToSafeString(),
                EndCursor = paginatedInfo.EndCursor.ToSafeString()
            },
            TotalCount = totalCount
        };

        connection.Edges.AddRange(edges.Select(mapper.MapToGrpcResponse));
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

        return mapper.MapToGrpcResponse(organization.OrganizationXeroConnection) ?? new XeroConnection();
    }

    public override async Task<global::Api.Shared.Services.Grpc.Skedular.Organization.V1.Organization> Admin_GetByXeroTenantId(
        Admin_GetByXeroTenantIdInput request,
        ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);

        var organization = await organizationService.GetByXeroTenantIdAsync(request.TenantId, context.CancellationToken);
        return mapper.MapToGrpcResponse(organization ?? new Shared.Models.Organization());
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

        return mapper.MapToGrpcResponse(xeroConnection) ?? new XeroConnection();
    }

    public override async Task<global::Api.Shared.Services.Grpc.Skedular.Organization.V1.Organization> Get(
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

        return mapper.MapToGrpcResponse(organization);
    }

    public override async Task<global::Api.Shared.Services.Grpc.Skedular.Organization.V1.Organization> Admin_AddMember(
        Admin_AddMemberInput request,
        ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);

        return mapper.MapToGrpcResponse(
            await organizationMemberService.AdminAddMemberAsync(request.Id, mapper.MapTo(request), context.CancellationToken));
    }

    public override async Task<MemberConnection> GetPaginatedMembers(GetPaginatedMembersInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);

        var (paginatedInfo, edges, totalCount) = await organizationMemberService.GetPaginatedOrganizationMembersAsync(
            new PaginationInputParam(request.After, request.First.FromNullInt(), request.Before, request.Last.FromNullInt()),
            new OrganizationMemberSearchCriteria(request.Where.OrganizationId, null, request.Where.NameContains, request.Where.CustomerId),
            request.OrderBy.Select(item =>
            {
                var direction = item.Direction == global::Api.Shared.Services.Grpc.Skedular.Organization.V1.OrderDirection.Ascending
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
                    _ => throw new ArgumentOutOfRangeException()
                };

                return new OrganizationMemberOrder(direction, field);
            }).ToList(),
            context.CancellationToken);

        var connection = new MemberConnection
        {
            PageInfo = new PageInfo
            {
                HasNextPage = paginatedInfo.HasNextPage,
                HasPreviousPage = paginatedInfo.HasPreviousPage,
                StartCursor = paginatedInfo.StartCursor.ToSafeString(),
                EndCursor = paginatedInfo.EndCursor.ToSafeString()
            },
            TotalCount = totalCount
        };

        connection.Edges.AddRange(edges.Select(mapper.MapToGrpcResponse));
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
            CanViewAnalytics = permissions.CanViewAnalytics
        };
    }

    public override async Task<TagConnection> GetPaginatedTags(GetPaginatedTagsInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);

        var (paginatedInfo, edges, totalCount) = await tagService.GetPaginatedTagsAsync(
            new PaginationInputParam(request.After, request.First.FromNullInt(), request.Before, request.Last.FromNullInt()),
            new TagSearchCriteria(request.Where.OrganizationId, null, request.Where.Types_, request.Where.NameContains),
            request.OrderBy.Select(item =>
            {
                var direction = item.Direction == global::Api.Shared.Services.Grpc.Skedular.Organization.V1.OrderDirection.Ascending
                    ? OrderDirection.Ascending
                    : OrderDirection.Descending;
                var field = item.Field switch
                {
                    TagOrderField.Name => OrganizationTagOrderField.Name,
                    TagOrderField.Description => OrganizationTagOrderField.Description,
                    _ => throw new ArgumentOutOfRangeException()
                };

                return new TagOrder(direction, field);
            }).ToList(),
            false,
            context.CancellationToken);

        var connection = new TagConnection
        {
            PageInfo = new PageInfo
            {
                HasNextPage = paginatedInfo.HasNextPage,
                HasPreviousPage = paginatedInfo.HasPreviousPage,
                StartCursor = paginatedInfo.StartCursor.ToSafeString(),
                EndCursor = paginatedInfo.EndCursor.ToSafeString()
            },
            TotalCount = totalCount
        };

        connection.Edges.AddRange(edges.Select(mapper.MapToGrpcResponseTag));
        return connection;
    }

    public override async Task<Tag> Admin_GetTag(Admin_GetTagInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);

        return mapper.MapToGrpcResponseTag(await tagService.GetByIdAsync(request.Id, true, context.CancellationToken));
    }

    public override async Task<TagConnection> Admin_GetPaginatedTags(Admin_GetPaginatedTagsInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);

        var (paginatedInfo, edges, totalCount) = await tagService.GetPaginatedTagsAsync(
            new PaginationInputParam(request.After, request.First.FromNullInt(), request.Before, request.Last.FromNullInt()),
            new TagSearchCriteria(request.Where.OrganizationId, null, request.Where.Types_, request.Where.NameContains),
            request.OrderBy.Select(item =>
            {
                var direction = item.Direction == global::Api.Shared.Services.Grpc.Skedular.Organization.V1.OrderDirection.Ascending
                    ? OrderDirection.Ascending
                    : OrderDirection.Descending;
                var field = item.Field switch
                {
                    TagOrderField.Name => OrganizationTagOrderField.Name,
                    TagOrderField.Description => OrganizationTagOrderField.Description,
                    _ => throw new ArgumentOutOfRangeException()
                };

                return new TagOrder(direction, field);
            }).ToList(),
            true,
            context.CancellationToken);

        var connection = new TagConnection
        {
            PageInfo = new PageInfo
            {
                HasNextPage = paginatedInfo.HasNextPage,
                HasPreviousPage = paginatedInfo.HasPreviousPage,
                StartCursor = paginatedInfo.StartCursor.ToSafeString(),
                EndCursor = paginatedInfo.EndCursor.ToSafeString()
            },
            TotalCount = totalCount
        };

        connection.Edges.AddRange(edges.Select(mapper.MapToGrpcResponseTag));
        return connection;
    }

    public override async Task<Tag> GetTag(GetTagInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);

        return mapper.MapToGrpcResponseTag(await tagService.GetByIdAsync(request.Id, false, context.CancellationToken));
    }

    public override async Task<Tag> AddTag(AddTagInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);

        return mapper.MapToGrpcResponseTag(await tagService.AddAsync(mapper.MapTo(request), false, context.CancellationToken));
    }

    public override async Task<Tag> UpdateTag(UpdateTagInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);

        return mapper.MapToGrpcResponseTag(await tagService.UpdateAsync(mapper.MapTo(request), context.CancellationToken));
    }

    public override async Task<Tag> RemoveTag(RemoveTagInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);

        return mapper.MapToGrpcResponseTag(await tagService.DeleteAsync(request.Id, context.CancellationToken));
    }

    public override async Task<CustomTagConnection> GetPaginatedCustomTags(GetPaginatedCustomTagsInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);

        var (paginatedInfo, edges, totalCount) = await tagService.GetPaginatedTagsAsync(
            new PaginationInputParam(request.After, request.First.FromNullInt(), request.Before, request.Last.FromNullInt()),
            new TagSearchCriteria(request.Where.OrganizationId, null, [OrganizationTagTypeConstants.Custom], request.Where.NameContains),
            request.OrderBy.Select(item =>
            {
                var direction = item.Direction == global::Api.Shared.Services.Grpc.Skedular.Organization.V1.OrderDirection.Ascending
                    ? OrderDirection.Ascending
                    : OrderDirection.Descending;
                var field = item.Field switch
                {
                    CustomTagOrderField.Name => OrganizationTagOrderField.Name,
                    CustomTagOrderField.Description => OrganizationTagOrderField.Description,
                    _ => throw new ArgumentOutOfRangeException()
                };

                return new TagOrder(direction, field);
            }).ToList(),
            false,
            context.CancellationToken);

        var connection = new CustomTagConnection
        {
            PageInfo = new PageInfo
            {
                HasNextPage = paginatedInfo.HasNextPage,
                HasPreviousPage = paginatedInfo.HasPreviousPage,
                StartCursor = paginatedInfo.StartCursor.ToSafeString(),
                EndCursor = paginatedInfo.EndCursor.ToSafeString()
            },
            TotalCount = totalCount
        };

        connection.Edges.AddRange(edges.Select(mapper.MapToGrpcResponseCustomTag));
        return connection;
    }

    public override async Task<CustomTag> GetCustomTag(GetCustomTagInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);

        return mapper.MapToGrpcResponseCustomTag(await tagService.GetByIdAsync(request.Id, false, context.CancellationToken));
    }

    public override async Task<CustomTag> Admin_GetCustomTag(Admin_GetCustomTagInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);

        return mapper.MapToGrpcResponseCustomTag(await tagService.GetByIdAsync(request.Id, true, context.CancellationToken));
    }

    public override async Task<CustomTag> AddCustomTag(AddCustomTagInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);

        return mapper.MapToGrpcResponseCustomTag(await tagService.AddAsync(mapper.MapTo(request), false, context.CancellationToken));
    }

    public override async Task<CustomTag> UpdateCustomTag(UpdateCustomTagInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);

        return mapper.MapToGrpcResponseCustomTag(await tagService.UpdateAsync(mapper.MapTo(request), context.CancellationToken));
    }

    public override async Task<CustomTag> RemoveCustomTag(RemoveCustomTagInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);

        return mapper.MapToGrpcResponseCustomTag(await tagService.DeleteAsync(request.Id, context.CancellationToken));
    }

    public override async Task<ZoneConnection> GetPaginatedZones(GetPaginatedZonesInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);

        var (paginatedInfo, edges, totalCount) = await tagService.GetPaginatedTagsAsync(
            new PaginationInputParam(request.After, request.First.FromNullInt(), request.Before, request.Last.FromNullInt()),
            new TagSearchCriteria(request.Where.OrganizationId, null, [OrganizationTagTypeConstants.Zone], request.Where.NameContains),
            request.OrderBy.Select(item =>
            {
                var direction = item.Direction == global::Api.Shared.Services.Grpc.Skedular.Organization.V1.OrderDirection.Ascending
                    ? OrderDirection.Ascending
                    : OrderDirection.Descending;
                var field = item.Field switch
                {
                    ZoneOrderField.Name => OrganizationTagOrderField.Name,
                    ZoneOrderField.Description => OrganizationTagOrderField.Description,
                    _ => throw new ArgumentOutOfRangeException()
                };

                return new TagOrder(direction, field);
            }).ToList(),
            false,
            context.CancellationToken);

        var connection = new ZoneConnection
        {
            PageInfo = new PageInfo
            {
                HasNextPage = paginatedInfo.HasNextPage,
                HasPreviousPage = paginatedInfo.HasPreviousPage,
                StartCursor = paginatedInfo.StartCursor.ToSafeString(),
                EndCursor = paginatedInfo.EndCursor.ToSafeString()
            },
            TotalCount = totalCount
        };

        connection.Edges.AddRange(edges.Select(mapper.MapToGrpcResponseZone));
        return connection;
    }

    public override async Task<Zone> Admin_GetZone(Admin_GetZoneInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);

        return mapper.MapToGrpcResponseZone(await tagService.GetByIdAsync(request.Id, true, context.CancellationToken));
    }

    public override async Task<Zone> GetZone(GetZoneInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);

        return mapper.MapToGrpcResponseZone(await tagService.GetByIdAsync(request.Id, false, context.CancellationToken));
    }

    public override async Task<Zone> AddZone(AddZoneInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);

        return mapper.MapToGrpcResponseZone(await tagService.AddAsync(mapper.MapTo(request), false, context.CancellationToken));
    }

    public override async Task<Zone> UpdateZone(UpdateZoneInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);

        return mapper.MapToGrpcResponseZone(await tagService.UpdateAsync(mapper.MapTo(request), context.CancellationToken));
    }

    public override async Task<Zone> RemoveZone(RemoveZoneInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);

        return mapper.MapToGrpcResponseZone(await tagService.DeleteAsync(request.Id, context.CancellationToken));
    }

    public override async Task<BillingDetails> GetBillingDetails(GetBillingDetailsInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);

        return mapper.MapToGrpcResponse(await organizationBillingService.GetAsync(
            request.OrganizationId,
            null,
            context.CancellationToken));
    }

    public override async Task<BillingDetails> AddBillingDetails(AddBillingDetailsInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);
        var organization = await organizationBillingService.AddAsync(mapper.MapTo(request), context.CancellationToken);

        return mapper.MapToGrpcResponse(organization.BillingDetails);
    }

    public override async Task<BillingDetails> UpdateBillingDetails(UpdateBillingDetailsInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);
        var organization = await organizationBillingService.UpdateAsync(mapper.MapTo(request), context.CancellationToken);

        return mapper.MapToGrpcResponse(organization.BillingDetails);
    }

    public override async Task<ProductTagConnection> GetPaginatedProductTags(GetPaginatedProductTagsInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);

        var (paginatedInfo, edges, totalCount) = await tagService.GetPaginatedTagsAsync(
            new PaginationInputParam(request.After, request.First.FromNullInt(), request.Before, request.Last.FromNullInt()),
            new TagSearchCriteria(request.Where.OrganizationId, null, [OrganizationTagTypeConstants.Product], request.Where.NameContains),
            request.OrderBy.Select(item =>
            {
                var direction = item.Direction == global::Api.Shared.Services.Grpc.Skedular.Organization.V1.OrderDirection.Ascending
                    ? OrderDirection.Ascending
                    : OrderDirection.Descending;
                var field = item.Field switch
                {
                    ProductTagOrderField.Name => OrganizationTagOrderField.Name,
                    ProductTagOrderField.Description => OrganizationTagOrderField.Description,
                    _ => throw new ArgumentOutOfRangeException()
                };

                return new TagOrder(direction, field);
            }).ToList(),
            false,
            context.CancellationToken);

        var connection = new ProductTagConnection
        {
            PageInfo = new PageInfo
            {
                HasNextPage = paginatedInfo.HasNextPage,
                HasPreviousPage = paginatedInfo.HasPreviousPage,
                StartCursor = paginatedInfo.StartCursor.ToSafeString(),
                EndCursor = paginatedInfo.EndCursor.ToSafeString()
            },
            TotalCount = totalCount
        };

        connection.Edges.AddRange(edges.Select(mapper.MapToGrpcResponseProductTag));
        return connection;
    }

    public override async Task<ProductTag> GetProductTag(GetProductTagInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);

        return mapper.MapToGrpcResponseProductTag(await tagService.GetByIdAsync(request.Id, false, context.CancellationToken));
    }

    public override async Task<ProductTag> Admin_GetProductTag(Admin_GetProductTagInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);

        return mapper.MapToGrpcResponseProductTag(await tagService.GetByIdAsync(request.Id, true, context.CancellationToken));
    }

    public override async Task<ProductTag> AddProductTag(AddProductTagInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);

        return mapper.MapToGrpcResponseProductTag(await tagService.AddAsync(mapper.MapTo(request), false, context.CancellationToken));
    }

    public override async Task<ProductTag> UpdateProductTag(UpdateProductTagInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);

        return mapper.MapToGrpcResponseProductTag(await tagService.UpdateAsync(mapper.MapTo(request), context.CancellationToken));
    }

    public override async Task<ProductTag> RemoveProductTag(RemoveProductTagInput request, ServerCallContext context)
    {
        grpcAuthenticator.VerifyAndEnrich(organizationConfiguration.ApiKey);

        return mapper.MapToGrpcResponseProductTag(await tagService.DeleteAsync(request.Id, context.CancellationToken));
    }
}
