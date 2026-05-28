import { fireEvent, render, screen } from '@testing-library/react';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import AvailabilityFilterBar from '../AvailabilityFilterBar';

const useFragmentMock = vi.fn();

vi.mock('react-relay', () => ({
  graphql: (value: TemplateStringsArray) => value[0],
  useFragment: (...args: unknown[]) => useFragmentMock(...args),
}));

vi.mock('@skedular/shared', () => ({
  startOfDay: (d: Date) => d,
  toShortDateWithoutWeekDay: (d: Date) => (d ? d.toString().slice(0, 10) : '2026-01-15'),
  endOfDay: (d: Date) => d,
}));

// Stub DayPicker to a simple date input so tests can interact with it
vi.mock('@/components/datePickers/day-picker', () => ({
  default: ({ onDateChanged }: { onDateChanged: (d: unknown) => void }) => (
    <input type="date" aria-label="Filter by date" defaultValue="2026-01-15" onChange={(e) => onDateChanged({ format: () => e.target.value })} />
  ),
}));

const locationsData = { locations: { edges: [{ node: { id: 'loc-1', name: 'HQ' } }, { node: { id: 'loc-2', name: 'West Office' } }] } };
const statusesData = [
  { type: 'AVAILABLE', name: 'Available' },
  { type: 'FULLY_BOOKED', name: 'Fully Booked' },
  { type: 'BLOCKED', name: 'Blocked' },
];

const defaultFilters = {
  date: '2026-01-15',
  locationIds: [],
  statuses: [],
};

describe('AvailabilityFilterBar', () => {
  beforeEach(() => {
    useFragmentMock.mockReset();
    useFragmentMock.mockImplementation((query: string) => {
      if (typeof query === 'string' && query.includes('locations')) {
        return locationsData;
      }
      return statusesData;
    });
  });

  it('renders date input with current value', () => {
    render(<AvailabilityFilterBar filters={defaultFilters} locationsRef={{} as never} statusesRef={[] as never} onChange={vi.fn()} />);
    const dateInput = screen.getByLabelText('Filter by date');
    expect(dateInput).toHaveValue('2026-01-15');
  });

  it('calls onChange when date changes', async () => {
    const onChange = vi.fn();
    const { container } = render(<AvailabilityFilterBar filters={defaultFilters} locationsRef={{} as never} statusesRef={[] as never} onChange={onChange} />);
    const dateInput = container.querySelector('input[type="date"]') as HTMLInputElement;
    fireEvent.change(dateInput, { target: { value: '2026-02-20' } });
    expect(onChange).toHaveBeenCalled();
  });

  it('dims controls when isPending is true', () => {
    const { container } = render(<AvailabilityFilterBar filters={defaultFilters} locationsRef={{} as never} statusesRef={[] as never} onChange={vi.fn()} isPending />);
    const box = container.querySelector('[aria-busy="true"]');
    expect(box).toBeTruthy();
  });
});
