import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import CssBaseline from '@mui/material/CssBaseline';
import Drawer from '@mui/material/Drawer';
import Grid from '@mui/material/Grid2';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import { Loading } from '@repo/shared/components/loading';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import graphql from 'babel-plugin-relay/macro';
import type { AppBarBreadcrumb } from 'components/appBar';
import { AppBar } from 'components/appBar';
import { FabNavigationMenu, LeftSideNavigationMenu } from 'components/navigationMenu';
import { Observability } from 'components/observability';
import { nanoid } from 'nanoid';
import { memo, useCallback, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import type { rootShell_rootQuery } from './__generated__/rootShell_rootQuery.graphql';

type Props = {
  queryReference: PreloadedQuery<rootShell_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  children: React.ReactNode;
  title?: string | null;
  appBarBreadcrumb?: AppBarBreadcrumb;
};

const RootQuery = graphql`
  query rootShell_rootQuery {
    me {
      id
    }
    billingCustomerRecordSynced
    bookingCustomerRecordSynced
    locationCustomerRecordSynced
    msTeamsCustomerRecordSynced
    notificationCustomerRecordSynced
    organizationCustomerRecordSynced
    paymentCustomerRecordSynced
    slackCustomerRecordSynced
    teamCustomerRecordSynced
    isAzureTenantInstalled
    azureTenantAdminConsentUrl
    ...appBar_query
  }
`;

const maxRetryAttemptsToReload = 20;
const drawerWidth = 250;

const RootShell = ({ queryReference, children, onReloadRequired, appBarBreadcrumb }: Props) => {
  const rootData = usePreloadedQuery<rootShell_rootQuery>(RootQuery, queryReference);
  const [reloadCount, setReloadCount] = useState(0);
  const areCustomerRecordsSync = useCallback(
    () =>
      rootData?.billingCustomerRecordSynced &&
      rootData?.bookingCustomerRecordSynced &&
      rootData?.locationCustomerRecordSynced &&
      rootData?.msTeamsCustomerRecordSynced &&
      rootData?.notificationCustomerRecordSynced &&
      rootData?.organizationCustomerRecordSynced &&
      rootData?.paymentCustomerRecordSynced &&
      rootData?.slackCustomerRecordSynced &&
      rootData?.teamCustomerRecordSynced,
    [
      rootData?.billingCustomerRecordSynced,
      rootData?.bookingCustomerRecordSynced,
      rootData?.locationCustomerRecordSynced,
      rootData?.msTeamsCustomerRecordSynced,
      rootData?.notificationCustomerRecordSynced,
      rootData?.organizationCustomerRecordSynced,
      rootData?.paymentCustomerRecordSynced,
      rootData?.slackCustomerRecordSynced,
      rootData?.teamCustomerRecordSynced,
    ],
  );

  useEffect(() => {
    if (reloadCount === maxRetryAttemptsToReload || (rootData.me && areCustomerRecordsSync())) {
      return;
    }

    const intervalId = setInterval(() => {
      onReloadRequired();
      setReloadCount(reloadCount + 1);
    }, 3000);

    return () => {
      clearInterval(intervalId);
    };
  }, [rootData.me, reloadCount, onReloadRequired, areCustomerRecordsSync]);

  const handleInstallClicked = () => {
    window.open(rootData.azureTenantAdminConsentUrl);
  };

  if (!rootData.isAzureTenantInstalled) {
    return (
      <Box display="flex" flexDirection="column" justifyContent="center" alignItems="center" minHeight="100vh">
        <Typography variant="h4">
          Your administrator needs to install UnityHub for you. This is a one-time setup. Please click the button below to start the installation.
        </Typography>
        <Button variant="contained" onClick={handleInstallClicked}>
          Install
        </Button>
      </Box>
    );
  }

  if (reloadCount === maxRetryAttemptsToReload) {
    return (
      <Box display="flex" flexDirection="column" justifyContent="center" alignItems="center" minHeight="100vh">
        <Typography variant="h4">There was an issue activating your account.</Typography>
      </Box>
    );
  }

  if (!rootData.me || !areCustomerRecordsSync()) {
    return <Loading message="Kindly hold on as we proceed to activate your account..." />;
  }

  return (
    <>
      <Observability />
      <Box sx={{ display: 'flex' }}>
        <CssBaseline enableColorScheme />
        <Drawer
          sx={{
            display: { xs: 'none', sm: 'block' },
            width: drawerWidth,
            flexShrink: 0,
            '& .MuiDrawer-paper': {
              width: drawerWidth,
              boxSizing: 'border-box',
            },
          }}
          variant="persistent"
          open={true}
        >
          <LeftSideNavigationMenu />
        </Drawer>
        <Grid container>
          <Grid sx={{ xs: 12, sm: 6, md: 3, lg: 2, xl: 2, flexGrow: 1, display: { xs: 'block', sm: 'none' } }}>
            <LeftSideNavigationMenu />
          </Grid>
          <Stack direction="column" sx={{ width: '100vw' }}>
            <AppBar rootDataRelay={rootData} onReloadRequired={onReloadRequired} breadcrumb={appBarBreadcrumb} />
          </Stack>
          <Grid sx={{ xs: 12, sm: 6, md: 3, lg: 2, xl: 2, flexGrow: 1, paddingLeft: 1 }}>{children}</Grid>
        </Grid>
      </Box>
      <FabNavigationMenu />
    </>
  );
};

const MemoRootShell = memo(RootShell);

type RelayProps = {
  children: React.ReactNode;
  title?: string | null;
  appBarBreadcrumb?: AppBarBreadcrumb;
};

const RootShellWithRelay = ({ title, children, appBarBreadcrumb }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<rootShell_rootQuery>(RootQuery);
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
      <MemoRootShell queryReference={queryReference} onReloadRequired={handleReloadRequired} title={title} appBarBreadcrumb={appBarBreadcrumb}>
        {children}
      </MemoRootShell>
    </ErrorBoundary>
  );
};

export default memo(RootShellWithRelay);
