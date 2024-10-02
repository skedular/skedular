import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import CardHeader from '@mui/material/CardHeader';
import Skeleton from '@mui/material/Skeleton';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import { LocationAvatar } from '@repo/shared/components/avatars';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { endOfWeek, startOfWeek } from '@repo/shared/libs/utils';
import graphql from 'babel-plugin-relay/macro';
import { memo, useEffect } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, usePreloadedQuery, useQueryLoader } from 'react-relay';
import type { locationBookingsCard_rootQuery } from './__generated__/locationBookingsCard_rootQuery.graphql';
import LocationPeopleBookings from './location-people-bookings';

type Props = {
  queryReference: PreloadedQuery<locationBookingsCard_rootQuery, Record<string, unknown>>;
  organizationId?: string;
  locationId: string;
  locationName: string;
  locationsConnectionIds: string[];
  hideRemoveLocationOption?: boolean;
};

const RootQuery = graphql`
  query locationBookingsCard_rootQuery(
    $peopleNameSearchText: String!
    $peopleSortingValues: [LocationMemberOrderInput!]!
    $locationId: String!
    $from: DateTime!
    $to: DateTime!
  ) {
    ...locationPeopleBookings_query
  }
`;

const LocationBookingsCard = ({
  queryReference,
  organizationId,
  locationId,
  locationName,
  locationsConnectionIds,
  hideRemoveLocationOption,
}: Props) => {
  const rootData = usePreloadedQuery<locationBookingsCard_rootQuery>(RootQuery, queryReference);

  return (
    <LocationPeopleBookings
      rootDataRelay={rootData}
      organizationId={organizationId}
      locationId={locationId}
      locationName={locationName}
      locationsConnectionIds={locationsConnectionIds}
      hideRemoveLocationOption={hideRemoveLocationOption}
    />
  );
};

const MemoLocationBookingsCard = memo(LocationBookingsCard);

type RelayProps = {
  organizationId?: string;
  locationId: string;
  locationName: string;
  locationsConnectionIds: string[];
  hideRemoveLocationOption?: boolean;
};

const LocationBookingsWithRelay = ({ organizationId, locationId, locationName, locationsConnectionIds, hideRemoveLocationOption }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<locationBookingsCard_rootQuery>(RootQuery);

  useEffect(() => {
    const startDate = startOfWeek();
    const endDate = endOfWeek(startDate);

    loadQuery(
      {
        peopleSortingValues: [
          {
            direction: 'Ascending',
            field: 'name',
          },
        ],
        peopleNameSearchText: '',
        locationId: locationId,
        from: startDate.toISOString(),
        to: endDate.toISOString(),
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, locationId, organizationId]);

  if (!queryReference) {
    return (
      <Card sx={{ maxWidth: 500, height: '100%' }}>
        <CardHeader
          title={
            <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
              <LocationAvatar name={{ name: locationName }} photo={{ url: null }} size="small" />
              <Typography variant="h6">{locationName}</Typography>
            </Stack>
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
      <MemoLocationBookingsCard
        queryReference={queryReference}
        organizationId={organizationId}
        locationId={locationId}
        locationName={locationName}
        locationsConnectionIds={locationsConnectionIds}
        hideRemoveLocationOption={hideRemoveLocationOption}
      />
    </ErrorBoundary>
  );
};

export default memo(LocationBookingsWithRelay);
