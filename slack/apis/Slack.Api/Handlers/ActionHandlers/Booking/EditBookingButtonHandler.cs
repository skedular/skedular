using Api.Shared.Services.Grpc.Skedular.Booking.V1;
using Api.Shared.Services.Grpc.Skedular.Location.V1;
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
using Customer = Slack.Shared.Models.Customer;
using GetInput = Api.Shared.Services.Grpc.Skedular.Booking.V1.GetInput;
using Icons = Slack.Shared.Constants.Icons;
using LocationService = Api.Shared.Services.Grpc.Skedular.Location.V1.LocationService;
using Option = SlackNet.Blocks.Option;
using OptionGroup = SlackNet.Blocks.OptionGroup;
using OrderDirection = Api.Shared.Services.Grpc.Skedular.Location.V1.OrderDirection;

namespace Slack.Api.Handlers.ActionHandlers.Booking;

public class EditBookingButtonHandler(
    AsyncPageRenderingService asyncPageRenderingService,
    SlackConfiguration slackConfiguration,
    BookingConfiguration bookingConfiguration,
    LocationConfiguration locationConfiguration,
    BookingService.BookingServiceClient bookingServiceClient,
    LocationService.LocationServiceClient locationServiceClient,
    IBookingService bookingService,
    IRepositoryFactory repositoryFactory,
    IWorkspaceMemberService workspaceMemberService,
    IMapper mapper,
    IPageNavigator pageNavigator)
    : IAsyncPageRenderingCallbacks, IBlockActionHandler<ButtonAction>, IViewSubmissionHandler
{
    private const string LocationsDesksKey = "LocationsDesks";
    private const string NotesKey = "BookingNotes";

    public async Task HandleAsync(ButtonAction action, BlockActionRequest request, CancellationToken cancellationToken)
    {
        var workspaceEntity =
            await repositoryFactory.WorkspaceRepository.GetByIdAsync(request.Team.Id, cancellationToken);
        if (workspaceEntity is null)
        {
            throw new SlackWorkspaceNotFound();
        }

        var (workspaceMemberEntity, customerId) =
            await workspaceMemberService.EnsureCustomerResourcesAllExistAsync(
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
            var permissions =
                await bookingService.GetOrganizationPermissionsAsync(workspace, workspaceMember, cancellationToken);
            if (!permissions.CanUpdateBookingOnBehalf)
            {
                throw new Unauthorized();
            }
        }

        var bookingDate = new SectionBlock
        {
            Text = booking.From.ToShortDateWithoutYear().ToPlainTextWithIcon(Icons.Calendar)
        };

        var organizationMember = new InputBlock
        {
            BlockId = OptionLoaderKeys.OrganizationMemberKey,
            Label = "Organization member".ToPlainText(),
            Element = new ExternalSelectMenu
            {
                ActionId = OptionLoaderKeys.OrganizationMemberKey,
                InitialOption =
                    new Option
                    {
                        Text = booking.Customer.GetCustomerName().ToOptionText(), Value = booking.Customer.Id
                    },
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
                InitialOption =
                    booking.Team is null
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
                ActionId = NotesKey,
                Placeholder = "e.g. I will be there from 9am",
                Multiline = true,
                InitialValue = booking.Notes
            },
            Optional = true
        };

        var blocks = new List<Block> { bookingDate, organizationMember };
        var locationDesks = await GetLocationDeskOptionsAsync(request, workspace, booking, cancellationToken);
        if (locationDesks is not null)
        {
            blocks.Add(locationDesks);
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

        var workspaceEntity =
            await repositoryFactory.WorkspaceRepository.GetByIdAsync(viewSubmission.Team.Id, cancellationToken);
        if (workspaceEntity is null)
        {
            throw new SlackWorkspaceNotFound();
        }

        var (workspaceMemberEntity, _) =
            await workspaceMemberService.EnsureCustomerResourcesAllExistAsync(
                workspaceEntity,
                viewSubmission.User.Id,
                cancellationToken);

        var workspace = mapper.MapTo(workspaceEntity);
        var workspaceMember = mapper.MapTo(workspaceMemberEntity, workspace);
        var context = EditBookingContext.Deserialize(viewSubmission.View.PrivateMetadata);
        var booking = mapper.MapTo(await bookingServiceClient.GetAsync(
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

        if (values.TryGetValue(LocationsDesksKey, out var locationDesksBlock))
        {
            if (locationDesksBlock.TryGetValue(LocationsDesksKey, out var locationDesks))
            {
                if (locationDesks is StaticMultiSelectValue value)
                {
                    var getPaginatedLocationsInput = new GetPaginatedLocationsInput
                    {
                        First = -1,
                        Last = -1,
                        Where = new LocationWhereInput { OrganizationId = workspace.Organization.Id }
                    };
                    getPaginatedLocationsInput.OrderBy.AddRange([
                        new LocationOrderInput { Direction = OrderDirection.Ascending, Field = LocationOrderField.Name }
                    ]);
                    var getLocationsResponse = await locationServiceClient.GetPaginatedLocationsAsync(
                        getPaginatedLocationsInput,
                        locationConfiguration.ApiKey.CreateMetadata(workspaceMember.Id),
                        cancellationToken: cancellationToken);
                    var locationIds = getLocationsResponse.Edges.Select(item => item.Node.Id).ToList();
                    var selectedLocationIds =
                        value.SelectedOptions.Where(item => locationIds.Contains(item.Value)).ToList();

                    if (selectedLocationIds.Count == 0)
                    {
                        booking.Location = null;
                    }
                    else if (selectedLocationIds.Count == 1)
                    {
                        booking.Location = new Shared.Models.Location { Id = selectedLocationIds.First().Value };
                    }
                    else
                    {
                        throw new InvalidOperationException("multiple locations not supported for booking");
                    }

                    booking.Desks = value.SelectedOptions.Where(item => !locationIds.Contains(item.Value))
                        .Select(item => new Shared.Models.Desk { Id = item.Value })
                        .ToList();
                }
                else
                {
                    throw new InvalidOperationException("locationDesks must be StaticMultiSelectValue");
                }
            }
            else
            {
                throw new InvalidOperationException("locationDesks block is missing");
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

        await pageNavigator.BackAsync(
            workspace,
            workspaceMember,
            new CommonPageContext(context.PageContext),
            viewSubmission.Hash, cancellationToken);

        return ViewSubmissionResponse.Null;
    }

    public Task HandleClose(ViewClosed viewClosed) => Task.CompletedTask;

    private async Task<InputBlock?> GetLocationDeskOptionsAsync(
        BlockActionRequest request,
        Workspace workspace,
        Shared.Models.Booking booking,
        CancellationToken cancellationToken)
    {
        var getAvailableDesksInput = new GetAvailableDesksInput
        {
            OrganizationId = workspace.Organization.Id, Date = booking.From.ToDate().ToTimestamp()
        };

        getAvailableDesksInput.DeskIdsToInclude.AddRange(booking.Desks.Select(item => item.Id));

        var availableDesks = (await bookingServiceClient.GetAvailableDesksAsync(
                getAvailableDesksInput,
                bookingConfiguration.ApiKey.CreateMetadata(request.User.Id),
                cancellationToken: cancellationToken)).Desks
            .Where(item => item.Location is not null)
            .ToList();

        var locations = availableDesks
            .Select(item => item.Location)
            .GroupBy(item => item.Id)
            .Select(item => item.First())
            .ToList();

        if (locations.Count == 0)
        {
            return null;
        }

        var locationGroupedWithDesksOptions = locations.Select(item =>
        {
            var locationWithoutDesk = new Option { Text = "No desk selected".ToOptionText(), Value = item.Id };
            var locationWithDesks = availableDesks
                .Where(desk => desk.Location is not null && desk.Location.Id == item.Id)
                .Select(desk =>
                {
                    var zones = desk.OrganizationZones
                        .Where(locationTag => !string.IsNullOrWhiteSpace(locationTag.Name))
                        .ToList();

                    var optionText = zones.Count == 0
                        ? desk.Name.ToOptionTextWithIcon(Icons.Desk)
                        : $"{desk.Name.ToTextWithIcon(Icons.Desk)} {string.Join(",", zones.Select(zone => zone.Name)).ToTextWithIcon(Icons.Zones)}"
                            .ToOptionText();

                    return new Option { Text = optionText, Value = desk.Id };
                })
                .ToList();

            return new OptionGroup
            {
                Label = item.Name,
                Options = new List<Option> { locationWithoutDesk }.Concat(locationWithDesks).ToList()
            };
        }).ToList();

        var deskIds = booking.Desks.Select(item => item.Id).ToList();
        var menu = new StaticMultiSelectMenu
        {
            ActionId = LocationsDesksKey,
            OptionGroups = locationGroupedWithDesksOptions,
            InitialOptions = locationGroupedWithDesksOptions
                .Where(item => item.Options.Any(option => deskIds.Contains(option.Value)))
                .SelectMany(item => item.Options)
                .Where(item => deskIds.Contains(item.Value))
                .ToList()
        };

        return new InputBlock
        {
            BlockId = LocationsDesksKey, Label = "Location/Desks".ToPlainText(), Element = menu, Optional = true
        };
    }
}
