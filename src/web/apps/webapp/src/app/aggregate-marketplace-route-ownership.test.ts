import { describe, expect, it } from 'vitest';
import { aggregateMarketplaceRouteOwnership, noRedirectUrlHandlings } from './aggregate-marketplace-route-ownership';

describe('aggregate marketplace route ownership', () => {
  it('uses only inventory-approved owner classifications', () => {
    const allowedOwners = ['webapp', 'webapp-teams', 'webapp-spaces', 'shared-entry-point', 'undecided'];

    expect(aggregateMarketplaceRouteOwnership.every((routeOwnership) => allowedOwners.includes(routeOwnership.ownerApp))).toBe(true);
  });

  it('keeps marketplace and customer routes in webapp', () => {
    expect(aggregateMarketplaceRouteOwnership).toEqual(
      expect.arrayContaining([
        expect.objectContaining({ routePattern: '/', ownerApp: 'webapp', disposition: 'keep' }),
        expect.objectContaining({ routePattern: '/marketplace/locations/[locationId]', ownerApp: 'webapp', disposition: 'keep' }),
        expect.objectContaining({ routePattern: '/marketplace/bookings', ownerApp: 'webapp', disposition: 'keep' }),
      ]),
    );
  });

  it('protects owner-specific marketplace behavior', () => {
    expect(aggregateMarketplaceRouteOwnership).toContainEqual(
      expect.objectContaining({ routePattern: '/marketplace/organizations/[organizationCustomDomain]/**', disposition: 'protect-unchanged', urlHandling: 'preserve-existing' }),
    );
  });

  it('classifies private administration routes out of customer navigation', () => {
    expect(aggregateMarketplaceRouteOwnership).toEqual(
      expect.arrayContaining([
        expect.objectContaining({ routePattern: '/organizations/[organizationCustomDomain]/**', ownerApp: 'webapp-teams', disposition: 'move' }),
        expect.objectContaining({ routePattern: '/spaces/**', ownerApp: 'webapp-spaces', disposition: 'move' }),
      ]),
    );
  });

  it('keeps teams and spaces ownership classifications out of webapp customer routing', () => {
    expect(aggregateMarketplaceRouteOwnership.find((routeOwnership) => routeOwnership.routePattern === '/organizations/[organizationCustomDomain]/**')?.ownerApp).toBe(
      'webapp-teams',
    );
    expect(aggregateMarketplaceRouteOwnership.find((routeOwnership) => routeOwnership.routePattern === '/spaces/**')?.ownerApp).toBe('webapp-spaces');
  });

  it('does not keep MS Teams routes in the customer webapp inventory', () => {
    expect(aggregateMarketplaceRouteOwnership.some((routeOwnership) => routeOwnership.routePattern.includes('msteams'))).toBe(false);
  });

  it('uses only in-place or preserve-existing URL handling decisions', () => {
    expect(aggregateMarketplaceRouteOwnership.every((routeOwnership) => noRedirectUrlHandlings.includes(routeOwnership.urlHandling))).toBe(true);
  });

  it('keeps unsupported and removed paths on non-redirect URL handling', () => {
    const relocatedRoutes = aggregateMarketplaceRouteOwnership.filter((routeOwnership) => ['move', 'remove-from-navigation'].includes(routeOwnership.disposition));

    expect(relocatedRoutes.length).toBeGreaterThan(0);
    expect(relocatedRoutes.every((routeOwnership) => routeOwnership.urlHandling === 'unavailable-in-place')).toBe(true);
  });
});
