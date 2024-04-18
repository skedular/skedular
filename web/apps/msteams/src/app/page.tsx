'use client';

import { Loading } from '@repo/shared/components/loading';
import { OrganizationOnboarding } from '@/components/organization/organizationOnboarding';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { RootShell } from '@/components/rootShell';
import { SmallMonthlyViewCalendar } from '@/components/smallMonthlyViewCalendar';
import type { pageHome_rootQuery } from '@/queries/__generated__/pageHome_rootQuery.graphql';
import { endOfMonth, startOfDay, startOfMonth } from '@repo/shared/libs/utils';
import { memo, useCallback, useEffect, useState } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, graphql, usePreloadedQuery, useQueryLoader } from 'react-relay';

type Props = {
  queryReference: PreloadedQuery<pageHome_rootQuery, Record<string, unknown>>;
  onReloadRequire: () => void;
};

const RootQuery = graphql`
  query pageHome_rootQuery(
    $organizationId: String!
    $locationId: String!
    $monthlyCalendarDateFrom: DateTime!
    $monthlyCalendarDateTo: DateTime!
    $dateToGetAvailableDesks: DateTime!
    $deskIdsToIncludeToGetAvailableDesks: [String!]!
    $bookingPeopleNameSearchText: String!
    $bookingDetailsSelectorOrganizationMembersSortingValues: [OrganizationMemberOrderInput!]
    $smallMonthlyViewCalendarBookingsSortingValues: [BookingOrderInput!]
  ) {
    bookingCustomerRecordSynced
    organizationCustomerRecordSynced
    ...rootShell_query
    ...organizationOnboarding_query
    ...smallMonthlyViewCalendar_query
  }
`;

const Home = ({ queryReference, onReloadRequire }: Props) => {
  const rootData = usePreloadedQuery<pageHome_rootQuery>(RootQuery, queryReference);
  const areAdditionalCustomerRecordsSync = useCallback(
    () => rootData?.bookingCustomerRecordSynced && rootData?.organizationCustomerRecordSynced,
    [rootData?.bookingCustomerRecordSynced, rootData?.organizationCustomerRecordSynced],
  );

  return (
    <RootShell
      rootDataRelay={rootData}
      onReloadRequire={onReloadRequire}
      areAdditionalCustomerRecordsSync={areAdditionalCustomerRecordsSync}
      additionalCustomerRecords={[rootData?.bookingCustomerRecordSynced, rootData?.organizationCustomerRecordSynced]}
    >
      <OrganizationOnboarding rootDataRelay={rootData} />
      <SmallMonthlyViewCalendar rootDataRelay={rootData} />
    </RootShell>
  );
};

const MemoHome = memo(Home);

type PropsWithRelay = {};

const HomeWithRelay = ({}: PropsWithRelay) => {
  const [queryReference, loadQuery] = useQueryLoader<pageHome_rootQuery>(RootQuery);
  const [date, setDate] = useState(startOfMonth(null));

  useEffect(() => {
    loadQuery(
      {
        monthlyCalendarDateFrom: startOfMonth(date).toISOString(),
        monthlyCalendarDateTo: endOfMonth(date).toISOString(),
        deskIdsToIncludeToGetAvailableDesks: [],
        organizationId: '',
        locationId: '',
        bookingPeopleNameSearchText: '',
        bookingDetailsSelectorOrganizationMembersSortingValues: [
          {
            direction: 'Ascending',
            field: 'name',
          },
        ],
        smallMonthlyViewCalendarBookingsSortingValues: [
          {
            direction: 'Ascending',
            field: 'from',
          },
        ],
        dateToGetAvailableDesks: startOfDay(null).toISOString(),
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, date]);

  const handleReloadRequire = () => {
    setDate(startOfMonth(null));
  };

  if (queryReference == null) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }: { error: RootError }) => <RelayError error={error} />}>
      <MemoHome queryReference={queryReference} onReloadRequire={handleReloadRequire} />
    </ErrorBoundary>
  );
};

export default memo(HomeWithRelay);
