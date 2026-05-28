import { act, fireEvent, render, screen } from '@testing-library/react';
import type { ReactNode } from 'react';
import { createContext } from 'react';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import OrganizationAdminSsoSection from './organization-admin-sso-section';

const patchCommit = vi.fn();
const removeCommit = vi.fn();

const organization = {
  id: 'org-1',
  name: 'Acme',
  customDomain: 'acme',
  ssoSettings: {
    id: 'sso-1',
    isActive: true,
    entityId: 'entity',
    loginUrl: 'https://login.example.com',
    appFederationMetadataUrl: 'https://login.example.com/metadata',
  },
};

vi.mock('@skedular/shared', () => ({
  PaletteModeContext: createContext('light'),
  getRelayErrorMessage: () => 'Relay error',
  keyboardTextFieldDebounceTimeout: 500,
}));

vi.mock('@skedular/ui', () => ({
  EditorActionBar: ({ primaryAction }: { primaryAction: string }) => <button type="submit">{primaryAction}</button>,
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
    appFederationMetadataUrl: 'App Federation Metadata Url',
    entityId: 'Entity Id',
    loginUrl: 'Login Url',
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
  useMutation: (mutation: string) => [mutation.includes('updateOrganizationSsoSettings') ? patchCommit : removeCommit],
  usePreloadedQuery: () => ({ organization }),
  useQueryLoader: () => [{}, vi.fn()],
}));

describe('OrganizationAdminSsoSection', () => {
  beforeEach(() => {
    vi.useFakeTimers();
    patchCommit.mockReset();
    removeCommit.mockReset();
  });

  it('patches SSO settings after inline debounce', async () => {
    render(<OrganizationAdminSsoSection organizationCustomDomain="acme" />);

    fireEvent.change(screen.getByRole('textbox', { name: 'Entity Id' }), { target: { value: 'updated-entity' } });
    await act(async () => {
      vi.advanceTimersByTime(1000);
    });

    expect(patchCommit).toHaveBeenCalledTimes(1);
    expect(patchCommit.mock.calls[0][0].variables.input).toMatchObject({
      organizationCustomDomain: 'acme',
      fieldsToUpdate: ['SSO_SETTINGS'],
      entityId: 'updated-entity',
      loginUrl: 'https://login.example.com',
      appFederationMetadataUrl: 'https://login.example.com/metadata',
      isActive: true,
    });
  });

  it('shows SSO fields without a manual update button', () => {
    render(<OrganizationAdminSsoSection organizationCustomDomain="acme" />);

    expect(screen.getByRole('textbox', { name: 'Entity Id' })).toBeInTheDocument();
    expect(screen.getByRole('textbox', { name: 'Login Url' })).toBeInTheDocument();
    expect(screen.getByRole('textbox', { name: 'App Federation Metadata Url' })).toBeInTheDocument();
    expect(screen.queryByRole('button', { name: 'Update' })).not.toBeInTheDocument();
  });

  it('patches SSO as inactive when the switch is disabled', async () => {
    render(<OrganizationAdminSsoSection organizationCustomDomain="acme" />);

    await act(async () => {
      fireEvent.click(screen.getByRole('switch', { name: 'Enable SSO across the organisation' }));
    });

    expect(patchCommit).toHaveBeenCalledTimes(1);
    expect(patchCommit.mock.calls[0][0].variables.input).toMatchObject({
      organizationCustomDomain: 'acme',
      fieldsToUpdate: ['SSO_SETTINGS'],
      entityId: 'entity',
      loginUrl: 'https://login.example.com',
      appFederationMetadataUrl: 'https://login.example.com/metadata',
      isActive: false,
    });
    expect(removeCommit).not.toHaveBeenCalled();
  });
});
