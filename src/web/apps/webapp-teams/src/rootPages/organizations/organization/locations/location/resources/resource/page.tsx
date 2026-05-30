import { RelayError, toRootError, useKnownParams } from '@skedular/shared';
import { Loading } from '@/components/loading';

import { EditResource } from '@/components/resource/editResource';
import { RootShell } from '@/components/rootShell';
import type { pageOrganizationLocationResource_rootQuery } from '@/queries/__generated__/pageOrganizationLocationResource_rootQuery.graphql';

import { memo, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { v7 as uuid } from 'uuid';

const RootQuery = graphql`
  query pageOrganizationLocationResource_rootQuery(
    $organizationCustomDomain: String!
    $locationId: String!
    $resourceId: String!
    $multipleChoicesCustomTagsSortingValues: [OrganizationTagOrderInput!]
    $multipleChoicesZonesSortingValues: [OrganizationTagOrderInput!]
  ) {
    resource(id: $resourceId) {
      name
    }
    ...editResource_query
  }
`;

type Props = {
  queryReference: PreloadedQuery<pageOrganizationLocationResource_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationCustomDomain: string;
};

const RootPage = ({ queryReference, onReloadRequired, organizationCustomDomain }: Props) => {
  const rootData = usePreloadedQuery<pageOrganizationLocationResource_rootQuery>(RootQuery, queryReference);

  if (!rootData.resource) {
    return null;
  }

  return (
    <RootShell>
      <EditResource rootDataRelay={rootData} onReloadRequired={onReloadRequired} organizationCustomDomain={organizationCustomDomain} />
    </RootShell>
  );
};

const MemoRootPage = memo(RootPage);

const RootPageWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<pageOrganizationLocationResource_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(uuid());
  const [, startTransition] = useTransition();
  const { organizationCustomDomain, locationId, resourceId } = useKnownParams();

  if (!organizationCustomDomain) {
    throw new Error('organizationCustomDomain is required');
  }

  if (!locationId) {
    throw new Error('locationId is required');
  }

  if (!resourceId) {
    throw new Error('resourceId is required');
  }

  useEffect(() => {
    loadQuery(
      {
        organizationCustomDomain,
        locationId,
        resourceId,
        multipleChoicesCustomTagsSortingValues: [
          {
            direction: 'ASCENDING',
            field: 'NAME',
          },
        ],
        multipleChoicesZonesSortingValues: [
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
  }, [loadQuery, triggerReloadId, organizationCustomDomain, locationId, resourceId]);

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
      <MemoRootPage queryReference={queryReference} onReloadRequired={handleReloadRequired} organizationCustomDomain={organizationCustomDomain} />
    </ErrorBoundary>
  );
};

export default memo(RootPageWithRelay);
