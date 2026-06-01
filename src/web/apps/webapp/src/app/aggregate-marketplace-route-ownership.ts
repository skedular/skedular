export type AggregateMarketplaceOwnerApp = 'webapp' | 'webapp-teams' | 'webapp-spaces' | 'shared-entry-point' | 'undecided';
export type AggregateMarketplaceDisposition = 'keep' | 'move' | 'remove-from-navigation' | 'preserve-shared' | 'protect-unchanged' | 'defer';
export type AggregateMarketplaceUrlHandling = 'serve-in-place' | 'unavailable-in-place' | 'preserve-existing' | 'not-applicable';

export type AggregateMarketplaceRouteOwnership = {
  routePattern: string;
  ownerApp: AggregateMarketplaceOwnerApp;
  disposition: AggregateMarketplaceDisposition;
  urlHandling: AggregateMarketplaceUrlHandling;
};

export const aggregateMarketplaceRouteOwnership = [
  { routePattern: '/', ownerApp: 'webapp', disposition: 'keep', urlHandling: 'serve-in-place' },
  { routePattern: '/marketplace/locations/[locationId]', ownerApp: 'webapp', disposition: 'keep', urlHandling: 'serve-in-place' },
  { routePattern: '/marketplace/products/[productId]', ownerApp: 'webapp', disposition: 'keep', urlHandling: 'serve-in-place' },
  { routePattern: '/marketplace/bookings', ownerApp: 'webapp', disposition: 'keep', urlHandling: 'serve-in-place' },
  { routePattern: '/marketplace/subscriptions', ownerApp: 'webapp', disposition: 'keep', urlHandling: 'serve-in-place' },
  { routePattern: '/marketplace/organizations/[organizationCustomDomain]/**', ownerApp: 'webapp', disposition: 'protect-unchanged', urlHandling: 'preserve-existing' },
  { routePattern: '/signin', ownerApp: 'shared-entry-point', disposition: 'preserve-shared', urlHandling: 'preserve-existing' },
  { routePattern: '/signup', ownerApp: 'shared-entry-point', disposition: 'preserve-shared', urlHandling: 'preserve-existing' },
  { routePattern: '/settings', ownerApp: 'shared-entry-point', disposition: 'preserve-shared', urlHandling: 'preserve-existing' },
  { routePattern: '/notifications', ownerApp: 'shared-entry-point', disposition: 'preserve-shared', urlHandling: 'preserve-existing' },
  { routePattern: '/organizations/[organizationCustomDomain]/**', ownerApp: 'webapp-teams', disposition: 'move', urlHandling: 'unavailable-in-place' },
  { routePattern: '/spaces/**', ownerApp: 'webapp-spaces', disposition: 'move', urlHandling: 'unavailable-in-place' },
] as const satisfies readonly AggregateMarketplaceRouteOwnership[];

export const noRedirectUrlHandlings: AggregateMarketplaceUrlHandling[] = ['serve-in-place', 'unavailable-in-place', 'preserve-existing', 'not-applicable'];

export const findAggregateMarketplaceRouteOwnership = (routePattern: string) =>
  aggregateMarketplaceRouteOwnership.find((routeOwnership) => routeOwnership.routePattern === routePattern);
