import Box from '@mui/material/Box';
import CssBaseline from '@mui/material/CssBaseline';
import Drawer from '@mui/material/Drawer';
import Grid from '@mui/material/Grid2';
import { SmallHeadingIconTypography, StackColumn } from '@repo/shared/components/commons';
import { Loading } from '@repo/shared/components/loading';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { SwitchToModernUIContext } from '@repo/shared/libs/providers';
import graphql from 'babel-plugin-relay/macro';
import { AppBar, OldAppBar } from 'components/appBar';
import { LeftSideNavigationMenu } from 'components/navigationMenu';
import { Observability } from 'components/observability';
import { nanoid } from 'nanoid';
import { memo, PropsWithChildren, useCallback, useContext, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { useParams } from 'react-router-dom';
import type { rootShell_rootQuery } from './__generated__/rootShell_rootQuery.graphql';

type Props = {
  queryReference: PreloadedQuery<rootShell_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  title?: string | null;
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
    ...oldAppBar_query
    ...appBar_query
  }
`;

const maxRetryAttemptsToReload = 20;
const drawerWithTextWidth = 250;
const drawerWithoutTextWidth = 80;

const RootShell = ({ queryReference, children, onReloadRequired }: PropsWithChildren<Props>) => {
  const rootData = usePreloadedQuery<rootShell_rootQuery>(RootQuery, queryReference);
  const switchToModernUI = useContext(SwitchToModernUIContext);

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

  if (reloadCount === maxRetryAttemptsToReload) {
    return (
      <Box display="flex" flexDirection="column" justifyContent="center" alignItems="center" minHeight="100vh">
        <SmallHeadingIconTypography label="There was an issue activating your account." />
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
            width: drawerWithTextWidth,
            flexShrink: 0,
            '& .MuiDrawer-paper': {
              width: drawerWithTextWidth,
              boxSizing: 'border-box',
            },
          }}
          variant="persistent"
          open={true}
        >
          <LeftSideNavigationMenu onReloadRequired={onReloadRequired} maxWidth={drawerWithTextWidth} />
        </Drawer>
        <Grid container>
          <Grid sx={{ xs: 12, sm: 6, md: 3, lg: 2, xl: 2, flexGrow: 1, display: { xs: 'block', sm: 'none' } }}>
            <LeftSideNavigationMenu onReloadRequired={onReloadRequired} maxWidth={drawerWithTextWidth} />
          </Grid>
          <StackColumn sx={{ width: '100vw' }}>
            {!switchToModernUI && <OldAppBar rootDataRelay={rootData} onReloadRequired={onReloadRequired} />}
            {switchToModernUI && <AppBar rootDataRelay={rootData} onReloadRequired={onReloadRequired} />}
          </StackColumn>
          <Grid sx={{ xs: 12, sm: 6, md: 3, lg: 2, xl: 2, flexGrow: 1, paddingLeft: 1 }}>{children}</Grid>
        </Grid>
      </Box>
    </>
  );
};

const MemoRootShell = memo(RootShell);

type RelayProps = {
  title?: string | null;
};

const RootShellWithRelay = ({ children, title }: PropsWithChildren<RelayProps>) => {
  const [queryReference, loadQuery] = useQueryLoader<rootShell_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(nanoid());
  const [, startTransition] = useTransition();
  const { organizationId } = useParams();
  let finalOrganizationId = '';

  if (typeof organizationId === 'string') {
    finalOrganizationId = organizationId;
  } else if (Array.isArray(organizationId)) {
    if (typeof organizationId[0] === 'undefined') {
      throw new Error('organizationId is required');
    }

    finalOrganizationId = organizationId[0];
  } else {
    throw new Error('organizationId is required');
  }

  useEffect(() => {
    loadQuery(
      {},
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId, finalOrganizationId]);

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
      <MemoRootShell queryReference={queryReference} onReloadRequired={handleReloadRequired} title={title}>
        {children}
      </MemoRootShell>
    </ErrorBoundary>
  );
};

export default memo(RootShellWithRelay);
