import type { observability_rootQuery } from '@/queries/__generated__/observability_rootQuery.graphql';
import { Loading } from '@repo/shared/components/loading';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { memo, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, graphql, usePreloadedQuery, useQueryLoader } from 'react-relay';
import LogRocket from './logrocket';

type Props = {
  queryReference: PreloadedQuery<observability_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
};

const RootQuery = graphql`
  query observability_rootQuery {
    ...logrocket_query
  }
`;

const Observability = ({ queryReference }: Props) => {
  const rootData = usePreloadedQuery<observability_rootQuery>(RootQuery, queryReference);

  return <>{process.env.NEXT_PUBLIC_LOGROCKET_APP_ID && <LogRocket rootDataRelay={rootData} />}</>;
};

const MemoObservability = memo(Observability);

const ObservabilityWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<observability_rootQuery>(RootQuery);
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
      <MemoObservability queryReference={queryReference} onReloadRequired={handleReloadRequired} />
    </ErrorBoundary>
  );
};

export default memo(ObservabilityWithRelay);
