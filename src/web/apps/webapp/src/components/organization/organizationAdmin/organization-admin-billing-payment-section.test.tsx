import { render, screen } from '@testing-library/react';
import type { ReactNode } from 'react';
import { createContext } from 'react';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import OrganizationAdminBillingPaymentSection from './organization-admin-billing-payment-section';

let organization: {
  id: string;
  billingDetails: null | {
    id: string;
    companyName: string;
    email: string;
    osmType: null;
    osmId: null;
    placeId: null;
    longitude: null;
    latitude: null;
    formattedAddress: null;
    addressLine1: string;
    addressLine2: null;
    suburb: null;
    city: string;
    province: null;
    zipcode: string;
    country: string;
    countryCode: string;
  };
  paymentMethods: Array<{
    id: string;
    cardBrand: string;
    cardExpiryMonth: string;
    cardExpiryYear: string;
    cardLastFourDigit: string;
  }>;
} = {
  id: 'org-1',
  billingDetails: {
    id: 'billing-1',
    companyName: 'Acme',
    email: 'billing@acme.com',
    osmType: null,
    osmId: null,
    placeId: null,
    longitude: null,
    latitude: null,
    formattedAddress: null,
    addressLine1: '1 Main St',
    addressLine2: null,
    suburb: null,
    city: 'Auckland',
    province: null,
    zipcode: '1010',
    country: 'New Zealand',
    countryCode: 'NZ',
  },
  paymentMethods: [],
};

// Mock both the barrel export path AND the direct path so the test passes
// both before (barrel import) and after (direct import) the barrel fix.
vi.mock('@skedular/ui', () => ({
  BodyIconTypography: ({ label }: { label: string }) => <span>{label}</span>,
  CreditCard: ({ lastFourDigits }: { lastFourDigits?: string | null }) => <div data-testid="credit-card">{lastFourDigits}</div>,
  FormFieldLabel: ({ children, label }: { children: ReactNode; label: string }) => (
    <label>
      {label}
      {children}
    </label>
  ),
  FormStackColumn: ({ children, onSubmit }: { children: ReactNode; onSubmit: () => void }) => <form onSubmit={onSubmit}>{children}</form>,
  SettingsSectionCard: ({ children, title }: { children: ReactNode; title?: string }) => (
    <section>
      {title && <h2>{title}</h2>}
      {children}
    </section>
  ),
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

vi.mock('@/components/address', () => ({
  PhysicalAddress: () => <div>Address</div>,
}));

vi.mock('@/components/icons', () => ({
  DeleteIcon: () => <span>Delete</span>,
  NewIcon: () => <span>New</span>,
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

vi.mock('@/components/organization/organizationAdmin/organization-admin-shared', () => ({
  billingSchema: {},
}));

vi.mock('countries-list', () => ({
  getCountryData: () => ({ name: 'New Zealand' }),
}));

vi.mock('mui-rff', () => ({
  makeRequired: () => ({}),
  makeValidate: () => () => ({}),
  TextField: ({ name }: { name: string }) => <input aria-label={name} />,
}));

vi.mock('react-toastify', () => ({
  toast: Object.assign(vi.fn(), { dark: vi.fn() }),
}));

vi.mock('usehooks-ts', () => ({
  useDebounceCallback: (fn: unknown) => fn,
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

describe('OrganizationAdminBillingPaymentSection', () => {
  beforeEach(() => {
    organization = {
      id: 'org-1',
      billingDetails: {
        id: 'billing-1',
        companyName: 'Acme',
        email: 'billing@acme.com',
        osmType: null,
        osmId: null,
        placeId: null,
        longitude: null,
        latitude: null,
        formattedAddress: null,
        addressLine1: '1 Main St',
        addressLine2: null,
        suburb: null,
        city: 'Auckland',
        province: null,
        zipcode: '1010',
        country: 'New Zealand',
        countryCode: 'NZ',
      },
      paymentMethods: [],
    };
  });

  it('renders a CreditCard for each payment method', () => {
    organization = {
      ...organization,
      paymentMethods: [
        {
          id: 'pm-1',
          cardBrand: 'visa',
          cardExpiryMonth: '12',
          cardExpiryYear: '2027',
          cardLastFourDigit: '4242',
        },
      ],
    };

    render(<OrganizationAdminBillingPaymentSection organizationCustomDomain="acme" />);

    expect(screen.getByTestId('credit-card')).toBeInTheDocument();
    expect(screen.getByTestId('credit-card')).toHaveTextContent('4242');
  });

  it('does not render a CreditCard when there are no payment methods', () => {
    render(<OrganizationAdminBillingPaymentSection organizationCustomDomain="acme" />);

    expect(screen.queryByTestId('credit-card')).not.toBeInTheDocument();
  });
});
