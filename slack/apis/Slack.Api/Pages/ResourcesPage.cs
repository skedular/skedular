using Api.Shared.Services.Grpc.Skedular.Customer.V1;
using Api.Shared.Services.Grpc.Skedular.Location.V1;
using Api.Shared.Services.Grpc.Skedular.Organization.V1;
using Enterprise.Shared;
using Enterprise.Shared.Exceptions;
using Enterprise.Shared.Grpc;
using Slack.Api.Components;
using Slack.Api.Mappers;
using Slack.Api.Services;
using Slack.Shared;
using Slack.Shared.Configurations;
using Slack.Shared.Constants;
using Slack.Shared.Context;
using Slack.Shared.Repositories;
using SlackNet;
using SlackNet.AspNetCore;
using SlackNet.Blocks;
using SlackNet.Interaction;
using OrganizationService = Api.Shared.Services.Grpc.Skedular.Organization.V1.OrganizationService;
using Button = SlackNet.Blocks.Button;
using CustomerService = Api.Shared.Services.Grpc.Skedular.Customer.V1.CustomerService;
using Icons = Slack.Shared.Constants.Icons;
using LocationService = Api.Shared.Services.Grpc.Skedular.Location.V1.LocationService;
using Option = SlackNet.Blocks.Option;
using OrderDirection = Api.Shared.Services.Grpc.Skedular.Location.V1.OrderDirection;
using Workspace = Slack.Shared.Models.Workspace;
using WorkspaceMember = Slack.Shared.Models.WorkspaceMember;

namespace Slack.Api.Pages;

public interface IResourcesPage
{
    Task RenderWithContextAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        CommonPageContext commonPageContext,
        string? hash,
        CancellationToken cancellationToken);
}

