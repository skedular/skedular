import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Typography from '@mui/material/Typography';
import { Loading } from '@repo/shared/components/loading';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import graphql from 'babel-plugin-relay/macro';
import { MainRootLayout } from 'components/layouts';
import { FabNavigationMenu, LeftSideNavigationMenu } from 'components/navigationMenu';
import { Observability } from 'components/observability';
import { memo, useCallback, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import type { rootShell_rootQuery } from './__generated__/rootShell_rootQuery.graphql';

type Props = {
  queryReference: PreloadedQuery<rootShell_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  children: React.ReactNode;
  title?: string | null;
  rightSideContent?: React.JSX.Element;
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
    ...observability_query
  }
`;

const maxRetryAttemptsToReload = 20;

const RootShell = ({ queryReference, children, onReloadRequired, rightSideContent }: Props) => {
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
        <Typography variant="h2">
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
        <Typography variant="h2">There was an issue activating your account.</Typography>
      </Box>
    );
  }

  if (!rootData.me || !areCustomerRecordsSync()) {
    return <Loading message="Kindly hold on as we proceed to activate your account..." />;
  }

  return (
    <>
      <Observability rootDataRelay={rootData} />
      <MainRootLayout leftSideContent={<LeftSideNavigationMenu />} rightSideContent={rightSideContent}>
        <Box
          sx={{
            display: 'flex',
            flexDirection: 'column',
            justifyContent: 'space-between',
            alignItems: 'left',
            p: '3rem',
          }}
        >
          {children}
        </Box>
      </MainRootLayout>
      <FabNavigationMenu />
    </>
  );
};

const MemoRootShell = memo(RootShell);

type RelayProps = {
  children: React.ReactNode;
  title?: string | null;
  rightSideContent?: React.JSX.Element;
};

const RootShellWithRelay = ({ title, children, rightSideContent }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<rootShell_rootQuery>(RootQuery);
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
      <MemoRootShell queryReference={queryReference} onReloadRequired={handleReloadRequired} title={title} rightSideContent={rightSideContent}>
        {children}
      </MemoRootShell>
    </ErrorBoundary>
  );
};

export default memo(RootShellWithRelay);
