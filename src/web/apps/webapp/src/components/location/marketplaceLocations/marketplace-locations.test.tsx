import { describe, expect, it } from 'vitest';
import { aggregateMarketplaceEmptyStateCopy, isCustomerBookableMarketplaceLocation, toAggregateDiscoveryLayoutMode } from './marketplace-location-eligibility';

describe('aggregate marketplace location filtering', () => {
  it('keeps locations that do not expose explicit eligibility flags yet', () => {
    expect(isCustomerBookableMarketplaceLocation({})).toBe(true);
  });

  it('excludes private, non-marketplace, and non-customer-bookable locations when eligibility flags are available', () => {
    expect(isCustomerBookableMarketplaceLocation({ marketplaceEnabled: false, customerBookable: true })).toBe(false);
    expect(isCustomerBookableMarketplaceLocation({ marketplaceEnabled: true, customerBookable: false })).toBe(false);
    expect(isCustomerBookableMarketplaceLocation({ marketplaceEnabled: true, customerBookable: true })).toBe(true);
  });

  it('defines a customer-safe empty state', () => {
    expect(aggregateMarketplaceEmptyStateCopy.title).toContain('No locations found');
    expect(aggregateMarketplaceEmptyStateCopy.body).toContain('Move or zoom the map');
  });

  it('uses a map-first layout on mobile and a split map/list layout on desktop', () => {
    expect(toAggregateDiscoveryLayoutMode(true)).toBe('map-first');
    expect(toAggregateDiscoveryLayoutMode(false)).toBe('split-map-list');
  });
});