public class ResourcesPage(
    AsyncPageRenderingService asyncPageRenderingService,
    SlackConfiguration slackConfiguration,
    LocationConfiguration locationConfiguration,
    LocationService.LocationServiceClient locationServiceClient,
    CustomerConfiguration customerConfiguration,
    CustomerService.CustomerServiceClient customerServiceClient,
    OrganizationConfiguration organizationConfiguration,
    OrganizationService.OrganizationServiceClient organizationServiceClient,
    IRepositoryFactory repositoryFactory,
    IWorkspaceMemberService workspaceMemberService,
    IBookingsPage bookingsPage,
    IBookingService bookingService,
    ILocationService locationService,
    IResourceComponents resourceComponents,
    ICommonComponents commonComponents,
    IMapper mapper,
    IBookingsPageContextService bookingsPageContextService) :
    IResourcesPage,
    IAsyncPageRenderingCallbacks,
    IBlockActionHandler<StaticSelectAction>,
    IBlockActionHandler<ButtonAction>
{
    private const int ResourcesPageSize = 5;
    private const string ResourcesCallback = "Resources";
    private const string FirstPageResources = "Resources_FirstPageResources";
    private const string PreviousPageResources = "Resources_PreviousPageResources";
    private const string NextPageResources = "Resources_NextPageResources";
    private const string LastPageResources = "Resources_LastPageResources";

    public async Task HandleAsync(ButtonAction action, BlockActionRequest request, CancellationToken cancellationToken)
    {
        var workspaceEntity = await repositoryFactory.WorkspaceRepository.GetByIdAsync(request.Team.Id, cancellationToken);
        if (workspaceEntity is null)
        {
            throw new SlackWorkspaceNotFound();
        }

        var (workspaceMemberEntity, _) = await workspaceMemberService.EnsureCustomerResourcesAllExistAsync(
            workspaceEntity,
            request.User.Id,
            cancellationToken);

        var workspace = mapper.MapTo(workspaceEntity);
        var workspaceMember = mapper.MapTo(workspaceMemberEntity, workspace);

        switch (action.ActionId)
        {
            case FirstPageResources:
                await RenderFirstPageAsync(
                    workspace,
                    workspaceMember,
                    CommonPageContext.Deserialize(action.Value),
                    request.View.Hash,
                    cancellationToken);
                break;

            case PreviousPageResources:
                await RenderPreviousPageAsync(
                    workspace,
                    workspaceMember,
                    CommonPageContext.Deserialize(action.Value),
                    request.View.Hash,
                    cancellationToken);
                break;

            case NextPageResources:
                await RenderNextPageAsync(
                    workspace,
                    workspaceMember,
                    CommonPageContext.Deserialize(action.Value),
                    request.View.Hash,
                    cancellationToken);
                break;

            case LastPageResources:
                await RenderLastPageAsync(
                    workspace,
                    workspaceMember,
                    CommonPageContext.Deserialize(action.Value),
                    request.View.Hash,
                    cancellationToken);
                break;

            case ResourceActionTypes.SetPreferredResource:
                await AddPreferredResourceAsync(
                    workspace,
                    workspaceMember,
                    SetPreferredResourceContext.Deserialize(action.Value),
                    request.View.Hash,
                    cancellationToken);
                break;

            case ResourceActionTypes.RemovePreferredResource:
                await RemovePreferredResourceAsync(
                    workspace,
                    workspaceMember,
                    RemovePreferredResourceContext.Deserialize(action.Value),
                    request.View.Hash,
                    cancellationToken);
                break;
        }
    }

    public async Task HandleAsync(StaticSelectAction action, BlockActionRequest request, CancellationToken cancellationToken)
    {
        var workspaceEntity = await repositoryFactory.WorkspaceRepository.GetByIdAsync(request.Team.Id, cancellationToken);
        if (workspaceEntity is null)
        {
            throw new SlackWorkspaceNotFound();
        }

        var (workspaceMemberEntity, _) = await workspaceMemberService.EnsureCustomerResourcesAllExistAsync(
            workspaceEntity,
            request.User.Id,
            cancellationToken);

        var workspace = mapper.MapTo(workspaceEntity);
        var workspaceMember = mapper.MapTo(workspaceMemberEntity, workspace);

        if (action.SelectedOption.Value.StartsWith(BookingActionTypes.Bookings))
        {
            var locationId = action.SelectedOption.Value[BookingActionTypes.Bookings.Length..];
            var bookingPermissions = await bookingService.GetOrganizationPermissionsAsync(workspace, workspaceMember, cancellationToken);
            if (!bookingPermissions.CanViewBookings)
            {
                throw new Unauthorized();
            }

            var context = CommonPageContext.Deserialize(request.View.PrivateMetadata);
            context.PageContext.BookingsPage = bookingsPageContextService.GetDefaultBookingsPageContext();
            context.PageContext.BookingsPage.LocationIds = [locationId];
            context.PageContext.PushCurrentPageToVisitedPages();

            await bookingsPage.RenderWithContextAsync(
                workspace,
                workspaceMember,
                new CommonPageContext(context.PageContext),
                request.View.Hash,
                cancellationToken);
        }
        else if (action.SelectedOption.Value.StartsWith(ResourceActionTypes.EditResource))
        {
            var context = EditResourceContext.Deserialize(request.View.PrivateMetadata);
            ArgumentNullException.ThrowIfNull(context.PageContext.ResourcesPage);

            var deskId = action.SelectedOption.Value[ResourceActionTypes.EditResource.Length..];
            var permissions =
                await locationService.GetPermissionsAsync(context.PageContext.ResourcesPage.LocationId, workspaceMember, cancellationToken);
            if (!permissions.CanModify)
            {
                throw new Unauthorized();
            }

            context.PageContext.PushCurrentPageToVisitedPages();
            context.LocationId = context.PageContext.ResourcesPage.LocationId;
            context.ResourceId = deskId;

            await OpenEditDeskDialogAsync(
                workspace,
                workspaceMember,
                request.TriggerId,
                context,
                cancellationToken);
        }
        else if (action.SelectedOption.Value.StartsWith(ResourceActionTypes.RemoveResource))
        {
            var context = RemoveResourceContext.Deserialize(request.View.PrivateMetadata);
            ArgumentNullException.ThrowIfNull(context.PageContext.ResourcesPage);

            var deskId = action.SelectedOption.Value[ResourceActionTypes.RemoveResource.Length..];
            var permissions =
                await locationService.GetPermissionsAsync(context.PageContext.ResourcesPage.LocationId, workspaceMember, cancellationToken);
            if (!permissions.CanDelete)
            {
                throw new Unauthorized();
            }

            context.PageContext.PushCurrentPageToVisitedPages();
            context.LocationId = context.PageContext.ResourcesPage.LocationId;
            context.ResourceId = deskId;

            await OpenRemoveResourceDialogAsync(
                workspace,
                workspaceMember,
                request.TriggerId,
                context,
                cancellationToken);
        }
    }

    public async Task Handle(ButtonAction action, BlockActionRequest request)
    {
        if (slackConfiguration.EnableAsyncMode)
        {
            asyncPageRenderingService.ButtonActionHandlerStream.OnNext((GetType(), action, request));
        }
        else
        {
            await HandleAsync(action, request, CancellationToken.None);
        }
    }

    public async Task Handle(StaticSelectAction action, BlockActionRequest request)
    {
        if (slackConfiguration.EnableAsyncMode)
        {
            asyncPageRenderingService.StaticSelectActionHandlerStream.OnNext((GetType(), action, request));
        }
        else
        {
            await HandleAsync(action, request, CancellationToken.None);
        }
    }

    public async Task RenderWithContextAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        CommonPageContext commonPageContext,
        string? hash,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(commonPageContext.PageContext.ResourcesPage);
        if (commonPageContext.PageContext.ResourcesPage.Pagination.IsEmpty())
        {
            await RenderFirstPageAsync(workspace, workspaceMember, commonPageContext, hash, cancellationToken);
        }
        else
        {
            await RenderInternalAsync(
                workspace,
                workspaceMember,
                commonPageContext.PageContext.ResourcesPage.Pagination.CurrentAfter,
                commonPageContext.PageContext.ResourcesPage.Pagination.CurrentFirst,
                commonPageContext.PageContext.ResourcesPage.Pagination.CurrentBefore,
                commonPageContext.PageContext.ResourcesPage.Pagination.CurrentLast,
                commonPageContext,
                hash,
                cancellationToken);
        }
    }

    private async Task RenderFirstPageAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        CommonPageContext commonPageContext,
        string? hash,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(commonPageContext.PageContext.ResourcesPage);
        await RenderInternalAsync(
            workspace,
            workspaceMember,
            null,
            ResourcesPageSize,
            null,
            null,
            commonPageContext,
            hash,
            cancellationToken);
    }

    private async Task RenderPreviousPageAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        CommonPageContext commonPageContext,
        string? hash,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(commonPageContext.PageContext.ResourcesPage);
        await RenderInternalAsync(
            workspace,
            workspaceMember,
            null,
            null,
            commonPageContext.PageContext.ResourcesPage.Pagination.Before,
            ResourcesPageSize,
            commonPageContext,
            hash,
            cancellationToken);
    }

    private async Task RenderNextPageAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        CommonPageContext commonPageContext,
        string? hash,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(commonPageContext.PageContext.ResourcesPage);
        await RenderInternalAsync(
            workspace,
            workspaceMember,
            commonPageContext.PageContext.ResourcesPage.Pagination.After,
            ResourcesPageSize,
            null,
            null,
            commonPageContext,
            hash,
            cancellationToken);
    }

    private async Task RenderLastPageAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        CommonPageContext commonPageContext,
        string? hash,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(commonPageContext.PageContext.ResourcesPage);
        await RenderInternalAsync(
            workspace,
            workspaceMember,
            null,
            null,
            null,
            ResourcesPageSize,
            commonPageContext,
            hash,
            cancellationToken);
    }

    private async Task RenderInternalAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        string? after,
        int? first,
        string? before,
        int? last,
        CommonPageContext commonPageContext,
        string? hash,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(commonPageContext.PageContext.ResourcesPage);

        commonPageContext.PageContext.CurrentPageType = PageType.Desks;

        var resourceConnection = await GetPaginatedResourcesAsync(
            workspaceMember,
            after,
            first,
            before,
            last,
            commonPageContext,
            cancellationToken);
        var resources = resourceConnection.Edges.Select(item => mapper.MapTo(item.Node)).ToList();
        var asyncBlocks = await Task.WhenAll(
            GetToolbarAsync(
                commonPageContext.PageContext.ResourcesPage.LocationId,
                workspaceMember,
                commonPageContext.PageContext,
                cancellationToken),
            resourceComponents.GetResourceCardsAsync(
                commonPageContext.PageContext.ResourcesPage.LocationId,
                workspaceMember,
                resources,
                commonPageContext.PageContext,
                cancellationToken));

        ICollection<Block>[] blocks =
        [
            GetTitle(),
            asyncBlocks[0],
            GetResourcessSearchCriteriaAndPaginationBlocks(resourceConnection, commonPageContext.PageContext),
            asyncBlocks[1]
        ];

        var slackApiClient = workspace.GetApiClient();
        await slackApiClient.ViewsPublishAsync(
            workspaceMember.Id,
            new HomeViewDefinition
            {
                CallbackId = ResourcesCallback,
                Blocks = blocks.SelectMany(item => item.Count == 0 ? item : item.Concat([new DividerBlock()])).SkipLast(1).ToList(),
                PrivateMetadata = commonPageContext.Serialize()
            },
            hash,
            cancellationToken);
    }

    public static void RegisterHandlers(AspNetSlackServiceConfiguration options) =>
        options
            .RegisterBlockActionHandler<StaticSelectAction, ResourcesPage>(ResourceActionTypes.ActionsMenu)
            .RegisterBlockActionHandler<ButtonAction, ResourcesPage>(FirstPageResources)
            .RegisterBlockActionHandler<ButtonAction, ResourcesPage>(LastPageResources)
            .RegisterBlockActionHandler<ButtonAction, ResourcesPage>(NextPageResources)
            .RegisterBlockActionHandler<ButtonAction, ResourcesPage>(PreviousPageResources)
            .RegisterBlockActionHandler<ButtonAction, ResourcesPage>(ResourceActionTypes.SetPreferredResource)
            .RegisterBlockActionHandler<ButtonAction, ResourcesPage>(ResourceActionTypes.RemovePreferredResource);

    private static ICollection<Block> GetTitle() =>
    [
        new SectionBlock { Text = "*Resources*".ToMarkdown() }
    ];

    private async Task<ICollection<Block>> GetToolbarAsync(
        string locationId,
        WorkspaceMember workspaceMember,
        PageContext pageContext,
        CancellationToken cancellationToken)
    {
        var homeAndBackButtons = commonComponents.GetHomeAndBackButtons(pageContext, workspaceMember.Timezone);
        var addDeskButton = await resourceComponents.GetAddResourceButtonAsync(locationId, workspaceMember, pageContext, cancellationToken);
        var feedbackButton = commonComponents.GetFeedbackButton(pageContext);

        return
        [
            new ActionsBlock
            {
                Elements = new List<IActionElement>().Concat(homeAndBackButtons).Concat(addDeskButton).Concat(feedbackButton).ToList()
            }
        ];
    }

    private async Task<ResourceConnection> GetPaginatedResourcesAsync(
        WorkspaceMember workspaceMember,
        string? after,
        int? first,
        string? before,
        int? last,
        CommonPageContext commonPageContext,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(commonPageContext.PageContext.ResourcesPage);
        var getPaginatedResourcesInput = new GetPaginatedResourcesInput
        {
            After = after.ToSafeString(),
            First = first.ToNullInt(),
            Before = before.ToSafeString(),
            Last = last.ToNullInt(),
            Where = new ResourceWhereInput { LocationId = commonPageContext.PageContext.ResourcesPage.LocationId }
        };

        getPaginatedResourcesInput.OrderBy.AddRange([
            new ResourceOrderInput { Direction = OrderDirection.Ascending, Field = ResourceOrderField.ResourceName }
        ]);

        return await locationServiceClient.GetPaginatedResourcesAsync(
            getPaginatedResourcesInput,
            locationConfiguration.ApiKey.CreateMetadata(workspaceMember.Id),
            cancellationToken: cancellationToken);
    }

    private static List<Block> GetResourcessSearchCriteriaAndPaginationBlocks(ResourceConnection resourceConnection, PageContext pageContext)
    {
        if (resourceConnection.Edges.Count == 0)
        {
            return [new SectionBlock { Text = "No resource found".ToMarkdown() }];
        }

        var totalDesksCount = new SectionBlock { Text = $"Total resources: {resourceConnection.TotalCount}".ToMarkdown() };
        if (resourceConnection.TotalCount <= ResourcesPageSize)
        {
            return [totalDesksCount];
        }

        pageContext = pageContext.Clone();
        ArgumentNullException.ThrowIfNull(pageContext.ResourcesPage);

        var paginationButtons = new List<IActionElement>();
        if (resourceConnection.PageInfo.HasPreviousPage)
        {
            pageContext.ResourcesPage.Pagination.First = ResourcesPageSize;
            pageContext.ResourcesPage.Pagination.After = null;
            pageContext.ResourcesPage.Pagination.Before = null;
            pageContext.ResourcesPage.Pagination.Last = null;

            paginationButtons.Add(new Button
            {
                ActionId = FirstPageResources, Text = Icons.FirstPage.ToPlainText(), Value = new CommonPageContext(pageContext).Serialize()
            });

            pageContext.ResourcesPage.Pagination.First = null;
            pageContext.ResourcesPage.Pagination.After = null;
            pageContext.ResourcesPage.Pagination.Before = resourceConnection.PageInfo.StartCursor;
            pageContext.ResourcesPage.Pagination.Last = ResourcesPageSize;

            paginationButtons.Add(new Button
            {
                ActionId = PreviousPageResources, Text = Icons.PreviousPage.ToPlainText(), Value = new CommonPageContext(pageContext).Serialize()
            });
        }

        if (resourceConnection.PageInfo.HasNextPage)
        {
            pageContext.ResourcesPage.Pagination.First = ResourcesPageSize;
            pageContext.ResourcesPage.Pagination.After = resourceConnection.PageInfo.EndCursor;
            pageContext.ResourcesPage.Pagination.Before = null;
            pageContext.ResourcesPage.Pagination.Last = null;

            paginationButtons.Add(new Button
            {
                ActionId = NextPageResources, Text = Icons.NextPage.ToPlainText(), Value = new CommonPageContext(pageContext).Serialize()
            });

            pageContext.ResourcesPage.Pagination.First = null;
            pageContext.ResourcesPage.Pagination.After = null;
            pageContext.ResourcesPage.Pagination.Before = null;
            pageContext.ResourcesPage.Pagination.Last = ResourcesPageSize;

            paginationButtons.Add(new Button
            {
                ActionId = LastPageResources, Text = Icons.LastPage.ToPlainText(), Value = new CommonPageContext(pageContext).Serialize()
            });
        }

        var paginationActionBlock = new ActionsBlock { Elements = paginationButtons };

        return [totalDesksCount, paginationActionBlock];
    }

    private async Task OpenEditDeskDialogAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        string triggerId,
        EditResourceContext context,
        CancellationToken cancellationToken)
    {
        var resource = await locationServiceClient.GetResourceAsync(
            new GetResourceInput { Id = context.ResourceId },
            locationConfiguration.ApiKey.CreateMetadata(workspaceMember.Id),
            cancellationToken: cancellationToken);

        var resourceType = new InputBlock
        {
            BlockId = OptionLoaderKeys.OrganizationResourceTypeKey,
            Label = "Resource Type".ToPlainText(),
            Element = new ExternalSelectMenu
            {
                ActionId = OptionLoaderKeys.OrganizationResourceTypeKey,
                InitialOption = new Option { Text = resource.ResourceType.Name.ToOptionText(), Value = resource.ResourceType.Id },
                MinQueryLength = 0
            },
            Optional = false
        };

        var name = new InputBlock
        {
            BlockId = ResourceActionTypes.Name,
            Label = "Name".ToPlainText(),
            Element = new PlainTextInput { ActionId = ResourceActionTypes.Name, InitialValue = resource.Name.ToSafeString() },
            Optional = false
        };

        var deactivated = new InputBlock
        {
            BlockId = ResourceActionTypes.Inactive,
            Label = "Activation Status".ToPlainText(),
            Element =
                new CheckboxGroup
                {
                    ActionId = ResourceActionTypes.Inactive,
                    Options = new List<Option> { new() { Text = "Inactive".ToPlainText(), Value = ResourceActionTypes.Inactive } }
                },
            Optional = true
        };

        var requireBookingApproval = new InputBlock
        {
            BlockId = ResourceActionTypes.RequireBookingApproval,
            Label = "Booking Approval Status".ToPlainText(),
            Element =
                new CheckboxGroup
                {
                    ActionId = ResourceActionTypes.RequireBookingApproval,
                    Options = new List<Option>
                    {
                        new() { Text = "Require Booking Approval".ToPlainText(), Value = ResourceActionTypes.RequireBookingApproval }
                    }
                },
            Optional = true
        };

        var capacity = new InputBlock
        {
            BlockId = ResourceActionTypes.Capacity,
            Label = "Capacity".ToPlainText(),
            Element = new PlainTextInput { ActionId = ResourceActionTypes.Capacity, InitialValue = resource.Capacity.ToString() },
            Optional = false
        };

        var blocks = new List<Block>
        {
            resourceType,
            name,
            deactivated,
            requireBookingApproval,
            capacity
        };

        var customTagConnection = await GetCustomTagsAsync(workspace, workspaceMember, cancellationToken);
        if (customTagConnection.Edges.Count != 0)
        {
            blocks.Add(new InputBlock
            {
                BlockId = CustomTagActionTypes.CustomTags,
                Label = "Tags".ToPlainText(),
                Element = new StaticMultiSelectMenu
                {
                    ActionId = CustomTagActionTypes.CustomTags,
                    Options = customTagConnection.Edges.Select(item => item.Node).Select(item => new Option
                    {
                        Text = item.Name.ToOptionText(),
                        Value = item.Id,
                        Description = string.IsNullOrWhiteSpace(item.Description) ? null : item.Description.ToPlainText()
                    }).ToList(),
                    InitialOptions = customTagConnection.Edges.Select(item => item.Node)
                        .Where(item => resource.OrganizationCustomTags.Select(tag => tag.Id).Contains(item.Id)).Select(item =>
                            new Option
                            {
                                Text = item.Name.ToOptionText(),
                                Value = item.Id,
                                Description = string.IsNullOrWhiteSpace(item.Description) ? null : item.Description.ToPlainText()
                            }).ToList()
                },
                Optional = true
            });
        }

        var zoneConnection = await GetZonesAsync(workspace, workspaceMember, cancellationToken);
        if (zoneConnection.Edges.Count != 0)
        {
            blocks.Add(new InputBlock
            {
                BlockId = ZoneActionTypes.Zones,
                Label = "Zones".ToPlainText(),
                Element = new StaticMultiSelectMenu
                {
                    ActionId = ZoneActionTypes.Zones,
                    Options = zoneConnection.Edges.Select(item => item.Node).Select(item => new Option
                    {
                        Text = item.Name.ToOptionText(),
                        Value = item.Id,
                        Description = string.IsNullOrWhiteSpace(item.Description) ? null : item.Description.ToPlainText()
                    }).ToList(),
                    InitialOptions = zoneConnection.Edges.Select(item => item.Node)
                        .Where(item => resource.OrganizationZones.Select(tag => tag.Id).Contains(item.Id)).Select(item =>
                            new Option
                            {
                                Text = item.Name.ToOptionText(),
                                Value = item.Id,
                                Description = string.IsNullOrWhiteSpace(item.Description) ? null : item.Description.ToPlainText()
                            }).ToList()
                },
                Optional = true
            });
        }

        var slackApiClient = workspace.GetApiClient();
        await slackApiClient.ViewsOpenAsync(
            triggerId,
            new ModalViewDefinition
            {
                CallbackId = ResourceCallbackTypes.EditResource,
                Title = "Edit Resource",
                Close = "Cancel",
                Submit = "Save",
                Blocks = blocks,
                PrivateMetadata = context.Serialize()
            },
            cancellationToken);
    }

    private async Task OpenRemoveResourceDialogAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        string triggerId,
        RemoveResourceContext context,
        CancellationToken cancellationToken)
    {
        var resource = await locationServiceClient.GetResourceAsync(
            new GetResourceInput { Id = context.ResourceId },
            locationConfiguration.ApiKey.CreateMetadata(workspaceMember.Id),
            cancellationToken: cancellationToken);

        var confirmationMessage = new SectionBlock { Text = $"Are you sure you want to remove the resource {resource.Name.ToSafeString()}?" };

        var slackApiClient = workspace.GetApiClient();
        await slackApiClient.ViewsOpenAsync(
            triggerId,
            new ModalViewDefinition
            {
                CallbackId = ResourceCallbackTypes.RemoveResource,
                Title = "Remove Resource",
                Close = "No",
                Submit = "Yes",
                Blocks = [confirmationMessage],
                PrivateMetadata = context.Serialize()
            },
            cancellationToken);
    }

    private async Task AddPreferredResourceAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        SetPreferredResourceContext context,
        string? hash,
        CancellationToken cancellationToken)
    {
        await customerServiceClient.AddPreferredResourceAsync(
            new AddPreferredResourceInput { ResourceId = context.ResourceId },
            customerConfiguration.ApiKey.CreateMetadata(workspaceMember.Id),
            cancellationToken: cancellationToken);

        await RenderWithContextAsync(workspace, workspaceMember, new CommonPageContext(context.PageContext), hash, cancellationToken);
    }

    private async Task RemovePreferredResourceAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        RemovePreferredResourceContext context,
        string? hash,
        CancellationToken cancellationToken)
    {
        await customerServiceClient.RemovePreferredResourceAsync(
            new RemovePreferredResourceInput { ResourceId = context.ResourceId },
            customerConfiguration.ApiKey.CreateMetadata(workspaceMember.Id),
            cancellationToken: cancellationToken);

        await RenderWithContextAsync(workspace, workspaceMember, new CommonPageContext(context.PageContext), hash, cancellationToken);
    }

    private async Task<CustomTagConnection> GetCustomTagsAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        CancellationToken cancellationToken)
    {
        var getPaginatedCustomTagsInput = new GetPaginatedCustomTagsInput
        {
            After = string.Empty,
            First = -1,
            Before = string.Empty,
            Last = -1,
            Where = new CustomTagWhereInput { OrganizationId = workspace.Organization.Id }
        };

        getPaginatedCustomTagsInput.OrderBy.AddRange([
            new CustomTagOrderInput
            {
                Direction = global::Api.Shared.Services.Grpc.Skedular.Organization.V1.OrderDirection.Ascending,
                Field = CustomTagOrderField.CustomTagName
            }
        ]);

        return await organizationServiceClient.GetPaginatedCustomTagsAsync(
            getPaginatedCustomTagsInput,
            organizationConfiguration.ApiKey.CreateMetadata(workspaceMember.Id),
            cancellationToken: cancellationToken);
    }

    private async Task<ZoneConnection> GetZonesAsync(Workspace workspace, WorkspaceMember workspaceMember, CancellationToken cancellationToken)
    {
        var getPaginatedZonesInput = new GetPaginatedZonesInput
        {
            After = string.Empty,
            First = -1,
            Before = string.Empty,
            Last = -1,
            Where = new ZoneWhereInput { OrganizationId = workspace.Organization.Id }
        };

        getPaginatedZonesInput.OrderBy.AddRange([
            new ZoneOrderInput
            {
                Direction = global::Api.Shared.Services.Grpc.Skedular.Organization.V1.OrderDirection.Ascending, Field = ZoneOrderField.ZoneName
            }
        ]);

        return await organizationServiceClient.GetPaginatedZonesAsync(
            getPaginatedZonesInput,
            organizationConfiguration.ApiKey.CreateMetadata(workspaceMember.Id),
            cancellationToken: cancellationToken);
    }
}
