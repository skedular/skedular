import { fireEvent, render, screen } from '@testing-library/react';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import dayjs from 'dayjs';
import NewBookingButton from './new-booking-button';

const pushMock = vi.fn();

vi.mock('next/navigation', () => ({
  useRouter: () => ({ push: pushMock }),
  usePathname: () => '/organizations/acme/locations',
  useSearchParams: () => new URLSearchParams('section=setup'),
}));

vi.mock(import('@/libs/providers'), async (importOriginal) => {
  const actual = await importOriginal();

  return {
    ...actual,
    useIntegratedPlatrform: () => ({ integratedPlatrform: 'web' }),
  };
});

describe('NewBookingButton', () => {
  beforeEach(() => {
    pushMock.mockReset();
  });

  it('navigates to the dedicated add booking page with context defaults', () => {
    render(
      <NewBookingButton
        organizationCustomDomain="acme"
        defaultLocationId="location-1"
        defaultDate={dayjs.utc('2026-04-12T00:00:00Z')}
        defaultResourceIds={['resource-1', 'resource-2']}
        label="Add Booking"
      />,
    );

    fireEvent.click(screen.getByRole('button', { name: /add booking/i }));

    expect(pushMock).toHaveBeenCalledWith(
      'web/organizations/acme/bookings/add?locationId=location-1&date=2026-04-12T00%3A00%3A00.000Z&resourceIds=resource-1%2Cresource-2&redirectUrl=%2Forganizations%2Facme%2Flocations%3Fsection%3Dsetup',
    );
  });
});
