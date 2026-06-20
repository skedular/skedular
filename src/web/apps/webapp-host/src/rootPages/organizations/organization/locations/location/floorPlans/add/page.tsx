import { RelayError, toRootError, useKnownParams } from '@skedular/shared';
import { AddFloorPlan } from '@/components/floorPlan/addFloorPlan';
import { Loading } from '@/components/loading';

import { RootShell } from '@/components/rootShell';

import type { pageOrganizationLocationFloorPlansAdd_rootQuery } from '@/queries/__generated__/pageOrganizationLocationFloorPlansAdd_rootQuery.graphql';
import { useRouter } from 'next/navigation';
import { memo, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { graphql, PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { v7 as uuid } from 'uuid';

const RootQuery = graphql`
  query pageOrganizationLocationFloorPlansAdd_rootQuery($locationId: String!) {
    location(id: $locationId) {
      name
    }
  }
`;

type Props = {
  queryReference: PreloadedQuery<pageOrganizationLocationFloorPlansAdd_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  locationId: string;
};

const RootPage = ({ queryReference, locationId }: Props) => {
  const rootData = usePreloadedQuery<pageOrganizationLocationFloorPlansAdd_rootQuery>(RootQuery, queryReference);
  const router = useRouter();

  const handleAdded = () => {
    router.back();
  };

  const handleCancelled = () => {
    router.back();
  };

  const handleReloadRequired = () => {};

  if (!rootData.location) {
    return null;
  }

  return (
    <RootShell>
      <AddFloorPlan locationId={locationId} showDismiss={false} onAdded={handleAdded} onCancel={handleCancelled} onReloadRequired={handleReloadRequired} />
    </RootShell>
  );
};

const MemoRootPage = memo(RootPage);

const RootPageWithRelay = () => {
  const [queryReference, loadQuery] = useQueryLoader<pageOrganizationLocationFloorPlansAdd_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(uuid());
  const [, startTransition] = useTransition();
  const { locationId } = useKnownParams();

  if (!locationId) {
    throw new Error('organizationStripeConnectAccountId is required');
  }

  useEffect(() => {
    loadQuery(
      {
        locationId,
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId, locationId]);

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
      <MemoRootPage queryReference={queryReference} onReloadRequired={handleReloadRequired} locationId={locationId} />
    </ErrorBoundary>
  );
};

export default memo(RootPageWithRelay);
