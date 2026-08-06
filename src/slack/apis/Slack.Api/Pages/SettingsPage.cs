using Api.Shared.Services;
using Enterprise.Shared;
using Slack.Api.Components;
using Slack.Api.Mappers;
using Slack.Api.Services;
using Slack.Shared;
using Slack.Shared.Configurations;
using Slack.Shared.Constants;
using Slack.Shared.Context;
using Slack.Shared.Repositories;
using Slack.Shared.Services.Cache;
using Slack.Shared.Services.CrossDomains;
using SlackNet;
using SlackNet.AspNetCore;
using SlackNet.Blocks;
using SlackNet.Interaction;
using Icons = Slack.Shared.Constants.Icons;
using Option = SlackNet.Blocks.Option;
using Workspace = Slack.Shared.Models.Workspace;
using WorkspaceMember = Slack.Shared.Models.WorkspaceMember;

namespace Slack.Api.Pages;

public interface ISettingsPage
{
    Task RenderWithContextAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        CommonPageContext commonPageContext,
        string? hash,
        CancellationToken cancellationToken);
}

public class SettingsPage(
    AsyncPageRenderingService asyncPageRenderingService,
    SlackConfigurationService slackConfigurationService,
    IWorkspaceMemberService workspaceMemberService,
    IRepositoryFactory repositoryFactory,
    IEntityMapper entityMapper,
    ICommonComponents commonComponents,
    IOrganizationPermissionsService organizationPermissionsService,
    IWorkspaceChannelService workspaceChannelService,
    ICachedOrganizationService cachedOrganizationService,
    IOrganizationBillingService organizationBillingService) :
    ITeamsPage,
    IAsyncPageRenderingCallbacks,
    ISettingsPage,
    IBlockActionHandler<StaticSelectAction>,
    IBlockActionHandler<ChannelSelectAction>,
    IBlockActionHandler<CheckboxGroupAction>
{
    private const string SettingsCallback = "Settings";
    private const string ActionsMenu = "Settings_ActionsMenu";
    private const string AutomaticallyUpdateProfileStatus = "AutomaticallyUpdateProfileStatus";
    private const string UpdateOrganizationSlackUpdateChannel = "UpdateOrganizationSlackUpdateChannel";

    public async Task HandleAsync(ChannelSelectAction action, BlockActionRequest request, CancellationToken cancellationToken)
    {
        var workspaceEntity = await repositoryFactory.WorkspaceRepository.GetByIdAsync(request.Team.Id, cancellationToken) ??
                              throw new SlackWorkspaceNotFound();
        _ = await workspaceMemberService.EnsureCustomerResourcesAllExistAsync(workspaceEntity, request.User.Id, cancellationToken);

        switch (action.ActionId)
        {
            case UpdateOrganizationSlackUpdateChannel:
                workspaceEntity.Organization.DailyUpdateChannel = string.IsNullOrWhiteSpace(action.SelectedChannel)
                    ? null
                    : await workspaceChannelService.EnsureChannelResourcesAllExistAsync(
                        workspaceEntity,
                        action.SelectedChannel,
                        cancellationToken);

                repositoryFactory.OrganizationRepository.Update(workspaceEntity.Organization);
                await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
                await cachedOrganizationService.UpdateByIdOrCustomDomainAsync(
                    workspaceEntity.Organization.Id,
                    workspaceEntity.Organization.CustomDomain,
                    cancellationToken);

                break;
        }
    }

    public async Task HandleAsync(CheckboxGroupAction action, BlockActionRequest request, CancellationToken cancellationToken)
    {
        var workspaceEntity = await repositoryFactory.WorkspaceRepository.GetByIdAsync(request.Team.Id, cancellationToken) ??
                              throw new SlackWorkspaceNotFound();
        var (workspaceMemberEntity, _) = await workspaceMemberService.EnsureCustomerResourcesAllExistAsync(
            workspaceEntity,
            request.User.Id,
            cancellationToken);

        switch (action.ActionId)
        {
            case AutomaticallyUpdateProfileStatus:
                var selectedOption = action.SelectedOptions.FirstOrDefault();
                workspaceMemberEntity.AutomaticallyUpdateProfileStatus = selectedOption is not null &&
                                                                         selectedOption.Value ==
                                                                         AutomaticallyUpdateProfileStatus;

                repositoryFactory.WorkspaceMemberRepository.Update(workspaceMemberEntity);
                await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
                break;
        }
    }

    public async Task HandleAsync(StaticSelectAction action, BlockActionRequest request, CancellationToken cancellationToken)
    {
        var workspaceEntity = await repositoryFactory.WorkspaceRepository.GetByIdAsync(request.Team.Id, cancellationToken) ??
                              throw new SlackWorkspaceNotFound();
        var (workspaceMemberEntity, _) = await workspaceMemberService.EnsureCustomerResourcesAllExistAsync(
            workspaceEntity,
            request.User.Id,
            cancellationToken);

        var workspace = entityMapper.MapTo(workspaceEntity);
        var workspaceMember = entityMapper.MapTo(workspaceMemberEntity, workspace);
        switch (action.SelectedOption.Value)
        {
            case BillingActionTypes.Billing:
                {
                    var permissions =
                        await organizationPermissionsService.GetPermissionsAsync(workspaceMember.Id, workspace.Organization.Id, cancellationToken);
                    var context = CommonPageContext.Deserialize(request.View.PrivateMetadata);
                    context.PageContext.PushCurrentPageToVisitedPages();
                    if (permissions.CanModify)
                    {
                        await OpenEditBillingInfoDialogAsync(workspace, workspaceMember, request.TriggerId, context, cancellationToken);
                    }
                    else
                    {
                        await OpenViewBillingInfoDialogAsync(workspace, workspaceMember, request.TriggerId, context, cancellationToken);
                    }
                }

                break;
        }
    }

    public async Task Handle(ChannelSelectAction action, BlockActionRequest request)
    {
        if (slackConfigurationService.EnableAsyncMode)
        {
            asyncPageRenderingService.ChannelSelectActionHandlerStream.OnNext((GetType(), action, request));
        }
        else
        {
            await HandleAsync(action, request, CancellationToken.None);
        }
    }

    public async Task Handle(CheckboxGroupAction action, BlockActionRequest request)
    {
        if (slackConfigurationService.EnableAsyncMode)
        {
            asyncPageRenderingService.CheckboxGroupActionHandlerStream.OnNext((GetType(), action, request));
        }
        else
        {
            await HandleAsync(action, request, CancellationToken.None);
        }
    }

    public async Task Handle(StaticSelectAction action, BlockActionRequest request)
    {
        if (slackConfigurationService.EnableAsyncMode)
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
        ArgumentNullException.ThrowIfNull(commonPageContext.PageContext.SettingsPage);

        commonPageContext.PageContext.CurrentPageType = PageType.Settings;

        IReadOnlyList<Block>[] blocks =
        [
            GetTitle(),
            await GetToolbarAsync(workspace, workspaceMember, commonPageContext.PageContext, cancellationToken),
            GetWorkspaceMemberSettings(workspaceMember),
            GetOrganizationSettings(workspace),
        ];

        var slackApiClient = workspace.GetApiClient();
        await slackApiClient.ViewsPublishAsync(
            workspaceMember.Id,
            new HomeViewDefinition
            {
                CallbackId = SettingsCallback,
                Blocks = blocks.SelectMany(item => item.Count == 0 ? item : item.Append(new DividerBlock())).SkipLast(1).ToList(),
                PrivateMetadata = commonPageContext.Serialize(),
            },
            hash,
            cancellationToken);
    }

    public static void RegisterHandlers(AspNetSlackServiceConfiguration options) =>
        options
            .RegisterBlockActionHandler<StaticSelectAction, SettingsPage>(ActionsMenu)
            .RegisterBlockActionHandler<CheckboxGroupAction, SettingsPage>(AutomaticallyUpdateProfileStatus)
            .RegisterBlockActionHandler<ChannelSelectAction, SettingsPage>(UpdateOrganizationSlackUpdateChannel);

    private static IReadOnlyList<Block> GetTitle() =>
    [
        new SectionBlock
        {
            Text = "*Settings*".ToMarkdown(),
        },
    ];

    private async Task<IReadOnlyList<Block>> GetToolbarAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        PageContext pageContext,
        CancellationToken cancellationToken)
    {
        var homeAndBackButtons = commonComponents.GetHomeAndBackButtons(pageContext, workspaceMember.Timezone);
        var feedbackButton = commonComponents.GetFeedbackButton(pageContext);
        var actionMenus = new List<StaticSelectMenu>();
        var permissions = await organizationPermissionsService.GetPermissionsAsync(workspaceMember.Id, workspace.Organization.Id, cancellationToken);
        if (permissions.CanView)
        {
            actionMenus.Add(new StaticSelectMenu
            {
                ActionId = ActionsMenu,
                Placeholder = "Go to...".ToPlainTextWithIcon(Icons.Goto),
                Options =
                [
                    new Option
                    {
                        Value = BillingActionTypes.Billing,
                        Text = "Billing".ToPlainTextWithIcon(Icons.Billing),
                    },
                ],
            });
        }

        return
        [
            new ActionsBlock
            {
                Elements = new List<IActionElement>().Concat(homeAndBackButtons).Concat(feedbackButton).Concat(actionMenus).ToList(),
            },
        ];
    }

    private static IReadOnlyList<Block> GetWorkspaceMemberSettings(WorkspaceMember workspaceMember)
    {
        var title = new SectionBlock
        {
            Text = "*Personal settings*".ToMarkdown(),
        };
        var automaticallyUpdateProfileStatusOption = new Option
        {
            Text = "Automatically update profile status".ToPlainText(),
            Value = AutomaticallyUpdateProfileStatus,
        };
        var automaticallyUpdateProfileStatus = new ActionsBlock
        {
            Elements =
            [
                new CheckboxGroup
                {
                    ActionId = AutomaticallyUpdateProfileStatus,
                    Options = new List<Option>
                    {
                        automaticallyUpdateProfileStatusOption,
                    },
                    InitialOptions =
                        workspaceMember.AutomaticallyUpdateProfileStatus is null || !workspaceMember.AutomaticallyUpdateProfileStatus.Value
                            ? []
                            : [automaticallyUpdateProfileStatusOption],
                },
            ],
        };

        return
        [
            title,
            automaticallyUpdateProfileStatus,
        ];
    }

    private static IReadOnlyList<Block> GetOrganizationSettings(Workspace workspace)
    {
        var title = new SectionBlock
        {
            Text = "*Organization settings*".ToMarkdown(),
        };
        var slackUpdateChannelTitle = new SectionBlock
        {
            Text = "*Slack update channel*".ToMarkdown(),
        };
        var channels = new ActionsBlock
        {
            Elements =
            [
                new ChannelSelectMenu
                {
                    ActionId = UpdateOrganizationSlackUpdateChannel,
                    InitialChannel = workspace.Organization.DailyUpdateChannel?.Id,
                },
            ],
        };

        return
        [
            title,
            slackUpdateChannelTitle,
            channels,
        ];
    }

    private async Task OpenViewBillingInfoDialogAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        string triggerId,
        CommonPageContext commonPageContext,
        CancellationToken cancellationToken)
    {
        var organizationBillingDetails = await organizationBillingService.GetAsync(workspaceMember.Id, workspace.Organization.Id, cancellationToken);
        var email = new SectionBlock
        {
            Text = $"Email: {organizationBillingDetails.Email.ToSafeString()}".ToPlainText(),
        };
        var addressLine1 = new SectionBlock
        {
            Text = $"Address Line 1: {organizationBillingDetails.AddressLine1.ToSafeString()}".ToPlainText(),
        };
        var addressLine2 = new SectionBlock
        {
            Text = $"Address Line 2: {organizationBillingDetails.AddressLine2.ToSafeString()}".ToPlainText(),
        };
        var suburb = new SectionBlock
        {
            Text = $"Suburb: {organizationBillingDetails.Suburb.ToSafeString()}".ToPlainText(),
        };
        var city = new SectionBlock
        {
            Text = $"City: {organizationBillingDetails.City.ToSafeString()}".ToPlainText(),
        };
        var province = new SectionBlock
        {
            Text = $"Province: {organizationBillingDetails.Province.ToSafeString()}".ToPlainText(),
        };
        var zipcode = new SectionBlock
        {
            Text = $"Zipcode: {organizationBillingDetails.Zipcode.ToSafeString()}".ToPlainText(),
        };
        var country = new SectionBlock
        {
            Text = $"Country: {organizationBillingDetails.Country.ToSafeString()}".ToPlainText(),
        };

        var slackApiClient = workspace.GetApiClient();
        await slackApiClient.ViewsOpenAsync(
            triggerId,
            new ModalViewDefinition
            {
                CallbackId = BillingCallbackTypes.ViewBilling,
                Title = "View Billing",
                Close = "Cancel",
                Submit = "Close",
                Blocks = [email, new DividerBlock(), addressLine1, addressLine2, suburb, city, province, zipcode, country],
                PrivateMetadata = commonPageContext.Serialize(),
            },
            cancellationToken);
    }

    private async Task OpenEditBillingInfoDialogAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        string triggerId,
        CommonPageContext commonPageContext,
        CancellationToken cancellationToken)
    {
        var organizationBillingDetails = await organizationBillingService.GetAsync(workspaceMember.Id, workspace.Organization.Id, cancellationToken);
        var companyName = new InputBlock
        {
            BlockId = BillingActionTypes.CompanyName,
            Label = "Company name".ToPlainText(),
            Element = new PlainTextInput
            {
                ActionId = BillingActionTypes.CompanyName,
                InitialValue = string.IsNullOrWhiteSpace(organizationBillingDetails.CompanyName)
                    ? null
                    : organizationBillingDetails.CompanyName.ToSafeString(),
            },
            Optional = false,
        };

        var email = new InputBlock
        {
            BlockId = BillingActionTypes.Email,
            Label = "Email".ToPlainText(),
            Element = new EmailTextInput
            {
                ActionId = BillingActionTypes.Email,
                InitialValue = string.IsNullOrWhiteSpace(organizationBillingDetails.Email)
                    ? null
                    : organizationBillingDetails.Email.ToSafeString(),
            },
            Optional = false,
        };

        var addressLine1 = new InputBlock
        {
            BlockId = BillingActionTypes.AddressLine1,
            Label = "Address line 1".ToPlainText(),
            Element = new PlainTextInput
            {
                ActionId = BillingActionTypes.AddressLine1,
                InitialValue = organizationBillingDetails.AddressLine1.ToSafeString(),
            },
            Optional = true,
        };

        var addressLine2 = new InputBlock
        {
            BlockId = BillingActionTypes.AddressLine2,
            Label = "Address line 2".ToPlainText(),
            Element = new PlainTextInput
            {
                ActionId = BillingActionTypes.AddressLine2,
                InitialValue = organizationBillingDetails.AddressLine2.ToSafeString(),
            },
            Optional = true,
        };

        var suburb = new InputBlock
        {
            BlockId = BillingActionTypes.Suburb,
            Label = "Suburb".ToPlainText(),
            Element =
                new PlainTextInput
                {
                    ActionId = BillingActionTypes.Suburb,
                    InitialValue = organizationBillingDetails.Suburb.ToSafeString(),
                },
            Optional = true,
        };

        var city = new InputBlock
        {
            BlockId = BillingActionTypes.City,
            Label = "City".ToPlainText(),
            Element = new PlainTextInput
            {
                ActionId = BillingActionTypes.City,
                InitialValue = organizationBillingDetails.City.ToSafeString(),
            },
            Optional = true,
        };

        var province = new InputBlock
        {
            BlockId = BillingActionTypes.Province,
            Label = "Province".ToPlainText(),
            Element = new PlainTextInput
            {
                ActionId = BillingActionTypes.Province,
                InitialValue = organizationBillingDetails.Province.ToSafeString(),
            },
            Optional = true,
        };

        var zipcode = new InputBlock
        {
            BlockId = BillingActionTypes.Zipcode,
            Label = "Zipcode".ToPlainText(),
            Element = new PlainTextInput
            {
                ActionId = BillingActionTypes.Zipcode,
                InitialValue = organizationBillingDetails.Zipcode.ToSafeString(),
            },
            Optional = true,
        };

        var country = new InputBlock
        {
            BlockId = OptionLoaderKeys.CountryKey,
            Label = "Country".ToPlainText(),
            Element = new ExternalSelectMenu
            {
                ActionId = OptionLoaderKeys.CountryKey,
                InitialOption = string.IsNullOrWhiteSpace(organizationBillingDetails.Country)
                    ? null
                    : new Option
                    {
                        Text = organizationBillingDetails.Country.ToOptionText(),
                        Value = organizationBillingDetails.Country,
                    },
                MinQueryLength = 3,
            },
            Optional = true,
        };

        var slackApiClient = workspace.GetApiClient();
        await slackApiClient.ViewsOpenAsync(
            triggerId,
            new ModalViewDefinition
            {
                CallbackId = BillingCallbackTypes.EditBilling,
                Title = "Edit Billing",
                Close = "Cancel",
                Submit = "Save",
                Blocks =
                [
                    companyName, email, new DividerBlock(), addressLine1, addressLine2, suburb, city, province, zipcode, country,
                ],
                PrivateMetadata = commonPageContext.Serialize(),
            },
            cancellationToken);
    }
}
