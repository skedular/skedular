import { act, fireEvent, render, screen } from '@testing-library/react';
import type { ReactNode } from 'react';
import { createContext } from 'react';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import EditStripeConnectAccount from './edit-stripe-connect-account';

const patchCommit = vi.fn();

const rootData = {
  organizationStripeConnectAccount: {
    id: 'stripe-1',
    name: 'Primary payouts',
    country: null,
    defaultCurrency: 'NZD',
    businessType: 'company',
    companyName: 'Acme',
    url: 'https://example.test',
    supportUrl: 'https://support.example.test',
    contactEmail: 'finance@example.test',
    contactPhone: '+640000000',
    onboardingUrl: null,
    chargesEnabled: true,
    payoutsEnabled: true,
    detailsSubmitted: true,
    isAuthorized: true,
    isOnboardingCompleted: true,
  },
};

vi.mock('@skedular/shared', () => ({
  PaletteModeContext: createContext('light'),
  getRelayErrorMessage: () => 'Relay error',
}));

vi.mock('@skedular/ui', () => ({
  BodyIconTypography: ({ label }: { label: string }) => <span>{label}</span>,
  CaptionIconTypography: ({ label }: { label: string }) => <span>{label}</span>,
  defaultPadding: 2,
  FormFieldLabel: ({ children, label }: { children: ReactNode; label: string }) => (
    <label>
      {label}
      {children}
    </label>
  ),
  FormStackColumn: ({ children, onSubmit }: { children: ReactNode; onSubmit: () => void }) => <form onSubmit={onSubmit}>{children}</form>,
  LeadIconTypography: ({ label }: { label: string }) => <h2>{label}</h2>,
  PageHeaderPanel: ({ children, title }: { children: ReactNode; title: string }) => (
    <header>
      <h1>{title}</h1>
      {children}
    </header>
  ),
  SectionIconTypography: ({ label }: { label: string }) => <h3>{label}</h3>,
  SmallIconTypography: ({ label }: { label: string }) => <span>{label}</span>,
  StackColumn: ({ children }: { children: ReactNode }) => <div>{children}</div>,
  StackRow: ({ children }: { children: ReactNode }) => <div>{children}</div>,
}));

vi.mock('@/components/notification', () => ({
  errorNotificationOptions: {},
  NotificationContent: ({ content }: { content: string }) => <span>{content}</span>,
}));

vi.mock('@/components/stripeConnectAccount', () => ({
  CompleteOnboardStripeConnectAccountButton: () => <button>Complete onboarding</button>,
}));

vi.mock('mui-rff', async () => {
  const { Field } = await import('react-final-form');

  return {
    makeRequired: () => ({}),
    makeValidate: () => () => ({}),
    TextField: ({ name }: { name: string }) => <Field name={name}>{({ input }) => <input aria-label="Nickname" {...input} />}</Field>,
  };
});

vi.mock('react-toastify', () => ({
  toast: Object.assign(vi.fn(), {
    dark: vi.fn(),
  }),
}));

vi.mock('react-relay', () => ({
  graphql: (strings: TemplateStringsArray) => strings.join(''),
  useFragment: () => rootData,
  useMutation: () => [patchCommit],
}));

describe('EditStripeConnectAccount', () => {
  beforeEach(() => {
    vi.useFakeTimers();
    patchCommit.mockReset();
  });

  it('patches the nickname after inline debounce', async () => {
    render(<EditStripeConnectAccount rootDataRelay={{} as never} onReloadRequired={vi.fn()} />);

    fireEvent.change(screen.getByRole('textbox', { name: 'Nickname' }), { target: { value: 'Marketplace payouts' } });
    await act(async () => {
      vi.advanceTimersByTime(1000);
    });

    expect(patchCommit).toHaveBeenCalledTimes(1);
    expect(patchCommit.mock.calls[0][0].variables.input).toMatchObject({
      id: 'stripe-1',
      fieldsToUpdate: ['NAME'],
      name: 'Marketplace payouts',
    });
  });

  it('shows the nickname field without a manual update button', () => {
    render(<EditStripeConnectAccount rootDataRelay={{} as never} onReloadRequired={vi.fn()} />);

    expect(screen.getByRole('textbox', { name: 'Nickname' })).toBeInTheDocument();
    expect(screen.queryByRole('button', { name: 'Update' })).not.toBeInTheDocument();
  });
});
