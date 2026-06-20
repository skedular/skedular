import { getResourceAssignmentPendingMessage } from './resource-assignment-status';
import { describe, expect, it } from 'vitest';

describe('weekly booking-shell operator copy', () => {
  it('explains automatic repair and individual overrides', () => {
    expect(getResourceAssignmentPendingMessage(false)).toBe('Skedular will keep trying to assign a compatible resource on this booking date.');
    expect(getResourceAssignmentPendingMessage(true)).toBe('This individual booking was updated by an operator and will not be changed automatically.');
  });
});
