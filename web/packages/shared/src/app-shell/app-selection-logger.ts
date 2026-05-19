import type { OrganisationType, ProductAppId } from '../app-products';

export type AppSelectionLogInput = {
  appId: ProductAppId;
  reason: 'direct-url' | 'organisation-filter' | 'customer-entry' | 'manual-review';
  organisationType?: OrganisationType;
  organisationCount?: number;
  correlationId?: string;
};

export type AppSelectionLogEvent = AppSelectionLogInput & {
  event: 'web_app_selection';
};

export type AppSelectionLogger = {
  info: (event: AppSelectionLogEvent, message: string) => void;
};

export const createAppSelectionLogEvent = (input: AppSelectionLogInput): AppSelectionLogEvent => ({
  event: 'web_app_selection',
  ...input,
});

export const logAppSelection = (logger: AppSelectionLogger, input: AppSelectionLogInput): AppSelectionLogEvent => {
  const event = createAppSelectionLogEvent(input);
  logger.info(event, 'Web app selection resolved');
  return event;
};
