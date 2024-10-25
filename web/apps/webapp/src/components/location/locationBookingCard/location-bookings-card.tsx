import { LocationLink } from '@/components/location';
import { OrganizationLink } from '@/components/organization';
import type { locationBookingsCard_rootQuery } from '@/queries/__generated__/locationBookingsCard_rootQuery.graphql';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import CardHeader from '@mui/material/CardHeader';
import Skeleton from '@mui/material/Skeleton';
import Stack from '@mui/material/Stack';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { startOfDay } from '@repo/shared/libs/utils';
import { memo, useEffect } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, graphql, usePreloadedQuery, useQueryLoader } from 'react-relay';
import LocationMembersBookings from './location-members-bookings';

type Props = {
  queryReference: PreloadedQuery<locationBookingsCard_rootQuery, Record<string, unknown>>;
  organizationId?: string;
  locationId: string;
  locationName?: string;
  locationsConnectionIds: string[];
  hideRemoveLocationOption?: boolean;
};

const RootQuery = graphql`
  query locationBookingsCard_rootQuery(
    $peopleSortingValues: [LocationMemberOrderInput!]!
    $organizationId: String!
    $organizationExists: Boolean!
    $locationId: String!
    $locationExists: Boolean!
    $teamId: String!
    $teamExists: Boolean!
    $from: DateTime!
    $to: DateTime!
  ) {
    ...locationMembersBookings_query
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
    <LocationMembersBookings
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
  organizationName?: string;
  locationId: string;
  locationName?: string;
  locationsConnectionIds: string[];
  hideRemoveLocationOption?: boolean;
};

const LocationBookingsWithRelay = ({
  organizationId,
  organizationName,
  locationId,
  locationName,
  locationsConnectionIds,
  hideRemoveLocationOption,
}: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<locationBookingsCard_rootQuery>(RootQuery);

  useEffect(() => {
    const startDate = startOfDay();
    const endDate = startDate.add(1, 'week');

    loadQuery(
      {
        peopleSortingValues: [
          {
            direction: 'Ascending',
            field: 'name',
          },
        ],
        organizationId: organizationId ?? '',
        organizationExists: !!organizationId,
        locationId,
        locationExists: !!locationId,
        teamId: '',
        teamExists: false,
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
            <Stack direction="column">
              <LocationLink organizationId={organizationId} id={locationId} name={locationName} />
              {organizationId && <OrganizationLink id={organizationId} name={organizationName} />}
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
