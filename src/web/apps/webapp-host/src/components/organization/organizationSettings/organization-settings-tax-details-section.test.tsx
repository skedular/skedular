import { act, fireEvent, render, screen } from '@testing-library/react';
import type { ReactNode } from 'react';
import { createContext } from 'react';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import OrganizationSettingsTaxDetailsSection from './organization-settings-tax-details-section';

const patchCommit = vi.fn();
const removeCommit = vi.fn();

let organization: {
  id: string;
  name: string;
  taxDetails: {
    isRegistered: boolean;
    taxId: string;
    taxRatePercentage: string;
  } | null;
} = {
  id: 'org-1',
  name: 'Acme',
  taxDetails: {
    isRegistered: false,
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
    makeValidate: () => (values: { isRegistered?: boolean; taxId?: string; taxRatePercentage?: string }) => ({
      ...(values.isRegistered && !values.taxId ? { taxId: 'Tax ID / VAT / GST Number is required.' } : {}),
      ...(values.isRegistered && !values.taxRatePercentage ? { taxRatePercentage: 'Tax rate is required.' } : {}),
    }),
    showErrorOnChange: ({ meta }: { meta: { error?: string; touched?: boolean; modified?: boolean } }) => Boolean(meta.error && (meta.touched || meta.modified)),
    TextField: ({
      name,
      showError,
      error,
      helperText,
    }: {
      name: string;
      showError?: (props: { meta: { error?: string; touched?: boolean; modified?: boolean } }) => boolean;
      error?: boolean;
      helperText?: string;
    }) => (
      <Field name={name}>
        {({ input, meta }) => (
          <>
            <input aria-label={labels[name] ?? name} {...input} />
            {error && helperText && <span>{helperText}</span>}
            {!error && showError?.({ meta }) && meta.error && <span>{meta.error}</span>}
          </>
        )}
      </Field>
    ),
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

describe('OrganizationSettingsTaxDetailsSection', () => {
  beforeEach(() => {
    vi.useFakeTimers();
    organization = {
      id: 'org-1',
      name: 'Acme',
      taxDetails: {
        isRegistered: false,
        taxId: 'NZ123',
        taxRatePercentage: '15',
      },
    };
    patchCommit.mockReset();
    removeCommit.mockReset();
  });

  it('autosaves the tax registration state when tax details are valid', async () => {
    render(<OrganizationSettingsTaxDetailsSection organizationCustomDomain="acme" />);

    await act(async () => {
      fireEvent.click(screen.getByRole('switch', { name: 'Is this business registered for tax (GST/VAT)?' }));
      vi.advanceTimersByTime(1000);
    });

    expect(patchCommit).toHaveBeenCalledTimes(1);
    expect(patchCommit.mock.calls[0][0].variables.input).toMatchObject({
      organizationCustomDomain: 'acme',
      fieldsToUpdate: ['IS_REGISTERED'],
      isRegistered: true,
      taxId: 'NZ123',
      taxRatePercentage: 15,
    });
  });

  it('keeps tax registration off and shows validation when required details are missing', async () => {
    organization = {
      id: 'org-1',
      name: 'Acme',
      taxDetails: null,
    };

    render(<OrganizationSettingsTaxDetailsSection organizationCustomDomain="acme" />);

    await act(async () => {
      fireEvent.click(screen.getByRole('switch', { name: 'Is this business registered for tax (GST/VAT)?' }));
      vi.advanceTimersByTime(1000);
    });

    expect(screen.getByRole('switch', { name: 'Is this business registered for tax (GST/VAT)?' })).not.toBeChecked();
    expect(screen.getByText('Tax ID / VAT / GST Number is required.')).toBeInTheDocument();
    expect(screen.getByText('Tax rate is required.')).toBeInTheDocument();
    expect(patchCommit).not.toHaveBeenCalled();
  });

  it('autosaves partial tax details when the organization is not registered for tax', async () => {
    organization = {
      id: 'org-1',
      name: 'Acme',
      taxDetails: null,
    };

    render(<OrganizationSettingsTaxDetailsSection organizationCustomDomain="acme" />);

    await act(async () => {
      fireEvent.change(screen.getByRole('textbox', { name: 'Tax ID / VAT / GST Number' }), { target: { value: 'NZ456' } });
      vi.advanceTimersByTime(1000);
    });

    expect(screen.queryByText('Tax rate is required.')).not.toBeInTheDocument();
    expect(patchCommit).toHaveBeenCalledTimes(1);
    expect(patchCommit.mock.calls[0][0].variables.input).toMatchObject({
      organizationCustomDomain: 'acme',
      fieldsToUpdate: ['TAX_ID'],
      isRegistered: false,
      taxId: 'NZ456',
      taxRatePercentage: null,
    });
  });

  it('autosaves an empty tax rate when the organization is not registered for tax', async () => {
    render(<OrganizationSettingsTaxDetailsSection organizationCustomDomain="acme" />);

    await act(async () => {
      fireEvent.change(screen.getByRole('textbox', { name: 'Tax Rate (%)' }), { target: { value: '' } });
      vi.advanceTimersByTime(1000);
    });

    expect(screen.queryByText('Tax rate is required.')).not.toBeInTheDocument();
    expect(patchCommit).toHaveBeenCalledTimes(1);
    expect(patchCommit.mock.calls[0][0].variables.input).toMatchObject({
      organizationCustomDomain: 'acme',
      fieldsToUpdate: ['TAX_RATE_PERCENTAGE'],
      isRegistered: false,
      taxId: 'NZ123',
      taxRatePercentage: null,
    });
    expect(patchCommit.mock.calls[0][0].optimisticResponse.updateOrganizationTaxDetails.organization.taxDetails.taxRatePercentage).toBe('');
  });

  it('displays a zero tax rate as empty', () => {
    organization = {
      id: 'org-1',
      name: 'Acme',
      taxDetails: {
        isRegistered: false,
        taxId: 'NZ123',
        taxRatePercentage: '0',
      },
    };

    render(<OrganizationSettingsTaxDetailsSection organizationCustomDomain="acme" />);

    expect(screen.getByRole('textbox', { name: 'Tax Rate (%)' })).toHaveValue('');
  });

  it('autosaves tax detail fields without a manual update button', async () => {
    render(<OrganizationSettingsTaxDetailsSection organizationCustomDomain="acme" />);

    await act(async () => {
      fireEvent.change(screen.getByRole('textbox', { name: 'Tax ID / VAT / GST Number' }), { target: { value: 'NZ456' } });
      vi.advanceTimersByTime(1000);
    });

    expect(patchCommit).toHaveBeenCalledTimes(1);
    expect(patchCommit.mock.calls[0][0].variables.input).toMatchObject({
      organizationCustomDomain: 'acme',
      fieldsToUpdate: ['TAX_ID'],
      isRegistered: false,
      taxId: 'NZ456',
      taxRatePercentage: 15,
    });
    expect(screen.queryByRole('button', { name: 'Update' })).not.toBeInTheDocument();
  });

  it('shows all tax fields', () => {
    render(<OrganizationSettingsTaxDetailsSection organizationCustomDomain="acme" />);

    expect(screen.getByRole('switch', { name: 'Is this business registered for tax (GST/VAT)?' })).toBeInTheDocument();
    expect(screen.getByRole('textbox', { name: 'Tax ID / VAT / GST Number' })).toBeInTheDocument();
    expect(screen.getByRole('textbox', { name: 'Tax Rate (%)' })).toBeInTheDocument();
  });
});
