import { BodyIconTypography, StackColumn } from '@/components/commons';
import { Loading } from '@/components/loading';
import BookProduct from '@/components/product/bookProduct/book-product';
import { RelayError, toRootError } from '@/components/relayError';
import { RootShell } from '@/components/rootShell';
import { useKnownParams } from '@/libs/providers';
import { startOfDay } from '@/libs/utils';
import type { pageOrganizationProductBook_rootQuery } from '@/queries/__generated__/pageOrganizationProductBook_rootQuery.graphql';
import { Breadcrumbs } from '@mui/material';
import Button from '@mui/material/Button';
import Box from '@mui/system/Box';
import { useRouter } from 'next/navigation';
import { memo, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { v7 as uuid } from 'uuid';

const RootQuery = graphql`
  query pageOrganizationProductBook_rootQuery(
    $organizationUniqueAlphanumericName: String!
    $productId: String!
    $dateFromToGetAvailableResources: DateTime!
    $dateUntilToGetAvailableResources: DateTime!
  ) {
    product(id: $productId) {
      listingMetadata {
        title
      }
    }
    ...bookProduct_query
    ...bookProduct_availableResources_query
  }
`;

type Props = {
  queryReference: PreloadedQuery<pageOrganizationProductBook_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationUniqueAlphanumericName: string;
};

const RootPage = ({ queryReference, onReloadRequired, organizationUniqueAlphanumericName }: Props) => {
  const rootData = usePreloadedQuery<pageOrganizationProductBook_rootQuery>(RootQuery, queryReference);
  const router = useRouter();

  const handleBackClick = () => {
    router.back();
  };

  if (!rootData.product) {
    return null;
  }

  const breadcrumbs = (
    <StackColumn sx={{ alignItems: 'flex-start' }} spacing={0}>
      <Button variant="text" onClick={handleBackClick} sx={{ whiteSpace: 'nowrap', textTransform: 'none' }}>
        {'< back'}
      </Button>
      <Box sx={{ display: { xs: 'none', sm: 'block' } }}>
        <Breadcrumbs>
          <BodyIconTypography label="Product" />
          <BodyIconTypography label={rootData.product.listingMetadata.title} />
        </Breadcrumbs>
      </Box>
    </StackColumn>
  );

  return (
    <RootShell collapsed hideOrganizationSelector hideWelcomeMessage showBreadcrumps breadcrumbs={breadcrumbs}>
      <BookProduct
        rootDataRelay={rootData}
        rootDataAvailableResourcesRelay={rootData}
        onReloadRequired={onReloadRequired}
        connectionIds={[]}
        organizationUniqueAlphanumericName={organizationUniqueAlphanumericName}
      />
    </RootShell>
  );
};

const MemoRootPage = memo(RootPage);

const RootPageWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<pageOrganizationProductBook_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(uuid());
  const [, startTransition] = useTransition();
  const { organizationUniqueAlphanumericName, productId } = useKnownParams();

  if (!organizationUniqueAlphanumericName) {
    throw new Error('organizationUniqueAlphanumericName is required');
  }

  if (!productId) {
    throw new Error('productId is required');
  }

  useEffect(() => {
    const date = startOfDay().add(8, 'hour');
    const startDate = date.toISOString();
    const endDate = date.add(9, 'hour').toISOString();

    loadQuery(
      {
        organizationUniqueAlphanumericName,
        productId,
        dateFromToGetAvailableResources: startDate,
        dateUntilToGetAvailableResources: endDate,
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId, organizationUniqueAlphanumericName, productId]);

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
      <MemoRootPage queryReference={queryReference} onReloadRequired={handleReloadRequired} organizationUniqueAlphanumericName={organizationUniqueAlphanumericName} />
    </ErrorBoundary>
  );
};

export default memo(RootPageWithRelay);
