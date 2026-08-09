import { render, screen } from '@testing-library/react';
import { describe, expect, it, vi } from 'vitest';
import dayjs from 'dayjs';
import utc from 'dayjs/plugin/utc';
import MarketplaceBookingModificationForm from './marketplace-booking-modification-form';

dayjs.extend(utc);

vi.mock('@mui/x-date-pickers/DatePicker', () => ({ DatePicker: () => <input aria-label="Date" /> }));
vi.mock('@mui/x-date-pickers-pro/TimeRangePicker', () => ({ TimeRangePicker: () => <input aria-label="Time range" /> }));

const baseProps = {
  initialFrom: '2026-08-08T09:00:00.000Z',
  initialUntil: '2026-08-08T10:00:00.000Z',
  currentResourceIds: ['resource-1'],
  currentResources: [{ id: 'resource-1', name: 'Desk 1' }],
  currentLocationId: 'location-1',
  locations: [{ id: 'location-1', name: 'Main office' }],
  resources: [{ id: 'resource-1', name: 'Desk 1', available: true }],
  canSelectResources: true,
  maximumResourceCount: 1,
  isSubmitting: false,
  onSubmit: vi.fn(),
  onCancel: vi.fn(),
};

describe('MarketplaceBookingModificationForm', () => {
  it('renders the operator/customer shared modification foundation with required reason', () => {
    render(<MarketplaceBookingModificationForm {...baseProps} />);
    expect(screen.getByText('Schedule')).toBeInTheDocument();
    expect(screen.getByRole('textbox', { name: 'Reason' })).toBeRequired();
    expect(screen.getByRole('button', { name: 'Update booking' })).toBeDisabled();
  });

  it('shows the resource entitlement limit', () => {
    render(<MarketplaceBookingModificationForm {...baseProps} maximumResourceCount={2} />);
    expect(screen.getByText('Resources (1/2)')).toBeInTheDocument();
    expect(screen.getByText('Select up to 2 resources.')).toBeInTheDocument();
  });
});
