import type { myBookings_bookings_query$key } from '@/queries/__generated__/myBookings_bookings_query.graphql';
import type { myBookings_bookings_refetchableFragment } from '@/queries/__generated__/myBookings_bookings_refetchableFragment.graphql';
import type { myBookings_query$key } from '@/queries/__generated__/myBookings_query.graphql';
import AvatarGroup from '@mui/material/AvatarGroup';
import Divider from '@mui/material/Divider';
import Grid from '@mui/material/Grid2';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import type { GridColDef } from '@mui/x-data-grid';
import { DataGrid, gridClasses } from '@mui/x-data-grid';
import { CustomerAvatar } from '@repo/shared/components/avatars';
import { Zones } from '@repo/shared/components/zone';
import { defaultPadding, defaultSpacing } from '@repo/shared/libs/theme';
import { toShortDateWithAdditionalDayInfo } from '@repo/shared/libs/utils';
import dayjs, { Dayjs } from 'dayjs';
import { memo, startTransition, useCallback, useEffect, useMemo } from 'react';
import { graphql, useFragment, useRefetchableFragment } from 'react-relay';
import MyBookingCard from './my-booking-card';

type Props = {
  rootDataRelay: myBookings_query$key;
  rootDataBookingRelay: myBookings_bookings_query$key;
  onReloadRequired: () => void;
  from: Dayjs;
  to: Dayjs;
  locationIds: string[];
  teamIds: string[];
  viewMode: 'list' | 'grid';
};

type CustomerDetails = {
  uniqueId: string;
  givenName?: string | null | undefined;
  middleName?: string | null | undefined;
  familyName?: string | null | undefined;
  name?: string | null | undefined;
  photoUrl?: string | null | undefined;
};

type LocationDetails = {
  name: string;
};

type ZoneDetails = {
  uniqueId: string;
  name: string | null | undefined;
  tagType?: string | null | undefined;
};

type DeskDetails = {
  name: string;
  zones: ReadonlyArray<ZoneDetails>;
};

type TeamDetails = {
  name: string;
};

type RowType = {
  id: string;
  location?: LocationDetails | null | undefined;
  team?: TeamDetails | null | undefined;
  desks: ReadonlyArray<DeskDetails>;
  zones: ReadonlyArray<ZoneDetails>;
  teammates: ReadonlyArray<CustomerDetails>;
};

