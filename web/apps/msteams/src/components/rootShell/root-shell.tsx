import Box from '@mui/material/Box';
import CssBaseline from '@mui/material/CssBaseline';
import Grid from '@mui/material/Grid2';
import { SmallHeadingIconTypography } from '@repo/shared/components/commons';
import { Loading } from '@repo/shared/components/loading';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { SwitchToModernUIContext } from '@repo/shared/libs/providers';
import graphql from 'babel-plugin-relay/macro';
import { AppBar } from 'components/appBar';
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
  collapsed?: boolean;
  hideWelcomeMessage?: boolean;
  showBreadcrumps?: boolean;
  breadcrumbs?: React.ReactNode | JSX.Element;
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

const RootShell = ({
  queryReference,
  children,
  onReloadRequired,
  collapsed,
  hideWelcomeMessage,
  showBreadcrumps,
  breadcrumbs,
}: PropsWithChildren<Props>) => {
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
        <LeftSideNavigationMenu collapsed={collapsed} />
        <Grid container sx={{ flexGrow: 1 }}>
          <AppBar rootDataRelay={rootData} hideWelcomeMessage={hideWelcomeMessage} showBreadcrumps={showBreadcrumps} breadcrumbs={breadcrumbs} />
          <Grid
            sx={{
              paddingLeft: switchToModernUI ? undefined : 2,
              paddingTop: switchToModernUI ? undefined : 2,
            }}
          >
            {children}
          </Grid>
        </Grid>
      </Box>
    </>
  );
};

const MemoRootShell = memo(RootShell);

type RelayProps = {
  collapsed?: boolean;
  hideWelcomeMessage?: boolean;
  showBreadcrumps?: boolean;
  breadcrumbs?: React.ReactNode | JSX.Element;
};

const RootShellWithRelay = ({ children, collapsed, hideWelcomeMessage, showBreadcrumps, breadcrumbs }: PropsWithChildren<RelayProps>) => {
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
      <MemoRootShell
        queryReference={queryReference}
        onReloadRequired={handleReloadRequired}
        collapsed={collapsed}
        hideWelcomeMessage={hideWelcomeMessage}
        showBreadcrumps={showBreadcrumps}
        breadcrumbs={breadcrumbs}
      >
        {children}
      </MemoRootShell>
    </ErrorBoundary>
  );
};

export default memo(RootShellWithRelay);
