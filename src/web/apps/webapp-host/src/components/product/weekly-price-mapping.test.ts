import { toRequiredDaysPerWeekInput } from './product-editor-shared';
import { describe, expect, it } from 'vitest';

describe('weekly price mapping', () => {
  it('sends the exact weekly count and clears it for other cadences', () => {
    expect(toRequiredDaysPerWeekInput('WEEKLY', '2')).toBe(2);
    expect(toRequiredDaysPerWeekInput('WEEKLY', '  ')).toBeNull();
    expect(toRequiredDaysPerWeekInput('MONTHLY', '2')).toBeNull();
  });
});
