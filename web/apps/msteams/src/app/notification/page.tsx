'use client';

import { Loading } from '@repo/shared/components/loading';
import { Notifications } from '@/components/notification/notifications';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { RootShell } from '@/components/rootShell';
import type { pageNotifications_rootQuery } from '@/queries/__generated__/pageNotifications_rootQuery.graphql';
import { memo, useCallback, useEffect, useState } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, graphql, usePreloadedQuery, useQueryLoader } from 'react-relay';

type Props = {
  queryReference: PreloadedQuery<pageNotifications_rootQuery, Record<string, unknown>>;
  onReloadRequire: () => void;
};

const RootQuery = graphql`
  query pageNotifications_rootQuery($myNotificationsSortingValues: [NotificationOrderInput!]!) {
    notificationCustomerRecordSynced
    ...rootShell_query
    ...notifications_query
  }
`;

const NotificationsPage = ({ queryReference, onReloadRequire }: Props) => {
  const rootData = usePreloadedQuery<pageNotifications_rootQuery>(RootQuery, queryReference);
  const areAdditionalCustomerRecordsSync = useCallback(
    () => rootData?.notificationCustomerRecordSynced,
    [rootData?.notificationCustomerRecordSynced],
  );

  return (
    <RootShell
      rootDataRelay={rootData}
      onReloadRequire={onReloadRequire}
      areAdditionalCustomerRecordsSync={areAdditionalCustomerRecordsSync}
      additionalCustomerRecords={[rootData?.notificationCustomerRecordSynced]}
    >
      <Notifications rootDataRelay={rootData} />
    </RootShell>
  );
};

const MemoNotificationsPage = memo(NotificationsPage);

type PropsWithRelay = {};

const NotificationsPageWithRelay = ({}: PropsWithRelay) => {
  const [queryReference, loadQuery] = useQueryLoader<pageNotifications_rootQuery>(RootQuery);
  const [triggerReload, setTriggerReload] = useState(0);

  useEffect(() => {
    loadQuery(
      {
        myNotificationsSortingValues: [
          {
            direction: 'Descending',
            field: 'eventRaisedAt',
          },
        ],
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReload]);

  const handleReloadRequire = () => {
    setTriggerReload(triggerReload + 1);
  };

  if (queryReference == null) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }: { error: RootError }) => <RelayError error={error} />}>
      <MemoNotificationsPage queryReference={queryReference} onReloadRequire={handleReloadRequire} />
    </ErrorBoundary>
  );
};

export default memo(NotificationsPageWithRelay);
