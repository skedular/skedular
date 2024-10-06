'use client';

import { OrganizationOnboarding } from '@/components/organization/organizationOnboarding';
import { RootShell } from '@/components/rootShell';
import { SmallMonthlyViewCalendar } from '@/components/smallMonthlyViewCalendar';
import type { pageHome_rootQuery } from '@/queries/__generated__/pageHome_rootQuery.graphql';
import { Loading } from '@repo/shared/components/loading';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { endOfMonth, startOfDay, startOfMonth } from '@repo/shared/libs/utils';
import { memo, useCallback, useEffect, useState } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, graphql, usePreloadedQuery, useQueryLoader } from 'react-relay';

type Props = {
  queryReference: PreloadedQuery<pageHome_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
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

const Home = ({ queryReference, onReloadRequired }: Props) => {
  const rootData = usePreloadedQuery<pageHome_rootQuery>(RootQuery, queryReference);
  const areAdditionalCustomerRecordsSync = useCallback(
    () => rootData?.bookingCustomerRecordSynced && rootData?.organizationCustomerRecordSynced,
    [rootData?.bookingCustomerRecordSynced, rootData?.organizationCustomerRecordSynced],
  );

  return (
    <RootShell
      rootDataRelay={rootData}
      onReloadRequired={onReloadRequired}
      areAdditionalCustomerRecordsSync={areAdditionalCustomerRecordsSync}
      additionalCustomerRecords={[rootData?.bookingCustomerRecordSynced, rootData?.organizationCustomerRecordSynced]}
    >
      <OrganizationOnboarding rootDataRelay={rootData} />
      <SmallMonthlyViewCalendar rootDataRelay={rootData} />
    </RootShell>
  );
};

const MemoHome = memo(Home);

const HomeWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<pageHome_rootQuery>(RootQuery);
  const [date, setDate] = useState(startOfMonth());

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
        dateToGetAvailableDesks: startOfDay().toISOString(),
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, date]);

  const handleReloadRequired = () => {
    setDate(startOfMonth());
  };

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }: { error: RootError }) => <RelayError error={error} />}>
      <MemoHome queryReference={queryReference} onReloadRequired={handleReloadRequired} />
    </ErrorBoundary>
  );
};

export default memo(HomeWithRelay);
