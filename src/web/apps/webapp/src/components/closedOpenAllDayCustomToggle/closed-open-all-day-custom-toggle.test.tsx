import { render } from '@testing-library/react';
import { describe, expect, it, vi } from 'vitest';
import ClosedOpenAllDayCustomToggle from './closed-open-all-day-custom-toggle';

vi.mock('@/components/icons', () => ({
  ClosedAllDayIcon: () => null,
  CustomOpeningHoursIcon: () => null,
  OpenAllDayIcon: () => null,
}));

describe('ClosedOpenAllDayCustomToggle', () => {
  it('renders without error', () => {
    const { container } = render(<ClosedOpenAllDayCustomToggle defaultValue="openAllDay" onChange={vi.fn()} />);
    expect(container).toBeTruthy();
  });
});
