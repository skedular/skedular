import { describe, expect, it } from 'vitest';
import { validateMigrationSliceRecord, type MigrationSliceRecord } from '../migration-slice';

const validSlice: MigrationSliceRecord = {
  slice_id: 'teams-create-private-organisation',
  target_app: 'WebApp Teams',
  journey: 'Private organisation creation',
  status: 'ready for review',
  route_retirement_audit: 'pass',
  verification_commands: ['pnpm webapp-teams#test'],
  ready_for_user_review: true,
  accepted_before_next_slice: false,
};

describe('validateMigrationSliceRecord', () => {
  it('accepts a ready-for-review slice after route audit and verification are present', () => {
    expect(validateMigrationSliceRecord(validSlice)).toEqual({ valid: true, errors: [] });
  });

  it('blocks ready-for-review slices with unresolved route retirement audit', () => {
    const result = validateMigrationSliceRecord({
      ...validSlice,
      route_retirement_audit: 'blocked',
    });

    expect(result.valid).toBe(false);
    expect(result.errors).toContain('route_retirement_audit cannot be blocked when a slice is ready for review');
  });
});
