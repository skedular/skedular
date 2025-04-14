import { GridContainer, SectionIconTypography, StackColumn } from '@/components/commons';
import { Loading } from '@/components/loading';
import type { RootError } from '@/components/relayError';
import { RelayError } from '@/components/relayError';
import { defaultPadding, maxScreenWidth } from '@/libs/theme';
import { startOfDay } from '@/libs/utils';
import type { organizationMarketplacePublic_rootQuery } from '@/queries/__generated__/organizationMarketplacePublic_rootQuery.graphql';
import Divider from '@mui/material/Divider';
import Grid from '@mui/material/Grid';
import Box from '@mui/system/Box';
import { nanoid } from 'nanoid';
import { memo, useEffect, useMemo, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import ProductCard from './product-card';

type Props = {
  queryReference: PreloadedQuery<organizationMarketplacePublic_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationId: string;
};

const RootQuery = graphql`
  query organizationMarketplacePublic_rootQuery($organizationId: String!, $productsSortingValues: [ProductOrderInput!]) {
    products(where: { organizationIds: [$organizationId], includeInactive: false }, orderBy: $productsSortingValues) {
      __id
      totalCount
      edges {
        node {
          id
          name
          organization {
            uniqueId
          }
          ...productCard_ProductDetails
        }
      }
    }
  }
`;

const OrganizationMarketplacePublic = ({ queryReference, onReloadRequired, organizationId }: Props) => {
  const rootData = usePreloadedQuery<organizationMarketplacePublic_rootQuery>(RootQuery, queryReference);
  const products = useMemo(() => rootData.products.edges.map((edge) => edge.node).sort((a, b) => a.name.localeCompare(b.name)), [rootData.products]);

  if (!rootData.products) {
    return <></>;
  }

  return (
    <>
      <StackColumn sx={{ maxWidth: maxScreenWidth }}>
        <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
          <SectionIconTypography label="Products" />
          <Divider />
          <Box sx={{ paddingBottom: defaultPadding }} />

          <GridContainer>
            {products.map((product) => (
              <Grid key={product.id}>
                <ProductCard rootDataRelay={product} onReloadRequired={onReloadRequired} organizationId={organizationId} />
              </Grid>
            ))}
          </GridContainer>
        </StackColumn>
      </StackColumn>
    </>
  );
};

const MemoOrganizationMarketplacePublic = memo(OrganizationMarketplacePublic);

type RelayProps = {
  organizationId: string;
};

const OrganizationMarketplacePublicWithRelay = ({ organizationId }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<organizationMarketplacePublic_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(nanoid());
  const [, startTransition] = useTransition();

  useEffect(() => {
    const today = startOfDay();

    loadQuery(
      {
        organizationId,
        productsSortingValues: [
          {
            direction: 'Ascending',
            field: 'Name',
          },
        ],
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId, organizationId]);

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
      <MemoOrganizationMarketplacePublic queryReference={queryReference} onReloadRequired={handleReloadRequired} organizationId={organizationId} />
    </ErrorBoundary>
  );
};

export default memo(OrganizationMarketplacePublicWithRelay);
