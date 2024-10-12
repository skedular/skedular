import { NewBookingButton } from '@/components/booking/addBooking';
import { LocationLink } from '@/components/location';
import { TeamLink } from '@/components/team';
import type { customerTomorrowSummary_query$key } from '@/queries/__generated__/customerTomorrowSummary_query.graphql';
import type { customerTomorrowSummary_RefetchableFragment } from '@/queries/__generated__/customerTomorrowSummary_RefetchableFragment.graphql';
import type { customerTomorrowSummary_rootQuery } from '@/queries/__generated__/customerTomorrowSummary_rootQuery.graphql';
import AvatarGroup from '@mui/material/AvatarGroup';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import CardHeader from '@mui/material/CardHeader';
import Divider from '@mui/material/Divider';
import Paper from '@mui/material/Paper';
import Popper from '@mui/material/Popper';
import Skeleton from '@mui/material/Skeleton';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import { CustomerAvatar } from '@repo/shared/components/avatars';
import { getBookingSummaryMessage } from '@repo/shared/components/booking';
import { DeskIcon, LocationIcon, TeamIcon } from '@repo/shared/components/icons';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { TAG_TYPE_LOCATION_ZONE, ZonesLine } from '@repo/shared/components/zone';
import { GlobalReloadIdContext } from '@repo/shared/libs/providers';
import { endOfDay, getCustomerFullName, startOfDay, toShortDateWithDayAndMonthOnly } from '@repo/shared/libs/utils';
import { Dayjs } from 'dayjs';
import { memo, startTransition, useContext, useEffect, useMemo, useState } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, graphql, usePreloadedQuery, useQueryLoader, useRefetchableFragment } from 'react-relay';

type Props = {
  queryReference: PreloadedQuery<customerTomorrowSummary_rootQuery, Record<string, unknown>>;
  date: Dayjs;
  onReloadRequired: () => void;
};

const RootQuery = graphql`
  query customerTomorrowSummary_rootQuery($from: DateTime!, $to: DateTime!) {
    me {
      id
    }
    myLocations {
      id
      name
      organization {
        uniqueId
        name
      }
    }
    myTeams {
      id
      name
      organization {
        uniqueId
        name
      }
    }
    ...customerTomorrowSummary_query
  }
`;

type CustomerDetails = {
  readonly uniqueId: string;
  readonly givenName?: string | null | undefined;
  readonly middleName?: string | null | undefined;
  readonly familyName?: string | null | undefined;
  readonly name?: string | null | undefined;
  readonly photoUrl?: string | null | undefined;
};

type LocationDetails = {
  readonly uniqueId: string;
  readonly name?: string | null | undefined;
};

type LocationTagDetails = {
  readonly uniqueId: string;
  readonly name?: string | null | undefined;
  readonly tagType?: string | null | undefined;
};

type DeskDetails = {
  readonly uniqueId: string;
  readonly name?: string | null | undefined;
  readonly locationTags: ReadonlyArray<LocationTagDetails>;
};

type TeamDetails = {
  readonly uniqueId: string;
  readonly name?: string | null | undefined;
};

type BookingDetails = {
  readonly id: string;
  readonly customer: CustomerDetails;
  readonly location?: LocationDetails | null | undefined;
  readonly team?: TeamDetails | null | undefined;
  readonly desks: ReadonlyArray<DeskDetails>;
};

