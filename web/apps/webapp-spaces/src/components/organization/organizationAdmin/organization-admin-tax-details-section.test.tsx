import { act, fireEvent, render, screen } from '@testing-library/react';
import type { ReactNode } from 'react';
import { createContext } from 'react';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import OrganizationAdminTaxDetailsSection from './organization-admin-tax-details-section';

const patchCommit = vi.fn();
const removeCommit = vi.fn();

const organization = {
  id: 'org-1',
  name: 'Acme',
  taxDetails: {
    taxId: 'NZ123',
    taxRatePercentage: '15',
  },
};

vi.mock('@skedular/shared', () => ({
  PaletteModeContext: createContext('light'),
  getRelayErrorMessage: () => 'Relay error',
}));

vi.mock('@skedular/ui', () => ({
  FormFieldLabel: ({ children, label }: { children: ReactNode; label: string }) => (
    <label>
      {label}
      {children}
    </label>
  ),
  FormStackColumn: ({ children, onSubmit }: { children: ReactNode; onSubmit: () => void }) => <form onSubmit={onSubmit}>{children}</form>,
  SettingsSectionCard: ({ children }: { children: ReactNode }) => <section>{children}</section>,
  StackColumn: ({ children }: { children: ReactNode }) => <div>{children}</div>,
}));

vi.mock('@/components/loading', () => ({
  Loading: () => <div>Loading</div>,
}));

vi.mock('@/components/notification', () => ({
  errorNotificationOptions: {},
  infoNotificationOptions: {},
  NotificationContent: ({ content }: { content: string }) => <span>{content}</span>,
  successNotificationOptions: {},
}));

vi.mock('mui-rff', async () => {
  const { Field } = await import('react-final-form');

  const labels: Record<string, string> = {
    taxId: 'Tax ID / VAT / GST Number',
    taxRatePercentage: 'Tax Rate (%)',
  };

  return {
    makeRequired: () => ({}),
    makeValidate: () => () => ({}),
    TextField: ({ name }: { name: string }) => <Field name={name}>{({ input }) => <input aria-label={labels[name] ?? name} {...input} />}</Field>,
  };
});

vi.mock('react-toastify', () => ({
  toast: Object.assign(vi.fn(), {
    dark: vi.fn(),
    update: vi.fn(),
  }),
}));

vi.mock('react-relay', () => ({
  graphql: (strings: TemplateStringsArray) => strings.join(''),
  useMutation: (mutation: string) => [mutation.includes('updateOrganizationTaxDetails') ? patchCommit : removeCommit],
  usePreloadedQuery: () => ({ organization }),
  useQueryLoader: () => [{}, vi.fn()],
}));

describe('OrganizationAdminTaxDetailsSection', () => {
  beforeEach(() => {
    vi.useFakeTimers();
    patchCommit.mockReset();
    removeCommit.mockReset();
  });

  it('recreates existing tax details when the autosave switch is re-enabled', async () => {
    render(<OrganizationAdminTaxDetailsSection organizationCustomDomain="acme" />);

    await act(async () => {
      fireEvent.click(screen.getByRole('switch', { name: 'Is this business registered for tax (GST/VAT)?' }));
    });
    await act(async () => {
      fireEvent.click(screen.getByRole('switch', { name: 'Is this business registered for tax (GST/VAT)?' }));
    });

    expect(removeCommit).toHaveBeenCalledTimes(1);
    expect(patchCommit).toHaveBeenCalledTimes(1);
    expect(patchCommit.mock.calls[0][0].variables.input).toMatchObject({
      organizationCustomDomain: 'acme',
      fieldsToUpdate: ['TAX_ID', 'TAX_RATE_PERCENTAGE'],
      taxId: 'NZ123',
      taxRatePercentage: 15,
    });
  });

  it('shows the tax fields without a manual update button', () => {
    render(<OrganizationAdminTaxDetailsSection organizationCustomDomain="acme" />);

    expect(screen.getByRole('textbox', { name: 'Tax ID / VAT / GST Number' })).toBeInTheDocument();
    expect(screen.getByRole('textbox', { name: 'Tax Rate (%)' })).toBeInTheDocument();
    expect(screen.queryByRole('button', { name: 'Update' })).not.toBeInTheDocument();
  });
});
