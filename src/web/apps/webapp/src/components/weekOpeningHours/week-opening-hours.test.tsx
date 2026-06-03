import { render } from '@testing-library/react';
import { describe, expect, it, vi } from 'vitest';
import WeekOpeningHours from './week-opening-hours';

vi.mock('@skedular/ui', () => ({
  ErrorTypography: ({ children }: { children: React.ReactNode }) => <span>{children}</span>,
  FormFieldLabel: ({ children }: { children: React.ReactNode }) => <label>{children}</label>,
  StackColumn: ({ children }: { children: React.ReactNode }) => <div>{children}</div>,
  StackRow: ({ children }: { children: React.ReactNode }) => <div>{children}</div>,
  defaultPadding: 2,
}));
vi.mock('@skedular/shared', () => ({
  getOpeningHoursFromDateTime: vi.fn(() => '09:00'),
  toOpeningHoursFromTime: vi.fn(() => null),
}));
vi.mock('@/components/closedOpenAllDayCustomToggle', () => ({
  ClosedOpenAllDayCustomToggle: () => <div data-testid="toggle" />,
}));
vi.mock('@mui/x-date-pickers-pro/TimeRangePicker', () => ({
  TimeRangePicker: () => <div data-testid="time-range-picker" />,
}));
vi.mock('usehooks-ts', () => ({
  useDebounceCallback: (fn: unknown) => fn,
}));
vi.mock('react-relay', () => ({
  graphql: vi.fn(),
  useFragment: vi.fn(() => ({
    openingHours: {
      monday: { closed: false, openAllDay: false, from: '09:00', until: '17:00' },
      tuesday: { closed: false, openAllDay: false, from: '09:00', until: '17:00' },
      wednesday: { closed: false, openAllDay: false, from: '09:00', until: '17:00' },
      thursday: { closed: false, openAllDay: false, from: '09:00', until: '17:00' },
      friday: { closed: false, openAllDay: false, from: '09:00', until: '17:00' },
      saturday: { closed: true, openAllDay: false, from: null, until: null },
      sunday: { closed: true, openAllDay: false, from: null, until: null },
    },
  })),
}));

const defaultValue = {
  monday: { closed: false, openAllDay: false, from: '09:00', until: '17:00' },
  tuesday: { closed: false, openAllDay: false, from: '09:00', until: '17:00' },
  wednesday: { closed: false, openAllDay: false, from: '09:00', until: '17:00' },
  thursday: { closed: false, openAllDay: false, from: '09:00', until: '17:00' },
  friday: { closed: false, openAllDay: false, from: '09:00', until: '17:00' },
  saturday: { closed: true, openAllDay: false, from: null, until: null },
  sunday: { closed: true, openAllDay: false, from: null, until: null },
};

describe('WeekOpeningHours', () => {
  it('renders without error', () => {
    const { container } = render(<WeekOpeningHours rootDataRelay={{} as never} defaultValue={defaultValue} onWeekOpeningHoursDetailUpdateClick={vi.fn()} />);
    expect(container).toBeTruthy();
  });
});
