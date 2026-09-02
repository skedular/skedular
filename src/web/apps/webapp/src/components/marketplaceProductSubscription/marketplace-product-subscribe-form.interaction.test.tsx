import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import dayjs from 'dayjs';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import MarketplaceProductSubscribeForm from './marketplace-product-subscribe-form';

const commitAddSubscription = vi.fn();

vi.mock('next/navigation', () => ({
  useRouter: () => ({ push: vi.fn() }),
  useSearchParams: () => ({ get: () => null }),
}));

vi.mock('react-relay', () => ({
  graphql: (strings: TemplateStringsArray) => strings.join(''),
  useFragment: () => ({
    me: { id: 'customer-1', emails: ['customer@example.com'] },
    currencies: [{ type: 'NZD', name: 'New Zealand dollar' }],
    paymentMethodTypes: [{ type: 'CARD', name: 'Card' }],
    product: {
      id: 'product-1',
      latestProductVersionId: 'version-1',
      type: { type: 'DESK', name: 'Desk' },
      organization: { customerFacingTermsAndConditionsUrl: null },
      listingMetadata: { title: 'Desk pass' },
      currency: { type: 'NZD', name: 'New Zealand dollar' },
      pricingOptions: [
        {
          id: 'weekly-two-days',
          index: 0,
          listingMetadata: { title: 'Two days', subTitle: null },
          purchaseCadence: 'WEEKLY',
          price: 100,
          isTaxInclusive: true,
          supportsSubscriptionAutoRenewal: true,
          billingMode: 'UPFRONT',
          acceptedPaymentMethods: ['CARD'],
          availableDays: ['TUESDAY', 'THURSDAY', 'FRIDAY'],
          requiredDaysPerWeek: 2,
          numberOfResourcesToBook: 1,
          cancellationPolicyType: 'NO_CANCELLATION',
          cancellationRefundRules: [],
        },
      ],
    },
  }),
  useMutation: () => [commitAddSubscription, false],
}));

vi.mock('@mui/x-date-pickers/DatePicker', () => ({ DatePicker: () => <input aria-label="Start date" /> }));
vi.mock('@mui/x-date-pickers-pro/TimeRangePicker', () => ({
  TimeRangePicker: () => <input aria-label="Booking time range" />,
}));
vi.mock('@/hooks/use-known-params', () => ({ default: () => ({ isCustomDomain: false, organizationCustomDomain: 'example' }) }));
vi.mock('@/components/marketplaceProduct', () => ({ CustomerTermsAndConditionsPanel: () => null }));
vi.mock('./marketplace-product-subscribe-summary', () => ({ default: () => <div>Summary</div> }));
vi.mock('react-toastify', () => ({ toast: Object.assign(vi.fn(), { error: vi.fn() }) }));

vi.mock('@skedular/shared', async (importOriginal) => {
  const actual = await importOriginal<typeof import('@skedular/shared')>();
  return { ...actual, startOfDay: () => dayjs.utc('2026-07-21T00:00:00.000Z'), useIntegratedPlatform: () => ({ integratedPlatform: 'web' }) };
});

describe('MarketplaceProductSubscribeForm weekly checkout', () => {
  beforeEach(() => commitAddSubscription.mockReset());

  it('enables checkout only after the exact weekday count is selected and submits those days', async () => {
    const user = userEvent.setup();

    render(<MarketplaceProductSubscribeForm bookingAvailable bookingAvailabilityMessage="Available" rootDataRelay={{} as never} />);

    expect(screen.getByLabelText('Plan')).toHaveTextContent('Two days');
    expect(screen.getByLabelText('Plan')).not.toHaveTextContent('WEEKLY');

    const submit = screen.getByRole('button', { name: 'Start plan' });
    expect(submit).toBeDisabled();

    await user.click(screen.getByRole('button', { name: 'Tuesday' }));
    expect(submit).toBeDisabled();

    await user.click(screen.getByRole('button', { name: 'Thursday' }));
    expect(submit).toBeEnabled();

    await user.click(submit);

    expect(commitAddSubscription).toHaveBeenCalledWith(
      expect.objectContaining({
        variables: expect.objectContaining({
          input: expect.objectContaining({
            pricingId: 'weekly-two-days',
            weeklySelectedDays: ['TUESDAY', 'THURSDAY'],
          }),
        }),
      }),
    );
  });
});
