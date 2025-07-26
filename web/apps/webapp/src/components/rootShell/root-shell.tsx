import { AppBar } from '@/components/appBar';
import { getSignInUrlAction } from '@/components/authActions';
import { LeadIconTypography, PushToRight, SmallHeadingIconTypography, StackRow } from '@/components/commons';
import { LogoutIcon, SsoSigninIcon } from '@/components/icons';
import { getInstallMsTeamsLink, getOrganizationSsoSignInBaseLink, getRootLink, getWelcomeLink } from '@/components/links';
import { Loading } from '@/components/loading';
import { LeftSideNavigationMenu } from '@/components/navigationMenu';
import { Observability } from '@/components/observability';
import type { RootError } from '@/components/relayError';
import { RelayError } from '@/components/relayError';
import { InMsTeamsContext, PaletteModeContext, useIntegratedPlatrform } from '@/libs/providers';
import { coal, emerald } from '@/libs/theme';
import type { rootShell_rootQuery } from '@/queries/__generated__/rootShell_rootQuery.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import CssBaseline from '@mui/material/CssBaseline';
import { useAuth } from '@workos-inc/authkit-nextjs/components';
import { useParams, usePathname, useRouter } from 'next/navigation';
import type { JSX, PropsWithChildren } from 'react';
import { memo, useCallback, useContext, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { v7 as uuid } from 'uuid';

type Props = {
  queryReference: PreloadedQuery<rootShell_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  collapsed?: boolean;
  hideOrganizationSelector?: boolean;
  hideWelcomeMessage?: boolean;
  showBreadcrumps?: boolean;
  breadcrumbs?: React.ReactNode | JSX.Element;
  organizationId: string;
};

const RootQuery = graphql`
  query rootShell_rootQuery($organizationId: String!, $organizationExists: Boolean!) {
    me {
      id
      isOnboardingDone
    }
    myOrganizations {
      id
    }
    bookingCustomerRecordSynced
    locationCustomerRecordSynced
    marketplaceCustomerRecordSynced
    msTeamsCustomerRecordSynced
    organizationCustomerRecordSynced
    slackCustomerRecordSynced
    teamCustomerRecordSynced
    coreCustomerRecordSynced
    pendingOrganizationInvitationsCount
    isAzureTenantInstalled
    azureTenantOrganization {
      id
    }
    isOrganizationSsoTokenValid(id: $organizationId) @include(if: $organizationExists)
    organization(id: $organizationId) @include(if: $organizationExists) {
      logoUrl
      name
    }
    ...appBar_query
    ...leftSideNavigationMenu_query
    ...observability_query
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
  organizationId,
}: PropsWithChildren<Props>) => {
  const rootData = usePreloadedQuery<rootShell_rootQuery>(RootQuery, queryReference);
  const { integratedPlatrform } = useIntegratedPlatrform();
  const paletteMode = useContext(PaletteModeContext);
  const inMsTeams = useContext(InMsTeamsContext);
  const router = useRouter();
  const pathName = usePathname();
  const { signOut } = useAuth();
  const [reloadCount, setReloadCount] = useState(0);
  const rootLink = getRootLink(integratedPlatrform);
  const welcomeLink = getWelcomeLink(integratedPlatrform);
  const installMsTeamsLink = getInstallMsTeamsLink();
  const areCustomerRecordsSync = useCallback(
    () =>
      rootData?.bookingCustomerRecordSynced &&
      rootData?.locationCustomerRecordSynced &&
      rootData?.marketplaceCustomerRecordSynced &&
      rootData?.msTeamsCustomerRecordSynced &&
      rootData?.organizationCustomerRecordSynced &&
      rootData?.slackCustomerRecordSynced &&
      rootData?.teamCustomerRecordSynced &&
      rootData?.coreCustomerRecordSynced,
    [
      rootData?.bookingCustomerRecordSynced,
      rootData?.locationCustomerRecordSynced,
      rootData?.marketplaceCustomerRecordSynced,
      rootData?.msTeamsCustomerRecordSynced,
      rootData?.organizationCustomerRecordSynced,
      rootData?.slackCustomerRecordSynced,
      rootData?.teamCustomerRecordSynced,
      rootData?.coreCustomerRecordSynced,
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
        <Button variant="contained" startIcon={<LogoutIcon />} onClick={async () => await handleSignOutClick}>
          Sign out
        </Button>
      </Box>
    );
  }

  if (!areCustomerRecordsSync()) {
    return <Loading message="Kindly hold on as we proceed to activate your account..." />;
  }

  return (
    <>
      <Observability rootDataRelay={rootData} onReloadRequired={onReloadRequired} />
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
          {rootData.me.isOnboardingDone && !rootData.isOrganizationSsoTokenValid && (
            <Card sx={{ textAlign: 'center', backgroundColor: paletteMode === 'dark' ? emerald : coal }}>
              <CardContent>
                <StackRow>
                  <LeadIconTypography
                    label={`Single sign-on to see results in the ${rootData.organization?.name} organization.`}
                    invertDefaultColor
                    startElement={<SsoSigninIcon />}
                  />
                  <PushToRight />
                  <Button variant="contained" href={getOrganizationSsoSignInBaseLink(integratedPlatrform, organizationId)} sx={{ whiteSpace: 'nowrap', textTransform: 'none' }}>
                    Single sign-on
                  </Button>
                </StackRow>
              </CardContent>
            </Card>
          )}
          {rootData.me.isOnboardingDone && rootData.isOrganizationSsoTokenValid && <>{children}</>}
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

const RootShellWithRelay = ({ children, collapsed, hideOrganizationSelector, hideWelcomeMessage, showBreadcrumps, breadcrumbs }: PropsWithChildren<RelayProps>) => {
  const [queryReference, loadQuery] = useQueryLoader<rootShell_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(uuid());
  const [, startTransition] = useTransition();
  const { user, loading } = useAuth();
  const [signInUrl, setSignInUrl] = useState('');
  const router = useRouter();
  const { organizationId } = useParams();
  const inMsTeams = useContext(InMsTeamsContext);

  let finalOrganizationId = '';
  if (typeof organizationId === 'string') {
    finalOrganizationId = organizationId;
  } else if (Array.isArray(organizationId)) {
    if (typeof organizationId[0] !== 'undefined') {
      finalOrganizationId = organizationId[0];
    }
  } else {
    throw new Error('organizationId is required');
  }

  useEffect(() => {
    if (inMsTeams) {
      return;
    }

    async function loadSignInUrl() {
      setSignInUrl(await getSignInUrlAction());
    }

    loadSignInUrl();
  }, [inMsTeams]);

  useEffect(() => {
    if (!inMsTeams) {
      if (loading || !signInUrl) {
        return;
      }

      if (!user && signInUrl) {
        router.push(signInUrl);
        return;
      }
    }

    loadQuery(
      {
        organizationId: finalOrganizationId,
        organizationExists: !!finalOrganizationId,
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId, finalOrganizationId, loading, user, router, signInUrl, inMsTeams]);

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
      <MemoRootShell
        queryReference={queryReference}
        onReloadRequired={handleReloadRequired}
        collapsed={collapsed}
        hideOrganizationSelector={hideOrganizationSelector}
        hideWelcomeMessage={hideWelcomeMessage}
        showBreadcrumps={showBreadcrumps}
        breadcrumbs={breadcrumbs}
        organizationId={finalOrganizationId}
      >
        {children}
      </MemoRootShell>
    </ErrorBoundary>
  );
};

export default memo(RootShellWithRelay);
