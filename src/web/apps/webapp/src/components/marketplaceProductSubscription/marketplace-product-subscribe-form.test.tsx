import { isWeeklySelectionComplete, toggleWeeklySelectedDay, toWeeklySelectedDaysInput } from './weekly-selection';
import { describe, expect, it } from 'vitest';

describe('marketplace weekly subscription selection', () => {
  it('submits the explicit weekly-only input', () => {
    expect(toWeeklySelectedDaysInput(true, ['TUESDAY', 'THURSDAY'])).toEqual(['TUESDAY', 'THURSDAY']);
    expect(toWeeklySelectedDaysInput(false, ['TUESDAY', 'THURSDAY'])).toEqual([]);
  });

  it('limits selections and retains them until the pricing option changes', () => {
    expect(toggleWeeklySelectedDay(['TUESDAY'], 'THURSDAY', 2)).toEqual(['TUESDAY', 'THURSDAY']);
    expect(toggleWeeklySelectedDay(['TUESDAY', 'THURSDAY'], 'FRIDAY', 2)).toEqual(['TUESDAY', 'THURSDAY']);
    expect(toggleWeeklySelectedDay(['TUESDAY', 'THURSDAY'], 'TUESDAY', 2)).toEqual(['THURSDAY']);
  });

  it('blocks checkout until the exact weekly count is selected', () => {
    expect(isWeeklySelectionComplete(true, ['TUESDAY'], 2)).toBe(false);
    expect(isWeeklySelectionComplete(true, ['TUESDAY', 'THURSDAY'], 2)).toBe(true);
    expect(isWeeklySelectionComplete(false, [], 0)).toBe(true);
  });
});
