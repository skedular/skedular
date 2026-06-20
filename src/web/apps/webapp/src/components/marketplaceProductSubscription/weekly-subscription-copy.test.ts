import { getResourceAssignmentPendingMessage } from './resource-assignment-status';
import { describe, expect, it } from 'vitest';

describe('weekly subscription customer copy', () => {
  it('explains whether an unassigned selected-day booking remains under automatic repair', () => {
    expect(getResourceAssignmentPendingMessage(false)).toBe('We will keep trying to assign a resource for this selected date.');
    expect(getResourceAssignmentPendingMessage(true)).toBe('A space administrator is handling this booking individually.');
  });
});
