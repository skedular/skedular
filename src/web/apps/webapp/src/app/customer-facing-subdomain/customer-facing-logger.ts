import { logAppSelection, type AppSelectionLogger } from '@skedular/shared';
import type { CustomerFacingEntryPoint } from './customer-facing-entry-point';

type Input = {
  logger: AppSelectionLogger;
  entryPoint: CustomerFacingEntryPoint;
  correlationId?: string;
};

const organisationTypeByEntryPoint: Partial<Record<CustomerFacingEntryPoint, 'marketplace' | 'private'>> = {
  'co-working-subdomain': 'marketplace',
  'private-organisation-subdomain': 'private',
};

export const logCustomerFacingEntryPoint = ({ logger, entryPoint, correlationId }: Input) =>
  logAppSelection(logger, {
    appId: 'webapp',
    reason: 'customer-entry',
    organisationType: organisationTypeByEntryPoint[entryPoint],
    correlationId: correlationId ?? `webapp-customer-entry-${entryPoint}`,
  });
