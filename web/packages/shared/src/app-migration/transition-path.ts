export type TransitionPathRecord = {
  source_route: string;
  target_route: string;
  action: 'keep' | 'redirect' | 'block' | 'transition';
  owner: 'WebApp' | 'WebApp Spaces' | 'WebApp Teams' | 'Shared';
  removal_condition?: string;
  backend_return_url_checked: boolean;
};

export type TransitionPathValidationResult = {
  valid: boolean;
  errors: readonly string[];
};

export const validateTransitionPathRecord = (record: TransitionPathRecord): TransitionPathValidationResult => {
  const errors: string[] = [];

  if (!record.source_route.trim()) {
    errors.push('source_route is required');
  }

  if (!record.target_route.trim()) {
    errors.push('target_route is required');
  }

  if ((record.action === 'redirect' || record.action === 'transition') && !record.removal_condition?.trim()) {
    errors.push('removal_condition is required for redirect or transition paths');
  }

  if (record.action === 'redirect' && !record.backend_return_url_checked) {
    errors.push('backend_return_url_checked must be true before redirecting a route');
  }

  return {
    valid: errors.length === 0,
    errors,
  };
};
