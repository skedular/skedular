import { GridContainer, PushToRight, SectionIconTypography, StackColumn } from '@/components/commons';
import { Loading } from '@/components/loading';
import NewProductButton from '@/components/product/addProduct/new-product-button';
import { RelayError, toRootError } from '@/components/relayError';
import { defaultPadding, maxScreenWidth } from '@/libs/theme';
import type { organizationProducts_rootQuery } from '@/queries/__generated__/organizationProducts_rootQuery.graphql';
import Divider from '@mui/material/Divider';
import Grid from '@mui/material/Grid';
import Box from '@mui/system/Box';
import { memo, useEffect, useMemo, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { v7 as uuid } from 'uuid';
import ProductCard from './product-card';

type Props = {
  queryReference: PreloadedQuery<organizationProducts_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationCustomDomain: string;
};

const RootQuery = graphql`
  query organizationProducts_rootQuery($organizationCustomDomain: String!, $productsSortingValues: [ProductOrderInput!]) {
    products(where: { organizationCustomDomains: [$organizationCustomDomain], includeInactive: true }, orderBy: $productsSortingValues) {
      __id
      totalCount
      edges {
        node {
          id
          listingMetadata {
            title
          }
          organization {
            id
          }
          ...productCard_ProductDetails
        }
      }
    }
    ...productCard_query
  }
`;

const OrganizationProducts = ({ queryReference, onReloadRequired, organizationCustomDomain }: Props) => {
  const rootData = usePreloadedQuery<organizationProducts_rootQuery>(RootQuery, queryReference);
  const connectionIds = useMemo(() => [rootData.products.__id], [rootData.products]);
  const products = useMemo(() => rootData.products.edges.map((edge) => edge.node), [rootData.products]);

  if (!rootData.products) {
    return null;
  }

  return (
    <StackColumn sx={{ maxWidth: maxScreenWidth }}>
      <GridContainer spacing={1} sx={{ padding: defaultPadding }}>
        <PushToRight />
        <NewProductButton organizationCustomDomain={organizationCustomDomain} />
      </GridContainer>
      <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
        <SectionIconTypography label="Products" />
        <Divider />
        <Box sx={{ paddingBottom: defaultPadding }} />

        <GridContainer>
          {products.map((product) => (
            <Grid key={product.id}>
              <ProductCard
                rootDataRelay={rootData}
                productDetailsRelay={product}
                onReloadRequired={onReloadRequired}
                organizationCustomDomain={organizationCustomDomain}
                connectionIds={connectionIds}
              />
            </Grid>
          ))}
        </GridContainer>
      </StackColumn>
    </StackColumn>
  );
};

const MemoOrganizationProducts = memo(OrganizationProducts);

type RelayProps = {
  organizationCustomDomain: string;
};

const OrganizationProductsWithRelay = ({ organizationCustomDomain }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<organizationProducts_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(uuid());
  const [, startTransition] = useTransition();

  useEffect(() => {
    loadQuery(
      {
        organizationCustomDomain,
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
  }, [loadQuery, triggerReloadId, organizationCustomDomain]);

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
      <MemoOrganizationProducts queryReference={queryReference} onReloadRequired={handleReloadRequired} organizationCustomDomain={organizationCustomDomain} />
    </ErrorBoundary>
  );
};

export default memo(OrganizationProductsWithRelay);
