'use client';

import { BodyIconTypography, StackColumn } from '@/components/commons';
import { Loading } from '@/components/loading';
import type { RootError } from '@/components/relayError';
import { RelayError } from '@/components/relayError';
import { RootShell } from '@/components/rootShell';
import { EditStripeConnectAccount } from '@/components/stripeConnectAccount/editStripeConnectAccount';
import type { pageOrganizationStripeConnectAccount_rootQuery } from '@/queries/__generated__/pageOrganizationStripeConnectAccount_rootQuery.graphql';
import { Breadcrumbs } from '@mui/material';
import Button from '@mui/material/Button';
import Box from '@mui/system/Box';
import { nanoid } from 'nanoid';
import { useParams, useRouter } from 'next/navigation';
import { memo, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';

const RootQuery = graphql`
  query pageOrganizationStripeConnectAccount_rootQuery(
    $organizationStripeConnectAccountId: String!
  ) {
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

const ProductPage = ({ queryReference, onReloadRequired }: Props) => {
  const rootData = usePreloadedQuery<pageOrganizationStripeConnectAccount_rootQuery>(RootQuery, queryReference);
  const router = useRouter();

  const handleBackClick = () => {
    router.back();
  };

  if (!rootData.organizationStripeConnectAccount) {
    return <></>;
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
      <EditStripeConnectAccount rootDataRelay={rootData} onReloadRequired={onReloadRequired}  />
    </RootShell>
  );
};

const MemoProductPage = memo(ProductPage);

const ProductPageWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<pageOrganizationStripeConnectAccount_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(nanoid());
  const [, startTransition] = useTransition();
  const { organizationStripeConnectAccountId } = useParams();
  let finalOrganizationStripeConnectAccountId = '';

  if (typeof organizationStripeConnectAccountId === 'string') {
    finalOrganizationStripeConnectAccountId = organizationStripeConnectAccountId;
  } else if (Array.isArray(organizationStripeConnectAccountId)) {
    if (typeof organizationStripeConnectAccountId[0] === 'undefined') {
      throw new Error('organizationStripeConnectAccountId is required');
    }

    finalOrganizationStripeConnectAccountId = organizationStripeConnectAccountId[0];
  } else {
    throw new Error('organizationStripeConnectAccountId is required');
  }

  useEffect(() => {
    loadQuery(
      {
        organizationStripeConnectAccountId: finalOrganizationStripeConnectAccountId,
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId,  finalOrganizationStripeConnectAccountId]);

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
      <MemoProductPage queryReference={queryReference} onReloadRequired={handleReloadRequired} />
    </ErrorBoundary>
  );
};

export default memo(ProductPageWithRelay);
