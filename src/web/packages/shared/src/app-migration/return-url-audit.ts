export const backendReturnUrlRiskTypes = ['payment', 'authentication', 'notification', 'external-callback', 'backend-redirect'] as const;

export type BackendReturnUrlRiskType = (typeof backendReturnUrlRiskTypes)[number];

export type BackendReturnUrlAuditInput = {
  route: string;
  action: 'keep' | 'transition' | 'redirect' | 'block' | 'delete';
  riskTypes: readonly BackendReturnUrlRiskType[];
  referencesFound: readonly string[];
  appSpecificTargetUrl?: string;
};

export type BackendReturnUrlAuditResult = {
  state: 'pass' | 'blocked' | 'not applicable';
  blockers: readonly string[];
};

export const auditBackendReturnUrlUsage = ({ route, action, riskTypes, referencesFound, appSpecificTargetUrl }: BackendReturnUrlAuditInput): BackendReturnUrlAuditResult => {
  const blockers: string[] = [];

  if (!route.trim()) {
    blockers.push('route is required');
  }

  if (riskTypes.length === 0 && referencesFound.length === 0) {
    return {
      state: blockers.length === 0 ? 'not applicable' : 'blocked',
      blockers,
    };
  }

  if ((action === 'delete' || action === 'redirect') && referencesFound.length > 0) {
    blockers.push('backend-originated return URL references must be replaced before delete or redirect');
  }

  if ((action === 'delete' || action === 'redirect' || action === 'transition') && riskTypes.length > 0 && !appSpecificTargetUrl?.trim()) {
    blockers.push('app-specific target URL strategy is required for risky route changes');
  }

  return {
    state: blockers.length === 0 ? 'pass' : 'blocked',
    blockers,
  };
};
