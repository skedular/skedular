type CustomerBookableMarketplaceLocation = object & {
  marketplaceEnabled?: boolean | null;
  customerBookable?: boolean | null;
};

export const isCustomerBookableMarketplaceLocation = (location: CustomerBookableMarketplaceLocation) =>
  location.marketplaceEnabled !== false && location.customerBookable !== false;

export const aggregateMarketplaceEmptyStateCopy = {
  title: 'No locations found in this area',
  body: 'Move or zoom the map to search another area for customer-bookable marketplace locations.',
};

export const toAggregateDiscoveryLayoutMode = (isMobileOrTablet: boolean) => (isMobileOrTablet ? 'map-first' : 'split-map-list');
