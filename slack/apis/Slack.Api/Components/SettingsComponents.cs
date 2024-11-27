using Api.Shared.Models;
using Enterprise.Shared.Time;
using Slack.Api.Services;
using Slack.Shared.Constants;
using Slack.Shared.Context;
using Slack.Shared.Models;
using SlackNet.Blocks;

namespace Slack.Api.Components;

public interface ISettingsComponents
{
    Task<ICollection<Block>> GetDefaultLocationOnboardingDoneAsync(
        WorkspaceMember workspaceMember,
        PageContext pageContext,
        CancellationToken cancellationToken);

    Task<ICollection<Block>> GetPreferredZoneOnboardingDoneAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        PageContext pageContext,
        CancellationToken cancellationToken);

    Task<ICollection<Block>> GetPreferredDeskOnboardingDoneAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        PageContext pageContext,
        CancellationToken cancellationToken);
}

public class SettingsComponents(
    ICustomerService customerService,
    ILocationService locationService,
    TimeProvider timeProvider) : ISettingsComponents
{
    public async Task<ICollection<Block>> GetDefaultLocationOnboardingDoneAsync(
        WorkspaceMember workspaceMember,
        PageContext pageContext,
        CancellationToken cancellationToken)
    {
        pageContext = pageContext.PushCurrentPageToVisitedPagesAndClone();
        var customer = await customerService.GetAsync(workspaceMember, cancellationToken);
        ArgumentNullException.ThrowIfNull(customer);

        if (customer.IsDefaultLocationOnboardingDone.HasValue && customer.IsDefaultLocationOnboardingDone.Value)
        {
            return [];
        }

        pageContext.LocationsPage = new LocationsPage(new PaginationContext());

        return
        [
            new SectionBlock
            {
                Text = "*Setting default location makes make a bookings easier. Setup one now?*".ToMarkdown()
            },
            new ActionsBlock
            {
                Elements =
                [
                    new Button
                    {
                        ActionId = LocationActionTypes.Locations,
                        Text = "Setup".ToPlainTextWithIcon(Icons.Settings),
                        Value = new CommonPageContext(pageContext).Serialize()
                    },
                    new Button
                    {
                        ActionId = LocationActionTypes.DismissSetupDefaultLocation,
                        Text = "Dismiss".ToPlainTextWithIcon(Icons.Cancel),
                        Value = new CommonPageContext(pageContext).Serialize()
                    }
                ]
            }
        ];
    }

    public async Task<ICollection<Block>> GetPreferredZoneOnboardingDoneAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        PageContext pageContext,
        CancellationToken cancellationToken)
    {
        pageContext = pageContext.PushCurrentPageToVisitedPagesAndClone();
        var customer = await customerService.GetAsync(workspaceMember, cancellationToken);
        ArgumentNullException.ThrowIfNull(customer);
        if (customer.IsPreferredZoneOnboardingDone.HasValue && customer.IsPreferredZoneOnboardingDone.Value)
        {
            return [];
        }

        var locations = await locationService.GetLocationsAsync(workspace, workspaceMember, cancellationToken);
        var locationsWithZones = locations
            .Where(item => item.Tags.Count(tag => tag.Type == LocationTagType.Zone) != 0)
            .ToList();
        if (locationsWithZones.Count == 0)
        {
            return [];
        }

        pageContext.ZonesPage = new ZonesPage(new PaginationContext());

        return
        [
            new SectionBlock
            {
                Text = "*Setting preferred zones makes make a bookings easier. Setup one now?*".ToMarkdown()
            },
            new ActionsBlock
            {
                Elements =
                [
                    new Button
                    {
                        ActionId = ZoneActionTypes.Zones,
                        Text = "Setup".ToPlainTextWithIcon(Icons.Settings),
                        Value = new CommonPageContext(pageContext).Serialize()
                    },
                    new Button
                    {
                        ActionId = ZoneActionTypes.DismissSetupPreferredZones,
                        Text = "Dismiss".ToPlainTextWithIcon(Icons.Cancel),
                        Value = new CommonPageContext(pageContext).Serialize()
                    }
                ]
            }
        ];
    }

    public async Task<ICollection<Block>> GetPreferredDeskOnboardingDoneAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        PageContext pageContext,
        CancellationToken cancellationToken)
    {
        pageContext = pageContext.PushCurrentPageToVisitedPagesAndClone();
        var customer = await customerService.GetAsync(workspaceMember, cancellationToken);
        ArgumentNullException.ThrowIfNull(customer);
        if (customer.IsPreferredDeskOnboardingDone.HasValue && customer.IsPreferredDeskOnboardingDone.Value)
        {
            return [];
        }

        var locations = await locationService.GetLocationsAsync(workspace, workspaceMember, cancellationToken);
        var locationsWithDesks = locations
            .Where(item => item.Desks.Count != 0)
            .ToList();
        if (locationsWithDesks.Count == 0)
        {
            return [];
        }

        var locationId = locationsWithDesks.First().Id;
        var startOfToday = timeProvider.GetUtcNow().StartOfDay(TimeZoneInfo.Utc);
        pageContext.DesksPage = new DesksPage(new PaginationContext(), locationId, startOfToday);

        return
        [
            new SectionBlock
            {
                Text = "*Setting preferred desks makes make a bookings easier. Setup one now?*".ToMarkdown()
            },
            new ActionsBlock
            {
                Elements =
                [
                    new Button
                    {
                        ActionId = DeskActionTypes.Desks,
                        Text = "Setup".ToPlainTextWithIcon(Icons.Settings),
                        Value = new CommonPageContext(pageContext).Serialize()
                    },
                    new Button
                    {
                        ActionId = DeskActionTypes.DismissSetupPreferredDesks,
                        Text = "Dismiss".ToPlainTextWithIcon(Icons.Cancel),
                        Value = new CommonPageContext(pageContext).Serialize()
                    }
                ]
            }
        ];
    }
}
