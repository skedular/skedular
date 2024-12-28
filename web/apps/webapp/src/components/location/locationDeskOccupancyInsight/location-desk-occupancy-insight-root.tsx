import type { locationDeskOccupancyInsightRoot_rootQuery } from '@/queries/__generated__/locationDeskOccupancyInsightRoot_rootQuery.graphql';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import CardHeader from '@mui/material/CardHeader';
import Skeleton from '@mui/material/Skeleton';
import { SectionIconTypography } from '@repo/shared/components/commons';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { startOfDay } from '@repo/shared/libs/utils';
import { nanoid } from 'nanoid';
import { memo, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, graphql, usePreloadedQuery, useQueryLoader } from 'react-relay';
import LocationDeskOccupancyInsight from './location-desk-occupancy-insight';

type Props = {
  queryReference: PreloadedQuery<locationDeskOccupancyInsightRoot_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  hideLocationDetails?: boolean;
};

const RootQuery = graphql`
  query locationDeskOccupancyInsightRoot_rootQuery($locationId: String!, $from: DateTime!, $to: DateTime!) {
    ...locationDeskOccupancyInsight_query
    ...locationDeskOccupancyInsight_locationAnalytics_query
  }
`;

const LocationDeskOccupancyInsightRoot = ({ queryReference, hideLocationDetails }: Props) => {
  const rootData = usePreloadedQuery<locationDeskOccupancyInsightRoot_rootQuery>(RootQuery, queryReference);

  return (
    <LocationDeskOccupancyInsight rootDataRelay={rootData} rootDataLocationAnalyticsRelay={rootData} hideLocationDetails={hideLocationDetails} />
  );
};

const MemoLocationDeskOccupancyInsightRoot = memo(LocationDeskOccupancyInsightRoot);

type RelayProps = {
  onReloadRequired: () => void;
  organizationId?: string;
  locationId: string;
  locationName?: string;
  hideLocationDetails?: boolean;
};

const LocationDeskOccupancyInsightRootWithRelay = ({
  organizationId,
  onReloadRequired,
  locationId,
  locationName,
  hideLocationDetails,
}: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<locationDeskOccupancyInsightRoot_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(nanoid());
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
      setTriggerReloadId(nanoid());

      onReloadRequired();
    });
  };

  if (!queryReference) {
    return (
      <Card sx={{ maxWidth: 500, height: '100%' }}>
        <CardHeader title={<SectionIconTypography label="Desk Occupancy Insights" invertDefaultColor />} />
        <CardContent>
          <Skeleton variant="rounded" width={470} height={350} />
        </CardContent>
      </Card>
    );
  }

  return (
    <ErrorBoundary fallbackRender={({ error }: { error: RootError }) => <RelayError error={error} />}>
      <MemoLocationDeskOccupancyInsightRoot
        queryReference={queryReference}
        onReloadRequired={handleReloadRequired}
        hideLocationDetails={hideLocationDetails}
      />
    </ErrorBoundary>
  );
};

export default memo(LocationDeskOccupancyInsightRootWithRelay);
