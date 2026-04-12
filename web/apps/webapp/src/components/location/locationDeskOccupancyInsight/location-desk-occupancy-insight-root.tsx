import { AnalyticsInsightCard } from '@/components/analytics';
import { RelayError, toRootError } from '@/components/relayError';
import { startOfDay } from '@/libs/utils';
import type { locationDeskOccupancyInsightRoot_rootQuery } from '@/queries/__generated__/locationDeskOccupancyInsightRoot_rootQuery.graphql';
import Skeleton from '@mui/material/Skeleton';
import { memo, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, graphql, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { v7 as uuid } from 'uuid';
import LocationDeskOccupancyInsight from './location-desk-occupancy-insight';

type Props = {
  queryReference: PreloadedQuery<locationDeskOccupancyInsightRoot_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
};

const RootQuery = graphql`
  query locationDeskOccupancyInsightRoot_rootQuery($locationId: String!, $from: DateTime!, $to: DateTime!) {
    ...locationDeskOccupancyInsight_query
    ...locationDeskOccupancyInsight_locationAnalytics_query
  }
`;

const LocationDeskOccupancyInsightRoot = ({ queryReference }: Props) => {
  const rootData = usePreloadedQuery<locationDeskOccupancyInsightRoot_rootQuery>(RootQuery, queryReference);

  return <LocationDeskOccupancyInsight rootDataRelay={rootData} rootDataLocationAnalyticsRelay={rootData} />;
};

const MemoLocationDeskOccupancyInsightRoot = memo(LocationDeskOccupancyInsightRoot);

type RelayProps = {
  onReloadRequired: () => void;
  locationId: string;
};

const LocationDeskOccupancyInsightRootWithRelay = ({ onReloadRequired, locationId }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<locationDeskOccupancyInsightRoot_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(uuid());
  const [, startTransition] = useTransition();

  useEffect(() => {
    const to = startOfDay();
    const from = to.subtract(30, 'days');

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
      <AnalyticsInsightCard title="Desk Occupancy Insights">
        <Skeleton variant="rounded" width="100%" height={350} />
      </AnalyticsInsightCard>
    );
  }

  return (
    <ErrorBoundary fallbackRender={({ error }) => <RelayError error={toRootError(error)} />}>
      <MemoLocationDeskOccupancyInsightRoot queryReference={queryReference} onReloadRequired={handleReloadRequired} />
    </ErrorBoundary>
  );
};

export default memo(LocationDeskOccupancyInsightRootWithRelay);
