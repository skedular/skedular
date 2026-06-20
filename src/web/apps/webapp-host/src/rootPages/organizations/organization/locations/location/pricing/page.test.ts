import { describe, expect, it } from 'vitest';
import { getWeeklyRequiredDaysError, hasValidWeeklyRequiredDays, sanitizeWeeklyRequiredDays } from './weekly-required-days';

describe('location weekly price validation', () => {
  it.each(['0', '1.5', '3'])('rejects invalid weekly required-day input before autosave: %s', (requiredDaysPerWeek) => {
    expect(hasValidWeeklyRequiredDays('WEEKLY', requiredDaysPerWeek, ['MONDAY', 'TUESDAY'])).toBe(false);
  });

  it('allows only a whole-number digit to be entered', () => {
    expect(sanitizeWeeklyRequiredDays('-1.5')).toBe('1');
  });

  it('treats no configured weekdays as all weekdays', () => {
    expect(hasValidWeeklyRequiredDays('WEEKLY', '7', [])).toBe(true);
    expect(getWeeklyRequiredDaysError('WEEKLY', '7', [])).toBeNull();
  });

  it('explains the available-weekday limit before autosave', () => {
    expect(getWeeklyRequiredDaysError('WEEKLY', '5', ['MONDAY', 'WEDNESDAY', 'SUNDAY'])).toBe('Choose a whole number from 1 to 3.');
  });

  it('accepts an empty weekly value and a whole number within the available-day pool', () => {
    expect(hasValidWeeklyRequiredDays('WEEKLY', '', ['MONDAY', 'TUESDAY'])).toBe(true);
    expect(hasValidWeeklyRequiredDays('WEEKLY', '2', ['MONDAY', 'TUESDAY'])).toBe(true);
  });
});
