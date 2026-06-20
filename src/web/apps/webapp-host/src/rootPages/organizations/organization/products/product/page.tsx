import { EditProduct } from '@/components/product/editProduct';
import { Loading } from '@/components/loading';
import { RootShell } from '@/components/rootShell';
import type { pageOrganizationProduct_rootQuery } from '@/queries/__generated__/pageOrganizationProduct_rootQuery.graphql';
import { RelayError, toRootError, useKnownParams } from '@skedular/shared';
import { memo, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { v7 as uuid } from 'uuid';

const RootQuery = graphql`
  query pageOrganizationProduct_rootQuery($organizationCustomDomain: String!, $productId: String!, $multipleChoicesProductTagsSortingValues: [OrganizationTagOrderInput!]) {
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
  organizationCustomDomain: string;
  onReloadRequired: () => void;
};

const RootPage = ({ queryReference, organizationCustomDomain, onReloadRequired }: Props) => {
  const rootData = usePreloadedQuery<pageOrganizationProduct_rootQuery>(RootQuery, queryReference);
  if (!rootData.product) return null;

  return (
    <RootShell>
      <EditProduct rootDataRelay={rootData} onReloadRequired={onReloadRequired} organizationCustomDomain={organizationCustomDomain} />
    </RootShell>
  );
};

const RootPageWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<pageOrganizationProduct_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(uuid());
  const [, startTransition] = useTransition();
  const { organizationCustomDomain, productId } = useKnownParams();
  if (!organizationCustomDomain || !productId) throw new Error('organizationCustomDomain and productId are required');

  useEffect(() => {
    loadQuery({ organizationCustomDomain, productId, multipleChoicesProductTagsSortingValues: [{ direction: 'ASCENDING', field: 'NAME' }] }, { fetchPolicy: 'store-and-network' });
  }, [loadQuery, organizationCustomDomain, productId, triggerReloadId]);

  if (!queryReference) return <Loading />;
  return (
    <ErrorBoundary fallbackRender={({ error }) => <RelayError error={toRootError(error)} />}>
      <RootPage queryReference={queryReference} organizationCustomDomain={organizationCustomDomain} onReloadRequired={() => startTransition(() => setTriggerReloadId(uuid()))} />
    </ErrorBoundary>
  );
};

export default memo(RootPageWithRelay);
