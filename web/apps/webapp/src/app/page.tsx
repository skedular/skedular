'use client';

import { OrganizationOnboarding } from '@/components/organization/organizationOnboarding';
import { RootShell } from '@/components/rootShell';
import { SmallMonthlyViewCalendar } from '@/components/smallMonthlyViewCalendar';
import type { pageHome_rootQuery } from '@/queries/__generated__/pageHome_rootQuery.graphql';
import { Loading } from '@repo/shared/components/loading';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { startOfMonth } from '@repo/shared/libs/utils';
import { memo, useCallback, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, graphql, usePreloadedQuery, useQueryLoader } from 'react-relay';

type Props = {
  queryReference: PreloadedQuery<pageHome_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
};

const RootQuery = graphql`
  query pageHome_rootQuery {
    bookingCustomerRecordSynced
    organizationCustomerRecordSynced
    ...rootShell_query
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
      <OrganizationOnboarding />
      <SmallMonthlyViewCalendar />
    </RootShell>
  );
};

const MemoHome = memo(Home);

const HomeWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<pageHome_rootQuery>(RootQuery);
  const [triggerReload, setTriggerReload] = useState(0);
  const [, startTransition] = useTransition();

  useEffect(() => {
    const date = startOfMonth();

    loadQuery(
      {},
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReload]);

  const handleReloadRequired = () => {
    startTransition(() => {
      setTriggerReload(triggerReload + 1);
    });
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
