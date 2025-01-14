import { AppBar } from '@/components/appBar';
import { LeftSideNavigationMenu } from '@/components/navigationMenu';
import { Notifications } from '@/components/notification/notifications';
import { Observability } from '@/components/observability';
import { OrganizationOnboarding } from '@/components/organization/organizationOnboarding';
import type { rootShell_rootQuery } from '@/queries/__generated__/rootShell_rootQuery.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import CssBaseline from '@mui/material/CssBaseline';
import { SmallHeadingIconTypography } from '@repo/shared/components/commons';
import { LogoutIcon } from '@repo/shared/components/icons';
import { Loading } from '@repo/shared/components/loading';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { nanoid } from 'nanoid';
import { signOut } from 'next-auth/react';
import { useParams } from 'next/navigation';
import type { PropsWithChildren } from 'react';
import { memo, useCallback, useEffect, useState, useTransition } from 'react';
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
    pendingInvitationsCount
    ...oldAppBar_query
    ...appBar_query
    ...leftSideNavigationMenu_query
  }
`;

const maxRetryAttemptsToReload = 20;

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

  return (
    <>
      <Observability />
      <Box sx={{ display: 'flex' }}>
        <CssBaseline enableColorScheme />
        <LeftSideNavigationMenu rootDataRelay={rootData} collapsed={collapsed} />
        <Box sx={{ flexGrow: 1 }}>
          <AppBar
            rootDataRelay={rootData}
            hideOrganizationSelector={hideOrganizationSelector}
            hideWelcomeMessage={hideWelcomeMessage}
            showBreadcrumps={showBreadcrumps}
            breadcrumbs={breadcrumbs}
          />
          {!rootData.myOrganizations ||
            (rootData.myOrganizations.length === 0 && rootData.pendingInvitationsCount === 0 && (
              <OrganizationOnboarding onReloadRequired={onReloadRequired} />
            ))}
          {!rootData.myOrganizations || (rootData.myOrganizations.length === 0 && rootData.pendingInvitationsCount > 0 && <Notifications />)}
          {rootData.myOrganizations && rootData.myOrganizations.length !== 0 && <>{children}</>}
        </Box>
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
