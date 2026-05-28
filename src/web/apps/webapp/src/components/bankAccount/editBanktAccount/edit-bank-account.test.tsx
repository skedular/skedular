import { act, fireEvent, render, screen } from '@testing-library/react';
import type { ReactNode } from 'react';
import { createContext } from 'react';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import EditBankAccount from './edit-bank-account';

const patchCommit = vi.fn();

const rootData = {
  organizationBankAccount: {
    id: 'bank-1',
    name: 'Operating account',
    bankName: 'Skedular Bank',
    accountHolderName: 'Acme',
    accountNumber: '123456',
    country: 'NZ',
  },
};

vi.mock('@/components/forms', async () => {
  const { Field } = await import('react-final-form');

  return {
    SingleChoiceCountry: ({ name }: { name: string }) => <Field name={name}>{({ input }) => <input aria-label="Country" {...input} />}</Field>,
  };
});

vi.mock('@skedular/shared', () => ({
  PaletteModeContext: createContext('light'),
  getRelayErrorMessage: () => 'Relay error',
}));

vi.mock('@skedular/ui', () => ({
  AppBarWithStackColumn: ({ children }: { children: ReactNode }) => <main>{children}</main>,
  BodyIconTypography: ({ label }: { label: string }) => <span>{label}</span>,
  defaultPadding: 2,
  FormFieldLabel: ({ children, label }: { children: ReactNode; label: string }) => (
    <label>
      {label}
      {children}
    </label>
  ),
  FormStackColumn: ({ children, onSubmit }: { children: ReactNode; onSubmit: () => void }) => <form onSubmit={onSubmit}>{children}</form>,
  GridContainer: ({ children }: { children: ReactNode }) => <div>{children}</div>,
  SectionIconTypography: ({ label }: { label: string }) => <h2>{label}</h2>,
  StackColumn: ({ children }: { children: ReactNode }) => <div>{children}</div>,
}));

vi.mock('@/components/notification', () => ({
  errorNotificationOptions: {},
  NotificationContent: ({ content }: { content: string }) => <span>{content}</span>,
}));

vi.mock('mui-rff', async () => {
  const { Field } = await import('react-final-form');

  const labels: Record<string, string> = {
    accountHolderName: 'Account Holder Name',
    accountNumber: 'Account Number',
    bankName: 'Bank Name',
    name: 'Name',
  };

  return {
    makeRequired: () => ({}),
    makeValidate: () => () => ({}),
    TextField: ({ name }: { name: string }) => <Field name={name}>{({ input }) => <input aria-label={labels[name] ?? name} {...input} />}</Field>,
  };
});

vi.mock('next/navigation', () => ({
  useRouter: () => ({
    back: vi.fn(),
  }),
}));

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

describe('EditBankAccount', () => {
  beforeEach(() => {
    vi.useFakeTimers();
    patchCommit.mockReset();
  });

  it('patches account fields after inline debounce', async () => {
    render(<EditBankAccount rootDataRelay={{} as never} onReloadRequired={vi.fn()} />);

    fireEvent.change(screen.getByRole('textbox', { name: 'Name' }), { target: { value: 'Primary operating account' } });
    await act(async () => {
      vi.advanceTimersByTime(1000);
    });

    expect(patchCommit).toHaveBeenCalledTimes(1);
    expect(patchCommit.mock.calls[0][0].variables.input).toMatchObject({
      id: 'bank-1',
      fieldsToUpdate: ['NAME'],
      name: 'Primary operating account',
    });
  });

  it('shows the account fields without a manual update button', () => {
    render(<EditBankAccount rootDataRelay={{} as never} onReloadRequired={vi.fn()} />);

    expect(screen.getByRole('textbox', { name: 'Name' })).toBeInTheDocument();
    expect(screen.queryByRole('button', { name: 'Update' })).not.toBeInTheDocument();
  });
});
