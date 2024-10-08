import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Typography from '@mui/material/Typography';
import { Loading } from '@repo/shared/components/loading';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import graphql from 'babel-plugin-relay/macro';
import { memo, useEffect } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import type { pageInstall_rootQuery } from './__generated__/pageInstall_rootQuery.graphql';

const RootQuery = graphql`
  query pageInstall_rootQuery {
    azureTenantAdminConsentUrl
  }
`;

type Props = {
  queryReference: PreloadedQuery<pageInstall_rootQuery, Record<string, unknown>>;
};

const Install = ({ queryReference }: Props) => {
  const rootData = usePreloadedQuery<pageInstall_rootQuery>(RootQuery, queryReference);

  const handleInstallClicked = () => {
    window.open(rootData.azureTenantAdminConsentUrl);
  };

  return (
    <Box display="flex" flexDirection="column" justifyContent="center" alignItems="center" minHeight="100vh">
      <Typography variant="h2">
        Your administrator needs to install UnityHub for you. This is a one-time setup. Please click the button below to start the installation.
      </Typography>
      <Button variant="contained" onClick={handleInstallClicked}>
        Install
      </Button>
    </Box>
  );
};

const MemoInstall = memo(Install);

const InstallWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<pageInstall_rootQuery>(RootQuery);

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
      <MemoInstall queryReference={queryReference} />
    </ErrorBoundary>
  );
};

export default memo(InstallWithRelay);
