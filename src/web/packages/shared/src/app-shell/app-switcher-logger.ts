import type { ProductAppId } from '../app-products';
import type { AppSwitcherModel } from './app-switcher-model';

export type AppSwitcherConfigurationLogEvent = {
  event: 'web_app_switcher_configuration';
  currentAppId: ProductAppId;
  availableDestinationCount: number;
  invalidDestinationAppIds: readonly ProductAppId[];
  missingDestinationAppIds: readonly ProductAppId[];
  correlationId?: string;
};

export type AppSwitcherSelectionLogEvent = {
  event: 'web_app_switcher_selection';
  currentAppId: ProductAppId;
  destinationAppId: ProductAppId;
  correlationId?: string;
};

export type AppSwitcherLogEvent = AppSwitcherConfigurationLogEvent | AppSwitcherSelectionLogEvent;

export type AppSwitcherLogger = {
  info: (event: AppSwitcherLogEvent, message: string) => void;
  warn?: (event: AppSwitcherLogEvent, message: string) => void;
};

export const createAppSwitcherConfigurationLogEvent = (model: AppSwitcherModel, correlationId?: string): AppSwitcherConfigurationLogEvent => ({
  event: 'web_app_switcher_configuration',
  currentAppId: model.currentAppId,
  availableDestinationCount: model.availableDestinationCount,
  invalidDestinationAppIds: model.destinations.filter((destination) => destination.availability === 'invalid-url').map((destination) => destination.appId),
  missingDestinationAppIds: model.destinations.filter((destination) => destination.availability === 'missing-url').map((destination) => destination.appId),
  correlationId,
});

export const createAppSwitcherSelectionLogEvent = (currentAppId: ProductAppId, destinationAppId: ProductAppId, correlationId?: string): AppSwitcherSelectionLogEvent => ({
  event: 'web_app_switcher_selection',
  currentAppId,
  destinationAppId,
  correlationId,
});

export const logAppSwitcherConfiguration = (logger: AppSwitcherLogger, model: AppSwitcherModel, correlationId?: string): AppSwitcherConfigurationLogEvent => {
  const event = createAppSwitcherConfigurationLogEvent(model, correlationId);
  const message = event.invalidDestinationAppIds.length > 0 ? 'Web app switcher configuration has invalid destinations' : 'Web app switcher configuration resolved';

  if (event.invalidDestinationAppIds.length > 0 && logger.warn) {
    logger.warn(event, message);
  } else {
    logger.info(event, message);
  }

  return event;
};

export const logAppSwitcherSelection = (
  logger: AppSwitcherLogger,
  currentAppId: ProductAppId,
  destinationAppId: ProductAppId,
  correlationId?: string,
): AppSwitcherSelectionLogEvent => {
  const event = createAppSwitcherSelectionLogEvent(currentAppId, destinationAppId, correlationId);
  logger.info(event, 'Web app switcher destination selected');
  return event;
};
