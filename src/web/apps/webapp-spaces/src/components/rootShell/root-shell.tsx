import { RelayError, toRootError, useIntegratedPlatform, useKnownParams } from '@skedular/shared';
import { AppBar } from '@/components/appBar';
import { InfoIcon, SignOutIcon } from '@/components/icons';
import { getOrganizationAdminSubscriptionsBaseLink, getOrganizationSsoSignInBaseLink, getRootLink, getSignOutReturnToLink, getWelcomeLink } from '@/components/links';
import { Loading } from '@/components/loading';
import { LeftSideNavigationMenu } from '@/components/navigationMenu';
import { Observability } from '@/components/observability';
import { SpacesSubscriptionProvider, type SpacesSubscriptionState } from './spaces-subscription-context';
import { getSpacesSubscriptionPresentation } from './spaces-subscription-presentation';

import type { rootShell_rootQuery } from '@/queries/__generated__/rootShell_rootQuery.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Alert from '@mui/material/Alert';
import CssBaseline from '@mui/material/CssBaseline';

import { BodyIconTypography, CaptionIconTypography, PushToRight, SmallHeadingIconTypography, StackColumn, StackRow } from '@skedular/ui';
import { useAuth } from '@workos-inc/authkit-nextjs/components';
import { usePathname, useRouter, useSearchParams } from 'next/navigation';
import type { PropsWithChildren } from 'react';
import { memo, useEffect, useState, useTransition } from 'react';
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
    organizationSpacesSubscription(organizationId: $organizationCustomDomain) {
      subscriptionStatus
      accessReason
      trialStartedAt
      trialEndsAt
      remainingTrialDays
      canUseProduct
      canAcceptBookings
      canProtectExistingCommitments
      upgradeRequired
      isComplimentaryBridge
      nextBillingAt
    }
    ...appBar_query
    ...leftSideNavigationMenu_query
    ...observability_query
  }
`;

const maxRetryAttemptsToReload = 20;

const RootShell = ({ queryReference, children, onReloadRequired, organizationCustomDomain }: PropsWithChildren<Props>) => {
  const rootData = usePreloadedQuery<rootShell_rootQuery>(RootQuery, queryReference);
  const { integratedPlatform } = useIntegratedPlatform();
  const router = useRouter();
  const pathName = usePathname();
  const searchParams = useSearchParams();
  const { signOut } = useAuth();
  const [reloadCount, setReloadCount] = useState(0);
  const rootLink = getRootLink(integratedPlatform);
  const welcomeLink = getWelcomeLink(integratedPlatform);
  const areCustomerRecordsSync = !!rootData?.customerReadinessSynced;
  const spacesSubscription = rootData.organizationSpacesSubscription as SpacesSubscriptionState | null;

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

  const subscriptionPresentation = getSpacesSubscriptionPresentation(spacesSubscription);
  const isPlanManagementRoute =
    pathName.endsWith('/admin') && ((searchParams.get('section') === 'setup' && searchParams.get('profileSection') === 'plan') || searchParams.get('section') === 'subscriptions');
  const canRenderProduct = !subscriptionPresentation.blocksProduct || isPlanManagementRoute;

  return (
    <SpacesSubscriptionProvider value={spacesSubscription}>
      <Observability rootDataRelay={rootData} onReloadRequired={onReloadRequired} />
      <Box sx={{ display: 'flex' }}>
        <CssBaseline enableColorScheme />
        <LeftSideNavigationMenu rootDataRelay={rootData} />
        <Box sx={{ flexGrow: 1 }}>
          <AppBar rootDataRelay={rootData} />
          {rootData.me.isOnboardingDone && subscriptionPresentation.showBanner ? (
            <Box sx={{ display: 'flex', justifyContent: 'center', px: { xs: 1, sm: 2, md: 3 }, pt: 1.5 }}>
              <Alert
                severity={subscriptionPresentation.severity}
                action={
                  <Button color="inherit" href={getOrganizationAdminSubscriptionsBaseLink(integratedPlatform, organizationCustomDomain)} sx={{ whiteSpace: 'nowrap' }}>
                    Upgrade
                  </Button>
                }
                sx={{ width: '100%', maxWidth: 1200 }}
              >
                {subscriptionPresentation.bannerMessage}
              </Alert>
            </Box>
          ) : null}
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
                      href={getOrganizationSsoSignInBaseLink(integratedPlatform, organizationCustomDomain)}
                      sx={{ whiteSpace: 'nowrap', textTransform: 'none' }}
                    >
                      Single sign-on
                    </Button>
                  </StackColumn>
                </StackRow>
              </Box>
            </Box>
          )}
          {rootData.me.isOnboardingDone && !rootData.organization?.isOwnershipVerified && rootData.organization?.type.type === 'MARKETPLACE' && (
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
          {rootData.me.isOnboardingDone && rootData.organization?.isSsoTokenValid && canRenderProduct ? (
            <>{children}</>
          ) : rootData.me.isOnboardingDone && rootData.organization?.isSsoTokenValid ? (
            <Box sx={{ maxWidth: 760, mx: 'auto', px: 2, py: 8 }}>
              <StackColumn spacing={2} sx={{ alignItems: 'flex-start' }}>
                <SmallHeadingIconTypography label="Your Spaces trial has ended" />
                <BodyIconTypography label="Upgrade to a paid plan to continue using Spaces and accepting bookings. Your listings, bookings, configuration, and history are preserved." />
                <Button variant="contained" href={getOrganizationAdminSubscriptionsBaseLink(integratedPlatform, organizationCustomDomain)}>
                  View upgrade options
                </Button>
              </StackColumn>
            </Box>
          ) : null}
        </Box>
      </Box>
    </SpacesSubscriptionProvider>
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
