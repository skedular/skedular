import { render } from '@testing-library/react';
import { describe, expect, it, vi } from 'vitest';
import WeekPicker from './week-picker';

vi.mock('@skedular/ui', () => ({
  SmallIconTypography: ({ children }: { children: React.ReactNode }) => <span>{children}</span>,
}));
const mockDay: Record<string, unknown> = {};
mockDay.format = vi.fn(() => 'Mon');
mockDay.add = vi.fn(() => mockDay);
mockDay.subtract = vi.fn(() => mockDay);
mockDay.isSame = vi.fn(() => false);
mockDay.isAfter = vi.fn(() => false);
mockDay.isBefore = vi.fn(() => false);
mockDay.month = vi.fn(() => 0);
mockDay.year = vi.fn(() => 2024);
mockDay.date = vi.fn(() => 1);
mockDay.day = vi.fn(() => 1);

vi.mock('@skedular/shared', () => ({
  startOfWeek: vi.fn(() => mockDay),
  endOfWeek: vi.fn(() => mockDay),
  isInSameWeek: vi.fn(() => false),
  isInSameMonth: vi.fn(() => true),
  isInSameYear: vi.fn(() => true),
}));
vi.mock('@/components/icons', () => ({
  ArrowDownIcon: () => null,
  ArrowLeftIcon: () => null,
  ArrowRightIcon: () => null,
  TodayIcon: () => null,
}));
vi.mock('@mui/x-date-pickers/DateCalendar', () => ({
  DateCalendar: () => <div data-testid="date-calendar" />,
}));
vi.mock('@mui/x-date-pickers/PickerDay', () => ({
  PickerDay: ({ children }: { children?: React.ReactNode }) => <div>{children}</div>,
}));

describe('WeekPicker', () => {
  it('renders without error', () => {
    const { container } = render(<WeekPicker onWeekChanged={vi.fn()} />);
    expect(container).toBeTruthy();
  });
});
