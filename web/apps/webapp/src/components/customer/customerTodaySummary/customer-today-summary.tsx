import type { customerTodaySummary_rootQuery } from '@/queries/__generated__/customerTodaySummary_rootQuery.graphql';
import AvatarGroup from '@mui/material/AvatarGroup';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import CardHeader from '@mui/material/CardHeader';
import Divider from '@mui/material/Divider';
import Skeleton from '@mui/material/Skeleton';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import { CustomerAvatar, LocationAvatar, TeamAvatar } from '@repo/shared/components/avatars';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { endOfDay, startOfDay, toShortDateWithDayAndMonthOnly } from '@repo/shared/libs/utils';
import { Dayjs } from 'dayjs';
import { memo, useEffect, useMemo, useState } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, graphql, usePreloadedQuery, useQueryLoader } from 'react-relay';

type Props = {
  queryReference: PreloadedQuery<customerTodaySummary_rootQuery, Record<string, unknown>>;
  today: Dayjs;
};

const RootQuery = graphql`
  query customerTodaySummary_rootQuery($from: DateTime!, $to: DateTime!) {
    me {
      id
    }
    allBookings(where: { fromGTE: $from, toLTE: $to }) {
      id
      from
      to
      customer {
        uniqueId
        name
        givenName
        middleName
        familyName
        photoUrl
      }
      location {
        uniqueId
      }
      team {
        uniqueId
      }
      desks {
        name
        locationTags {
          uniqueId
          name
          tagType
        }
      }
    }
    myLocations {
      id
      name
      organization {
        uniqueId
      }
    }
    myTeams {
      id
      name
      organization {
        uniqueId
      }
    }
  }
`;

const CustomerTodaySummary = ({ queryReference }: Props) => {
  const rootData = usePreloadedQuery<customerTodaySummary_rootQuery>(RootQuery, queryReference);
  const myBookings = useMemo(
    () => rootData.allBookings.filter(({ customer: { uniqueId } }) => uniqueId === rootData.me?.id),
    [rootData.allBookings, rootData.me?.id],
  );
  const otherBookings = useMemo(
    () => rootData.allBookings.filter(({ id }) => myBookings.every(({ id: myBookingId }) => myBookingId !== id)),
    [myBookings, rootData.allBookings],
  );
  const groupedOtherBookingsByLocation = useMemo(
    () =>
      otherBookings.reduce(
        (acc, booking) => {
          const locationId = booking.location?.uniqueId;
          if (locationId) {
            if (!acc[locationId]) {
              acc[locationId] = [];
            }

            if (acc[locationId].every(({ customer: { uniqueId } }) => uniqueId !== booking.customer?.uniqueId)) {
              acc[locationId].push(booking);
            }
          }

          return acc;
        },
        {} as Record<string, typeof otherBookings>,
      ),
    [otherBookings],
  );
  const groupedOtherBookingsByTeam = useMemo(
    () =>
      otherBookings.reduce(
        (acc, booking) => {
          const teamId = booking.team?.uniqueId;
          if (teamId) {
            if (!acc[teamId]) {
              acc[teamId] = [];
            }

            if (acc[teamId].every(({ customer: { uniqueId } }) => uniqueId !== booking.customer?.uniqueId)) {
              acc[teamId].push(booking);
            }
          }

          return acc;
        },
        {} as Record<string, typeof otherBookings>,
      ),
    [otherBookings],
  );

  const MyBookingsComponents = (
    <Stack direction="column" sx={{ paddingTop: 1, paddingBottom: 1 }}>
      <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
        <Typography variant="h6">You</Typography>
      </Stack>
      <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
        {myBookings.map((booking) => {
          return <Stack key={booking.id} direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}></Stack>;
        })}
      </Stack>
    </Stack>
  );

  const BookingsByLocationsComponents = ({ locationId, bookings }: { locationId: string; bookings: typeof otherBookings }) => {
    const location = rootData.myLocations.find(({ id }) => id === locationId);

    return (
      <Stack direction="column" sx={{ paddingTop: 1, paddingBottom: 1 }}>
        <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
          <LocationAvatar name={{ name: location?.name }} photo={{ url: null }} />
          <Typography variant="h6">{location?.name}</Typography>
        </Stack>
        <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
          <AvatarGroup max={10}>
            {bookings.map((booking) => (
              <CustomerAvatar key={booking.customer?.uniqueId} name={booking.customer} photo={{ url: booking.customer?.photoUrl }} showFullName />
            ))}
          </AvatarGroup>
        </Stack>
      </Stack>
    );
  };

  const BookingsByTeamsComponents = ({ teamId, bookings }: { teamId: string; bookings: typeof otherBookings }) => {
    const team = rootData.myTeams.find(({ id }) => id === teamId);

    return (
      <Stack direction="column" sx={{ paddingTop: 1, paddingBottom: 1 }}>
        <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
          <TeamAvatar name={{ name: team?.name }} photo={{ url: null }} />
          <Typography variant="h6">{team?.name}</Typography>
        </Stack>
        <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
          <AvatarGroup max={10}>
            {bookings.map((booking) => (
              <CustomerAvatar key={booking.customer?.uniqueId} name={booking.customer} photo={{ url: booking.customer?.photoUrl }} showFullName />
            ))}
          </AvatarGroup>
        </Stack>
      </Stack>
    );
  };

  const summerizedRows: JSX.Element[] = [
    MyBookingsComponents,
    ...Object.entries(groupedOtherBookingsByLocation).map(([locationId, bookings]) => (
      <BookingsByLocationsComponents key={locationId} locationId={locationId} bookings={bookings} />
    )),
    ...Object.entries(groupedOtherBookingsByTeam).map(([teamId, bookings]) => (
      <BookingsByTeamsComponents key={teamId} teamId={teamId} bookings={bookings} />
    )),
  ];

  const allSummerizedRowsExceptLast = summerizedRows.length === 0 ? [] : summerizedRows.slice(0, summerizedRows.length - 1);

  return (
    <Card sx={{ maxWidth: 500, height: '100%' }}>
      <CardHeader
        title={
          <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
            <Typography variant="body1">{`Today ${toShortDateWithDayAndMonthOnly(startOfDay())}`}</Typography>
          </Stack>
        }
      />
      <CardContent>
        {summerizedRows.length !== 0 && (
          <>
            {allSummerizedRowsExceptLast.map((row) => (
              <>
                {row}
                <Divider />
              </>
            ))}
            {summerizedRows[summerizedRows.length - 1]}
          </>
        )}
      </CardContent>
    </Card>
  );
};

const MemoCustomerTodaySummary = memo(CustomerTodaySummary);

type RelayProps = {};

const CustomerTodaySummaryWithRelay = ({}: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<customerTodaySummary_rootQuery>(RootQuery);
  const [today] = useState(startOfDay());

  useEffect(() => {
    loadQuery(
      {
        from: today.toISOString(),
        to: endOfDay(today).toISOString(),
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, today]);

  if (!queryReference) {
    return (
      <Card sx={{ maxWidth: 500, height: '100%' }}>
        <CardContent>
          <Skeleton variant="rounded" width={470} height={350} />
        </CardContent>
      </Card>
    );
  }

  return (
    <ErrorBoundary fallbackRender={({ error }: { error: RootError }) => <RelayError error={error} />}>
      <MemoCustomerTodaySummary queryReference={queryReference} today={today} />
    </ErrorBoundary>
  );
};

export default memo(CustomerTodaySummaryWithRelay);
