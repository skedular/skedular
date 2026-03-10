import { OrganizationStoreFrontAppBar } from '@/components/appBar';
import { SmallHeadingIconTypography } from '@/components/commons';
import { SignOutIcon } from '@/components/icons';
import { getRootLink } from '@/components/links';
import { Loading } from '@/components/loading';
import { Observability } from '@/components/observability';
import { RelayError, toRootError } from '@/components/relayError';
import { useIntegratedPlatrform, useKnownParams } from '@/libs/providers';
import type { organizationStoreFrontRootShell_rootQuery } from '@/queries/__generated__/organizationStoreFrontRootShell_rootQuery.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import CssBaseline from '@mui/material/CssBaseline';
import { useAuth } from '@workos-inc/authkit-nextjs/components';
import { useRouter } from 'next/navigation';
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
  query organizationStoreFrontRootShell_rootQuery($organizationUniqueAlphanumericName: String!) {
    me {
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
    ...organizationStoreFrontAppBar_query
    ...observability_query
  }
`;

const maxRetryAttemptsToReload = 20;

const OrganizationStoreFrontRootShell = ({ queryReference, children, onReloadRequired }: PropsWithChildren<Props>) => {
  const rootData = usePreloadedQuery<organizationStoreFrontRootShell_rootQuery>(RootQuery, queryReference);
  const { integratedPlatrform } = useIntegratedPlatrform();
  const router = useRouter();
  const { signOut } = useAuth();
  const [reloadCount, setReloadCount] = useState(0);
  const rootLink = getRootLink(integratedPlatrform);
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
        <Box sx={{ flexGrow: 1 }}>
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
  const router = useRouter();
  const { organizationUniqueAlphanumericName } = useKnownParams();

  if (!organizationUniqueAlphanumericName) {
    throw new Error('organizationUniqueAlphanumericName is required');
  }

  useEffect(() => {
    loadQuery(
      { organizationUniqueAlphanumericName },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId, organizationUniqueAlphanumericName, router]);

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
      <MemoOrganizationStoreFrontRootShell queryReference={queryReference} onReloadRequired={handleReloadRequired}>
        {children}
      </MemoOrganizationStoreFrontRootShell>
    </ErrorBoundary>
  );
};

export default memo(OrganizationStoreFrontRootShellWithRelay);
