using Api.Shared.Services.Grpc.UnityHub.Booking.V1;
using Enterprise.Shared;
using Enterprise.Shared.Exceptions;
using Enterprise.Shared.Grpc;
using Enterprise.Shared.Random;
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
using BookingService = Api.Shared.Services.Grpc.UnityHub.Booking.V1.BookingService;
using Customer = Slack.Shared.Models.Customer;
using Option = SlackNet.Blocks.Option;

namespace Slack.Api.Handlers.ActionHandlers.Booking;

public class AddBookingButtonHandler(
    AsyncPageRenderingService asyncPageRenderingService,
    BookingConfiguration bookingConfiguration,
    ICustomerService customerService,
    ILocationService locationService,
    ITeamService teamService,
    IBookingService bookingService,
    BookingService.BookingServiceClient bookingServiceClient,
    IRepositoryFactory repositoryFactory,
    IWorkspaceMemberService workspaceMemberService,
    IMapper mapper,
    IRandomHelper randomHelper,
    TimeProvider timeProvider,
    IPageNavigator pageNavigator)
    : IAsyncPageRenderingCallbacks, IBlockActionHandler<ButtonAction>, IViewSubmissionHandler
{
    private const string DateKey = "Date";
    private const string NotesKey = "BookingNotes";

    public async Task HandleAsync(ButtonAction action, BlockActionRequest request, CancellationToken cancellationToken)
    {
        var workspaceEntity =
            await repositoryFactory.WorkspaceRepository.GetByIdAsync(request.Team.Id, cancellationToken);
        if (workspaceEntity is null)
        {
            throw new SlackWorkspaceNotFound();
        }

        var (workspaceMemberEntity, _) =
            await workspaceMemberService.EnsureCustomerResourcesAllExistAsync(
                workspaceEntity,
                request.User.Id,
                cancellationToken);

        var workspace = mapper.MapTo(workspaceEntity);
        var workspaceMember = mapper.MapTo(workspaceMemberEntity, workspace);
        var customer = await customerService.GetAsync(workspaceMember, cancellationToken);
        ArgumentNullException.ThrowIfNull(customer);
        var context = AddBookingContext.Deserialize(action.Value);
        var bookingDate = new InputBlock
        {
            BlockId = DateKey,
            Label = "Date".ToPlainText(),
            Element = new DatePicker
            {
                ActionId = DateKey,
                InitialDate =
                    (context.From ?? timeProvider.GetUtcNow().StartOfDay(TimeZoneInfo.Utc)).ToDateTime()
            },
            Optional = false
        };

        var asyncBlocks = await Task.WhenAll(
            GetOrganizationMemberBlockAsync(workspace, workspaceMember, context, customer, cancellationToken),
            GetLocationBlockAsync(workspace, workspaceMember, context, customer, cancellationToken),
            GetTeamBlockAsync(workspace, workspaceMember, context, customer, cancellationToken));

        var notes = new InputBlock
        {
            BlockId = NotesKey,
            Label = "Notes".ToPlainText(),
            Element = new PlainTextInput
            {
                ActionId = NotesKey, Placeholder = "e.g. I will be there from 9am", Multiline = true
            },
            Optional = true
        };

        var slackApiClient = workspace.GetApiClient();
        await slackApiClient.Views.Open(
            request.TriggerId,
            new ModalViewDefinition
            {
                CallbackId = BookingCallbackTypes.AddBooking,
                Title = "Add Booking",
                Close = "Cancel",
                Submit = "Add",
                Blocks = new List<Block> { bookingDate }
                    .Concat(asyncBlocks[0])
                    .Concat(asyncBlocks[1])
                    .Concat(asyncBlocks[2])
                    .Concat([notes]).ToList(),
                PrivateMetadata = action.Value
            });
    }

    public Task Handle(ButtonAction action, BlockActionRequest request)
    {
        asyncPageRenderingService.ButtonActionHandlerStream.OnNext((GetType(), action, request));

        return Task.CompletedTask;
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

        var (workspaceMemberEntity, customerId) =
            await workspaceMemberService.EnsureCustomerResourcesAllExistAsync(
                workspaceEntity,
                viewSubmission.User.Id,
                cancellationToken);

        var workspace = mapper.MapTo(workspaceEntity);
        var workspaceMember = mapper.MapTo(workspaceMemberEntity, workspace);
        var context = AddBookingContext.Deserialize(viewSubmission.View.PrivateMetadata);
        var addInput = new AddInput { Id = randomHelper.Generate() };
        var values = viewSubmission.View.State.Values;
        if (values.TryGetValue(DateKey, out var dateBlock))
        {
            if (dateBlock.TryGetValue(DateKey, out var date))
            {
                if (date is DatePickerValue value)
                {
                    ArgumentNullException.ThrowIfNull(value.SelectedDate);
                    var from = value.SelectedDate.Value.ToDateTimeOffset();
                    addInput.From = from.ToTimestamp();
                    addInput.To = from.EndOfDay().ToTimestamp();
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

        if (values.TryGetValue(OptionLoaderKeys.OrganizationMemberKey, out var organizationMemberBlock))
        {
            if (organizationMemberBlock.TryGetValue(OptionLoaderKeys.OrganizationMemberKey, out var organizationMember))
            {
                if (organizationMember is ExternalSelectValue value)
                {
                    ArgumentException.ThrowIfNullOrWhiteSpace(value.SelectedOption?.Value);
                    addInput.CustomerId = value.SelectedOption.Value;
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
            addInput.CustomerId = customerId;
        }

        if (values.TryGetValue(OptionLoaderKeys.OrganizationLocationKey, out var locationBlock))
        {
            if (locationBlock.TryGetValue(OptionLoaderKeys.OrganizationLocationKey, out var location))
            {
                if (location is ExternalSelectValue value)
                {
                    addInput.LocationId = string.IsNullOrWhiteSpace(value.SelectedOption?.Value)
                        ? string.Empty
                        : value.SelectedOption.Value;
                }
                else
                {
                    throw new InvalidOperationException("location must be ExternalSelectValue");
                }
            }
            else
            {
                throw new InvalidOperationException("location block is missing");
            }
        }
        else
        {
            throw new InvalidOperationException("location block is missing");
        }

        if (values.TryGetValue(OptionLoaderKeys.OrganizationTeamKey, out var teamBlock))
        {
            if (teamBlock.TryGetValue(OptionLoaderKeys.OrganizationTeamKey, out var team))
            {
                if (team is ExternalSelectValue value)
                {
                    addInput.TeamId = string.IsNullOrWhiteSpace(value.SelectedOption?.Value)
                        ? string.Empty
                        : value.SelectedOption.Value;
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
                    addInput.Notes = value.Value.ToSafeString();
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

        await bookingServiceClient.AddAsync(
            addInput,
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

    private async Task<ICollection<Block>> GetOrganizationMemberBlockAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        AddBookingContext context,
        Customer customer,
        CancellationToken cancellationToken)
    {
        var permissions =
            await bookingService.GetOrganizationPermissionsAsync(workspace, workspaceMember, cancellationToken);
        if (!permissions.CanAddBookingOnBehalf)
        {
            return [];
        }

        if (context.CustomerId is null)
        {
            return
            [
                new InputBlock
                {
                    BlockId = OptionLoaderKeys.OrganizationMemberKey,
                    Label = "Organization member".ToPlainText(),
                    Element = new ExternalSelectMenu
                    {
                        ActionId = OptionLoaderKeys.OrganizationMemberKey,
                        InitialOption =
                            new Option { Text = customer.GetCustomerName().ToOptionText(), Value = customer.Id },
                        MinQueryLength = 0
                    },
                    Optional = false
                }
            ];
        }

        var customerToAddToBooking = await customerService.GetByIdAsync(context.CustomerId, cancellationToken);
        ArgumentNullException.ThrowIfNull(customerToAddToBooking);

        return
        [
            new InputBlock
            {
                BlockId = OptionLoaderKeys.OrganizationMemberKey,
                Label = "Organization member".ToPlainText(),
                Element = new ExternalSelectMenu
                {
                    ActionId = OptionLoaderKeys.OrganizationMemberKey,
                    InitialOption =
                        new Option
                        {
                            Text = customerToAddToBooking.GetCustomerName().ToOptionText(),
                            Value = customerToAddToBooking.Id
                        },
                    MinQueryLength = 0
                },
                Optional = false
            }
        ];
    }

    private async Task<ICollection<Block>> GetLocationBlockAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        AddBookingContext context,
        Customer customer,
        CancellationToken cancellationToken)
    {
        if (context.LocationId is null)
        {
            var defaultLocation = customer.DefaultLocations.FirstOrDefault(item =>
                item.Organization is not null && item.Organization.Id == workspace.Organization.Id);

            return
            [
                new InputBlock
                {
                    BlockId = OptionLoaderKeys.OrganizationLocationKey,
                    Label = "Location".ToPlainText(),
                    Element = new ExternalSelectMenu
                    {
                        ActionId = OptionLoaderKeys.OrganizationLocationKey,
                        InitialOption =
                            defaultLocation is null
                                ? null
                                : new Option { Text = defaultLocation.Name.ToOptionText(), Value = defaultLocation.Id },
                        MinQueryLength = 0
                    },
                    Optional = false
                }
            ];
        }

        var locationToAddToBooking =
            await locationService.GetLocationAsync(context.LocationId, workspaceMember, cancellationToken);
        ArgumentNullException.ThrowIfNull(locationToAddToBooking);

        return
        [
            new InputBlock
            {
                BlockId = OptionLoaderKeys.OrganizationLocationKey,
                Label = "Location".ToPlainText(),
                Element = new ExternalSelectMenu
                {
                    ActionId = OptionLoaderKeys.OrganizationLocationKey,
                    InitialOption =
                        new Option
                        {
                            Text = locationToAddToBooking.Name.ToOptionText(), Value = locationToAddToBooking.Id
                        },
                    MinQueryLength = 0
                },
                Optional = true
            }
        ];
    }

    private async Task<ICollection<Block>> GetTeamBlockAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        AddBookingContext context,
        Customer customer,
        CancellationToken cancellationToken)
    {
        if (context.TeamId is null)
        {
            var defaultTeam = customer.DefaultTeams.FirstOrDefault(item =>
                item.Organization is not null && item.Organization.Id == workspace.Organization.Id);

            return
            [
                new InputBlock
                {
                    BlockId = OptionLoaderKeys.OrganizationTeamKey,
                    Label = "Team".ToPlainText(),
                    Element = new ExternalSelectMenu
                    {
                        ActionId = OptionLoaderKeys.OrganizationTeamKey,
                        InitialOption =
                            defaultTeam is null
                                ? null
                                : new Option { Text = defaultTeam.Name.ToOptionText(), Value = defaultTeam.Id },
                        MinQueryLength = 0
                    },
                    Optional = true
                }
            ];
        }

        var teamToAddToBooking =
            await teamService.GetTeamAsync(context.TeamId, workspaceMember, cancellationToken);
        ArgumentNullException.ThrowIfNull(teamToAddToBooking);

        return
        [
            new InputBlock
            {
                BlockId = OptionLoaderKeys.OrganizationTeamKey,
                Label = "Team".ToPlainText(),
                Element = new ExternalSelectMenu
                {
                    ActionId = OptionLoaderKeys.OrganizationTeamKey,
                    InitialOption =
                        new Option { Text = teamToAddToBooking.Name.ToOptionText(), Value = teamToAddToBooking.Id },
                    MinQueryLength = 0
                },
                Optional = false
            }
        ];
    }
}
