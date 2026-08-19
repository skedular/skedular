import { NoOrganizationAppBar } from '@/components/appBar';
import { SignOutIcon } from '@/components/icons';
import { getSignOutReturnToLink } from '@/components/links';
import { Loading } from '@/components/loading';
import { Observability } from '@/components/observability';
import { RelayError, toRootError } from '@skedular/shared';

import type { noOrganizationRootShell_rootQuery } from '@/queries/__generated__/noOrganizationRootShell_rootQuery.graphql';
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
  queryReference: PreloadedQuery<noOrganizationRootShell_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
};

const RootQuery = graphql`
  query noOrganizationRootShell_rootQuery {
    me {
      id
    }
    customerReadinessSynced
    ...noOrganizationAppBar_query
    ...observability_query
  }
`;

const maxRetryAttemptsToReload = 20;

const NoOrganizationRootShell = ({ queryReference, children, onReloadRequired }: PropsWithChildren<Props>) => {
  const rootData = usePreloadedQuery<noOrganizationRootShell_rootQuery>(RootQuery, queryReference);
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
        <Box sx={{ flexGrow: 1 }}>
          <NoOrganizationAppBar rootDataRelay={rootData} showLogo />
          {children}
        </Box>
      </Box>
    </>
  );
};

const MemoNoOrganizationRootShell = memo(NoOrganizationRootShell);

const NoOrganizationRootShellWithRelay = ({ children }: PropsWithChildren) => {
  const [queryReference, loadQuery] = useQueryLoader<noOrganizationRootShell_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(uuid());
  const [, startTransition] = useTransition();

  useEffect(() => {
    loadQuery(
      {},
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId]);

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
      <MemoNoOrganizationRootShell queryReference={queryReference} onReloadRequired={handleReloadRequired}>
        {children}
      </MemoNoOrganizationRootShell>
    </ErrorBoundary>
  );
};

export default memo(NoOrganizationRootShellWithRelay);
