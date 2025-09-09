import { GridContainer, SectionIconTypography, StackColumn } from '@/components/commons';
import { Loading } from '@/components/loading';
import type { RootError } from '@/components/relayError';
import { RelayError } from '@/components/relayError';
import { defaultPadding, maxScreenWidth } from '@/libs/theme';
import type { organizationMarketplacePublic_rootQuery } from '@/queries/__generated__/organizationMarketplacePublic_rootQuery.graphql';
import Divider from '@mui/material/Divider';
import Grid from '@mui/material/Grid';
import Box from '@mui/system/Box';
import { memo, useEffect, useMemo, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { v7 as uuid } from 'uuid';
import ProductCard from './product-card';

type Props = {
  queryReference: PreloadedQuery<organizationMarketplacePublic_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationUniqueAlphanumericName: string;
};

const RootQuery = graphql`
  query organizationMarketplacePublic_rootQuery($organizationUniqueAlphanumericName: String!, $productsSortingValues: [ProductOrderInput!]) {
    products(where: { organizationUniqueAlphanumericNames: [$organizationUniqueAlphanumericName], includeInactive: false }, orderBy: $productsSortingValues) {
      __id
      totalCount
      edges {
        node {
          id
          name
          organization {
            id
          }
          ...productCard_ProductDetails
        }
      }
    }
  }
`;

const OrganizationMarketplacePublic = ({ queryReference, onReloadRequired, organizationUniqueAlphanumericName }: Props) => {
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
                <ProductCard rootDataRelay={product} onReloadRequired={onReloadRequired} organizationUniqueAlphanumericName={organizationUniqueAlphanumericName} />
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
  organizationUniqueAlphanumericName: string;
};

const OrganizationMarketplacePublicWithRelay = ({ organizationUniqueAlphanumericName }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<organizationMarketplacePublic_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(uuid());
  const [, startTransition] = useTransition();

  useEffect(() => {
    loadQuery(
      {
        organizationUniqueAlphanumericName,
        productsSortingValues: [
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
  }, [loadQuery, triggerReloadId, organizationUniqueAlphanumericName]);

  const handleReloadRequired = () => {
    startTransition(() => {
      setTriggerReloadId(uuid());
    });
  };

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }: { error: RootError }) => <RelayError error={error} />}>
      <MemoOrganizationMarketplacePublic
        queryReference={queryReference}
        onReloadRequired={handleReloadRequired}
        organizationUniqueAlphanumericName={organizationUniqueAlphanumericName}
      />
    </ErrorBoundary>
  );
};

export default memo(OrganizationMarketplacePublicWithRelay);
