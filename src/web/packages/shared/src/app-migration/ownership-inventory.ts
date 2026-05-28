export const ownershipTargets = ['WebApp', 'WebApp Spaces', 'WebApp Teams', '@skedular/ui', '@skedular/shared', 'transition'] as const;
export const ownershipItemTypes = ['route', 'page', 'component', 'hook', 'utility', 'provider', 'query', 'generated artefact', 'config', 'documentation'] as const;
export const ownershipRiskValues = ['yes', 'no', 'unknown'] as const;

export type OwnershipTarget = (typeof ownershipTargets)[number];
export type OwnershipItemType = (typeof ownershipItemTypes)[number];
export type OwnershipRiskValue = (typeof ownershipRiskValues)[number];

export type OwnershipInventoryItem = {
  item_path: string;
  item_type: OwnershipItemType;
  current_owner: string;
  target_owner: OwnershipTarget;
  reason: string;
  backend_return_url_risk: OwnershipRiskValue;
  relay_impact: OwnershipRiskValue;
  tests_required: string;
  transition_condition?: string;
};

export type OwnershipInventoryValidationResult = {
  valid: boolean;
  errors: readonly string[];
};

const isOneOf = <T extends readonly string[]>(values: T, value: string): value is T[number] => values.includes(value as T[number]);

export const validateOwnershipInventoryItem = (item: OwnershipInventoryItem): OwnershipInventoryValidationResult => {
  const errors: string[] = [];

  if (!item.item_path.trim()) {
    errors.push('item_path is required');
  }

  if (!isOneOf(ownershipItemTypes, item.item_type)) {
    errors.push('item_type is invalid');
  }

  if (!item.current_owner.trim()) {
    errors.push('current_owner is required');
  }

  if (!isOneOf(ownershipTargets, item.target_owner)) {
    errors.push('target_owner is invalid');
  }

  if (!item.reason.trim()) {
    errors.push('reason is required');
  }

  if (!isOneOf(ownershipRiskValues, item.backend_return_url_risk)) {
    errors.push('backend_return_url_risk is invalid');
  }

  if (!isOneOf(ownershipRiskValues, item.relay_impact)) {
    errors.push('relay_impact is invalid');
  }

  if (!item.tests_required.trim()) {
    errors.push('tests_required is required');
  }

  if (item.target_owner === 'transition' && !item.transition_condition?.trim()) {
    errors.push('transition_condition is required for transition ownership');
  }

  return {
    valid: errors.length === 0,
    errors,
  };
};
