using Api.Shared.Services;
using Enterprise.Shared;
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
using Icons = Slack.Shared.Constants.Icons;
using Option = SlackNet.Blocks.Option;

namespace Slack.Api.Handlers.ActionHandlers.Booking;

public class EditBookingButtonHandler(
    AsyncPageRenderingService asyncPageRenderingService,
    SlackConfigurationService slackConfigurationService,
    IRepositoryFactory repositoryFactory,
    IWorkspaceMemberService workspaceMemberService,
    IEntityMapper entityMapper,
    IPageNavigator pageNavigator,
    IBookingService bookingService)
    : IAsyncPageRenderingCallbacks, IBlockActionHandler<ButtonAction>, IViewSubmissionHandler
{
    private const string ResourcesKey = "Resources";
    private const string NotesKey = "Notes";

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
        var context = EditBookingContext.Deserialize(action.Value);
        var booking = await bookingService.GetAsync(workspaceMember.Id, context.BookingId, cancellationToken);
        var bookingDate = new SectionBlock
        {
            Text = booking.From.ToShortDateWithoutYear().ToPlainTextWithIcon(Icons.Calendar),
        };
        if (booking.InvolvedCustomers.Count != 1)
        {
            // TODO: 20250427 - Morteza: We currently do not support handling multiple customers involved in a single booking in Slack 
            return;
        }

        if (booking.InvolvedTeams.Count != 0 && booking.InvolvedTeams.Count != 1)
        {
            // TODO: 20250427 - Morteza: We currently do not support handling multiple teams involved in a single booking in Slack 
            return;
        }

        var customer = booking.InvolvedCustomers.First();
        var team = booking.InvolvedTeams.FirstOrDefault();
        var organizationMemberBlock = new InputBlock
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
        };

        var teamBlock = new InputBlock
        {
            BlockId = OptionLoaderKeys.OrganizationTeamKey,
            Label = "Team".ToPlainText(),
            Element = new ExternalSelectMenu
            {
                ActionId = OptionLoaderKeys.OrganizationTeamKey,
                InitialOption = team is null
                    ? null
                    : new Option
                    {
                        Text = team.Name.ToOptionText(),
                        Value = team.Id,
                    },
                MinQueryLength = 0,
            },
            Optional = true,
        };

        var notes = new InputBlock
        {
            BlockId = NotesKey,
            Label = "Notes".ToPlainText(),
            Element = new PlainTextInput
            {
                ActionId = NotesKey,
                Placeholder = "e.g., I will be there from 9am",
                Multiline = true,
                InitialValue = booking.Notes,
            },
            Optional = true,
        };

        var blocks = new List<Block>
        {
            bookingDate,
            organizationMemberBlock,
        };
        var locationResources = await GetResourceOptionsAsync(request, workspace, booking, cancellationToken);
        if (locationResources is not null)
        {
            blocks.Add(locationResources);
        }

        blocks.Add(teamBlock);
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
        var (workspaceMemberEntity, _) = await workspaceMemberService.EnsureCustomerResourcesAllExistAsync(
            workspaceEntity,
            viewSubmission.User.Id,
            cancellationToken);

        var workspace = entityMapper.MapTo(workspaceEntity);
        var workspaceMember = entityMapper.MapTo(workspaceMemberEntity, workspace);
        var context = EditBookingContext.Deserialize(viewSubmission.View.PrivateMetadata);
        var booking = await bookingService.GetAsync(workspaceMember.Id, context.BookingId, cancellationToken);
        var values = viewSubmission.View.State.Values;
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
            throw new InvalidOperationException("organizationMember block is missing");
        }

        if (values.TryGetValue(ResourcesKey, out var locationResourcesBlock))
        {
            if (locationResourcesBlock.TryGetValue(ResourcesKey, out var block))
            {
                if (block is StaticMultiSelectValue value)
                {
                    booking.Resources = value.SelectedOptions.Select(item => new Shared.Models.Resource
                    {
                        Id = item.Value,
                    }).ToList();
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
            if (teamBlock.TryGetValue(OptionLoaderKeys.OrganizationTeamKey, out var block))
            {
                if (block is ExternalSelectValue value)
                {
                    booking.InvolvedTeams = string.IsNullOrWhiteSpace(value.SelectedOption?.Value)
                        ? []
                        :
                        [
                            new Shared.Models.Team
                            {
                                Id = value.SelectedOption.Value,
                            },
                        ];
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

        await bookingService.UpdatePrivateAsync(workspaceMember.Id, booking, cancellationToken);

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
        var availableResources = await bookingService.GetAvailableResourcesAsync(
            request.User.Id,
            workspace.Organization.Id,
            booking.From.ToDate(),
            booking.Until.ToDate(),
            booking.Resources.Select(item => item.Id).ToList(),
            cancellationToken);

        var resourcesOptions = availableResources.Select(item =>
        {
            var zones = item.Zones.Where(tag => !string.IsNullOrWhiteSpace(tag.Name)).ToList();
            var optionText = zones.Count == 0
                ? item.Name.ToOptionTextWithIcon(Icons.Resource)
                : $"{item.Name.ToTextWithIcon(Icons.Resource)} {string.Join(",", zones.Select(zone => zone.Name)).ToTextWithIcon(Icons.Zones)}"
                    .ToOptionText();

            return new Option
            {
                Text = optionText,
                Value = item.Id,
            };
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
            InitialOptions = resourcesOptions.Where(item => resourceIds.Contains(item.Value)).ToList(),
        };

        return new InputBlock
        {
            BlockId = ResourcesKey,
            Label = "Resources".ToPlainText(),
            Element = menu,
            Optional = true,
        };
    }
}
