import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import CardHeader from '@mui/material/CardHeader';
import Link from '@mui/material/Link';
import Skeleton from '@mui/material/Skeleton';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { startOfDay } from '@repo/shared/libs/utils';
import graphql from 'babel-plugin-relay/macro';
import { memo, useEffect } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import type { locationBookingInsightRoot_rootQuery } from './__generated__/locationBookingInsightRoot_rootQuery.graphql';
import LocationBookingInsight from './location-booking-insight';

type Props = {
  queryReference: PreloadedQuery<locationBookingInsightRoot_rootQuery, Record<string, unknown>>;
  organizationId?: string;
  locationId: string;
  hideLocationDetails?: boolean;
};

const RootQuery = graphql`
  query locationBookingInsightRoot_rootQuery($locationId: String!, $from: DateTime!, $to: DateTime!) {
    ...locationBookingInsight_query
  }
`;

const LocationBookingInsightRoot = ({ queryReference, organizationId, locationId, hideLocationDetails }: Props) => {
  const rootData = usePreloadedQuery<locationBookingInsightRoot_rootQuery>(RootQuery, queryReference);

  return (
    <LocationBookingInsight
      rootDataRelay={rootData}
      organizationId={organizationId}
      locationId={locationId}
      hideLocationDetails={hideLocationDetails}
    />
  );
};

const MemoLocationBookingInsightRoot = memo(LocationBookingInsightRoot);

type RelayProps = {
  organizationId?: string;
  locationId: string;
  locationName: string;
  hideLocationDetails?: boolean;
};

const LocationBookingInsightRootWithRelay = ({ organizationId, locationId, locationName, hideLocationDetails }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<locationBookingInsightRoot_rootQuery>(RootQuery);

  useEffect(() => {
    const to = startOfDay(null);
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
  }, [loadQuery, locationId]);

  if (!queryReference) {
    return (
      <Card sx={{ maxWidth: 500, height: '100%' }}>
        <CardHeader
          title={
            <>
              <Typography variant="h5" color="primary">
                Booking Insights
              </Typography>
              {!hideLocationDetails && (
                <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
                  <Link
                    href={
                      organizationId
                        ? `/organization/${organizationId}/location/${locationId}?tab=analytics`
                        : `/location/${locationId}?tab=analytics`
                    }
                  >
                    {locationName && <Typography variant="h6">{locationName}</Typography>}
                  </Link>
                </Stack>
              )}
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
      <MemoLocationBookingInsightRoot
        queryReference={queryReference}
        organizationId={organizationId}
        locationId={locationId}
        hideLocationDetails={hideLocationDetails}
      />
    </ErrorBoundary>
  );
};

export default memo(LocationBookingInsightRootWithRelay);
