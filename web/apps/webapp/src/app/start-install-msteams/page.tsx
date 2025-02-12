'use client';

import { Loading } from '@/components/loading';
import type { RootError } from '@/components/relayError';
import { RelayError } from '@/components/relayError';
import type { pageStartInstallMsTeams_rootQuery } from '@/queries/__generated__/pageStartInstallMsTeams_rootQuery.graphql';
import { nanoid } from 'nanoid';
import { memo, useEffect, useRef, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';

const RootQuery = graphql`
  query pageStartInstallMsTeams_rootQuery {
    azureTenantAdminConsentUrl
  }
`;

type Props = {
  queryReference: PreloadedQuery<pageStartInstallMsTeams_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
};

const StartInstallMsTeamsPage = ({ queryReference, onReloadRequired }: Props) => {
  const rootData = usePreloadedQuery<pageStartInstallMsTeams_rootQuery>(RootQuery, queryReference);
  const hasOpened = useRef(false);

  useEffect(() => {
    if (!hasOpened.current) {
      window.open(rootData.azureTenantAdminConsentUrl);
      hasOpened.current = true;
    }
  }, [rootData.azureTenantAdminConsentUrl]);

  return <></>;
};

const MemoStartInstallMsTeamsPage = memo(StartInstallMsTeamsPage);

const StartInstallMsTeamsPageWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<pageStartInstallMsTeams_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(nanoid());
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
      setTriggerReloadId(nanoid());
    });
  };

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }: { error: RootError }) => <RelayError error={error} />}>
      <MemoStartInstallMsTeamsPage queryReference={queryReference} onReloadRequired={handleReloadRequired} />
    </ErrorBoundary>
  );
};

export default memo(StartInstallMsTeamsPageWithRelay);
