import { Loading } from '@/components/loading';
import { MarketplaceLocations } from '@/components/location/marketplaceLocations';
import type { RootError } from '@/components/relayError';
import { RelayError } from '@/components/relayError';
import { NoOrganizationRootShell } from '@/components/rootShell';
import type { pageHome_rootQuery } from '@/queries/__generated__/pageHome_rootQuery.graphql';
import { memo, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { v7 as uuid } from 'uuid';

type Props = {
  queryReference: PreloadedQuery<pageHome_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
};

const RootQuery = graphql`
  query pageHome_rootQuery($searchBoundaries: PolygonInput, $locationsSortingValues: [LocationOrderInput!]) {
    ...marketplaceLocations_locations_query
  }
`;

const RootPage = ({ queryReference, onReloadRequired }: Props) => {
  const rootData = usePreloadedQuery<pageHome_rootQuery>(RootQuery, queryReference);

  return (
    <NoOrganizationRootShell collapsed={true}>
      <MarketplaceLocations rootDataRelay={rootData} onReloadRequired={onReloadRequired} />
    </NoOrganizationRootShell>
  );
};

const MemoRootPage = memo(RootPage);

const RootPageWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<pageHome_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(uuid());
  const [, startTransition] = useTransition();

  useEffect(() => {
    loadQuery(
      {
        locationsSortingValues: [
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
  }, [loadQuery, triggerReloadId]);

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
      <MemoRootPage queryReference={queryReference} onReloadRequired={handleReloadRequired} />
    </ErrorBoundary>
  );
};

export default memo(RootPageWithRelay);
