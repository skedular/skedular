import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { describe, expect, it, vi } from 'vitest';
import dayjs from 'dayjs';
import type { modifyMarketplaceBookingDialog_query$key } from '@/queries/__generated__/modifyMarketplaceBookingDialog_query.graphql';
import ModifyMarketplaceBookingDialog from './modify-marketplace-booking-dialog';

const commitModifyMarketplaceBooking = vi.fn();
const refetchResourceSelection = vi.fn();
let resourceSelection = {
  canSelectResources: true,
  maximumResourceCount: 1,
  availableResourceIds: ['resource-1', 'resource-2'],
  eligibleLocations: [{ uniqueId: 'location-1', name: 'Main office' }],
  eligibleResources: [{ resource: { id: 'resource-1', name: 'Desk one' } }, { resource: { id: 'resource-2', name: 'Desk two' } }],
};

vi.mock('react-relay', () => ({
  graphql: (strings: TemplateStringsArray) => strings.join(''),
  useMutation: () => [commitModifyMarketplaceBooking, false],
  useRefetchableFragment: () => [{ booking: { marketplaceBookingResourceSelection: resourceSelection } }, refetchResourceSelection],
}));

vi.mock('@mui/x-date-pickers/DatePicker', () => ({
  DatePicker: ({ label, onChange }: { label: string; onChange?: (value: unknown) => void }) => (
    <button aria-label={label} onClick={() => onChange?.(dayjs('2026-08-09'))}>
      {label}
    </button>
  ),
}));
vi.mock('@mui/x-date-pickers-pro/TimeRangePicker', () => ({
  TimeRangePicker: ({ onChange }: { onChange?: (value: unknown) => void }) => (
    <button aria-label="time range" onClick={() => onChange?.([dayjs('2026-08-08T11:00:00Z'), dayjs('2026-08-08T12:00:00Z')])}>
      time range
    </button>
  ),
}));

vi.mock('react-toastify', () => ({ toast: vi.fn() }));

describe('ModifyMarketplaceBookingDialog resource selection', () => {
  it('requires a reason before updating the booking', () => {
    resourceSelection = {
      canSelectResources: false,
      maximumResourceCount: 0,
      availableResourceIds: [],
      eligibleLocations: [],
      eligibleResources: [],
    };
    render(
      <ModifyMarketplaceBookingDialog
        bookingId="booking-1"
        expectedVersion={3}
        initialFrom="2026-08-08T09:00:00.000Z"
        initialUntil="2026-08-08T10:00:00.000Z"
        rootDataRelay={{} as modifyMarketplaceBookingDialog_query$key}
        onClose={vi.fn()}
        onModified={vi.fn()}
      />,
    );

    expect(screen.getByRole('button', { name: 'Update booking' })).toBeDisabled();
  });

  it('sends the current explicit resource selection', async () => {
    const user = userEvent.setup();
    resourceSelection = {
      canSelectResources: true,
      maximumResourceCount: 1,
      availableResourceIds: ['resource-1', 'resource-2'],
      eligibleLocations: [{ uniqueId: 'location-1', name: 'Main office' }],
      eligibleResources: [{ resource: { id: 'resource-1', name: 'Desk one' } }, { resource: { id: 'resource-2', name: 'Desk two' } }],
    };
    render(
      <ModifyMarketplaceBookingDialog
        bookingId="booking-1"
        expectedVersion={3}
        initialFrom="2026-08-08T09:00:00.000Z"
        initialUntil="2026-08-08T10:00:00.000Z"
        currentResourceIds={['resource-1']}
        rootDataRelay={{} as modifyMarketplaceBookingDialog_query$key}
        onClose={vi.fn()}
        onModified={vi.fn()}
      />,
    );

    await user.type(screen.getByRole('textbox', { name: 'Reason' }), 'Customer requested a different time');
    await user.click(screen.getByRole('button', { name: 'Update booking' }));

    expect(commitModifyMarketplaceBooking).toHaveBeenCalledWith(
      expect.objectContaining({
        variables: expect.objectContaining({
          input: expect.objectContaining({ resourceIds: ['resource-1'] }),
        }),
      }),
    );
  });

  it('does not render a picker when resource selection is unavailable', () => {
    resourceSelection = {
      canSelectResources: false,
      maximumResourceCount: 0,
      availableResourceIds: [],
      eligibleLocations: [],
      eligibleResources: [],
    };
    render(
      <ModifyMarketplaceBookingDialog
        bookingId="booking-1"
        expectedVersion={3}
        initialFrom="2026-08-08T09:00:00.000Z"
        initialUntil="2026-08-08T10:00:00.000Z"
        rootDataRelay={{} as modifyMarketplaceBookingDialog_query$key}
        onClose={vi.fn()}
        onModified={vi.fn()}
      />,
    );

    expect(screen.queryByText(/Resources \(/)).not.toBeInTheDocument();
  });

  it('refetches resource availability when the date and time range change', async () => {
    const user = userEvent.setup();
    resourceSelection = {
      canSelectResources: true,
      maximumResourceCount: 1,
      availableResourceIds: ['resource-1'],
      eligibleLocations: [{ uniqueId: 'location-1', name: 'Main office' }],
      eligibleResources: [{ resource: { id: 'resource-1', name: 'Desk one' } }],
    };
    render(
      <ModifyMarketplaceBookingDialog
        bookingId="booking-1"
        expectedVersion={3}
        initialFrom="2026-08-08T09:00:00.000Z"
        initialUntil="2026-08-08T10:00:00.000Z"
        currentLocationId="location-1"
        currentResourceIds={['resource-1']}
        rootDataRelay={{} as modifyMarketplaceBookingDialog_query$key}
        onClose={vi.fn()}
        onModified={vi.fn()}
      />,
    );
    refetchResourceSelection.mockClear();
    await user.click(screen.getByRole('button', { name: 'Date' }));
    await user.click(screen.getByRole('button', { name: 'time range' }));
    expect(refetchResourceSelection).toHaveBeenCalledWith(
      expect.objectContaining({
        bookingId: 'booking-1',
        from: expect.any(String),
        until: expect.any(String),
        locationId: 'location-1',
      }),
      { fetchPolicy: 'store-and-network' },
    );
  });
});
