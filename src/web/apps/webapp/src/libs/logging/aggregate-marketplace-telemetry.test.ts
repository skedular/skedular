import { describe, expect, it, vi } from 'vitest';
import {
  logAggregateMarketplaceDiscoveryCompleted,
  logAggregateMarketplaceDiscoveryStarted,
  logAggregateMarketplaceFailure,
  logAggregateMarketplaceLocationSelected,
  logCustomerPurchaseHubLoaded,
  logCustomerMarketplaceBookingModificationCompleted,
  logCustomerMarketplaceBookingModificationFailed,
  logCustomerMarketplaceBookingModificationStarted,
  logCustomerSelfServiceActionRejected,
  logCustomerSelfServiceActionStarted,
  logOwnerSpecificMarketplaceEntryResolved,
  logUnsupportedWebappPathHandled,
  type AggregateMarketplaceLogger,
} from './aggregate-marketplace-telemetry';

const createLogger = (): AggregateMarketplaceLogger => ({
  info: vi.fn(),
  warn: vi.fn(),
  error: vi.fn(),
});

describe('aggregate marketplace telemetry', () => {
  it('logs aggregate marketplace discovery lifecycle with safe structured context', () => {
    const logger = createLogger();

    logAggregateMarketplaceDiscoveryStarted({ logger, correlationId: 'request-1', isSignedIn: false, hasFilters: true });
    logAggregateMarketplaceDiscoveryCompleted({ logger, correlationId: 'request-1', eligibleLocationCount: 2, isEmptyState: false });

    expect(logger.info).toHaveBeenNthCalledWith(
      1,
      expect.objectContaining({ event: 'AggregateMarketplaceDiscoveryStarted', correlationId: 'request-1', isSignedIn: false, hasFilters: true }),
      'Aggregate marketplace discovery started',
    );
    expect(logger.info).toHaveBeenNthCalledWith(
      2,
      expect.objectContaining({ event: 'AggregateMarketplaceDiscoveryCompleted', correlationId: 'request-1', eligibleLocationCount: 2, isEmptyState: false }),
      'Aggregate marketplace discovery completed',
    );
  });

  it('logs location selection and purchase hub load without sensitive customer payloads', () => {
    const logger = createLogger();

    logAggregateMarketplaceLocationSelected({ logger, correlationId: 'request-2', locationId: 'location-1', organizationId: 'organization-1' });
    logCustomerPurchaseHubLoaded({ logger, correlationId: 'request-2', customerIdHash: 'customer-hash', bookingCount: 1, subscriptionCount: 1 });

    expect(logger.info).toHaveBeenNthCalledWith(
      1,
      expect.objectContaining({ event: 'AggregateMarketplaceLocationSelected', locationId: 'location-1', organizationId: 'organization-1' }),
      'Aggregate marketplace location selected',
    );
    expect(logger.info).toHaveBeenNthCalledWith(
      2,
      expect.not.objectContaining({ customerEmail: expect.any(String), customerName: expect.any(String) }),
      'Customer purchase hub loaded',
    );
  });

  it('logs self-service action decisions and unsupported paths as operator-visible warnings', () => {
    const logger = createLogger();

    logCustomerSelfServiceActionStarted({ logger, correlationId: 'request-3', actionType: 'cancel', purchaseType: 'booking', purchaseId: 'booking-1' });
    logCustomerSelfServiceActionRejected({ logger, correlationId: 'request-3', actionType: 'refund', purchaseType: 'subscription', reasonCode: 'policy-blocked' });
    logUnsupportedWebappPathHandled({ logger, correlationId: 'request-3', pathCategory: 'admin', ownerClassification: 'webapp-teams' });

    expect(logger.info).toHaveBeenCalledWith(
      expect.objectContaining({ event: 'CustomerSelfServiceActionStarted', actionType: 'cancel', purchaseType: 'booking' }),
      'Customer self-service action started',
    );
    expect(logger.warn).toHaveBeenNthCalledWith(
      1,
      expect.objectContaining({ event: 'CustomerSelfServiceActionRejected', reasonCode: 'policy-blocked' }),
      'Customer self-service action rejected',
    );
    expect(logger.warn).toHaveBeenNthCalledWith(
      2,
      expect.objectContaining({ event: 'UnsupportedWebappPathHandled', pathCategory: 'admin', ownerClassification: 'webapp-teams' }),
      'Unsupported webapp path handled in place',
    );
  });

  it('logs booking modification lifecycle without customer content', () => {
    const logger = createLogger();

    logCustomerMarketplaceBookingModificationStarted({ logger, correlationId: 'request-5', bookingId: 'booking-1' });
    logCustomerMarketplaceBookingModificationCompleted({ logger, correlationId: 'request-5', bookingId: 'booking-1' });
    logCustomerMarketplaceBookingModificationFailed({ logger, correlationId: 'request-5', bookingId: 'booking-1', reasonCode: 'availability_conflict' });

    expect(logger.info).toHaveBeenNthCalledWith(
      1,
      expect.not.objectContaining({ customerEmail: expect.any(String), customerName: expect.any(String) }),
      'Customer marketplace booking modification started',
    );
    expect(logger.warn).toHaveBeenCalledWith(
      expect.objectContaining({ event: 'CustomerMarketplaceBookingModificationFailed', bookingId: 'booking-1', reasonCode: 'availability_conflict' }),
      'Customer marketplace booking modification failed',
    );
  });

  it('logs owner-specific marketplace resolution and failure paths', () => {
    const logger = createLogger();

    logOwnerSpecificMarketplaceEntryResolved({ logger, correlationId: 'request-4', isCustomDomain: true, entryPointType: 'co-working-subdomain' });
    logAggregateMarketplaceFailure({ logger, correlationId: 'request-4', workflow: 'discovery', reasonCode: 'query-failed' });

    expect(logger.info).toHaveBeenCalledWith(
      expect.objectContaining({ event: 'OwnerSpecificMarketplaceEntryResolved', isCustomDomain: true, entryPointType: 'co-working-subdomain' }),
      'Owner-specific marketplace entry resolved',
    );
    expect(logger.error).toHaveBeenCalledWith(
      expect.objectContaining({ event: 'AggregateMarketplaceFailure', workflow: 'discovery', reasonCode: 'query-failed' }),
      'Aggregate marketplace workflow failed',
    );
  });
});
