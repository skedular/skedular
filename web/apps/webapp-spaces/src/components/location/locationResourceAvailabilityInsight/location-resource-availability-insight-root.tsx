import { AnalyticsInsightCard } from '@/components/analytics';
import { RelayError, toRootError } from '@/components/relayError';
import type { locationResourceAvailabilityInsightRoot_rootQuery } from '@/queries/__generated__/locationResourceAvailabilityInsightRoot_rootQuery.graphql';
import Skeleton from '@mui/material/Skeleton';
import { startOfDay } from '@skedular/shared';
import { memo, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { type PreloadedQuery, graphql, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { v7 as uuid } from 'uuid';
import LocationResourceAvailabilityInsight from './location-resource-availability-insight';

type Props = {
  queryReference: PreloadedQuery<locationResourceAvailabilityInsightRoot_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  resourceType?: string;
};

const RootQuery = graphql`
  query locationResourceAvailabilityInsightRoot_rootQuery($locationId: String!, $from: DateTime!, $to: DateTime!) {
    ...locationResourceAvailabilityInsight_locationAnalytics_query
  }
`;

const LocationResourceAvailabilityInsightRoot = ({ queryReference, resourceType }: Props) => {
  const rootData = usePreloadedQuery<locationResourceAvailabilityInsightRoot_rootQuery>(RootQuery, queryReference);

  return <LocationResourceAvailabilityInsight rootDataLocationAnalyticsRelay={rootData} resourceType={resourceType} />;
};

const MemoLocationResourceAvailabilityInsightRoot = memo(LocationResourceAvailabilityInsightRoot);

type RelayProps = {
  onReloadRequired: () => void;
  locationId: string;
  resourceType?: string;
};

const LocationResourceAvailabilityInsightRootWithRelay = ({ onReloadRequired, locationId, resourceType }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<locationResourceAvailabilityInsightRoot_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(uuid());
  const [, startTransition] = useTransition();

  useEffect(() => {
    const to = startOfDay();
    const from = to.subtract(6, 'months');

    loadQuery(
      {
        locationId,
        from: from.toISOString(),
        to: to.toISOString(),
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId, locationId]);

  const handleReloadRequired = () => {
    startTransition(() => {
      setTriggerReloadId(uuid());

      onReloadRequired();
    });
  };

  if (!queryReference) {
    return (
      <AnalyticsInsightCard title="Resource Availability Insights">
        <Skeleton variant="rounded" width="100%" height={350} />
      </AnalyticsInsightCard>
    );
  }

  return (
    <ErrorBoundary fallbackRender={({ error }) => <RelayError error={toRootError(error)} />}>
      <MemoLocationResourceAvailabilityInsightRoot queryReference={queryReference} onReloadRequired={handleReloadRequired} resourceType={resourceType} />
    </ErrorBoundary>
  );
};

export default memo(LocationResourceAvailabilityInsightRootWithRelay);
