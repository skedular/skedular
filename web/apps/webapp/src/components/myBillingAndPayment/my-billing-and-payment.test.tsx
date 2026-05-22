import { act, fireEvent, render, screen } from '@testing-library/react';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import MyBillingAndPayment from './my-billing-and-payment';

const addCommit = vi.fn();
const updateCommit = vi.fn();
const removeCommit = vi.fn();

vi.mock('@/components/address', async () => {
  const { Field } = await import('react-final-form');

  return {
    PhysicalAddress: ({ addressLine1Name, cityName, countryName, zipcodeName }: { addressLine1Name: string; cityName: string; countryName: string; zipcodeName: string }) => (
      <>
        <Field name={addressLine1Name}>{({ input }) => <input aria-label="Address" {...input} />}</Field>
        <Field name={cityName}>{({ input }) => <input aria-label="City" {...input} />}</Field>
        <Field name={zipcodeName}>{({ input }) => <input aria-label="Zipcode" {...input} />}</Field>
        <Field name={countryName}>{({ input }) => <input aria-label="Country" {...input} />}</Field>
      </>
    ),
  };
});

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

  return {
    makeRequired: () => ({}),
    makeValidate: () => () => ({}),
    TextField: ({ name }: { name: string }) => <Field name={name}>{({ input }) => <input aria-label={name} {...input} />}</Field>,
  };
});

vi.mock('next/navigation', () => ({
  usePathname: () => '/billing-and-payment',
  useSearchParams: () => new URLSearchParams('section=billing-details'),
}));

vi.mock('react-toastify', () => ({
  toast: Object.assign(vi.fn(), {
    dark: vi.fn(),
    update: vi.fn(),
  }),
}));

vi.mock('react-relay', () => ({
  graphql: (strings: TemplateStringsArray) => strings.join(''),
  useMutation: (mutation: string) => [mutation.includes('updateMyBillingDetails') ? updateCommit : mutation.includes('addMyBillingDetails') ? addCommit : removeCommit],
  usePreloadedQuery: () => ({
    me: {
      id: 'customer-1',
      billingDetails: {
        id: 'billing-1',
        companyName: 'Acme',
        email: 'billing@example.com',
        osmType: null,
        osmId: null,
        placeId: null,
        longitude: null,
        latitude: null,
        formattedAddress: null,
        addressLine1: '1 Main Street',
        addressLine2: null,
        suburb: null,
        city: 'Wellington',
        province: null,
        zipcode: '6011',
        country: 'New Zealand',
        countryCode: 'NZ',
      },
    },
  }),
  useQueryLoader: () => [{}, vi.fn()],
  useRefetchableFragment: () => [{ me: { paymentMethods: [] } }, vi.fn()],
}));

describe('MyBillingAndPayment', () => {
  beforeEach(() => {
    vi.useFakeTimers();
    addCommit.mockReset();
    updateCommit.mockReset();
    removeCommit.mockReset();
  });

  it('autosaves the changed billing email after the debounce', async () => {
    render(<MyBillingAndPayment onReloadRequired={vi.fn()} />);

    fireEvent.change(screen.getByRole('textbox', { name: 'email' }), { target: { value: 'accounts@example.com' } });
    await act(async () => {
      vi.advanceTimersByTime(1000);
    });

    expect(updateCommit).toHaveBeenCalledTimes(1);
    expect(updateCommit.mock.calls[0][0].variables.input).toMatchObject({
      id: 'billing-1',
      fieldsToUpdate: ['EMAIL'],
      email: 'accounts@example.com',
    });
  });

  it('removes the manual billing update action', () => {
    render(<MyBillingAndPayment onReloadRequired={vi.fn()} />);

    expect(screen.queryByRole('button', { name: 'Update' })).not.toBeInTheDocument();
  });
});
