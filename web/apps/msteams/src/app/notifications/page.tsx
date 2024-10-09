import { Loading } from '@repo/shared/components/loading';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import graphql from 'babel-plugin-relay/macro';
import { Notifications } from 'components/notification/notifications';
import { RootShell } from 'components/rootShell';
import { memo, useCallback, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import type { pageNotifications_rootQuery } from './__generated__/pageNotifications_rootQuery.graphql';

type Props = {
  queryReference: PreloadedQuery<pageNotifications_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
};

const RootQuery = graphql`
  query pageNotifications_rootQuery {
    notificationCustomerRecordSynced
    ...rootShell_query
  }
`;

const NotificationsPage = ({ queryReference, onReloadRequired }: Props) => {
  const rootData = usePreloadedQuery<pageNotifications_rootQuery>(RootQuery, queryReference);
  const areAdditionalCustomerRecordsSync = useCallback(
    () => rootData?.notificationCustomerRecordSynced,
    [rootData?.notificationCustomerRecordSynced],
  );

  return (
    <RootShell
      rootDataRelay={rootData}
      onReloadRequired={onReloadRequired}
      areAdditionalCustomerRecordsSync={areAdditionalCustomerRecordsSync}
      additionalCustomerRecords={[rootData?.notificationCustomerRecordSynced]}
    >
      <Notifications />
    </RootShell>
  );
};

const MemoNotificationsPage = memo(NotificationsPage);

const NotificationsPageWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<pageNotifications_rootQuery>(RootQuery);
  const [triggerReload, setTriggerReload] = useState(0);
  const [, startTransition] = useTransition();

  useEffect(() => {
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
      <MemoNotificationsPage queryReference={queryReference} onReloadRequired={handleReloadRequired} />
    </ErrorBoundary>
  );
};

export default memo(NotificationsPageWithRelay);