const MyBookings = ({ rootDataRelay, rootDataBookingRelay, onReloadRequired, from, to, locationIds, teamIds, viewMode }: Props) => {
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

  const [rootDataRefetchable, refetch] = useRefetchableFragment<myBookings_bookings_refetchableFragment, myBookings_bookings_query$key>(
    graphql`
      fragment myBookings_bookings_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: null })
      @refetchable(queryName: "myBookings_bookings_refetchableFragment") {
        bookings(
          first: $count
          after: $cursor
          where: {
            organizationIds: [$organizationId]
            locationIds: $locationIds
            teamIds: $teamIds
            fromGTE: $bookingsSearchCriteriaFrom
            fromLTE: $bookingsSearchCriteriaTo
            combineOrganizationsLocationsTeams: true
          }
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
                deskTypes {
                  uniqueId
                  name
                }
                zones {
                  uniqueId
                  name
                }
              }
              ...myBookingCard_BookingDetails
            }
          }
        }
      }
    `,
    rootDataBookingRelay,
  );

  const bookings = useMemo(() => {
    if (!rootDataRefetchable.bookings) {
      return [];
    }

    return rootDataRefetchable.bookings.edges.map((edge) => edge.node);
  }, [rootDataRefetchable.bookings]);

  const connectionIds = useMemo(() => (rootDataRefetchable.bookings ? [rootDataRefetchable.bookings.__id] : []), [rootDataRefetchable.bookings]);
  const myBookings = useMemo(() => bookings.filter((booking) => booking.customer?.uniqueId === rootData.me?.id), [bookings, rootData.me?.id]);

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

  const handleRefetch = useCallback(
    (from: Dayjs, to: Dayjs, locationIds: string[], teamIds: string[]) => {
      startTransition(() => {
        refetch(
          {
            bookingsSearchCriteriaFrom: from.toISOString(),
            bookingsSearchCriteriaTo: to.toISOString(),
            locationIds,
            teamIds,
          },
          {
            fetchPolicy: 'store-and-network',
          },
        );
      });
    },
    [refetch],
  );

  useEffect(() => handleRefetch(from, to, locationIds, teamIds), [handleRefetch, from, to, locationIds, teamIds]);

  const rows: RowType[] = myBookings.map((myBooking) => {
    const zones = myBooking.desks
      .flatMap(({ zones }) => zones)
      .reduce((acc: ZoneDetails[], zone) => {
        if (!acc.some((item) => item.uniqueId === zone.uniqueId)) {
          acc.push(zone);
        }

        return acc;
      }, []);

    const key = convertDateToKey(myBooking.from);
    const teammates: CustomerDetails[] =
      groupedBookingsByFromDate[key]
        ?.filter((booking) => booking.customer?.uniqueId !== rootData.me?.id && booking.location?.uniqueId === myBooking.location?.uniqueId)
        .map((booking) => booking.customer) ?? [];

    return {
      id: myBooking.id,
      location: myBooking.location,
      team: myBooking.team,
      desks: myBooking.desks,
      zones,
      teammates,
      date: toShortDateWithAdditionalDayInfo(dayjs(myBooking.from)),
    };
  });

  const columns: GridColDef<(typeof rows)[number]>[] = [
    {
      field: 'location',
      headerName: 'Location',
      editable: false,
      renderCell: (params) => params.value?.name ?? 'N/A',
      display: 'text',
      minWidth: 200,
    },
    {
      field: 'team',
      headerName: 'Team',
      editable: false,
      renderCell: (params) => params.value?.name ?? 'N/A',
      display: 'text',
      minWidth: 200,
    },
    {
      field: 'desks',
      headerName: 'Desks',
      editable: false,
      renderCell: (params) => {
        if (!params.value || params.value.length === 0) {
          return 'N/A';
        }

        const desks = params.value?.map((item: DeskDetails) => item.name).join(', ');
        return desks.length === 0 ? 'N/A' : desks;
      },
      display: 'text',
      minWidth: 200,
    },
    {
      field: 'zones',
      headerName: 'Zones',
      editable: false,
      renderCell: (params) => (
        <>
          {params.value.length === 0 && 'N/A'}
          {params.value.length !== 0 && <Zones zones={params.value.map((zone: ZoneDetails) => ({ id: zone.uniqueId, name: zone.name }))} />}
        </>
      ),
      display: 'flex',
      minWidth: 200,
    },
    {
      field: 'teammates',
      headerName: 'Teammates',
      editable: false,
      renderCell: (params) => (
        <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
          <AvatarGroup max={5}>
            {params.value?.map((customer: CustomerDetails) => (
              <CustomerAvatar key={customer?.uniqueId} name={customer} photo={{ url: customer?.photoUrl }} size="medium" showFullName />
            ))}
          </AvatarGroup>
        </Stack>
      ),
      display: 'flex',
      minWidth: 200,
    },
    {
      field: 'date',
      headerName: 'Date',
      editable: false,
      sortable: true,
      display: 'text',
      minWidth: 300,
    },
  ];

  if (!rootDataRefetchable.bookings) {
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

      {viewMode === 'grid' && (
        <Grid container spacing={defaultSpacing} sx={{ alignItems: 'flex-start' }}>
          {myBookings.map((myBooking) => {
            const key = convertDateToKey(myBooking.from);
            const otherTeammates = groupedBookingsByFromDate[key]?.filter(
              (booking) => booking.customer?.uniqueId !== rootData.me?.id && booking.location?.uniqueId === myBooking.location?.uniqueId,
            );

            return (
              <Grid key={myBooking.id}>
                <MyBookingCard
                  bookingDetailsRelay={myBooking}
                  connectionIds={connectionIds}
                  otherTeammates={otherTeammates!.map(({ customer }) => customer)}
                />
              </Grid>
            );
          })}
        </Grid>
      )}

      {viewMode === 'list' && (
        <DataGrid
          rows={rows}
          columns={columns}
          ignoreDiacritics
          disableRowSelectionOnClick
          hideFooter
          getRowHeight={() => 'auto'}
          rowSpacingType="margin"
          getRowSpacing={() => ({ top: 3, bottom: 3 })}
          sx={{
            [`& .${gridClasses.cell}`]: {
              paddingTop: 1,
              paddingBottom: 1,
            },
            [`& .${gridClasses.row}`]: {
              paddingLeft: 1,
              paddingTop: 1,
              paddingBottom: 1,
              borderRadius: 2,
              backgroundColor: (theme) => theme.palette.background.paper,
            },
          }}
        />
      )}
    </Stack>
  );
};

export default memo(MyBookings);
