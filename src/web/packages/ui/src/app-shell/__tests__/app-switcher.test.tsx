import MenuList from '@mui/material/MenuList';
import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { describe, expect, it, vi } from 'vitest';
import AppSwitcher, { type AppSwitcherModel } from '../app-switcher';

const createModel = (overrides?: Partial<AppSwitcherModel>): AppSwitcherModel => ({
  currentAppId: 'webapp',
  availableDestinationCount: 2,
  hasSwitchTargets: true,
  destinations: [
    {
      appId: 'webapp',
      displayName: 'Skedular',
      shortName: 'Skedular',
      isCurrent: true,
      availability: 'current',
      href: 'https://app.skedular.test/',
    },
    {
      appId: 'webapp-teams',
      displayName: 'Skedular Teams',
      shortName: 'Teams',
      isCurrent: false,
      availability: 'available',
      href: 'https://teams.skedular.test/',
    },
    {
      appId: 'webapp-spaces',
      displayName: 'Skedular Spaces',
      shortName: 'Spaces',
      isCurrent: false,
      availability: 'available',
      href: 'https://spaces.skedular.test/',
    },
  ],
  ...overrides,
});

describe('AppSwitcher', () => {
  it('renders active app destination links and invokes destination selection', async () => {
    const user = userEvent.setup();
    const onDestinationSelect = vi.fn();

    render(<AppSwitcher model={createModel()} onDestinationSelect={onDestinationSelect} />);

    await user.click(screen.getByRole('button', { name: 'Switch app' }));
    const teamsDestination = screen.getByRole('menuitem', { name: /Skedular Teams/ });

    expect(teamsDestination).toHaveAttribute('href', 'https://teams.skedular.test/');
    await user.click(teamsDestination);
    expect(onDestinationSelect).toHaveBeenCalledWith(expect.objectContaining({ appId: 'webapp-teams' }));
  });

  it('labels the current app without rendering it as an active switch link', async () => {
    const user = userEvent.setup();

    render(<AppSwitcher model={createModel()} />);

    await user.click(screen.getByRole('button', { name: 'Switch app' }));

    expect(screen.getByText('Current app')).toBeInTheDocument();
    expect(screen.getByText('Current')).toBeInTheDocument();
    expect(screen.getByText('Skedular').closest('li')).toHaveAttribute('aria-disabled', 'true');
  });

  it('hides inactive destinations and omits the switcher when no active targets exist', async () => {
    const { rerender } = render(
      <AppSwitcher
        model={createModel({
          destinations: [
            {
              appId: 'webapp',
              displayName: 'Skedular',
              shortName: 'Skedular',
              isCurrent: true,
              availability: 'current',
            },
            {
              appId: 'webapp-teams',
              displayName: 'Skedular Teams',
              shortName: 'Teams',
              isCurrent: false,
              availability: 'invalid-url',
            },
          ],
          availableDestinationCount: 0,
          hasSwitchTargets: false,
        })}
      />,
    );

    expect(screen.queryByRole('button', { name: 'Switch app' })).not.toBeInTheDocument();

    rerender(<AppSwitcher model={createModel()} />);
    await userEvent.click(screen.getByRole('button', { name: 'Switch app' }));
    expect(screen.queryByText('Invalid app')).not.toBeInTheDocument();
  });

  it('renders an accessible icon button for compact navigation', async () => {
    const user = userEvent.setup();

    render(<AppSwitcher model={createModel()} buttonMode="icon" />);

    await user.click(screen.getByRole('button', { name: 'Switch app' }));

    expect(screen.getByRole('menu', { name: 'Skedular app switcher' })).toBeInTheDocument();
    expect(screen.getByRole('menuitem', { name: /Skedular Teams/ })).toHaveAttribute('href', 'https://teams.skedular.test/');
  });

  it('renders as a profile menu item trigger', async () => {
    const user = userEvent.setup();

    render(
      <MenuList>
        <AppSwitcher model={createModel()} buttonMode="menu-item" />
      </MenuList>,
    );

    await user.click(screen.getByRole('menuitem', { name: 'Switch app' }));

    expect(screen.getByRole('menu', { name: 'Skedular app switcher' })).toBeInTheDocument();
    expect(screen.getByRole('menuitem', { name: /Skedular Spaces/ })).toHaveAttribute('href', 'https://spaces.skedular.test/');
  });
});
