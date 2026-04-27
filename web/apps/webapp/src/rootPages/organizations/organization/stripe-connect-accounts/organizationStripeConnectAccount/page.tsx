import { BodyIconTypography, StackColumn } from '@skedular/ui';
import { Loading } from '@/components/loading';
import { RelayError, toRootError } from '@/components/relayError';
import { RootShell } from '@/components/rootShell';
import { EditStripeConnectAccount } from '@/components/stripeConnectAccount/editStripeConnectAccount';
import { useKnownParams } from '@skedular/shared';
import type { pageOrganizationStripeConnectAccount_rootQuery } from '@/queries/__generated__/pageOrganizationStripeConnectAccount_rootQuery.graphql';
import { Breadcrumbs } from '@mui/material';
import Button from '@mui/material/Button';
import Box from '@mui/system/Box';
import { useRouter } from 'next/navigation';
import { memo, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { v7 as uuid } from 'uuid';

const RootQuery = graphql`
  query pageOrganizationStripeConnectAccount_rootQuery($organizationStripeConnectAccountId: String!) {
    organizationStripeConnectAccount(id: $organizationStripeConnectAccountId) {
      name
    }
    ...editStripeConnectAccount_query
  }
`;

type Props = {
  queryReference: PreloadedQuery<pageOrganizationStripeConnectAccount_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
};

const RootPage = ({ queryReference, onReloadRequired }: Props) => {
  const rootData = usePreloadedQuery<pageOrganizationStripeConnectAccount_rootQuery>(RootQuery, queryReference);
  const router = useRouter();

  const handleBackClick = () => {
    router.back();
  };

  if (!rootData.organizationStripeConnectAccount) {
    return null;
  }

  const breadcrumbs = (
    <StackColumn sx={{ alignItems: 'flex-start' }} spacing={0}>
      <Button variant="text" onClick={handleBackClick} sx={{ whiteSpace: 'nowrap', textTransform: 'none' }}>
        {'< back'}
      </Button>
      <Box sx={{ display: { xs: 'none', sm: 'block' } }}>
        <Breadcrumbs>
          <BodyIconTypography label="Stripe Connect Account" />
          <BodyIconTypography label={rootData.organizationStripeConnectAccount.name} />
        </Breadcrumbs>
      </Box>
    </StackColumn>
  );

  return (
    <RootShell collapsed hideOrganizationSelector hideWelcomeMessage showBreadcrumps breadcrumbs={breadcrumbs}>
      <EditStripeConnectAccount rootDataRelay={rootData} onReloadRequired={onReloadRequired} />
    </RootShell>
  );
};

const MemoRootPage = memo(RootPage);

const RootPageWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<pageOrganizationStripeConnectAccount_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(uuid());
  const [, startTransition] = useTransition();
  const { organizationStripeConnectAccountId } = useKnownParams();

  if (!organizationStripeConnectAccountId) {
    throw new Error('organizationStripeConnectAccountId is required');
  }

  useEffect(() => {
    loadQuery(
      {
        organizationStripeConnectAccountId,
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId, organizationStripeConnectAccountId]);

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
      <MemoRootPage queryReference={queryReference} onReloadRequired={handleReloadRequired} />
    </ErrorBoundary>
  );
};

export default memo(RootPageWithRelay);
