import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Link from '@mui/material/Link';
import { RefreshIcon } from '@repo/shared/components/icons';
import { Loading } from '@repo/shared/components/loading';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import graphql from 'babel-plugin-relay/macro';
import { memo, useEffect, useRef } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import type { pageStartInstall_rootQuery } from './__generated__/pageStartInstall_rootQuery.graphql';

const RootQuery = graphql`
  query pageStartInstall_rootQuery {
    azureTenantAdminConsentUrl
  }
`;

type Props = {
  queryReference: PreloadedQuery<pageStartInstall_rootQuery, Record<string, unknown>>;
};

const StartInstall = ({ queryReference }: Props) => {
  const rootData = usePreloadedQuery<pageStartInstall_rootQuery>(RootQuery, queryReference);
  const hasOpened = useRef(false);

  useEffect(() => {
    if (!hasOpened.current) {
      window.open(rootData.azureTenantAdminConsentUrl);
      hasOpened.current = true;
    }
  }, [rootData.azureTenantAdminConsentUrl]);

  return (
    <Box display="flex" flexDirection="column" justifyContent="center" alignItems="center" minHeight="100vh">
      <Button LinkComponent={Link} variant="contained" href="/" startIcon={<RefreshIcon />}>
        Refresh
      </Button>
    </Box>
  );
};

const MemoStartInstall = memo(StartInstall);

const StartInstallWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<pageStartInstall_rootQuery>(RootQuery);

  useEffect(() => {
    loadQuery(
      {},
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery]);

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }: { error: RootError }) => <RelayError error={error} />}>
      <MemoStartInstall queryReference={queryReference} />
    </ErrorBoundary>
  );
};

export default memo(StartInstallWithRelay);
