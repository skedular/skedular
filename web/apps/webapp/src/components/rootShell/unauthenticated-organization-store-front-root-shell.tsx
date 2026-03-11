import { UnauthenticatedOrganizationStoreFrontAppBar } from '@/components/appBar';
import { Loading } from '@/components/loading';
import { UnathenticatedObservability } from '@/components/observability';
import { RelayError, toRootError } from '@/components/relayError';
import { useKnownParams } from '@/libs/providers';
import type { unauthenticatedOrganizationStoreFrontRootShell_rootQuery } from '@/queries/__generated__/unauthenticatedOrganizationStoreFrontRootShell_rootQuery.graphql';
import Box from '@mui/material/Box';
import CssBaseline from '@mui/material/CssBaseline';
import type { PropsWithChildren } from 'react';
import { memo, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { v7 as uuid } from 'uuid';

type Props = {
  queryReference: PreloadedQuery<unauthenticatedOrganizationStoreFrontRootShell_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
};

const RootQuery = graphql`
  query unauthenticatedOrganizationStoreFrontRootShell_rootQuery($organizationUniqueAlphanumericName: String!) {
    ...unauthenticatedOrganizationStoreFrontAppBar_query
  }
`;

const UnauthenticatedOrganizationStoreFrontRootShell = ({ children, queryReference }: PropsWithChildren<Props>) => {
  const rootData = usePreloadedQuery<unauthenticatedOrganizationStoreFrontRootShell_rootQuery>(RootQuery, queryReference);

  return (
    <>
      <UnathenticatedObservability />
      <Box sx={{ display: 'flex', minHeight: '100vh', bgcolor: (theme) => theme.palette.background.default }}>
        <CssBaseline enableColorScheme />
        <Box component="main" sx={{ flexGrow: 1, minWidth: 0 }}>
          <UnauthenticatedOrganizationStoreFrontAppBar rootDataRelay={rootData} />
          {children}
        </Box>
      </Box>
    </>
  );
};

const MemoUnauthenticatedOrganizationStoreFrontRootShell = memo(UnauthenticatedOrganizationStoreFrontRootShell);

const UnauthenticatedOrganizationStoreFrontRootShellWithRelay = ({ children }: PropsWithChildren) => {
  const [queryReference, loadQuery] = useQueryLoader<unauthenticatedOrganizationStoreFrontRootShell_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(uuid());
  const [, startTransition] = useTransition();
  const { organizationUniqueAlphanumericName } = useKnownParams();

  if (!organizationUniqueAlphanumericName) {
    throw new Error('organizationUniqueAlphanumericName is required');
  }

  useEffect(() => {
    loadQuery(
      {
        organizationUniqueAlphanumericName,
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId, organizationUniqueAlphanumericName]);

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
      <MemoUnauthenticatedOrganizationStoreFrontRootShell queryReference={queryReference} onReloadRequired={handleReloadRequired}>
        {children}
      </MemoUnauthenticatedOrganizationStoreFrontRootShell>
    </ErrorBoundary>
  );
};

export default memo(UnauthenticatedOrganizationStoreFrontRootShellWithRelay);