const CustomerTomorrowSummary = ({ queryReference, date, onReloadRequired }: Props) => {
  const rootDataRelay = usePreloadedQuery<customerTomorrowSummary_rootQuery>(RootQuery, queryReference);
  const [rootData, refetch] = useRefetchableFragment<customerTomorrowSummary_RefetchableFragment, customerTomorrowSummary_query$key>(
    graphql`
      fragment customerTomorrowSummary_query on Query @refetchable(queryName: "customerTomorrowSummary_RefetchableFragment") {
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
            name
          }
          team {
            uniqueId
            name
          }
          desks {
            uniqueId
            name
            locationTags {
              uniqueId
              name
              tagType
            }
          }
        }
      }
    `,
    rootDataRelay,
  );

  const globalReloadId = useContext(GlobalReloadIdContext);
  useEffect(() => {
    startTransition(() => {
      refetch(
        {},
        {
          fetchPolicy: 'store-and-network',
        },
      );
    });
  }, [refetch, globalReloadId, date]);

  const [bookingPopperAnchorEl, setBookingPopperAnchorEl] = useState<null | HTMLElement>(null);
  const [bookingPopperLatestUniqueId, setBookingPopperLatestUniqueId] = useState<string>('');
  const [bookingPopperMessage, setBookingPopperMessage] = useState<string>('');

  const myBookings = useMemo(
    () => (rootData.allBookings ? rootData.allBookings.filter(({ customer: { uniqueId } }) => uniqueId === rootDataRelay.me?.id) : []),
    [rootData.allBookings, rootDataRelay.me?.id],
  );
  const otherBookings = useMemo(
    () => (rootData.allBookings ? rootData.allBookings.filter(({ id }) => myBookings.every(({ id: myBookingId }) => myBookingId !== id)) : []),
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

  const getMyBookingComponent = (booking: BookingDetails) => (
    <Stack key={booking.id} direction="column">
      {booking.location && (
        <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
          <LocationIcon />
          <Typography variant="body1" component="div">
            {booking.location.name}
          </Typography>
        </Stack>
      )}

      {booking.team && (
        <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
          <TeamIcon />
          <Typography variant="body1" component="div">
            {booking.team.name}
          </Typography>
        </Stack>
      )}

      {booking.desks?.map(({ uniqueId, name, locationTags }) => {
        const zones = locationTags.filter(({ tagType }) => tagType === TAG_TYPE_LOCATION_ZONE);

        return (
          <Stack key={uniqueId} direction="row" spacing={1} sx={{ alignItems: 'center' }}>
            <DeskIcon />
            <Typography variant="body1" component="div">
              {name}
            </Typography>
            <ZonesLine zones={zones.map(({ uniqueId, name }) => ({ id: uniqueId, name }))} />
          </Stack>
        );
      })}
    </Stack>
  );

  const getMyBookingsComponents = (bookings: BookingDetails[]) => (
    <Stack direction="column">
      <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
        {bookings.length === 0 && <Typography variant="h6">You have no booking</Typography>}
        {bookings.length !== 0 && <Typography variant="h6">You</Typography>}
      </Stack>
      <Stack direction="column">
        {bookings.length === 0 && (
          <NewBookingButton hideLocationControl={false} hideOrganizationControl={false} onReloadRequired={onReloadRequired} />
        )}
        {bookings.length !== 0 && (
          <>
            {bookings.slice(0, bookings.length - 1).map((booking, index) => (
              <Stack key={index} direction="column" spacing={1}>
                {getMyBookingComponent(booking)}
                <Divider />
              </Stack>
            ))}
            {getMyBookingComponent(bookings[bookings.length - 1]!)}
          </>
        )}
      </Stack>
    </Stack>
  );

  const getBookingsByLocationsComponents = (locationId: string, bookings: typeof otherBookings) => {
    const location = rootDataRelay.myLocations?.find(({ id }) => id === locationId);
    if (!location) {
      return <></>;
    }

    return (
      <Stack direction="column" spacing={1}>
        <LocationLink
          organizationId={location.organization?.uniqueId}
          id={locationId}
          name={location.name}
          enableViewDetails
          onReloadRequired={onReloadRequired}
        />
        <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
          <AvatarGroup max={10}>
            {bookings.map((booking) => (
              <CustomerAvatar
                key={booking.customer?.uniqueId}
                name={booking.customer}
                photo={{ url: booking.customer?.photoUrl }}
                size="small"
                onClick={(event: React.MouseEvent<HTMLElement>) => {
                  setBookingPopperMessage(`${getCustomerFullName(booking.customer)} - ${getBookingSummaryMessage(booking, false)}`);

                  const uiqueId = `${locationId}-${booking.id}`;

                  if (bookingPopperLatestUniqueId !== uiqueId) {
                    setBookingPopperAnchorEl(event.currentTarget);
                  } else {
                    setBookingPopperAnchorEl(bookingPopperAnchorEl ? null : event.currentTarget);
                  }

                  setBookingPopperLatestUniqueId(uiqueId);
                }}
              />
            ))}
          </AvatarGroup>
        </Stack>
      </Stack>
    );
  };

  const getBookingsByTeamsComponents = (teamId: string, bookings: typeof otherBookings) => {
    const team = rootDataRelay.myTeams?.find(({ id }) => id === teamId);
    if (!team) {
      return <></>;
    }

    return (
      <Stack direction="column" spacing={1}>
        <TeamLink organizationId={team.organization?.uniqueId} id={teamId} name={team.name} enableViewDetails onReloadRequired={onReloadRequired} />
        <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
          <AvatarGroup max={10}>
            {bookings.map((booking) => (
              <CustomerAvatar
                key={booking.customer?.uniqueId}
                name={booking.customer}
                photo={{ url: booking.customer?.photoUrl }}
                size="small"
                onClick={(event: React.MouseEvent<HTMLElement>) => {
                  setBookingPopperMessage(`${getCustomerFullName(booking.customer)} - ${getBookingSummaryMessage(booking, false)}`);

                  const uiqueId = `${teamId}-${booking.id}`;

                  if (bookingPopperLatestUniqueId !== uiqueId) {
                    setBookingPopperAnchorEl(event.currentTarget);
                  } else {
                    setBookingPopperAnchorEl(bookingPopperAnchorEl ? null : event.currentTarget);
                  }

                  setBookingPopperLatestUniqueId(uiqueId);
                }}
              />
            ))}
          </AvatarGroup>
        </Stack>
      </Stack>
    );
  };

  const summerizedRows: JSX.Element[] = [
    ...Object.entries(groupedOtherBookingsByLocation).map(([locationId, bookings]) => getBookingsByLocationsComponents(locationId, bookings)),
    ...Object.entries(groupedOtherBookingsByTeam).map(([teamId, bookings]) => getBookingsByTeamsComponents(teamId, bookings)),
  ];

  return (
    <>
      <Card sx={{ maxWidth: 500, height: '100%' }}>
        <CardHeader
          title={
            <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
              <Typography variant="body1">{`Tomorrow ${toShortDateWithDayAndMonthOnly(date)}`}</Typography>
            </Stack>
          }
          subheader={getMyBookingsComponents(myBookings)}
        />
        <CardContent>
          {summerizedRows.length !== 0 && (
            <>
              {summerizedRows.slice(0, summerizedRows.length - 1).map((row, index) => (
                <Stack key={index} direction="column" spacing={1}>
                  {row}
                  <Divider />
                </Stack>
              ))}
              {summerizedRows[summerizedRows.length - 1]}
            </>
          )}
        </CardContent>
      </Card>
      <Popper open={Boolean(bookingPopperAnchorEl)} anchorEl={bookingPopperAnchorEl} placement="right-start">
        <Paper sx={{ border: 1, p: 1 }}>
          <Typography variant="body1">{bookingPopperMessage}</Typography>
        </Paper>
      </Popper>
    </>
  );
};

const MemoCustomerTomorrowSummary = memo(CustomerTomorrowSummary);

type RelayProps = {};

const CustomerTomorrowSummaryWithRelay = ({}: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<customerTomorrowSummary_rootQuery>(RootQuery);
  const [tomorrow] = useState(startOfDay().add(1, 'day'));

  useEffect(() => {
    loadQuery(
      {
        from: tomorrow.toISOString(),
        to: endOfDay(tomorrow).toISOString(),
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, tomorrow]);

  const handleReloadRequired = () => {
    loadQuery(
      {
        from: tomorrow.toISOString(),
        to: endOfDay(tomorrow).toISOString(),
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  };

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
      <MemoCustomerTomorrowSummary queryReference={queryReference} date={tomorrow} onReloadRequired={handleReloadRequired} />
    </ErrorBoundary>
  );
};

export default memo(CustomerTomorrowSummaryWithRelay);
