import { Loading } from '@/components/loading';
import EntitlementBookingForm from '@/components/marketplaceEntitlement/entitlement-booking-page';
import { MarketplaceProductBookingSignIn } from '@/components/marketplaceProductBooking';
import { OrganizationStoreFrontRootShell, UnauthenticatedOrganizationStoreFrontRootShell } from '@/components/rootShell';
import type { pageMarketplaceEntitlementBooking_rootQuery } from '@/queries/__generated__/pageMarketplaceEntitlementBooking_rootQuery.graphql';
import { useAuth } from '@workos-inc/authkit-nextjs/components';
import { useParams } from 'next/navigation';
import { memo } from 'react';
import { graphql, useLazyLoadQuery } from 'react-relay';

const RootQuery = graphql`
  query pageMarketplaceEntitlementBooking_rootQuery($entitlementId: String!) {
    entitlement(id: $entitlementId) {
      id
      productId
      organizationCustomDomain
    }
  }
`;

const EntitlementBookingRoute = () => {
  const { entitlementId } = useParams<{ entitlementId: string }>();
  const { user, loading } = useAuth();
  const data = useLazyLoadQuery<pageMarketplaceEntitlementBooking_rootQuery>(RootQuery, { entitlementId }, { fetchPolicy: 'network-only' });

  if (loading) {
    return <Loading />;
  }

  if (!user) {
    return (
      <UnauthenticatedOrganizationStoreFrontRootShell>
        <MarketplaceProductBookingSignIn />
      </UnauthenticatedOrganizationStoreFrontRootShell>
    );
  }

  if (!data.entitlement) return <Loading />;

  return (
    <OrganizationStoreFrontRootShell>
      <EntitlementBookingForm entitlementId={data.entitlement.id} />
    </OrganizationStoreFrontRootShell>
  );
};

export default memo(EntitlementBookingRoute);
