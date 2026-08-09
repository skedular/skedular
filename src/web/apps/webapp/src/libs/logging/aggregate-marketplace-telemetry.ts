type LogPayload = Record<string, boolean | number | string | undefined>;

export type AggregateMarketplaceLogger = {
  info: (payload: LogPayload, message: string) => void;
  warn: (payload: LogPayload, message: string) => void;
  error: (payload: LogPayload, message: string) => void;
};

type BaseTelemetryInput = {
  logger: AggregateMarketplaceLogger;
  correlationId?: string;
};

const withCorrelation = (event: string, correlationId: string | undefined, payload: LogPayload = {}) => ({
  event,
  correlationId: correlationId ?? `webapp-${event}`,
  ...payload,
});

export const logAggregateMarketplaceDiscoveryStarted = ({ logger, correlationId, isSignedIn, hasFilters }: BaseTelemetryInput & { isSignedIn: boolean; hasFilters: boolean }) =>
  logger.info(withCorrelation('AggregateMarketplaceDiscoveryStarted', correlationId, { isSignedIn, hasFilters }), 'Aggregate marketplace discovery started');

export const logAggregateMarketplaceDiscoveryCompleted = ({
  logger,
  correlationId,
  eligibleLocationCount,
  isEmptyState,
}: BaseTelemetryInput & { eligibleLocationCount: number; isEmptyState: boolean }) =>
  logger.info(withCorrelation('AggregateMarketplaceDiscoveryCompleted', correlationId, { eligibleLocationCount, isEmptyState }), 'Aggregate marketplace discovery completed');

export const logAggregateMarketplaceLocationSelected = ({
  logger,
  correlationId,
  locationId,
  organizationId,
}: BaseTelemetryInput & { locationId: string; organizationId: string }) =>
  logger.info(withCorrelation('AggregateMarketplaceLocationSelected', correlationId, { locationId, organizationId }), 'Aggregate marketplace location selected');

export const logCustomerPurchaseHubLoaded = ({
  logger,
  correlationId,
  customerIdHash,
  bookingCount,
  subscriptionCount,
}: BaseTelemetryInput & { customerIdHash: string; bookingCount: number; subscriptionCount: number }) =>
  logger.info(withCorrelation('CustomerPurchaseHubLoaded', correlationId, { customerIdHash, bookingCount, subscriptionCount }), 'Customer purchase hub loaded');

export const logCustomerSelfServiceActionStarted = ({
  logger,
  correlationId,
  actionType,
  purchaseType,
  purchaseId,
}: BaseTelemetryInput & { actionType: string; purchaseType: string; purchaseId: string }) =>
  logger.info(withCorrelation('CustomerSelfServiceActionStarted', correlationId, { actionType, purchaseType, purchaseId }), 'Customer self-service action started');

export const logCustomerSelfServiceActionRejected = ({
  logger,
  correlationId,
  actionType,
  purchaseType,
  reasonCode,
}: BaseTelemetryInput & { actionType: string; purchaseType: string; reasonCode: string }) =>
  logger.warn(withCorrelation('CustomerSelfServiceActionRejected', correlationId, { actionType, purchaseType, reasonCode }), 'Customer self-service action rejected');

export const logCustomerMarketplaceBookingModificationStarted = ({ logger, correlationId, bookingId }: BaseTelemetryInput & { bookingId: string }) =>
  logger.info(withCorrelation('CustomerMarketplaceBookingModificationStarted', correlationId, { bookingId }), 'Customer marketplace booking modification started');

export const logCustomerMarketplaceBookingModificationCompleted = ({ logger, correlationId, bookingId }: BaseTelemetryInput & { bookingId: string }) =>
  logger.info(withCorrelation('CustomerMarketplaceBookingModificationCompleted', correlationId, { bookingId }), 'Customer marketplace booking modification completed');

export const logCustomerMarketplaceBookingModificationFailed = ({ logger, correlationId, bookingId, reasonCode }: BaseTelemetryInput & { bookingId: string; reasonCode: string }) =>
  logger.warn(withCorrelation('CustomerMarketplaceBookingModificationFailed', correlationId, { bookingId, reasonCode }), 'Customer marketplace booking modification failed');

export const logUnsupportedWebappPathHandled = ({
  logger,
  correlationId,
  pathCategory,
  ownerClassification,
}: BaseTelemetryInput & { pathCategory: string; ownerClassification?: string }) =>
  logger.warn(withCorrelation('UnsupportedWebappPathHandled', correlationId, { pathCategory, ownerClassification }), 'Unsupported webapp path handled in place');

export const logOwnerSpecificMarketplaceEntryResolved = ({
  logger,
  correlationId,
  isCustomDomain,
  entryPointType,
}: BaseTelemetryInput & { isCustomDomain: boolean; entryPointType: string }) =>
  logger.info(withCorrelation('OwnerSpecificMarketplaceEntryResolved', correlationId, { isCustomDomain, entryPointType }), 'Owner-specific marketplace entry resolved');

export const logAggregateMarketplaceFailure = ({ logger, correlationId, workflow, reasonCode }: BaseTelemetryInput & { workflow: string; reasonCode: string }) =>
  logger.error(withCorrelation('AggregateMarketplaceFailure', correlationId, { workflow, reasonCode }), 'Aggregate marketplace workflow failed');
