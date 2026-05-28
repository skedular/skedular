import { render, screen } from '@testing-library/react';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import TeamCard from './team-card';

const pushMock = vi.fn();
const useFragmentMock = vi.fn();

const teamFragmentData = {
  id: 'team-1',
  name: 'Operations',
  members: { edges: [] },
  featureImages: [],
  canModify: true,
  canDelete: true,
  organization: { customDomain: 'acme' },
};

vi.mock('next/navigation', () => ({
  useRouter: () => ({ push: pushMock }),
}));

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
  getOrganizationBookingsBaseLink: () => '/bookings',
  getOrganizationTeamSetupBaseLink: () => '/teams/setup',
}));

vi.mock('@/components/moreActionsMenu', () => ({
  MoreActionsMenu: () => null,
  moreActionsMenuAllOptions: {
    EditTeam: [{ id: 'EditTeam', label: 'Edit Team' }],
    DeleteTeam: [{ id: 'DeleteTeam', label: 'Delete Team' }],
    ViewTeamBookings: [{ id: 'ViewTeamBookings', label: 'View Team Bookings' }],
  },
  MoreActionsMenuOptionType: {
    EditTeam: 'EditTeam',
    DeleteTeam: 'DeleteTeam',
    ViewTeamBookings: 'ViewTeamBookings',
  },
}));

vi.mock('@/components/icons', () => ({
  EllipseMenuIcon: () => <span>menu</span>,
  LocationIcon: () => <span>location-icon</span>,
  TeamIcon: () => <span>team-icon</span>,
}));

vi.mock('react-relay', () => ({
  graphql: (value: TemplateStringsArray) => value[0],
  useFragment: (...args: unknown[]) => useFragmentMock(...args),
  useMutation: () => [vi.fn()],
}));

describe('TeamCard', () => {
  beforeEach(() => {
    useFragmentMock.mockReset();
    pushMock.mockReset();
    useFragmentMock.mockImplementation(() => teamFragmentData);
  });

  it('renders the modern compact team card layout', () => {
    render(
      <TeamCard
        teamDetailsRelay={{} as never}
        connectionIds={[]}
        teammates={[
          {
            id: 'customer-1',
            name: 'Alex',
            photoUrl: null,
          },
        ]}
      />,
    );

    expect(screen.getByText('Operations')).toBeInTheDocument();
    expect(screen.getByText('Members')).toBeInTheDocument();
    expect(screen.getByText('1 member')).toBeInTheDocument();
    expect(screen.queryByText('View bookings')).not.toBeInTheDocument();
  });
});
