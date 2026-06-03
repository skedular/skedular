import { render } from '@testing-library/react';
import { describe, expect, it, vi } from 'vitest';
import DayPicker from './day-picker';

vi.mock('@skedular/ui', () => ({
  LeadIconTypography: ({ children }: { children: React.ReactNode }) => <span>{children}</span>,
  SmallIconTypography: ({ children }: { children: React.ReactNode }) => <span>{children}</span>,
  PushToRight: ({ children }: { children: React.ReactNode }) => <div>{children}</div>,
  StackRow: ({ children }: { children: React.ReactNode }) => <div>{children}</div>,
}));
vi.mock('@skedular/shared', () => ({
  startOfDay: vi.fn(() => ({ format: vi.fn(() => 'today') })),
  toShortDateWithoutWeekDay: vi.fn(() => 'Mon, 1 Jan'),
}));
vi.mock('@/components/generics', () => ({
  EmptyCalendarToolbar: () => null,
  SimpleCalendarSlotProps: {},
}));
vi.mock('@/components/icons', () => ({
  CalendarIcon: () => null,
}));
vi.mock('@/components/styled', () => ({
  DefaultSelect: ({ children }: { children?: React.ReactNode }) => <select>{children}</select>,
}));
vi.mock('@mui/x-date-pickers/StaticDatePicker', () => ({
  StaticDatePicker: () => <div data-testid="static-date-picker" />,
}));

describe('DayPicker', () => {
  it('renders without error', () => {
    const { container } = render(<DayPicker onDateChanged={vi.fn()} />);
    expect(container).toBeTruthy();
  });
});
