import { Loading } from '@/components/loading';
import { RelayError, toRootError } from '@skedular/shared';
import type { organizationProducts_rootQuery } from '@/queries/__generated__/organizationProducts_rootQuery.graphql';
import { memo, useEffect, useMemo, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { v7 as uuid } from 'uuid';
import OrganizationProductsPageShell from './organization-products-page-shell';
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
          ...productCard_ProductDetails
        }
      }
    }
    ...productCard_query
  }
`;

const OrganizationProducts = ({ queryReference, organizationCustomDomain }: Props) => {
  const rootData = usePreloadedQuery<organizationProducts_rootQuery>(RootQuery, queryReference);
  const connectionIds = useMemo(() => [rootData.products.__id], [rootData.products]);
  const products = useMemo(() => rootData.products.edges.map((edge) => edge.node), [rootData.products]);

  if (!rootData.products) {
    return null;
  }

  return (
    <OrganizationProductsPageShell isEmpty={products.length === 0}>
      {products.map((product) => (
        <ProductCard key={product.id} rootDataRelay={rootData} productDetailsRelay={product} organizationCustomDomain={organizationCustomDomain} connectionIds={connectionIds} />
      ))}
    </OrganizationProductsPageShell>
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
