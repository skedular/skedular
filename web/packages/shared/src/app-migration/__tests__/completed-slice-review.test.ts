import { describe, expect, it } from 'vitest';
import { validateCompletedMigrationSliceReview } from '../migration-slice';

describe('validateCompletedMigrationSliceReview', () => {
  it('requires lint, test, build, and manual review evidence', () => {
    expect(
      validateCompletedMigrationSliceReview({
        slice_id: 'spaces-products-operator-shell',
        lint: 'pnpm webapp-spaces#lint passed',
        tests: 'pnpm webapp-spaces#test passed',
        build: 'pnpm webapp-spaces#build passed',
        manual_review: 'http://localhost:15004/products',
        ready_for_user_review: true,
      }),
    ).toEqual({ valid: true, errors: [] });
  });

  it('rejects completed review slices without manual review details', () => {
    const result = validateCompletedMigrationSliceReview({
      slice_id: 'spaces-products-operator-shell',
      lint: 'pnpm webapp-spaces#lint passed',
      tests: 'pnpm webapp-spaces#test passed',
      build: 'pnpm webapp-spaces#build passed',
      manual_review: '',
      ready_for_user_review: true,
    });

    expect(result.valid).toBe(false);
    expect(result.errors).toContain('manual_review path is required');
  });
});
