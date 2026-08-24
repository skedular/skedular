import { fireEvent, render, screen } from '@testing-library/react';
import { describe, expect, it, vi } from 'vitest';
import { usePreloadedQuery } from 'react-relay';
import { SpacesQuotaStatusInner } from '@/components/organization/organizationSettings/organization-spaces-quota-status';
import { SpacesQuotaUpgradePrompt } from '../spaces-quota-upgrade-prompt';

vi.mock('react-relay', () => ({
  graphql: (strings: TemplateStringsArray) => strings.join(''),
  usePreloadedQuery: vi.fn(() => ({
    bookingSpacesQuotaStatus: {
      currentUsage: 95,
      planCode: 1,
      quotaLimit: 100,
      remainingQuota: 5,
      quotaExceeded: false,
    },
  })),
}));

describe('SpacesQuotaUpgradePrompt', () => {
  it('renders backend-provided self-service upgrade plans', () => {
    const onUpgradeClick = vi.fn();

    render(
      <SpacesQuotaUpgradePrompt
        currentUsage={100}
        quotaLimit={100}
        upgradePlans={[
          {
            planCode: 5,
            name: 'Growth',
            availability: 'SelfService',
            priceDescription: '$49/month',
          },
        ]}
        onUpgradeClick={onUpgradeClick}
      />,
    );

    fireEvent.click(screen.getByRole('button', { name: /growth/i }));

    expect(screen.getByText(/your organization has used 100 of 100 booking instances/i)).toBeInTheDocument();
    expect(onUpgradeClick).toHaveBeenCalledWith(5);
  });

  it('renders contact guidance when no upgrade plan is available', () => {
    render(<SpacesQuotaUpgradePrompt currentUsage={1500} quotaLimit={1500} upgradePlans={[]} />);

    expect(screen.getByText(/please contact sales for options/i)).toBeInTheDocument();
  });
});

describe('SpacesQuotaStatusInner', () => {
  it('renders current backend quota status', () => {
    render(<SpacesQuotaStatusInner queryReference={{} as never} />);

    expect(screen.getByText(/booking usage: 95 \/ 100/i)).toBeInTheDocument();
    expect(screen.getByText(/5 booking instances remaining this period/i)).toBeInTheDocument();
  });

  it('does not render quota usage for legacy Early Bird', () => {
    vi.mocked(usePreloadedQuery).mockReturnValueOnce({
      bookingSpacesQuotaStatus: {
        currentUsage: 100,
        planCode: 4,
        quotaLimit: null,
        remainingQuota: null,
        quotaExceeded: false,
      },
    } as never);

    const { container } = render(<SpacesQuotaStatusInner queryReference={{} as never} />);

    expect(container).toBeEmptyDOMElement();
  });
});
