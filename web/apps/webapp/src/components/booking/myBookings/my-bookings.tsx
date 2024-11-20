import type { myBookings_bookings_query$key } from '@/queries/__generated__/myBookings_bookings_query.graphql';
import type { myBookings_bookings_refetchableFragment } from '@/queries/__generated__/myBookings_bookings_refetchableFragment.graphql';
import type { myBookings_query$key } from '@/queries/__generated__/myBookings_query.graphql';
import AvatarGroup from '@mui/material/AvatarGroup';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import CardHeader from '@mui/material/CardHeader';
import Chip from '@mui/material/Chip';
import Divider from '@mui/material/Divider';
import Grid from '@mui/material/Grid2';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import { CustomerAvatar } from '@repo/shared/components/avatars';
import { CalendarIcon, DeskIcon, LocationIcon, TeamIcon, ZoneIcon } from '@repo/shared/components/icons';
import { TAG_TYPE_LOCATION_ZONE } from '@repo/shared/components/zone';
import { defaultPadding, defaultSpacing } from '@repo/shared/libs/theme';
import { isTodayDate, isTomorrowDate, toShortDate } from '@repo/shared/libs/utils';
import dayjs, { Dayjs } from 'dayjs';
import { Fragment, memo, useMemo } from 'react';
import { graphql, useFragment, usePaginationFragment } from 'react-relay';

type Props = {
  rootDataRelay: myBookings_query$key;
  rootDataBookingRelay: myBookings_bookings_query$key;
  onReloadRequired: () => void;
  organizationId: string;
};

const MyBookings = ({ rootDataRelay, rootDataBookingRelay, onReloadRequired, organizationId }: Props) => {
  const rootData = useFragment<myBookings_query$key>(
    graphql`
      fragment myBookings_query on Query {
        me {
          id
        }
      }
    `,
    rootDataRelay,
  );

  const { data: rootDataBookings, refetch: refetchBookings } = usePaginationFragment<
    myBookings_bookings_refetchableFragment,
    myBookings_bookings_query$key
  >(
    graphql`
      fragment myBookings_bookings_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: null })
      @refetchable(queryName: "myBookings_bookings_refetchableFragment") {
        bookings(
          first: $count
          after: $cursor
          where: { organizationIds: [$organizationId], fromGTE: $bookingsSearchCriteriaFrom, fromLTE: $bookingsSearchCriteriaTo }
          orderBy: [{ field: From, direction: Ascending }]
        ) @connection(key: "myBookings_bookings") {
          __id
          totalCount
          edges {
            node {
              id
              from
              to
              notes
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
        }
      }
    `,
    rootDataBookingRelay,
  );

  const bookings = useMemo(() => {
    if (!rootDataBookings.bookings) {
      return [];
    }

    return rootDataBookings.bookings.edges.map((edge) => edge.node);
  }, [rootDataBookings.bookings]);

  const myBookings = useMemo(() => {
    return bookings.filter((booking) => booking.customer?.uniqueId === rootData.me?.id);
  }, [bookings, rootData.me?.id]);

  const convertDateToKey = (date: Dayjs) => dayjs(date).format('YYYY-MM-DD');

  const groupedBookingsByFromDate = useMemo(() => {
    return bookings.reduce(
      (acc, booking) => {
        const key = convertDateToKey(booking.from);

        if (!acc[key]) {
          acc[key] = [];
        }

        acc[key].push(booking);

        return acc;
      },
      {} as Record<string, typeof bookings>,
    );
  }, [bookings]);

  if (!rootDataBookings.bookings) {
    return <></>;
  }

  return (
    <Stack
      direction="column"
      spacing={1}
      sx={{
        paddingLeft: defaultPadding,
        paddingRight: defaultPadding,
        paddingTop: defaultPadding,
      }}
    >
      <Typography variant="h5">My Bookings</Typography>

      <Divider />

      <Grid container spacing={defaultSpacing} sx={{ alignItems: 'flex-start' }}>
        {myBookings.map((myBooking) => {
          const date = dayjs(myBooking.from);
          let dateValue = '';
          if (isTodayDate(date)) {
            dateValue = `Today, ${toShortDate(date)}`;
          } else if (isTomorrowDate(date)) {
            dateValue = `Tomorrow, ${toShortDate(date)}`;
          } else {
            dateValue = toShortDate(date);
          }

          const key = convertDateToKey(myBooking.from);
          const otherTeammatesBookings = groupedBookingsByFromDate[key]?.filter((booking) => booking.customer?.uniqueId !== rootData.me?.id);

          return (
            <Grid key={myBooking.id}>
              <Card sx={{ width: 250 }}>
                <CardHeader
                  title={
                    <Stack direction="row" spacing={1} sx={{ alignItems: 'center' }}>
                      <LocationIcon fontSize="medium" />
                      <Typography variant="h6">{myBooking.location?.name}</Typography>
                    </Stack>
                  }
                />
                <CardContent>
                  <Stack direction="row" spacing={1} sx={{ alignItems: 'center', paddingTop: 1, paddingBottom: 1 }}>
                    <CalendarIcon fontSize="medium" />
                    <Typography variant="body1">{dateValue}</Typography>
                  </Stack>

                  <Divider />

                  <Stack direction="row" spacing={1} sx={{ alignItems: 'center', paddingTop: 1, paddingBottom: 1 }}>
                    <TeamIcon fontSize="medium" />
                    <Typography variant="body1">{myBooking.team ? myBooking.team.name : 'N/A'}</Typography>
                  </Stack>

                  <Divider />

                  {myBooking.desks.length === 0 && (
                    <Stack direction="row" spacing={1} sx={{ alignItems: 'center', paddingTop: 1, paddingBottom: 1 }}>
                      <DeskIcon fontSize="medium" />
                      <Typography variant="body1">N/A</Typography>
                    </Stack>
                  )}

                  {myBooking.desks.length > 0 && (
                    <>
                      {myBooking.desks.map((desk) => {
                        const zones = desk.locationTags.filter(({ tagType }) => tagType === TAG_TYPE_LOCATION_ZONE);

                        return (
                          <Fragment key={desk.uniqueId}>
                            <Stack direction="row" spacing={1} sx={{ alignItems: 'center', paddingTop: 1, paddingBottom: 1 }}>
                              <DeskIcon fontSize="medium" />
                              <Typography variant="body1">{desk.name}</Typography>
                            </Stack>

                            <Divider />

                            <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap', paddingTop: 1, paddingBottom: 1 }}>
                              <ZoneIcon fontSize="medium" />
                              {zones.map((zone) => (
                                <Chip key={zone.uniqueId} label={zone.name} />
                              ))}
                            </Stack>
                          </Fragment>
                        );
                      })}
                    </>
                  )}

                  <Divider />

                  <Stack direction="column" spacing={1} sx={{ paddingTop: 1, paddingBottom: 1 }}>
                    <Typography variant="body1">Other teammates coming</Typography>
                    <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
                      <AvatarGroup max={5}>
                        {otherTeammatesBookings?.map((booking) => (
                          <CustomerAvatar
                            key={booking.customer?.uniqueId}
                            name={booking.customer}
                            photo={{ url: booking.customer?.photoUrl }}
                            size="medium"
                            showFullName
                          />
                        ))}
                      </AvatarGroup>
                    </Stack>
                  </Stack>
                </CardContent>
              </Card>
            </Grid>
          );
        })}
      </Grid>
    </Stack>
  );
};

export default memo(MyBookings);
