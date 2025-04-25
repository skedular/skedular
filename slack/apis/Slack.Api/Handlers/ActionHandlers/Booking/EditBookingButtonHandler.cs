using Api.Shared.Services.Grpc.Skedular.Booking.V1;
using Api.Shared.Services.Grpc.Skedular.Location.V1;
using Api.Shared.Services.Models;
using Enterprise.Shared;
using Enterprise.Shared.Exceptions;
using Enterprise.Shared.Grpc;
using Enterprise.Shared.Time;
using Google.Protobuf.WellKnownTypes;
using Slack.Api.Mappers;
using Slack.Api.Pages;
using Slack.Api.Services;
using Slack.Shared;
using Slack.Shared.Configurations;
using Slack.Shared.Constants;
using Slack.Shared.Context;
using Slack.Shared.Models;
using Slack.Shared.Repositories;
using SlackNet;
using SlackNet.Blocks;
using SlackNet.Interaction;
using BookingService = Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingService;
using BookingType = Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingType;
using Customer = Slack.Shared.Models.Customer;
using GetInput = Api.Shared.Services.Grpc.Skedular.Booking.V1.GetInput;
using Icons = Slack.Shared.Constants.Icons;
using Option = SlackNet.Blocks.Option;
using OrderDirection = Api.Shared.Services.Grpc.Skedular.Location.V1.OrderDirection;

namespace Slack.Api.Handlers.ActionHandlers.Booking;

