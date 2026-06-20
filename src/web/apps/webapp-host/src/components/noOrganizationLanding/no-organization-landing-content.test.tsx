import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import NoOrganizationLandingContent from './no-organization-landing-content';

const pushMock = vi.fn();
const usePreloadedQueryMock = vi.fn();
const useFragmentMock = vi.fn();

vi.mock('next/navigation', () => ({
  useRouter: () => ({ push: pushMock }),
}));

vi.mock(import('@skedular/shared'), async (importOriginal) => {
  const actual = await importOriginal();

  return {
    ...actual,
    useIntegratedPlatform: () => ({ integratedPlatform: 'spaces' }),
  };
});

vi.mock('react-relay', () => ({
  graphql: (value: TemplateStringsArray) => value[0],
  usePreloadedQuery: (...args: unknown[]) => usePreloadedQueryMock(...args),
  useFragment: (...args: unknown[]) => useFragmentMock(...args),
}));

describe('NoOrganizationLandingContent', () => {
  beforeEach(() => {
    pushMock.mockReset();
    usePreloadedQueryMock.mockReset();
    useFragmentMock.mockReset();
    vi.spyOn(console, 'info').mockImplementation(() => undefined);

    usePreloadedQueryMock.mockReturnValue({
      me: {
        id: 'user-1',
        isOnboardingDone: true,
      },
    });
  });

  it('renders the no-organization create prompt', () => {
    useFragmentMock.mockReturnValue({ myOrganizations: [] });

    render(<NoOrganizationLandingContent queryRef={{} as never} />);

    expect(screen.getByText('Create your first workspace organization')).toBeInTheDocument();
    expect(
      screen.getByText('Host is for managing your organization, locations, products, bookings, renters, and payouts. Booking resources are managed automatically.'),
    ).toBeInTheDocument();
    expect(screen.getByRole('link', { name: 'Add Organization' })).toHaveAttribute('href', 'spaces/organizations/add-marketplace');
  });

  it('renders one organization with the correct heading, CTA, and navigation target', async () => {
    useFragmentMock.mockReturnValue({
      myOrganizations: [{ name: 'Acme', uniqueId: 'org-1', customDomain: 'acme', logoUrl: null }],
    });

    render(<NoOrganizationLandingContent queryRef={{} as never} />);

    expect(screen.getByText('Select your workspace organization')).toBeInTheDocument();
    expect(screen.getByRole('link', { name: 'Add Organization' })).toHaveAttribute('href', 'spaces/organizations/add-marketplace');
    expect(screen.getByText('Acme')).toBeInTheDocument();

    await userEvent.click(screen.getByRole('button', { name: /Acme/i }));

    expect(pushMock).toHaveBeenCalledWith('spaces/organizations/acme');
  });

  it('renders multiple organizations as selectable cards', () => {
    useFragmentMock.mockReturnValue({
      myOrganizations: [
        { name: 'Acme', uniqueId: 'org-1', customDomain: 'acme', logoUrl: null },
        { name: 'Beta', uniqueId: 'org-2', customDomain: null, logoUrl: null },
      ],
    });

    render(<NoOrganizationLandingContent queryRef={{} as never} />);

    expect(screen.getByText('Select a workspace organization')).toBeInTheDocument();
    expect(screen.getByText('Acme')).toBeInTheDocument();
    expect(screen.getByText('Beta')).toBeInTheDocument();
  });

  it('does not render content while onboarding redirect has priority', () => {
    usePreloadedQueryMock.mockReturnValue({
      me: {
        id: 'user-1',
        isOnboardingDone: false,
      },
    });
    useFragmentMock.mockReturnValue({ myOrganizations: [{ name: 'Acme', uniqueId: 'org-1', customDomain: 'acme', logoUrl: null }] });

    const { container } = render(<NoOrganizationLandingContent queryRef={{} as never} />);

    expect(container).toBeEmptyDOMElement();
  });
});
