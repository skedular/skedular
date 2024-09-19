import { Loading } from '@repo/shared/components/loading';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import graphql from 'babel-plugin-relay/macro';
import { Notifications } from 'components/notification/notifications';
import { RootShell } from 'components/rootShell';
import { memo, useCallback, useEffect, useState } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import type { notifications_rootQuery } from './__generated__/notifications_rootQuery.graphql';

type Props = {
  queryReference: PreloadedQuery<notifications_rootQuery, Record<string, unknown>>;
  onReloadRequire: () => void;
};

const RootQuery = graphql`
  query notifications_rootQuery($myNotificationsSortingValues: [NotificationOrderInput!]!) {
    notificationCustomerRecordSynced
    ...rootShell_query
    ...notifications_query
  }
`;

const NotificationsPage = ({ queryReference, onReloadRequire }: Props) => {
  const rootData = usePreloadedQuery<notifications_rootQuery>(RootQuery, queryReference);
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

const NotificationsPageWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<notifications_rootQuery>(RootQuery);
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

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }: { error: RootError }) => <RelayError error={error} />}>
      <MemoNotificationsPage queryReference={queryReference} onReloadRequire={handleReloadRequire} />
    </ErrorBoundary>
  );
};

export default memo(NotificationsPageWithRelay);
