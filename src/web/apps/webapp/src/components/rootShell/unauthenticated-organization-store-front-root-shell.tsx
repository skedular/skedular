import { UnauthenticatedOrganizationStoreFrontAppBar } from '@/components/appBar';
import { Loading } from '@/components/loading';
import { UnathenticatedObservability } from '@/components/observability';
import StoreFrontBrowserMetadata from '@/components/organizationStoreFrontGuest/store-front-browser-metadata';
import { RelayError, toRootError } from '@skedular/shared';
import type { unauthenticatedOrganizationStoreFrontRootShell_rootQuery } from '@/queries/__generated__/unauthenticatedOrganizationStoreFrontRootShell_rootQuery.graphql';
import Box from '@mui/material/Box';
import CssBaseline from '@mui/material/CssBaseline';
import type { PropsWithChildren } from 'react';
import { memo, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { v7 as uuid } from 'uuid';
import useKnownParams from '@/hooks/use-known-params';

type Props = {
  queryReference: PreloadedQuery<unauthenticatedOrganizationStoreFrontRootShell_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
};

const RootQuery = graphql`
  query unauthenticatedOrganizationStoreFrontRootShell_rootQuery($organizationCustomDomain: String!) {
    organizationPublic(customDomain: $organizationCustomDomain) {
      name
      logoUrl
    }
    ...unauthenticatedOrganizationStoreFrontAppBar_query
  }
`;

const UnauthenticatedOrganizationStoreFrontRootShell = ({ children, queryReference }: PropsWithChildren<Props>) => {
  const rootData = usePreloadedQuery<unauthenticatedOrganizationStoreFrontRootShell_rootQuery>(RootQuery, queryReference);

  return (
    <>
      {rootData.organizationPublic && <StoreFrontBrowserMetadata organizationName={rootData.organizationPublic.name} organizationLogoUrl={rootData.organizationPublic.logoUrl} />}
      <UnathenticatedObservability />
      <Box sx={{ display: 'flex', width: '100%', maxWidth: '100vw', minHeight: '100vh', overflowX: 'clip', bgcolor: (theme) => theme.palette.background.default }}>
        <CssBaseline enableColorScheme />
        <Box component="main" sx={{ flexGrow: 1, width: '100%', minWidth: 0, maxWidth: '100%', overflowX: 'clip' }}>
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
  const { organizationCustomDomain } = useKnownParams();

  if (!organizationCustomDomain) {
    throw new Error('organizationCustomDomain is required');
  }

  useEffect(() => {
    loadQuery(
      {
        organizationCustomDomain,
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId, organizationCustomDomain]);

  const handleReloadRequired = () => {
    startTransition(() => {
      setTriggerReloadId(uuid());
    });
  };

  return !queryReference ? (
    <Loading />
  ) : (
    <ErrorBoundary fallbackRender={({ error }) => <RelayError error={toRootError(error)} />}>
      <MemoUnauthenticatedOrganizationStoreFrontRootShell queryReference={queryReference} onReloadRequired={handleReloadRequired}>
        {children}
      </MemoUnauthenticatedOrganizationStoreFrontRootShell>
    </ErrorBoundary>
  );
};

export default memo(UnauthenticatedOrganizationStoreFrontRootShellWithRelay);
