import { BodyIconTypography, StackColumn } from '@/components/commons';
import { Loading } from '@/components/loading';
import { EditProduct } from '@/components/product/editProduct';
import { RelayError, toRootError } from '@/components/relayError';
import { RootShell } from '@/components/rootShell';
import { useKnownParams } from '@/libs/providers';
import type { pageOrganizationProduct_rootQuery } from '@/queries/__generated__/pageOrganizationProduct_rootQuery.graphql';
import { Breadcrumbs } from '@mui/material';
import Button from '@mui/material/Button';
import Box from '@mui/system/Box';
import { useRouter } from 'next/navigation';
import { memo, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { v7 as uuid } from 'uuid';

const RootQuery = graphql`
  query pageOrganizationProduct_rootQuery(
    $organizationUniqueAlphanumericName: String!
    $productId: String!
    $multipleChoicesProductTagsSortingValues: [OrganizationTagOrderInput!]
  ) {
    product(id: $productId) {
      listingMetadata {
        title
      }
    }
    ...editProduct_query
  }
`;

type Props = {
  queryReference: PreloadedQuery<pageOrganizationProduct_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationUniqueAlphanumericName: string;
};

const RootPage = ({ queryReference, onReloadRequired, organizationUniqueAlphanumericName }: Props) => {
  const rootData = usePreloadedQuery<pageOrganizationProduct_rootQuery>(RootQuery, queryReference);
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
      <EditProduct rootDataRelay={rootData} onReloadRequired={onReloadRequired} organizationUniqueAlphanumericName={organizationUniqueAlphanumericName} />
    </RootShell>
  );
};

const MemoRootPage = memo(RootPage);

const RootPageWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<pageOrganizationProduct_rootQuery>(RootQuery);
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
    loadQuery(
      {
        organizationUniqueAlphanumericName,
        productId,
        multipleChoicesProductTagsSortingValues: [
          {
            direction: 'ASCENDING',
            field: 'NAME',
          },
        ],
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
