import { fireEvent, render, screen } from '@testing-library/react';
import { describe, expect, it, vi } from 'vitest';
import OrganizationTeamSectionNav from './organization-team-section-nav';

const mockMatchMedia = (matches: boolean) => {
  Object.defineProperty(window, 'matchMedia', {
    writable: true,
    value: vi.fn().mockImplementation((query: string) => ({
      matches,
      media: query,
      onchange: null,
      addListener: vi.fn(),
      removeListener: vi.fn(),
      addEventListener: vi.fn(),
      removeEventListener: vi.fn(),
      dispatchEvent: vi.fn(),
    })),
  });
};

vi.mock('next/link', () => ({
  default: ({ children, href, ...props }: React.AnchorHTMLAttributes<HTMLAnchorElement>) => (
    <a href={typeof href === 'string' ? href : '#'} {...props}>
      {children}
    </a>
  ),
}));

vi.mock(import('@skedular/shared'), async (importOriginal) => {
  const actual = await importOriginal();

  return {
    ...actual,
    useIntegratedPlatrform: () => ({ integratedPlatrform: 'web' }),
  };
});

vi.mock('@/components/links', () => ({
  getOrganizationBookingsBaseLink: () => '/organizations/acme/bookings?teamId=team-1',
  getOrganizationTeamSetupBaseLink: () => '/organizations/acme/teams/team-1?section=setup',
  getOrganizationTeamMembersBaseLink: () => '/organizations/acme/teams/team-1?section=members',
  getOrganizationTeamManageTeamBaseLink: () => '/organizations/acme/teams/team-1?section=manage-team',
}));

describe('OrganizationTeamSectionNav', () => {
  it('renders route-backed section links and includes the team bookings action', () => {
    mockMatchMedia(false);

    render(<OrganizationTeamSectionNav activeSection="members" organizationCustomDomain="acme" teamId="team-1" stickyTop={64} />);

    const membersTab = screen.getByRole('link', { name: 'Members' });
    const setupTab = screen.getByRole('link', { name: 'Team Setup' });
    const bookingsLink = screen.getByRole('link', { name: 'View team bookings' });

    expect(membersTab).toHaveAttribute('href', '/organizations/acme/teams/team-1?section=members');
    expect(setupTab).toHaveAttribute('href', '/organizations/acme/teams/team-1?section=setup');
    expect(bookingsLink).toHaveAttribute('href', '/organizations/acme/bookings?teamId=team-1');
    expect(membersTab.className).toContain('MuiButton-contained');
    expect(setupTab.className).toContain('MuiButton-text');
  });

  it('collapses sections into a menu on narrower screens', () => {
    mockMatchMedia(true);

    render(<OrganizationTeamSectionNav activeSection="members" organizationCustomDomain="acme" teamId="team-1" stickyTop={64} />);

    fireEvent.click(screen.getByRole('button', { name: 'Section: Members' }));

    const setupMenuItem = screen.getByRole('menuitem', { name: 'Team Setup' });
    const membersMenuItem = screen.getByRole('menuitem', { name: 'Members' });

    expect(setupMenuItem).toHaveAttribute('href', '/organizations/acme/teams/team-1?section=setup');
    expect(membersMenuItem).toHaveAttribute('href', '/organizations/acme/teams/team-1?section=members');
    expect(membersMenuItem.className).toContain('Mui-selected');
  });
});
