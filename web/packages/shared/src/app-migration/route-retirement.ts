export const routeRetirementActions = ['keep', 'redirect', 'block', 'delete', 'transition'] as const;
export const returnUrlAuditStates = ['pass', 'blocked', 'not applicable'] as const;

export type RouteRetirementAction = (typeof routeRetirementActions)[number];
export type ReturnUrlAuditState = (typeof returnUrlAuditStates)[number];

export type RouteRetirementRecord = {
  route: string;
  current_owner: string;
  target_owner: string;
  action: RouteRetirementAction;
  backend_originated_return_url_audit: ReturnUrlAuditState;
  configuration_source: string;
  removal_condition?: string;
  manual_review_path: string;
  notes: string;
};

export type RouteRetirementValidationResult = {
  valid: boolean;
  errors: readonly string[];
};

export type RouteRetirementRegisterValidationResult = {
  valid: boolean;
  errors: readonly string[];
  routeCount: number;
};

const isOneOf = <T extends readonly string[]>(values: T, value: string): value is T[number] => values.includes(value as T[number]);

export const validateRouteRetirementRecord = (record: RouteRetirementRecord): RouteRetirementValidationResult => {
  const errors: string[] = [];

  if (!record.route.trim()) {
    errors.push('route is required');
  }

  if (!record.current_owner.trim()) {
    errors.push('current_owner is required');
  }

  if (!record.target_owner.trim()) {
    errors.push('target_owner is required');
  }

  if (!isOneOf(routeRetirementActions, record.action)) {
    errors.push('action is invalid');
  }

  if (!isOneOf(returnUrlAuditStates, record.backend_originated_return_url_audit)) {
    errors.push('backend_originated_return_url_audit is invalid');
  }

  if ((record.action === 'delete' || record.action === 'redirect') && record.backend_originated_return_url_audit !== 'pass') {
    errors.push('delete or redirect requires a passed backend-originated return URL audit');
  }

  if (record.action === 'transition' && !record.removal_condition?.trim()) {
    errors.push('removal_condition is required for transition routes');
  }

  if (!record.manual_review_path.trim()) {
    errors.push('manual_review_path is required');
  }

  return {
    valid: errors.length === 0,
    errors,
  };
};

export const validateRouteRetirementRegister = (records: readonly RouteRetirementRecord[]): RouteRetirementRegisterValidationResult => {
  const errors: string[] = [];
  const seenRoutes = new Set<string>();

  records.forEach((record, index) => {
    const routeKey = record.route.trim();

    if (seenRoutes.has(routeKey)) {
      errors.push(`route ${routeKey} is duplicated`);
    }

    seenRoutes.add(routeKey);

    const validation = validateRouteRetirementRecord(record);
    validation.errors.forEach((error) => {
      errors.push(`record ${index + 1} (${record.route}): ${error}`);
    });
  });

  return {
    valid: errors.length === 0,
    errors,
    routeCount: records.length,
  };
};
