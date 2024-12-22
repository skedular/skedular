import { AppBar, OldAppBar } from '@/components/appBar';
import { LeftSideNavigationMenuContent } from '@/components/navigationMenu';
import { Observability } from '@/components/observability';
import { OrganizationOnboarding } from '@/components/organization/organizationOnboarding';
import type { rootShell_rootQuery } from '@/queries/__generated__/rootShell_rootQuery.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import CssBaseline from '@mui/material/CssBaseline';
import Drawer from '@mui/material/Drawer';
import Grid from '@mui/material/Grid2';
import { SmallHeadingIconTypography, StackColumn } from '@repo/shared/components/commons';
import { LogoutIcon } from '@repo/shared/components/icons';
import { Loading } from '@repo/shared/components/loading';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { SwitchToModernUIContext } from '@repo/shared/libs/providers';
import { nanoid } from 'nanoid';
import { signOut } from 'next-auth/react';
import { useParams } from 'next/navigation';
import { PropsWithChildren, memo, useCallback, useContext, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, graphql, usePreloadedQuery, useQueryLoader } from 'react-relay';

type Props = {
  queryReference: PreloadedQuery<rootShell_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  collapsed?: boolean;
  hideOrganizationSelector?: boolean;
  hideWelcomeMessage?: boolean;
  showBreadcrumps?: boolean;
  breadcrumbs?: React.ReactNode | JSX.Element;
};

const RootQuery = graphql`
  query rootShell_rootQuery($organizationId: String!, $organizationExists: Boolean!) {
    me {
      id
    }
    myOrganizations {
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
    ...leftSideNavigationMenuContent_query
  }
`;

const maxRetryAttemptsToReload = 20;
const drawerWithTextWidth = 250;
const drawerWithoutTextWidth = 80;

const RootShell = ({
  queryReference,
  children,
  onReloadRequired,
  collapsed,
  hideOrganizationSelector,
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

  const handleSignOutClick = () => {
    signOut();
  };

  if (reloadCount === maxRetryAttemptsToReload) {
    return (
      <Box display="flex" flexDirection="column" justifyContent="center" alignItems="center" minHeight="100vh">
        <SmallHeadingIconTypography label="There was an issue activating your account. Kindly sign out and then sign back in to resolve the problem." />
        <Button variant="contained" startIcon={<LogoutIcon />} onClick={handleSignOutClick}>
          Sign out
        </Button>
      </Box>
    );
  }

  if (!rootData.me || !areCustomerRecordsSync()) {
    return <Loading message="Kindly hold on as we proceed to activate your account..." />;
  }

  const finalDrawerWidth = collapsed ? drawerWithoutTextWidth : drawerWithTextWidth;

  return (
    <>
      <Observability />
      <Box sx={{ display: 'flex' }}>
        <CssBaseline enableColorScheme />
        <Drawer
          sx={{
            display: { xs: 'none', sm: 'block' },
            width: finalDrawerWidth,
            flexShrink: 0,
            '& .MuiDrawer-paper': {
              width: finalDrawerWidth,
              boxSizing: 'border-box',
            },
          }}
          variant="persistent"
          open={true}
        >
          <LeftSideNavigationMenuContent
            rootDataRelay={rootData}
            onReloadRequired={onReloadRequired}
            maxWidth={finalDrawerWidth}
            showIconsOnly={collapsed}
          />
        </Drawer>
        <Grid container>
          <Grid
            sx={{
              xs: 12,
              sm: 6,
              md: 3,
              lg: 2,
              xl: 2,
              flexGrow: 1,
              display: { xs: 'block', sm: 'none' },
              backgroundColor: (theme) => theme.palette.background.paper,
            }}
          >
            <LeftSideNavigationMenuContent
              rootDataRelay={rootData}
              onReloadRequired={onReloadRequired}
              maxWidth={finalDrawerWidth}
              showIconsOnly={collapsed}
            />
          </Grid>
          <StackColumn sx={{ width: '100vw' }}>
            {!switchToModernUI && <OldAppBar rootDataRelay={rootData} onReloadRequired={onReloadRequired} />}
            {switchToModernUI && (
              <AppBar
                rootDataRelay={rootData}
                onReloadRequired={onReloadRequired}
                hideOrganizationSelector={hideOrganizationSelector}
                hideWelcomeMessage={hideWelcomeMessage}
                showBreadcrumps={showBreadcrumps}
                breadcrumbs={breadcrumbs}
              />
            )}
          </StackColumn>
          {!rootData.myOrganizations ||
            (rootData.myOrganizations.length === 0 && (
              <Grid sx={{ xs: 12, sm: 6, md: 3, lg: 2, xl: 2, flexGrow: 1 }}>
                <OrganizationOnboarding onReloadRequired={onReloadRequired} />
              </Grid>
            ))}
          {rootData.myOrganizations && rootData.myOrganizations.length !== 0 && (
            <Grid sx={{ xs: 12, sm: 6, md: 3, lg: 2, xl: 2, flexGrow: 1 }}>{children}</Grid>
          )}
        </Grid>
      </Box>
    </>
  );
};

const MemoRootShell = memo(RootShell);

type RelayProps = {
  collapsed?: boolean;
  hideOrganizationSelector?: boolean;
  hideWelcomeMessage?: boolean;
  showBreadcrumps?: boolean;
  breadcrumbs?: React.ReactNode | JSX.Element;
};

const RootShellWithRelay = ({
  children,
  collapsed,
  hideOrganizationSelector,
  hideWelcomeMessage,
  showBreadcrumps,
  breadcrumbs,
}: PropsWithChildren<RelayProps>) => {
  const [queryReference, loadQuery] = useQueryLoader<rootShell_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(nanoid());
  const [, startTransition] = useTransition();
  const { organizationId } = useParams();

  let finalOrganizationId = '';
  if (typeof organizationId === 'string') {
    finalOrganizationId = organizationId;
  } else if (Array.isArray(organizationId)) {
    if (typeof organizationId[0] !== 'undefined') {
      finalOrganizationId = organizationId[0];
    }
  }

  useEffect(() => {
    loadQuery(
      {
        organizationId: finalOrganizationId,
        organizationExists: !!finalOrganizationId,
      },
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
        hideOrganizationSelector={hideOrganizationSelector}
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
