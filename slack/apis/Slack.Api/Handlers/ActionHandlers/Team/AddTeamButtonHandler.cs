using Api.Shared.Services.Grpc.Skedular.Team.V1;
using Enterprise.Shared;
using Enterprise.Shared.Exceptions;
using Enterprise.Shared.Grpc;
using Enterprise.Shared.Random;
using Slack.Api.Mappers;
using Slack.Api.Pages;
using Slack.Api.Services;
using Slack.Shared;
using Slack.Shared.Configurations;
using Slack.Shared.Constants;
using Slack.Shared.Context;
using Slack.Shared.Repositories;
using SlackNet;
using SlackNet.Blocks;
using SlackNet.Interaction;
using AddInput = Api.Shared.Services.Grpc.Skedular.Team.V1.AddInput;
using TeamService = Api.Shared.Services.Grpc.Skedular.Team.V1.TeamService;

namespace Slack.Api.Handlers.ActionHandlers.Team;

public class AddTeamButtonHandler(
    AsyncPageRenderingService asyncPageRenderingService,
    SlackConfiguration slackConfiguration,
    TeamConfiguration teamConfiguration,
    TeamService.TeamServiceClient teamServiceClient,
    ICustomerService customerService,
    IRepositoryFactory repositoryFactory,
    IWorkspaceMemberService workspaceMemberService,
    IWorkspaceChannelService workspaceChannelService,
    IMapper mapper,
    IRandomHelper randomHelper,
    IPageNavigator pageNavigator)
    : IAsyncPageRenderingCallbacks, IBlockActionHandler<ButtonAction>, IViewSubmissionHandler
{
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

        var name = new InputBlock
        {
            BlockId = TeamActionTypes.Name,
            Label = "Name".ToPlainText(),
            Element = new PlainTextInput { ActionId = TeamActionTypes.Name },
            Optional = false
        };

        var about = new InputBlock
        {
            BlockId = TeamActionTypes.About,
            Label = "About".ToPlainText(),
            Element = new PlainTextInput { ActionId = TeamActionTypes.About, Multiline = true },
            Optional = true
        };

        var timezone = new InputBlock
        {
            BlockId = OptionLoaderKeys.TimezoneKey,
            Label = "Timezone".ToPlainText(),
            Element = new ExternalSelectMenu { ActionId = OptionLoaderKeys.TimezoneKey, MinQueryLength = 3 },
            Optional = true
        };

        var primaryLocation = new InputBlock
        {
            BlockId = TeamActionTypes.PrimaryLocation,
            Label = "Primary Location".ToPlainText(),
            Element = new ExternalSelectMenu
            {
                ActionId = OptionLoaderKeys.OrganizationLocationKey, InitialOption = null, MinQueryLength = 0
            },
            Optional = true
        };

        var updateChannel = new InputBlock
        {
            BlockId = TeamActionTypes.SlackUpdateChannel,
            Label = "Slack update channel".ToPlainText(),
            Element = new ChannelSelectMenu { ActionId = TeamActionTypes.SlackUpdateChannel },
            Optional = true
        };

        var organizationMembers = new InputBlock
        {
            BlockId = OptionLoaderKeys.OrganizationMemberAndCustomerPairKey,
            Label = "Members".ToPlainText(),
            Element = new ExternalMultiSelectMenu
            {
                ActionId = OptionLoaderKeys.OrganizationMemberAndCustomerPairKey, MinQueryLength = 0
            },
            Optional = false
        };

        var slackApiClient = workspace.GetApiClient();
        await slackApiClient.ViewsOpenAsync(
            request.TriggerId,
            new ModalViewDefinition
            {
                CallbackId = TeamCallbackTypes.AddTeam,
                Title = "Add Team",
                Close = "Cancel",
                Submit = "Add",
                Blocks = [name, about, timezone, primaryLocation, updateChannel, organizationMembers],
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
        var context = CommonPageContext.Deserialize(viewSubmission.View.PrivateMetadata);
        var values = viewSubmission.View.State.Values;
        var teamId = randomHelper.Generate();
        var addInput = new AddInput { Id = teamId, OrganizationId = workspace.Organization.Id };

        if (values.TryGetValue(TeamActionTypes.Name, out var nameBlock))
        {
            if (nameBlock.TryGetValue(TeamActionTypes.Name, out var name))
            {
                if (name is PlainTextInputValue value)
                {
                    ArgumentException.ThrowIfNullOrWhiteSpace(value.Value);
                    addInput.Name = value.Value.ToSafeString();
                }
                else
                {
                    throw new InvalidOperationException("name must be PlainTextInputValue");
                }
            }
            else
            {
                throw new InvalidOperationException("name block is missing");
            }
        }
        else
        {
            throw new InvalidOperationException("name block is missing");
        }

        if (values.TryGetValue(TeamActionTypes.About, out var aboutBlock))
        {
            if (aboutBlock.TryGetValue(TeamActionTypes.About, out var about))
            {
                if (about is PlainTextInputValue value)
                {
                    addInput.About = value.Value.ToSafeString();
                }
                else
                {
                    throw new InvalidOperationException("about must be PlainTextInputValue");
                }
            }
            else
            {
                throw new InvalidOperationException("about block is missing");
            }
        }
        else
        {
            throw new InvalidOperationException("about block is missing");
        }

        if (values.TryGetValue(OptionLoaderKeys.TimezoneKey, out var timezoneBlock))
        {
            if (timezoneBlock.TryGetValue(OptionLoaderKeys.TimezoneKey, out var timezone))
            {
                if (timezone is ExternalSelectValue value)
                {
                    addInput.Timezone = string.IsNullOrWhiteSpace(value.SelectedOption?.Value)
                        ? string.Empty
                        : value.SelectedOption.Value;
                }
                else
                {
                    throw new InvalidOperationException("timezone must be ExternalSelectValue");
                }
            }
            else
            {
                throw new InvalidOperationException("timezone block is missing");
            }
        }
        else
        {
            throw new InvalidOperationException("timezone block is missing");
        }

        if (values.TryGetValue(TeamActionTypes.PrimaryLocation, out var primaryLocationBlock))
        {
            if (primaryLocationBlock.TryGetValue(OptionLoaderKeys.OrganizationLocationKey, out var primaryLocation))
            {
                if (primaryLocation is ExternalSelectValue value)
                {
                    addInput.PrimaryLocationId = string.IsNullOrWhiteSpace(value.SelectedOption?.Value)
                        ? string.Empty
                        : value.SelectedOption.Value;
                }
                else
                {
                    throw new InvalidOperationException("primary location must be ExternalSelectValue");
                }
            }
            else
            {
                throw new InvalidOperationException("primaryLocation block is missing");
            }
        }
        else
        {
            throw new InvalidOperationException("primaryLocation block is missing");
        }

        if (values.TryGetValue(OptionLoaderKeys.OrganizationMemberAndCustomerPairKey, out var organizationMembersBlock))
        {
            if (organizationMembersBlock.TryGetValue(OptionLoaderKeys.OrganizationMemberAndCustomerPairKey,
                    out var organizationMembers))
            {
                if (organizationMembers is ExternalMultiSelectValue value)
                {
                    if (value.SelectedOptions.Count == 0)
                    {
                        throw new ArgumentException("No members selected.");
                    }

                    addInput.Members.AddRange(value.SelectedOptions
                        .Select(item =>
                        {
                            var memberCustomerIdPair = item.Value.Split(Global.OptionLoaderValueSeparator);
                            var organizationMemberId = memberCustomerIdPair.First();
                            var customerId = memberCustomerIdPair.Last();

                            ArgumentException.ThrowIfNullOrWhiteSpace(organizationMemberId);
                            ArgumentException.ThrowIfNullOrWhiteSpace(customerId);

                            return new TeamMember
                            {
                                Id = randomHelper.Generate(),
                                Role = Role.Member,
                                Customer = new Customer { Id = customerId },
                                OrganizationMember = new OrganizationMember
                                {
                                    Id = organizationMemberId, Customer = new Customer { Id = customerId }
                                }
                            };
                        }));
                }
                else
                {
                    throw new InvalidOperationException("timezone must be ExternalSelectValue");
                }
            }
            else
            {
                throw new InvalidOperationException("timezone block is missing");
            }
        }
        else
        {
            throw new InvalidOperationException("timezone block is missing");
        }

        if (values.TryGetValue(TeamActionTypes.SlackUpdateChannel, out var slackUpdateChannelBlock))
        {
            if (slackUpdateChannelBlock.TryGetValue(TeamActionTypes.SlackUpdateChannel, out var slackUpdateChannel))
            {
                if (slackUpdateChannel is ChannelSelectValue value)
                {
                    var teamEntity = await repositoryFactory.TeamRepository.UpsertNakedAsync(teamId, cancellationToken);
                    teamEntity.DailyUpdateChannel = string.IsNullOrWhiteSpace(value.SelectedChannel)
                        ? null
                        : await workspaceChannelService.EnsureChannelResourcesAllExistAsync(
                            workspaceEntity,
                            value.SelectedChannel,
                            cancellationToken);
                    await repositoryFactory.TeamRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
                }
                else
                {
                    throw new InvalidOperationException("slack update channel must be ExternalSelectValue");
                }
            }
            else
            {
                throw new InvalidOperationException("slack update channel block is missing");
            }
        }
        else
        {
            throw new InvalidOperationException("slack update channel block is missing");
        }

        await teamServiceClient.AddAsync(
            addInput,
            teamConfiguration.ApiKey.CreateMetadata(workspaceMember.Id),
            cancellationToken: cancellationToken);

        await pageNavigator.BackAsync(
            workspace,
            workspaceMember,
            new CommonPageContext(context.PageContext),
            viewSubmission.Hash, cancellationToken);

        return ViewSubmissionResponse.Null;
    }

    public Task HandleClose(ViewClosed viewClosed) => Task.CompletedTask;
}
