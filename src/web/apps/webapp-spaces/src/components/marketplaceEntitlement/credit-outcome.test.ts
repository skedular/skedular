import { describe, expect, it } from 'vitest';
import { getCreditOutcomeMessage } from './credit-outcome';

describe('getCreditOutcomeMessage', () => {
  it('returns the restore outcome', () => expect(getCreditOutcomeMessage(true, 'The booking credit will be restored.')).toContain('restored'));
  it('returns the forfeit outcome', () => expect(getCreditOutcomeMessage(true, 'The booking credit will be forfeited.')).toContain('forfeited'));
  it('hides outcomes for reservation bookings', () => expect(getCreditOutcomeMessage(false, 'restored')).toBeNull());
});
