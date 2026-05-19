export const migrationSliceStatuses = ['proposed', 'in progress', 'ready for review', 'accepted', 'blocked'] as const;

export type MigrationSliceStatus = (typeof migrationSliceStatuses)[number];

export type MigrationSliceRecord = {
  slice_id: string;
  target_app: 'WebApp' | 'WebApp Spaces' | 'WebApp Teams' | 'Shared';
  journey: string;
  status: MigrationSliceStatus;
  route_retirement_audit: 'pass' | 'blocked' | 'not applicable';
  verification_commands: readonly string[];
  ready_for_user_review: boolean;
  accepted_before_next_slice: boolean;
};

export type MigrationSliceValidationResult = {
  valid: boolean;
  errors: readonly string[];
};

export type CompletedMigrationSliceReview = {
  slice_id: string;
  lint: string;
  tests: string;
  build: string;
  manual_review: string;
  ready_for_user_review: boolean;
};

export const validateMigrationSliceRecord = (record: MigrationSliceRecord): MigrationSliceValidationResult => {
  const errors: string[] = [];

  if (!record.slice_id.trim()) {
    errors.push('slice_id is required');
  }

  if (!record.journey.trim()) {
    errors.push('journey is required');
  }

  if (record.status === 'ready for review' && !record.ready_for_user_review) {
    errors.push('ready_for_user_review must be true when status is ready for review');
  }

  if (record.status === 'accepted' && !record.accepted_before_next_slice) {
    errors.push('accepted_before_next_slice must be true when status is accepted');
  }

  if (record.status === 'ready for review' && record.route_retirement_audit === 'blocked') {
    errors.push('route_retirement_audit cannot be blocked when a slice is ready for review');
  }

  if (record.status === 'ready for review' && record.verification_commands.length === 0) {
    errors.push('verification_commands are required when a slice is ready for review');
  }

  return {
    valid: errors.length === 0,
    errors,
  };
};

export const validateCompletedMigrationSliceReview = (review: CompletedMigrationSliceReview): MigrationSliceValidationResult => {
  const errors: string[] = [];

  if (!review.slice_id.trim()) {
    errors.push('slice_id is required');
  }

  if (!review.lint.trim()) {
    errors.push('lint result is required');
  }

  if (!review.tests.trim()) {
    errors.push('test result is required');
  }

  if (!review.build.trim()) {
    errors.push('build result is required');
  }

  if (!review.manual_review.trim()) {
    errors.push('manual_review path is required');
  }

  if (!review.ready_for_user_review) {
    errors.push('ready_for_user_review must be true for completed review slices');
  }

  return {
    valid: errors.length === 0,
    errors,
  };
};
