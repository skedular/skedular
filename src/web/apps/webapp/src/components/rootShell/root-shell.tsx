import { AppBar } from '@/components/appBar';
import { InfoIcon, SignOutIcon } from '@/components/icons';
import { getInstallMsTeamsLink, getOrganizationSsoSignInBaseLink, getRootLink, getSignOutReturnToLink, getWelcomeLink } from '@/components/links';
import { Loading } from '@/components/loading';
import { LeftSideNavigationMenu } from '@/components/navigationMenu';
import { Observability } from '@/components/observability';
import { RelayError, toRootError } from '@/components/relayError';
import useKnownParams from '@/hooks/use-known-params';
import type { rootShell_rootQuery } from '@/queries/__generated__/rootShell_rootQuery.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import CssBaseline from '@mui/material/CssBaseline';
import { InMsTeamsContext, useIntegratedPlatrform } from '@skedular/shared';
import { BodyIconTypography, CaptionIconTypography, PushToRight, SmallHeadingIconTypography, StackColumn, StackRow } from '@skedular/ui';
import { useAuth } from '@workos-inc/authkit-nextjs/components';
import { usePathname, useRouter } from 'next/navigation';
import type { PropsWithChildren } from 'react';
import { memo, useContext, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { v7 as uuid } from 'uuid';

type Props = {
  queryReference: PreloadedQuery<rootShell_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationCustomDomain: string;
};

const RootQuery = graphql`
  query rootShell_rootQuery($organizationCustomDomain: String!) {
    me {
      id
      isOnboardingDone
    }
    customerReadinessSynced
    pendingOrganizationInvitationsCount
    isAzureTenantInstalled
    azureTenantOrganization {
      id
    }
    organization(customDomain: $organizationCustomDomain) {
      logoUrl
      name
      isSsoTokenValid
      isOwnershipVerified
      type {
        type
      }
    }
    ...appBar_query
    ...leftSideNavigationMenu_query
    ...observability_query
  }
`;

const maxRetryAttemptsToReload = 20;

const RootShell = ({ queryReference, children, onReloadRequired, organizationCustomDomain }: PropsWithChildren<Props>) => {
  const rootData = usePreloadedQuery<rootShell_rootQuery>(RootQuery, queryReference);
  const { integratedPlatrform } = useIntegratedPlatrform();
  const inMsTeams = useContext(InMsTeamsContext);
  const router = useRouter();
  const pathName = usePathname();
  const { signOut } = useAuth();
  const [reloadCount, setReloadCount] = useState(0);
  const rootLink = getRootLink(integratedPlatrform);
  const welcomeLink = getWelcomeLink(integratedPlatrform);
  const installMsTeamsLink = getInstallMsTeamsLink();
  const areCustomerRecordsSync = !!rootData?.customerReadinessSynced;

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
    await signOut({ returnTo: getSignOutReturnToLink() });
  };

  if (reloadCount === maxRetryAttemptsToReload) {
    return (
      <Box sx={{ display: 'flex', flexDirection: 'column', justifyContent: 'center', alignItems: 'center', minHeight: '100vh' }}>
        <SmallHeadingIconTypography label="There was an issue activating your account. Kindly sign out and then sign back in to resolve the problem." />
        <Button variant="contained" startIcon={<SignOutIcon />} onClick={handleSignOutClick}>
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
        <LeftSideNavigationMenu rootDataRelay={rootData} />
        <Box sx={{ flexGrow: 1 }}>
          <AppBar rootDataRelay={rootData} />
          {rootData.me.isOnboardingDone && !rootData.organization?.isSsoTokenValid && (
            <Box sx={{ display: 'flex', justifyContent: 'center', px: { xs: 1, sm: 2, md: 3 }, pt: 1.5 }}>
              <Box
                sx={{
                  width: '100%',
                  maxWidth: 1200,
                  mx: 'auto',
                  borderRadius: 3,
                  border: 1,
                  borderColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(217, 119, 6, 0.18)' : 'rgba(251, 191, 36, 0.28)'),
                  backgroundColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(255, 251, 235, 0.92)' : 'rgba(120, 53, 15, 0.24)'),
                  boxShadow: (theme) => (theme.palette.mode === 'light' ? '0 8px 20px rgba(120, 53, 15, 0.06)' : 'none'),
                  px: 2,
                  py: 1.5,
                }}
              >
                <StackRow sx={{ alignItems: 'flex-start', gap: 1.5 }}>
                  <InfoIcon color="warning" excludeTooltip sx={{ mt: 0.25 }} />
                  <StackColumn sx={{ gap: 0.25 }}>
                    <BodyIconTypography label={`Single sign-on to see results in the ${rootData.organization?.name} organization.`} />
                    <PushToRight />
                    <Button
                      variant="contained"
                      href={getOrganizationSsoSignInBaseLink(integratedPlatrform, organizationCustomDomain)}
                      sx={{ whiteSpace: 'nowrap', textTransform: 'none' }}
                    >
                      Single sign-on
                    </Button>
                  </StackColumn>
                </StackRow>
              </Box>
            </Box>
          )}
          {rootData.me.isOnboardingDone &&
            !rootData.organization?.isOwnershipVerified &&
            (rootData.organization?.type.type === 'MARKETPLACE' || rootData.organization?.type.type === 'INDIVIDUAL') && (
              <Box sx={{ display: 'flex', justifyContent: 'center', px: { xs: 1, sm: 2, md: 3 }, pt: 1.5 }}>
                <Box
                  sx={{
                    width: '100%',
                    maxWidth: 1200,
                    mx: 'auto',
                    borderRadius: 3,
                    border: 1,
                    borderColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(217, 119, 6, 0.18)' : 'rgba(251, 191, 36, 0.28)'),
                    backgroundColor: (theme) => (theme.palette.mode === 'light' ? 'rgba(255, 251, 235, 0.92)' : 'rgba(120, 53, 15, 0.24)'),
                    boxShadow: (theme) => (theme.palette.mode === 'light' ? '0 8px 20px rgba(120, 53, 15, 0.06)' : 'none'),
                    px: 2,
                    py: 1.5,
                  }}
                >
                  <StackRow sx={{ alignItems: 'flex-start', gap: 1.5 }}>
                    <InfoIcon color="warning" excludeTooltip sx={{ mt: 0.25 }} />
                    <StackColumn sx={{ gap: 0.25 }}>
                      <BodyIconTypography label="Ownership verification in progress" />
                      <CaptionIconTypography label="We need to verify ownership for your organization. We will get back to you within 24 hours." />
                    </StackColumn>
                  </StackRow>
                </Box>
              </Box>
            )}
          {rootData.me.isOnboardingDone && rootData.organization?.isSsoTokenValid && <>{children}</>}
        </Box>
      </Box>
    </>
  );
};

const MemoRootShell = memo(RootShell);

type RelayProps = object;

const RootShellWithRelay = ({ children }: PropsWithChildren<RelayProps>) => {
  const [queryReference, loadQuery] = useQueryLoader<rootShell_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(uuid());
  const [, startTransition] = useTransition();
  const { organizationCustomDomain } = useKnownParams();

  if (!organizationCustomDomain) {
    throw new Error('organizationCustomDomain is required');
  }

  useEffect(() => {
    loadQuery(
      {
        organizationCustomDomain,
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId, organizationCustomDomain]);

  const handleReloadRequired = () => {
    startTransition(() => {
      setTriggerReloadId(uuid());
    });
  };

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }) => <RelayError error={toRootError(error)} />}>
      <MemoRootShell queryReference={queryReference} onReloadRequired={handleReloadRequired} organizationCustomDomain={organizationCustomDomain}>
        {children}
      </MemoRootShell>
    </ErrorBoundary>
  );
};

export default memo(RootShellWithRelay);
