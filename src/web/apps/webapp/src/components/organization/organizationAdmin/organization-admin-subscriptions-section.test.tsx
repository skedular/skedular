import { render, screen } from '@testing-library/react';
import type { ReactNode } from 'react';
import { createContext } from 'react';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import OrganizationAdminSubscriptionsSection from './organization-admin-subscriptions-section';

let organization: {
  id: string;
  name: string;
  hasAttachedPaymentMethod: boolean;
  paymentMethods: Array<{
    id: string;
    cardBrand: string;
    cardExpiryMonth: string;
    cardExpiryYear: string;
    cardLastFourDigit: string;
  }>;
  activeOffering: null | {
    id: string;
    isEnterprise: boolean;
    name: string;
    start: string;
    end: string | null;
    unitPrice: number;
    featureSet: string[];
    underPriceLines: string[];
    free: boolean;
  };
  availableOfferings: Array<{
    isEnterprise: boolean;
    code: string;
    name: string;
    unitPrice: number;
    featureSet: string[];
    underPriceLines: string[];
    free: boolean;
  }>;
} = {
  id: 'org-1',
  name: 'Acme',
  hasAttachedPaymentMethod: false,
  paymentMethods: [],
  activeOffering: null,
  availableOfferings: [],
};

// Mock both the barrel path and the direct sub-path so the test passes
// both before (barrel import) and after (direct import) the barrel fix.
vi.mock('@skedular/ui', () => ({
  BodyIconTypography: ({ label }: { label: string }) => <span>{label}</span>,
  coal: '#333',
  CreditCard: ({ lastFourDigits }: { lastFourDigits?: string | null }) => <div data-testid="credit-card">{lastFourDigits}</div>,
  defaultButtonStyle: {},
  emerald: '#00c853',
  ExtraLargeHeadingIconTypography: ({ label }: { label: string }) => <h1>{label}</h1>,
  SettingsSectionCard: ({ children, title }: { children: ReactNode; title?: string }) => (
    <section>
      {title && <h2>{title}</h2>}
      {children}
    </section>
  ),
  SmallIconTypography: ({ label }: { label: string }) => <small>{label}</small>,
  StackColumn: ({ children }: { children: ReactNode }) => <div>{children}</div>,
  StackRow: ({ children }: { children: ReactNode }) => <div>{children}</div>,
}));

vi.mock('@skedular/ui/commons/credit-card', () => ({
  default: ({ lastFourDigits }: { lastFourDigits?: string | null }) => <div data-testid="credit-card">{lastFourDigits}</div>,
}));

vi.mock('@skedular/shared', () => ({
  PaletteModeContext: createContext('light'),
  getRelayErrorMessage: () => 'Relay error',
}));

vi.mock('@/components/icons', () => ({
  DeleteIcon: () => <span>Delete</span>,
  ErrorIcon: () => <span>Error</span>,
  NewIcon: () => <span>New</span>,
  TickIcon: () => <span>Tick</span>,
}));

vi.mock('@/components/loading', () => ({
  Loading: () => <div>Loading</div>,
}));

vi.mock('@/components/notification', () => ({
  errorNotificationOptions: {},
  NotificationContent: ({ content }: { content: string }) => <span>{content}</span>,
}));

vi.mock('@/components/organization/addOrganizationPaymentMethod', () => ({
  AddOrganizationPaymentMethodDialog: () => <div>Add Payment Dialog</div>,
}));

vi.mock('react-toastify', () => ({
  toast: Object.assign(vi.fn(), { dark: vi.fn() }),
}));

vi.mock('uuid', () => ({
  v7: () => 'test-uuid',
}));

vi.mock('react-relay', () => ({
  graphql: (strings: TemplateStringsArray) => strings.join(''),
  useMutation: () => [vi.fn()],
  usePreloadedQuery: () => ({ organization }),
  useQueryLoader: () => [{}, vi.fn()],
}));

describe('OrganizationAdminSubscriptionsSection', () => {
  beforeEach(() => {
    organization = {
      id: 'org-1',
      name: 'Acme',
      hasAttachedPaymentMethod: false,
      paymentMethods: [],
      activeOffering: null,
      availableOfferings: [],
    };
  });

  it('renders a CreditCard for each payment method', () => {
    organization = {
      ...organization,
      hasAttachedPaymentMethod: true,
      paymentMethods: [
        {
          id: 'pm-1',
          cardBrand: 'visa',
          cardExpiryMonth: '11',
          cardExpiryYear: '2028',
          cardLastFourDigit: '1234',
        },
      ],
    };

    render(<OrganizationAdminSubscriptionsSection organizationCustomDomain="acme" />);

    expect(screen.getByTestId('credit-card')).toBeInTheDocument();
    expect(screen.getByTestId('credit-card')).toHaveTextContent('1234');
  });

  it('does not render a CreditCard when there are no payment methods', () => {
    render(<OrganizationAdminSubscriptionsSection organizationCustomDomain="acme" />);

    expect(screen.queryByTestId('credit-card')).not.toBeInTheDocument();
  });
});
