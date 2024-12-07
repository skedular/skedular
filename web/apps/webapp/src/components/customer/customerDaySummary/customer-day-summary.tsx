import { NewBookingButton } from '@/components/booking/addBooking';
import { LocationLink } from '@/components/location';
import { TeamLink } from '@/components/team';
import type { customerDaySummary_query$key } from '@/queries/__generated__/customerDaySummary_query.graphql';
import type { customerDaySummary_refetchableFragment } from '@/queries/__generated__/customerDaySummary_refetchableFragment.graphql';
import type { customerDaySummary_rootQuery } from '@/queries/__generated__/customerDaySummary_rootQuery.graphql';
import AvatarGroup from '@mui/material/AvatarGroup';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import CardHeader from '@mui/material/CardHeader';
import Divider from '@mui/material/Divider';
import Paper from '@mui/material/Paper';
import Popover from '@mui/material/Popover';
import Skeleton from '@mui/material/Skeleton';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import { CustomerAvatar } from '@repo/shared/components/avatars';
import { getBookingSummaryMessage } from '@repo/shared/components/booking';
import { DeskIcon, LocationIcon, TeamIcon } from '@repo/shared/components/icons';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { ZonesLine } from '@repo/shared/components/zone';
import { GlobalReloadIdContext } from '@repo/shared/libs/providers';
import { endOfDay, getCustomerFullName, toShortDateWithDayAndMonthOnlyWithAdditionalDayInfo } from '@repo/shared/libs/utils';
import { Dayjs } from 'dayjs';
import { nanoid } from 'nanoid';
import type { JSX } from 'react';
import { memo, startTransition, useContext, useEffect, useMemo, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { PreloadedQuery, graphql, usePreloadedQuery, useQueryLoader, useRefetchableFragment } from 'react-relay';

type Props = {
  queryReference: PreloadedQuery<customerDaySummary_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  date: Dayjs;
  minWidth?: number;
};

const RootQuery = graphql`
  query customerDaySummary_rootQuery($organizationId: String!, $nullableOrganizationId: String, $from: DateTime!, $to: DateTime!) {
    me {
      id
    }
    myLocations(organizationId: $nullableOrganizationId) {
      id
      name
      organization {
        uniqueId
        name
      }
    }
    myTeams(organizationId: $nullableOrganizationId) {
      id
      name
      organization {
        uniqueId
        name
      }
    }
    ...customerDaySummary_query
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

type ZoneDetails = {
  readonly uniqueId: string;
  readonly name?: string | null | undefined;
  readonly tagType?: string | null | undefined;
};

type DeskDetails = {
  readonly uniqueId: string;
  readonly name?: string | null | undefined;
  readonly zones: ReadonlyArray<ZoneDetails>;
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

const CustomerDaySummary = ({ queryReference, onReloadRequired, date, minWidth }: Props) => {
  const rootDataRelay = usePreloadedQuery<customerDaySummary_rootQuery>(RootQuery, queryReference);
  const [rootData, refetch] = useRefetchableFragment<customerDaySummary_refetchableFragment, customerDaySummary_query$key>(
    graphql`
      fragment customerDaySummary_query on Query @refetchable(queryName: "customerDaySummary_refetchableFragment") {
        allBookings(where: { fromGTE: $from, toLTE: $to, organizationIds: [$organizationId] }) {
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
            deskTypes {
              uniqueId
              name
            }
            zones {
              uniqueId
              name
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

      {booking.desks?.map(({ uniqueId, name, zones }) => (
        <Stack key={uniqueId} direction="row" spacing={1} sx={{ alignItems: 'center' }}>
          <DeskIcon />
          <Typography variant="body1" component="div">
            {name}
          </Typography>
          <ZonesLine zones={zones.map(({ uniqueId, name }) => ({ id: uniqueId, name }))} />
        </Stack>
      ))}
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
          <NewBookingButton hideLocationControl={false} hideOrganizationControl={false} onReloadRequired={onReloadRequired} defaultDate={date} />
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
      <>
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
      </>
    );
  };

  const getBookingsByTeamsComponents = (teamId: string, bookings: typeof otherBookings) => {
    const team = rootDataRelay.myTeams?.find(({ id }) => id === teamId);
    if (!team) {
      return <></>;
    }

    return (
      <>
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
      </>
    );
  };

  const summerizedRows: JSX.Element[] = [
    ...Object.entries(groupedOtherBookingsByLocation).map(([locationId, bookings]) => getBookingsByLocationsComponents(locationId, bookings)),
    ...Object.entries(groupedOtherBookingsByTeam).map(([teamId, bookings]) => getBookingsByTeamsComponents(teamId, bookings)),
  ];

  const handleClose = () => {
    setBookingPopperAnchorEl(null);
    setBookingPopperLatestUniqueId('');
  };

  return (
    <>
      <Card sx={{ maxWidth: 500, minWidth }}>
        <CardHeader
          title={
            <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
              <Typography variant="body1">{toShortDateWithDayAndMonthOnlyWithAdditionalDayInfo(date)}</Typography>
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
      <Popover
        open={Boolean(bookingPopperAnchorEl)}
        anchorEl={bookingPopperAnchorEl}
        onClose={handleClose}
        anchorOrigin={{
          vertical: 'bottom',
          horizontal: 'left',
        }}
      >
        <Paper sx={{ border: 1, p: 1 }}>
          <Typography variant="body1">{bookingPopperMessage}</Typography>
        </Paper>
      </Popover>
    </>
  );
};

const MemoCustomerDaySummary = memo(CustomerDaySummary);

type RelayProps = {
  date: Dayjs;
  minWidth?: number;
  organizationId?: string;
};

const CustomerDaySummaryWithRelay = ({ date, minWidth, organizationId }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<customerDaySummary_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(nanoid());
  const [, startTransition] = useTransition();

  useEffect(() => {
    loadQuery(
      {
        from: date.toISOString(),
        to: endOfDay(date).toISOString(),
        organizationId: organizationId ?? '',
        nullableOrganizationId: organizationId,
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId, date, organizationId]);

  const handleReloadRequired = () => {
    startTransition(() => {
      setTriggerReloadId(nanoid());
    });
  };

  if (!queryReference) {
    return (
      <Card sx={{ maxWidth: 500 }}>
        <CardContent>
          <Skeleton variant="rounded" width={470} height={350} />
        </CardContent>
      </Card>
    );
  }

  return (
    <ErrorBoundary fallbackRender={({ error }: { error: RootError }) => <RelayError error={error} />}>
      <MemoCustomerDaySummary queryReference={queryReference} date={date} onReloadRequired={handleReloadRequired} minWidth={minWidth} />
    </ErrorBoundary>
  );
};

export default memo(CustomerDaySummaryWithRelay);