public class EditBookingButtonHandler(
    AsyncPageRenderingService asyncPageRenderingService,
    SlackConfiguration slackConfiguration,
    BookingConfiguration bookingConfiguration,
    BookingService.BookingServiceClient bookingServiceClient,
    IBookingService bookingService,
    IRepositoryFactory repositoryFactory,
    IWorkspaceMemberService workspaceMemberService,
    IMapper mapper,
    IPageNavigator pageNavigator)
    : IAsyncPageRenderingCallbacks, IBlockActionHandler<ButtonAction>, IViewSubmissionHandler
{
    private const string ResourcesKey = "Resources";
    private const string NotesKey = "Notes";

    public async Task HandleAsync(ButtonAction action, BlockActionRequest request, CancellationToken cancellationToken)
    {
        var workspaceEntity = await repositoryFactory.WorkspaceRepository.GetByIdAsync(request.Team.Id, cancellationToken);
        if (workspaceEntity is null)
        {
            throw new SlackWorkspaceNotFound();
        }

        var (workspaceMemberEntity, customerId) = await workspaceMemberService.EnsureCustomerResourcesAllExistAsync(
            workspaceEntity,
            request.User.Id,
            cancellationToken);

        var workspace = mapper.MapTo(workspaceEntity);
        var workspaceMember = mapper.MapTo(workspaceMemberEntity, workspace);
        var context = EditBookingContext.Deserialize(action.Value);
        var booking = mapper.MapTo(await bookingServiceClient.GetAsync(
            new GetInput { Id = context.BookingId },
            bookingConfiguration.ApiKey.CreateMetadata(workspaceMember.Id),
            cancellationToken: cancellationToken));

        if (booking.Customer.Id != customerId)
        {
            var permissions = await bookingService.GetOrganizationPermissionsAsync(workspace, workspaceMember, cancellationToken);
            if (!permissions.CanUpdateBookingOnBehalf)
            {
                throw new Unauthorized();
            }
        }

        var bookingDate = new SectionBlock { Text = booking.From.ToShortDateWithoutYear().ToPlainTextWithIcon(Icons.Calendar) };

        var organizationMember = new InputBlock
        {
            BlockId = OptionLoaderKeys.OrganizationMemberKey,
            Label = "Organization member".ToPlainText(),
            Element = new ExternalSelectMenu
            {
                ActionId = OptionLoaderKeys.OrganizationMemberKey,
                InitialOption = new Option { Text = booking.Customer.ToDisplayableName().ToOptionText(), Value = booking.Customer.Id },
                MinQueryLength = 0
            },
            Optional = false
        };

        var team = new InputBlock
        {
            BlockId = OptionLoaderKeys.OrganizationTeamKey,
            Label = "Team".ToPlainText(),
            Element = new ExternalSelectMenu
            {
                ActionId = OptionLoaderKeys.OrganizationTeamKey,
                InitialOption = booking.Team is null
                    ? null
                    : new Option { Text = booking.Team.Name.ToOptionText(), Value = booking.Team.Id },
                MinQueryLength = 0
            },
            Optional = true
        };

        var notes = new InputBlock
        {
            BlockId = NotesKey,
            Label = "Notes".ToPlainText(),
            Element = new PlainTextInput
            {
                ActionId = NotesKey, Placeholder = "e.g. I will be there from 9am", Multiline = true, InitialValue = booking.Notes
            },
            Optional = true
        };

        var blocks = new List<Block> { bookingDate, organizationMember };
        var locationResources = await GetResourceOptionsAsync(request, workspace, booking, cancellationToken);
        if (locationResources is not null)
        {
            blocks.Add(locationResources);
        }

        blocks.Add(team);
        blocks.Add(notes);

        var slackApiClient = workspace.GetApiClient();
        await slackApiClient.ViewsOpenAsync(
            request.TriggerId,
            new ModalViewDefinition
            {
                CallbackId = BookingCallbackTypes.EditBooking,
                Title = "Edit Booking",
                Close = "Cancel",
                Submit = "Save",
                Blocks = blocks,
                PrivateMetadata = action.Value
            },
            cancellationToken);
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

    public async Task<ViewSubmissionResponse> Handle(ViewSubmission viewSubmission)
    {
        var cancellationToken = CancellationToken.None;
        var workspaceEntity = await repositoryFactory.WorkspaceRepository.GetByIdAsync(viewSubmission.Team.Id, cancellationToken);
        if (workspaceEntity is null)
        {
            throw new SlackWorkspaceNotFound();
        }

        var (workspaceMemberEntity, _) = await workspaceMemberService.EnsureCustomerResourcesAllExistAsync(
            workspaceEntity,
            viewSubmission.User.Id,
            cancellationToken);

        var workspace = mapper.MapTo(workspaceEntity);
        var workspaceMember = mapper.MapTo(workspaceMemberEntity, workspace);
        var context = EditBookingContext.Deserialize(viewSubmission.View.PrivateMetadata);
        var booking = mapper.MapTo(
            await bookingServiceClient.GetAsync(
                new GetInput { Id = context.BookingId },
                bookingConfiguration.ApiKey.CreateMetadata(workspaceMember.Id),
                cancellationToken: cancellationToken));

        var values = viewSubmission.View.State.Values;
        if (values.TryGetValue(OptionLoaderKeys.OrganizationMemberKey, out var organizationMemberBlock))
        {
            if (organizationMemberBlock.TryGetValue(OptionLoaderKeys.OrganizationMemberKey, out var organizationMember))
            {
                if (organizationMember is ExternalSelectValue value)
                {
                    ArgumentException.ThrowIfNullOrWhiteSpace(value.SelectedOption?.Value);
                    booking.Customer = new Customer { Id = value.SelectedOption.Value };
                }
                else
                {
                    throw new InvalidOperationException("organizationMember must be ExternalSelectValue");
                }
            }
            else
            {
                throw new InvalidOperationException("organizationMember block is missing");
            }
        }
        else
        {
            throw new InvalidOperationException("organizationMember block is missing");
        }

        if (values.TryGetValue(ResourcesKey, out var locationResourcesBlock))
        {
            if (locationResourcesBlock.TryGetValue(ResourcesKey, out var locationResources))
            {
                if (locationResources is StaticMultiSelectValue value)
                {
                    var getPaginatedLocationsInput = new GetPaginatedLocationsInput
                    {
                        First = -1, Last = -1, Where = new LocationWhereInput { OrganizationId = workspace.Organization.Id }
                    };
                    getPaginatedLocationsInput.OrderBy.AddRange([
                        new LocationOrderInput { Direction = OrderDirection.Ascending, Field = LocationOrderField.Name }
                    ]);
                    booking.Resources = value.SelectedOptions.Select(item => new Shared.Models.Resource { Id = item.Value }).ToList();
                }
                else
                {
                    throw new InvalidOperationException("locationResources must be StaticMultiSelectValue");
                }
            }
            else
            {
                throw new InvalidOperationException("locationResources block is missing");
            }
        }

        if (values.TryGetValue(OptionLoaderKeys.OrganizationTeamKey, out var teamBlock))
        {
            if (teamBlock.TryGetValue(OptionLoaderKeys.OrganizationTeamKey, out var team))
            {
                if (team is ExternalSelectValue value)
                {
                    booking.Team = string.IsNullOrWhiteSpace(value.SelectedOption?.Value)
                        ? null
                        : new Shared.Models.Team { Id = value.SelectedOption.Value };
                }
                else
                {
                    throw new InvalidOperationException("team must be ExternalSelectValue");
                }
            }
            else
            {
                throw new InvalidOperationException("team block is missing");
            }
        }
        else
        {
            throw new InvalidOperationException("team block is missing");
        }

        if (values.TryGetValue(NotesKey, out var notesBlock))
        {
            if (notesBlock.TryGetValue(NotesKey, out var notes))
            {
                if (notes is PlainTextInputValue value)
                {
                    booking.Notes = value.Value.ToSafeString();
                }
                else
                {
                    throw new InvalidOperationException("notes must be PlainTextInputValue");
                }
            }
            else
            {
                throw new InvalidOperationException("notes block is missing");
            }
        }
        else
        {
            throw new InvalidOperationException("notes block is missing");
        }

        var updateBooking = mapper.MapTo(booking);
        updateBooking.Type = BookingType.WorkingFromOffice;

        await bookingServiceClient.UpdateAsync(
            updateBooking,
            bookingConfiguration.ApiKey.CreateMetadata(workspaceMember.Id),
            cancellationToken: cancellationToken);

        await pageNavigator.BackAsync(workspace, workspaceMember, new CommonPageContext(context.PageContext), viewSubmission.Hash, cancellationToken);

        return ViewSubmissionResponse.Null;
    }

    public Task HandleClose(ViewClosed viewClosed) => Task.CompletedTask;

    private async Task<InputBlock?> GetResourceOptionsAsync(
        BlockActionRequest request,
        Workspace workspace,
        Shared.Models.Booking booking,
        CancellationToken cancellationToken)
    {
        var getAvailableResourcesInput = new GetAvailableResourcesInput
        {
            OrganizationId = workspace.Organization.Id, From = booking.From.ToDate().ToTimestamp(), Until = booking.Until.ToDate().ToTimestamp()
        };

        getAvailableResourcesInput.ResourceIdsToInclude.AddRange(booking.Resources.Select(item => item.Id));

        var availableResources = (await bookingServiceClient.GetAvailableResourcesAsync(
                getAvailableResourcesInput,
                bookingConfiguration.ApiKey.CreateMetadata(request.User.Id),
                cancellationToken: cancellationToken)).Resources
            .Where(item => item.Location is not null)
            .ToList();

        var resourcesOptions = availableResources.Select(item =>
        {
            var zones = item.OrganizationZones.Where(locationTag => !string.IsNullOrWhiteSpace(locationTag.Name)).ToList();
            var optionText = zones.Count == 0
                ? item.Name.ToOptionTextWithIcon(Icons.Resource)
                : $"{item.Name.ToTextWithIcon(Icons.Resource)} {string.Join(",", zones.Select(zone => zone.Name)).ToTextWithIcon(Icons.Zones)}"
                    .ToOptionText();

            return new Option { Text = optionText, Value = item.Id };
        }).ToList();

        if (resourcesOptions.Count == 0)
        {
            return null;
        }

        var resourceIds = booking.Resources.Select(item => item.Id).ToList();
        var menu = new StaticMultiSelectMenu
        {
            ActionId = ResourcesKey,
            Options = resourcesOptions,
            InitialOptions = resourcesOptions.Where(item => resourceIds.Contains(item.Value)).ToList()
        };

        return new InputBlock { BlockId = ResourcesKey, Label = "Resources".ToPlainText(), Element = menu, Optional = true };
    }
}
