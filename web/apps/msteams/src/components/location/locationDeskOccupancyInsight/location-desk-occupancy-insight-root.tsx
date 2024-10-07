import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import CardHeader from '@mui/material/CardHeader';
import Skeleton from '@mui/material/Skeleton';
import Typography from '@mui/material/Typography';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { startOfDay } from '@repo/shared/libs/utils';
import graphql from 'babel-plugin-relay/macro';
import { LocationLink } from 'components/location';
import { memo, useEffect } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import type { locationDeskOccupancyInsightRoot_rootQuery } from './__generated__/locationDeskOccupancyInsightRoot_rootQuery.graphql';
import LocationDeskOccupancyInsight from './location-desk-occupancy-insight';

type Props = {
  queryReference: PreloadedQuery<locationDeskOccupancyInsightRoot_rootQuery, Record<string, unknown>>;
  organizationId?: string;
  locationId: string;
  hideLocationDetails?: boolean;
};

const RootQuery = graphql`
  query locationDeskOccupancyInsightRoot_rootQuery($locationId: String!, $locationExists: Boolean!, $from: DateTime!, $to: DateTime!) {
    ...locationDeskOccupancyInsight_query
  }
`;

const LocationDeskOccupancyInsightRoot = ({ queryReference, organizationId, locationId, hideLocationDetails }: Props) => {
  const rootData = usePreloadedQuery<locationDeskOccupancyInsightRoot_rootQuery>(RootQuery, queryReference);

  return (
    <LocationDeskOccupancyInsight
      rootDataRelay={rootData}
      organizationId={organizationId}
      locationId={locationId}
      hideLocationDetails={hideLocationDetails}
    />
  );
};

const MemoLocationDeskOccupancyInsightRoot = memo(LocationDeskOccupancyInsightRoot);

type RelayProps = {
  organizationId?: string;
  locationId: string;
  locationName: string;
  hideLocationDetails?: boolean;
};

const LocationDeskOccupancyInsightRootWithRelay = ({ organizationId, locationId, locationName, hideLocationDetails }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<locationDeskOccupancyInsightRoot_rootQuery>(RootQuery);

  useEffect(() => {
    const to = startOfDay();
    const from = to.subtract(30, 'days');

    loadQuery(
      {
        locationId,
        locationExists: !!locationId,
        from: from.toISOString(),
        to: to.toISOString(),
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, locationId]);

  if (!queryReference) {
    return (
      <Card sx={{ maxWidth: 500, height: '100%' }}>
        <CardHeader
          title={
            <>
              <Typography variant="h5" color="primary">
                Desk Occupancy Insights
              </Typography>
              {!hideLocationDetails && <LocationLink organizationId={organizationId} id={locationId} name={locationName} analayticsLink />}
            </>
          }
        />
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
        organizationId={organizationId}
        locationId={locationId}
        hideLocationDetails={hideLocationDetails}
      />
    </ErrorBoundary>
  );
};

export default memo(LocationDeskOccupancyInsightRootWithRelay);
