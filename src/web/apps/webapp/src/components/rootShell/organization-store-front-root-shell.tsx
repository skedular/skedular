import { OrganizationStoreFrontAppBar } from '@/components/appBar';
import { SignOutIcon } from '@/components/icons';
import { getSignOutReturnToLink } from '@/components/links';
import { Loading } from '@/components/loading';
import { Observability } from '@/components/observability';
import StoreFrontBrowserMetadata from '@/components/organizationStoreFrontGuest/store-front-browser-metadata';
import { RelayError, toRootError } from '@skedular/shared';
import useKnownParams from '@/hooks/use-known-params';
import type { organizationStoreFrontRootShell_rootQuery } from '@/queries/__generated__/organizationStoreFrontRootShell_rootQuery.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import CssBaseline from '@mui/material/CssBaseline';
import { SmallHeadingIconTypography } from '@skedular/ui';
import { useAuth } from '@workos-inc/authkit-nextjs/components';
import type { PropsWithChildren } from 'react';
import { memo, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { v7 as uuid } from 'uuid';

type Props = {
  queryReference: PreloadedQuery<organizationStoreFrontRootShell_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
};

const RootQuery = graphql`
  query organizationStoreFrontRootShell_rootQuery($organizationCustomDomain: String!) {
    me {
      id
    }
    customerReadinessSynced
    organizationPublic(customDomain: $organizationCustomDomain) {
      name
      logoUrl
    }
    ...organizationStoreFrontAppBar_query
    ...observability_query
  }
`;

const maxRetryAttemptsToReload = 20;

const OrganizationStoreFrontRootShell = ({ queryReference, children, onReloadRequired }: PropsWithChildren<Props>) => {
  const rootData = usePreloadedQuery<organizationStoreFrontRootShell_rootQuery>(RootQuery, queryReference);
  const { signOut } = useAuth();
  const [reloadCount, setReloadCount] = useState(0);
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

  const handleSignOutClick = async () => {
    await signOut({ returnTo: getSignOutReturnToLink() });
  };

  // Always render metadata regardless of sync state so title/icon are set
  // immediately when org data arrives, not gated behind customer record syncing.
  const browserMetadata = rootData.organizationPublic ? (
    <StoreFrontBrowserMetadata organizationName={rootData.organizationPublic.name} organizationLogoUrl={rootData.organizationPublic.logoUrl} />
  ) : null;

  if (reloadCount === maxRetryAttemptsToReload) {
    return (
      <>
        {browserMetadata}
        <Box sx={{ display: 'flex', flexDirection: 'column', justifyContent: 'center', alignItems: 'center', minHeight: '100vh' }}>
          <SmallHeadingIconTypography label="There was an issue activating your account. Kindly sign out and then sign back in to resolve the problem." />
          <Button variant="contained" startIcon={<SignOutIcon />} onClick={handleSignOutClick}>
            Sign out
          </Button>
        </Box>
      </>
    );
  }

  if (!areCustomerRecordsSync) {
    return (
      <>
        {browserMetadata}
        <Loading message="Kindly hold on as we proceed to activate your account..." />
      </>
    );
  }

  return (
    <>
      {browserMetadata}
      <Observability rootDataRelay={rootData} onReloadRequired={onReloadRequired} />
      <Box sx={{ display: 'flex', width: '100%', maxWidth: '100vw', minHeight: '100vh', overflowX: 'clip', bgcolor: (theme) => theme.palette.background.default }}>
        <CssBaseline enableColorScheme />
        <Box component="main" sx={{ flexGrow: 1, width: '100%', minWidth: 0, maxWidth: '100%', overflowX: 'clip' }}>
          <OrganizationStoreFrontAppBar rootDataRelay={rootData} />
          {children}
        </Box>
      </Box>
    </>
  );
};

const MemoOrganizationStoreFrontRootShell = memo(OrganizationStoreFrontRootShell);

const OrganizationStoreFrontRootShellWithRelay = ({ children }: PropsWithChildren) => {
  const [queryReference, loadQuery] = useQueryLoader<organizationStoreFrontRootShell_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(uuid());
  const [, startTransition] = useTransition();
  const { organizationCustomDomain } = useKnownParams();

  if (!organizationCustomDomain) {
    throw new Error('organizationCustomDomain is required');
  }

  useEffect(() => {
    loadQuery(
      { organizationCustomDomain },
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

  return !queryReference ? (
    <Loading />
  ) : (
    <ErrorBoundary fallbackRender={({ error }) => <RelayError error={toRootError(error)} />}>
      <MemoOrganizationStoreFrontRootShell queryReference={queryReference} onReloadRequired={handleReloadRequired}>
        {children}
      </MemoOrganizationStoreFrontRootShell>
    </ErrorBoundary>
  );
};

export default memo(OrganizationStoreFrontRootShellWithRelay);
