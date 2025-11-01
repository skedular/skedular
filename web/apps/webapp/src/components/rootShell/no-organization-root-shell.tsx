import { NoOrganizationAppBar } from '@/components/appBar';
import { SmallHeadingIconTypography } from '@/components/commons';
import { SignOutIcon } from '@/components/icons';
import { getInstallMsTeamsLink, getRootLink, getWelcomeLink } from '@/components/links';
import { Loading } from '@/components/loading';
import { NoOrganizationLeftSideNavigationMenu } from '@/components/navigationMenu';
import { Observability } from '@/components/observability';
import type { RootError } from '@/components/relayError';
import { RelayError } from '@/components/relayError';
import { InMsTeamsContext, useIntegratedPlatrform } from '@/libs/providers';
import type { noOrganizationRootShell_rootQuery } from '@/queries/__generated__/noOrganizationRootShell_rootQuery.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import CssBaseline from '@mui/material/CssBaseline';
import { useAuth } from '@workos-inc/authkit-nextjs/components';
import { usePathname, useRouter } from 'next/navigation';
import type { JSX, PropsWithChildren } from 'react';
import { memo, useContext, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { v7 as uuid } from 'uuid';

type Props = {
  queryReference: PreloadedQuery<noOrganizationRootShell_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  collapsed?: boolean;
  hideOrganizationSelector?: boolean;
  hideWelcomeMessage?: boolean;
  showBreadcrumps?: boolean;
  breadcrumbs?: React.ReactNode | JSX.Element;
};

const RootQuery = graphql`
  query noOrganizationRootShell_rootQuery {
    me {
      id
      isOnboardingDone
    }
    bookingCustomerRecordSynced
    locationCustomerRecordSynced
    marketplaceCustomerRecordSynced
    msTeamsCustomerRecordSynced
    organizationCustomerRecordSynced
    slackCustomerRecordSynced
    teamCustomerRecordSynced
    coreCustomerRecordSynced
    isAzureTenantInstalled
    azureTenantOrganization {
      id
    }
    ...noOrganizationAppBar_query
    ...observability_query
  }
`;

const maxRetryAttemptsToReload = 20;

const NoOrganizationRootShell = ({
  queryReference,
  children,
  onReloadRequired,
  collapsed,
  hideOrganizationSelector,
  hideWelcomeMessage,
  showBreadcrumps,
  breadcrumbs,
}: PropsWithChildren<Props>) => {
  const rootData = usePreloadedQuery<noOrganizationRootShell_rootQuery>(RootQuery, queryReference);
  const { integratedPlatrform } = useIntegratedPlatrform();
  const inMsTeams = useContext(InMsTeamsContext);
  const router = useRouter();
  const pathName = usePathname();
  const { signOut } = useAuth();
  const [reloadCount, setReloadCount] = useState(0);
  const rootLink = getRootLink(integratedPlatrform);
  const welcomeLink = getWelcomeLink(integratedPlatrform);
  const installMsTeamsLink = getInstallMsTeamsLink();
  const areCustomerRecordsSync = !!(
    rootData?.bookingCustomerRecordSynced &&
    rootData?.locationCustomerRecordSynced &&
    rootData?.marketplaceCustomerRecordSynced &&
    rootData?.msTeamsCustomerRecordSynced &&
    rootData?.organizationCustomerRecordSynced &&
    rootData?.slackCustomerRecordSynced &&
    rootData?.teamCustomerRecordSynced &&
    rootData?.coreCustomerRecordSynced
  );

  useEffect(() => {
    if (reloadCount === maxRetryAttemptsToReload || (rootData.me && areCustomerRecordsSync)) {
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

  useEffect(() => {
    if (!inMsTeams) {
      return;
    }

    if (!rootData.isAzureTenantInstalled || !rootData.azureTenantOrganization) {
      router.push(installMsTeamsLink);
    }
  }, [inMsTeams, rootData.isAzureTenantInstalled, rootData.azureTenantOrganization, installMsTeamsLink, router]);

  useEffect(() => {
    if (pathName === rootLink && !rootData.me.isOnboardingDone) {
      router.push(welcomeLink);
    }
  }, [rootData.me.isOnboardingDone, welcomeLink, pathName, rootLink, router]);

  const handleSignOutClick = async () => {
    await signOut();
    router.push(rootLink);
  };

  if (reloadCount === maxRetryAttemptsToReload) {
    return (
      <Box display="flex" flexDirection="column" justifyContent="center" alignItems="center" minHeight="100vh">
        <SmallHeadingIconTypography label="There was an issue activating your account. Kindly sign out and then sign back in to resolve the problem." />
        <Button variant="contained" startIcon={<SignOutIcon />} onClick={async () => await handleSignOutClick}>
          Sign out
        </Button>
      </Box>
    );
  }

  if (!areCustomerRecordsSync) {
    return <Loading message="Kindly hold on as we proceed to activate your account..." />;
  }

  return (
    <>
      <Observability rootDataRelay={rootData} onReloadRequired={onReloadRequired} />
      <Box sx={{ display: 'flex' }}>
        <CssBaseline enableColorScheme />
        <NoOrganizationLeftSideNavigationMenu collapsed={collapsed} />
        <Box sx={{ flexGrow: 1 }}>
          <NoOrganizationAppBar
            rootDataRelay={rootData}
            hideOrganizationSelector={hideOrganizationSelector}
            hideWelcomeMessage={hideWelcomeMessage}
            showBreadcrumps={showBreadcrumps}
            breadcrumbs={breadcrumbs}
          />
          {children}
        </Box>
      </Box>
    </>
  );
};

const MemoNoOrganizationRootShell = memo(NoOrganizationRootShell);

type RelayProps = {
  collapsed?: boolean;
  hideOrganizationSelector?: boolean;
  hideWelcomeMessage?: boolean;
  showBreadcrumps?: boolean;
  breadcrumbs?: React.ReactNode | JSX.Element;
};

const NoOrganizationRootShellWithRelay = ({ children, collapsed, hideOrganizationSelector, hideWelcomeMessage, showBreadcrumps, breadcrumbs }: PropsWithChildren<RelayProps>) => {
  const [queryReference, loadQuery] = useQueryLoader<noOrganizationRootShell_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(uuid());
  const [, startTransition] = useTransition();
  const router = useRouter();

  useEffect(() => {
    loadQuery(
      {},
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId, router]);

  const handleReloadRequired = () => {
    startTransition(() => {
      setTriggerReloadId(uuid());
    });
  };

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }: { error: RootError }) => <RelayError error={error} />}>
      <MemoNoOrganizationRootShell
        queryReference={queryReference}
        onReloadRequired={handleReloadRequired}
        collapsed={collapsed}
        hideOrganizationSelector={hideOrganizationSelector}
        hideWelcomeMessage={hideWelcomeMessage}
        showBreadcrumps={showBreadcrumps}
        breadcrumbs={breadcrumbs}
      >
        {children}
      </MemoNoOrganizationRootShell>
    </ErrorBoundary>
  );
};

export default memo(NoOrganizationRootShellWithRelay);
