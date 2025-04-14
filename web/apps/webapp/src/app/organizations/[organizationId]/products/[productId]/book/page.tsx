'use client';

import { BodyIconTypography, StackColumn } from '@/components/commons';
import { Loading } from '@/components/loading';
import BookProduct from '@/components/product/bookProduct/book-product';
import type { RootError } from '@/components/relayError';
import { RelayError } from '@/components/relayError';
import { RootShell } from '@/components/rootShell';
import { startOfDay } from '@/libs/utils';
import type { pageOrganizationProductBook_rootQuery } from '@/queries/__generated__/pageOrganizationProductBook_rootQuery.graphql';
import { Breadcrumbs } from '@mui/material';
import Button from '@mui/material/Button';
import Box from '@mui/system/Box';
import { nanoid } from 'nanoid';
import { useParams, useRouter } from 'next/navigation';
import { memo, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';

const RootQuery = graphql`
  query pageOrganizationProductBook_rootQuery($productId: String!) {
    product(id: $productId) {
      name
    }
    ...bookProduct_query
  }
`;

type Props = {
  queryReference: PreloadedQuery<pageOrganizationProductBook_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationId: string;
  productId: string;
};

const OrganizationProductBookPage = ({ queryReference, onReloadRequired, organizationId, productId }: Props) => {
  const rootData = usePreloadedQuery<pageOrganizationProductBook_rootQuery>(RootQuery, queryReference);
  const router = useRouter();

  const handleBackClick = () => {
    router.back();
  };

  if (!rootData.product) {
    return <></>;
  }

  const breadcrumbs = (
    <StackColumn sx={{ alignItems: 'flex-start' }} spacing={0}>
      <Button variant="text" onClick={handleBackClick} sx={{ whiteSpace: 'nowrap', textTransform: 'none' }}>
        {'< back'}
      </Button>
      <Box sx={{ display: { xs: 'none', sm: 'block' } }}>
        <Breadcrumbs>
          <BodyIconTypography label="Product" />
          <BodyIconTypography label={rootData.product.name} />
        </Breadcrumbs>
      </Box>
    </StackColumn>
  );

  return (
    <RootShell collapsed hideOrganizationSelector hideWelcomeMessage showBreadcrumps breadcrumbs={breadcrumbs}>
      <BookProduct rootDataRelay={rootData} onReloadRequired={onReloadRequired} organizationId={organizationId} />
    </RootShell>
  );
};

const MemoOrganizationProductBookPage = memo(OrganizationProductBookPage);

const OrganizationProductBookPageWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<pageOrganizationProductBook_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(nanoid());
  const [, startTransition] = useTransition();
  const { organizationId, productId } = useParams();
  let finalOrganizationId = '';

  if (typeof organizationId === 'string') {
    finalOrganizationId = organizationId;
  } else if (Array.isArray(organizationId)) {
    if (typeof organizationId[0] === 'undefined') {
      throw new Error('organizationId is required');
    }

    finalOrganizationId = organizationId[0];
  } else {
    throw new Error('organizationId is required');
  }

  let finalProductId = '';

  if (typeof productId === 'string') {
    finalProductId = productId;
  } else if (Array.isArray(productId)) {
    if (typeof productId[0] === 'undefined') {
      throw new Error('productId is required');
    }

    finalProductId = productId[0];
  } else {
    throw new Error('productId is required');
  }

  useEffect(() => {
    const date = startOfDay();
    const startDate = date.toISOString();
    const endDate = date.add(1, 'day').toISOString();

    loadQuery(
      {
        productId: finalProductId,
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId, finalOrganizationId, finalProductId]);

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
      <MemoOrganizationProductBookPage queryReference={queryReference} onReloadRequired={handleReloadRequired} organizationId={finalOrganizationId} productId={finalProductId} />
    </ErrorBoundary>
  );
};

export default memo(OrganizationProductBookPageWithRelay);
