import { Loading } from '@/components/loading';
import { RelayError, toRootError } from '@skedular/shared';
import type { pageStartInstallMsTeams_rootQuery } from '@/queries/__generated__/pageStartInstallMsTeams_rootQuery.graphql';
import { memo, useEffect, useRef, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { v7 as uuid } from 'uuid';

const RootQuery = graphql`
  query pageStartInstallMsTeams_rootQuery {
    azureTenantAdminConsentUrl
  }
`;

type Props = {
  queryReference: PreloadedQuery<pageStartInstallMsTeams_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
};

const RootPage = ({ queryReference }: Props) => {
  const rootData = usePreloadedQuery<pageStartInstallMsTeams_rootQuery>(RootQuery, queryReference);
  const hasOpened = useRef(false);

  useEffect(() => {
    if (!hasOpened.current) {
      window.open(rootData.azureTenantAdminConsentUrl);
      hasOpened.current = true;
    }
  }, [rootData.azureTenantAdminConsentUrl]);

  return null;
};

const MemoRootPage = memo(RootPage);

const RootPageWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<pageStartInstallMsTeams_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(uuid());
  const [, startTransition] = useTransition();

  useEffect(() => {
    loadQuery(
      {},
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId]);

  const handleReloadRequired = () => {
    startTransition(() => {
      setTriggerReloadId(uuid());
    });
  };

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }) => <RelayError error={toRootError(error)} />}>
      <MemoRootPage queryReference={queryReference} onReloadRequired={handleReloadRequired} />
    </ErrorBoundary>
  );
};

export default memo(RootPageWithRelay);
