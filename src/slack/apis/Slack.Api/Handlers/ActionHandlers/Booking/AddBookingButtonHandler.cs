using Api.Shared.Services;
using Api.Shared.Services.Models;
using Enterprise.Shared;
using Enterprise.Shared.Random;
using Enterprise.Shared.Time;
using Slack.Api.Mappers;
using Slack.Api.Pages;
using Slack.Api.Services;
using Slack.Shared;
using Slack.Shared.Configurations;
using Slack.Shared.Constants;
using Slack.Shared.Context;
using Slack.Shared.Models;
using Slack.Shared.Repositories;
using Slack.Shared.Services.CrossDomains;
using SlackNet;
using SlackNet.Blocks;
using SlackNet.Interaction;
using Customer = Slack.Shared.Models.Customer;
using Option = SlackNet.Blocks.Option;
using Organization = Slack.Shared.Models.Organization;

namespace Slack.Api.Handlers.ActionHandlers.Booking;

public class AddBookingButtonHandler(
    AsyncPageRenderingService asyncPageRenderingService,
    SlackConfigurationService slackConfigurationService,
    ICustomerService customerService,
    ITeamService teamService,
    IRepositoryFactory repositoryFactory,
    IWorkspaceMemberService workspaceMemberService,
    IEntityMapper entityMapper,
    IRandomHelper randomHelper,
    TimeProvider timeProvider,
    IPageNavigator pageNavigator,
    IBookingService bookingService)
    : IAsyncPageRenderingCallbacks, IBlockActionHandler<ButtonAction>, IViewSubmissionHandler
{
    private const string DateKey = "Date";
    private const string NotesKey = "BookingNotes";

    public async Task HandleAsync(ButtonAction action, BlockActionRequest request, CancellationToken cancellationToken)
    {
        var workspaceEntity = await repositoryFactory.WorkspaceRepository.GetByIdAsync(request.Team.Id, cancellationToken) ??
                              throw new SlackWorkspaceNotFound();
        var (workspaceMemberEntity, _) = await workspaceMemberService.EnsureCustomerResourcesAllExistAsync(
            workspaceEntity,
            request.User.Id,
            cancellationToken);

        var workspace = entityMapper.MapTo(workspaceEntity);
        var workspaceMember = entityMapper.MapTo(workspaceMemberEntity, workspace);
        var customer = await customerService.GetAsync(workspaceMember.Id, cancellationToken) ?? throw new CustomerNotFound();
        var context = AddBookingContext.Deserialize(action.Value);
        var bookingDate = new InputBlock
        {
            BlockId = DateKey,
            Label = "Date".ToPlainText(),
            Element = new DatePicker
            {
                ActionId = DateKey,
                InitialDate = (context.From ?? timeProvider.GetUtcNow().StartOfDay()).ToDateTime(),
            },
            Optional = false,
        };

        var asyncBlocks = await Task.WhenAll(
            GetOrganizationMemberBlockAsync(context, customer, cancellationToken),
            GetTeamBlockAsync(workspaceMember, context, cancellationToken));

        var notes = new InputBlock
        {
            BlockId = NotesKey,
            Label = "Notes".ToPlainText(),
            Element = new PlainTextInput
            {
                ActionId = NotesKey,
                Placeholder = "e.g., I will be there from 9am",
                Multiline = true,
            },
            Optional = true,
        };

        var slackApiClient = workspace.GetApiClient();
        await slackApiClient.ViewsOpenAsync(
            request.TriggerId,
            new ModalViewDefinition
            {
                CallbackId = BookingCallbackTypes.AddBooking,
                Title = "Make a booking",
                Close = "Cancel",
                Submit = "Add",
                Blocks = new List<Block>
                {
                    bookingDate,
                }.Concat(asyncBlocks[0]).Concat(asyncBlocks[1]).Append(notes).ToList(),
                PrivateMetadata = action.Value,
            },
            cancellationToken);
    }

    public async Task Handle(ButtonAction action, BlockActionRequest request)
    {
        if (slackConfigurationService.EnableAsyncMode)
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

        var workspaceEntity = await repositoryFactory.WorkspaceRepository.GetByIdAsync(viewSubmission.Team.Id, cancellationToken) ??
                              throw new SlackWorkspaceNotFound();
        var (workspaceMemberEntity, customerId) = await workspaceMemberService.EnsureCustomerResourcesAllExistAsync(
            workspaceEntity,
            viewSubmission.User.Id,
            cancellationToken);

        var workspace = entityMapper.MapTo(workspaceEntity);
        var workspaceMember = entityMapper.MapTo(workspaceMemberEntity, workspace);
        var context = AddBookingContext.Deserialize(viewSubmission.View.PrivateMetadata);
        var booking = new Shared.Models.Booking
        {
            Id = randomHelper.Generate(),
            Category = BookingCategory.WorkingFromOffice,
            InvolvedOrganizations =
            [
                new Organization
                {
                    Id = workspace.Organization.Id,
                },
            ],
        };

        var values = viewSubmission.View.State.Values;
        if (values.TryGetValue(DateKey, out var dateBlock))
        {
            if (dateBlock.TryGetValue(DateKey, out var block))
            {
                if (block is DatePickerValue value)
                {
                    ArgumentNullException.ThrowIfNull(value.SelectedDate);
                    var from = value.SelectedDate.Value.ToDateTimeOffset();
                    booking.From = from;
                    booking.Until = from.EndOfDay();
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
            if (organizationMemberBlock.TryGetValue(OptionLoaderKeys.OrganizationMemberKey, out var block))
            {
                if (block is ExternalSelectValue value)
                {
                    ArgumentException.ThrowIfNullOrWhiteSpace(value.SelectedOption?.Value);
                    booking.InvolvedCustomers =
                    [
                        new Customer
                        {
                            Id = value.SelectedOption.Value,
                        },
                    ];
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
            booking.InvolvedCustomers =
            [
                new Customer
                {
                    Id = customerId,
                },
            ];
        }

        if (values.TryGetValue(OptionLoaderKeys.OrganizationTeamKey, out var teamBlock))
        {
            if (teamBlock.TryGetValue(OptionLoaderKeys.OrganizationTeamKey, out var block))
            {
                if (block is ExternalSelectValue value)
                {
                    if (!string.IsNullOrWhiteSpace(value.SelectedOption?.Value))
                    {
                        booking.InvolvedTeams =
                        [
                            new Shared.Models.Team
                            {
                                Id = value.SelectedOption.Value,
                            },
                        ];
                    }
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
            if (notesBlock.TryGetValue(NotesKey, out var block))
            {
                if (block is PlainTextInputValue value)
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

        await bookingService.AddPrivateAsync(workspaceMember.Id, booking, cancellationToken);

        await pageNavigator.BackAsync(
            workspace,
            workspaceMember,
            new CommonPageContext(context.PageContext),
            viewSubmission.Hash, cancellationToken);

        return ViewSubmissionResponse.Null;
    }

    public Task HandleClose(ViewClosed viewClosed) => Task.CompletedTask;

    private async Task<IReadOnlyList<Block>> GetOrganizationMemberBlockAsync(
        AddBookingContext context,
        Customer customer,
        CancellationToken cancellationToken)
    {
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
                        InitialOption = new Option
                        {
                            Text = customer.DisplayableName.ToOptionText(),
                            Value = customer.Id,
                        },
                        MinQueryLength = 0,
                    },
                    Optional = false,
                },
            ];
        }

        var customerToAddToBooking = await customerService.AdminGetAsync(context.CustomerId, cancellationToken);
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
                            Text = customerToAddToBooking.DisplayableName.ToOptionText(),
                            Value = customerToAddToBooking.Id,
                        },
                    MinQueryLength = 0,
                },
                Optional = false,
            },
        ];
    }

    private async Task<IReadOnlyList<Block>> GetTeamBlockAsync(
        WorkspaceMember workspaceMember,
        AddBookingContext context,
        CancellationToken cancellationToken)
    {
        if (context.TeamId is null)
        {
            return
            [
                new InputBlock
                {
                    BlockId = OptionLoaderKeys.OrganizationTeamKey,
                    Label = "Team".ToPlainText(),
                    Element = new ExternalSelectMenu
                    {
                        ActionId = OptionLoaderKeys.OrganizationTeamKey,
                        InitialOption = null,
                        MinQueryLength = 0,
                    },
                    Optional = true,
                },
            ];
        }

        var teamToAddToBooking = await teamService.GetAsync(workspaceMember.Id, context.TeamId, cancellationToken);

        return
        [
            new InputBlock
            {
                BlockId = OptionLoaderKeys.OrganizationTeamKey,
                Label = "Team".ToPlainText(),
                Element = new ExternalSelectMenu
                {
                    ActionId = OptionLoaderKeys.OrganizationTeamKey,
                    InitialOption = new Option
                    {
                        Text = teamToAddToBooking.Name.ToOptionText(),
                        Value = teamToAddToBooking.Id,
                    },
                    MinQueryLength = 0,
                },
                Optional = false,
            },
        ];
    }
}
