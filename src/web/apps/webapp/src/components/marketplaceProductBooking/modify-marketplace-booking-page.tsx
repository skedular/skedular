'use client';

import { Loading } from '@/components/loading';
import { RelayError, toRootError } from '@skedular/shared';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { useEffect } from 'react';
import { useRouter } from 'next/navigation';
import useKnownParams from '@/hooks/use-known-params';
import type { modifyMarketplaceBookingPage_query } from '@/queries/__generated__/modifyMarketplaceBookingPage_query.graphql';
import ModifyMarketplaceBookingDialog from './modify-marketplace-booking-dialog';

const Query = graphql`
  query modifyMarketplaceBookingPage_query($bookingId: String!) {
    ...modifyMarketplaceBookingDialog_query @arguments(bookingId: $bookingId)
    booking(id: $bookingId) {
      id
      entityFrameworkVersion
      from
      until
      involvedLocations {
        uniqueId
      }
      bookingResources {
        resource {
          id
          name
        }
      }
    }
  }
`;

const ModifyMarketplaceBookingPageContent = ({ queryReference }: { queryReference: PreloadedQuery<modifyMarketplaceBookingPage_query> }) => {
  const data = usePreloadedQuery<modifyMarketplaceBookingPage_query>(Query, queryReference);
  const router = useRouter();
  const booking = data.booking;
  if (!booking) {
    return null;
  }

  return (
    <ModifyMarketplaceBookingDialog
      page
      bookingId={booking.id}
      expectedVersion={booking.entityFrameworkVersion}
      initialFrom={booking.from}
      initialUntil={booking.until}
      currentResourceIds={booking.bookingResources.map(({ resource }) => resource.id)}
      currentResources={booking.bookingResources.map(({ resource }) => resource)}
      currentLocationId={booking.involvedLocations[0]?.uniqueId}
      rootDataRelay={data}
      onClose={() => router.back()}
      onModified={() => router.back()}
    />
  );
};

const ModifyMarketplaceBookingPage = () => {
  const { bookingId } = useKnownParams();
  const [queryReference, loadQuery] = useQueryLoader<modifyMarketplaceBookingPage_query>(Query);

  useEffect(() => {
    if (bookingId) {
      loadQuery({ bookingId }, { fetchPolicy: 'store-and-network' });
    }
  }, [bookingId, loadQuery]);

  if (!bookingId || !queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }) => <RelayError error={toRootError(error)} />}>
      <ModifyMarketplaceBookingPageContent queryReference={queryReference} />
    </ErrorBoundary>
  );
};

export default ModifyMarketplaceBookingPage;
