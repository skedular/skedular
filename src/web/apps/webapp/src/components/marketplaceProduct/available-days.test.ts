import dayjs from 'dayjs';
import { describe, expect, it } from 'vitest';
import { getAvailableDaysGuidance, getAvailableDaysLabel, isDateAvailableForPrice } from './available-days';

describe('available days', () => {
  it('treats an empty collection as every calendar day', () => {
    expect(getAvailableDaysLabel([])).toBe('Available every calendar day');
    expect(isDateAvailableForPrice(dayjs('2026-07-19'), [])).toBe(true);
  });

  it('uses the Sunday-through-Saturday calendar mapping for date eligibility', () => {
    expect(getAvailableDaysLabel(['MONDAY', 'SUNDAY'])).toBe('Available on Sunday, Monday');
    expect(isDateAvailableForPrice(dayjs('2026-07-19'), ['SUNDAY'])).toBe(true);
    expect(isDateAvailableForPrice(dayjs('2026-07-20'), ['SUNDAY'])).toBe(false);
  });

  it('explains that existing availability rules still apply', () => {
    expect(getAvailableDaysGuidance(['SATURDAY'])).toContain('Opening hours and resource availability still apply');
  });
});
