import { getModernOrganizationBookingBaseLink } from '@/components/organization';
import type { myBookings_bookings_query$key } from '@/queries/__generated__/myBookings_bookings_query.graphql';
import type { myBookings_bookings_refetchableFragment } from '@/queries/__generated__/myBookings_bookings_refetchableFragment.graphql';
import type { myBookings_deleteBookingMutation } from '@/queries/__generated__/myBookings_deleteBookingMutation.graphql';
import type { myBookings_query$key } from '@/queries/__generated__/myBookings_query.graphql';
import AvatarGroup from '@mui/material/AvatarGroup';
import Divider from '@mui/material/Divider';
import Grid from '@mui/material/Grid2';
import IconButton from '@mui/material/IconButton';
import Box from '@mui/system/Box';
import type { GridColDef } from '@mui/x-data-grid';
import { DataGrid } from '@mui/x-data-grid';
import { CustomerAvatar } from '@repo/shared/components/avatars';
import { GridContainer, SectionIconTypography, SmallIconTypography, StackColumn } from '@repo/shared/components/commons';
import { EllipseMenuIcon } from '@repo/shared/components/icons';
import {
  MoreActionsMenu,
  moreActionsMenuAllOptions,
  MoreActionsMenuItemType,
  MoreActionsMenuOptionType,
} from '@repo/shared/components/moreActionsMenu';
import {
  errorNotificationOptions,
  infoNotificationOptions,
  NotificationContent,
  successNotificationOptions,
} from '@repo/shared/components/notification';
import { Zones } from '@repo/shared/components/zone';
import { PaletteModeContext, UpdateGlobalReloadIdContext } from '@repo/shared/libs/providers';
import { defaultGridStyle, defaultPadding } from '@repo/shared/libs/theme';
import { getCustomerFullName, joinErrors, toShortDate, toShortDateWithAdditionalDayInfo } from '@repo/shared/libs/utils';
import dayjs, { Dayjs } from 'dayjs';
import { nanoid } from 'nanoid';
import { useRouter } from 'next/navigation';
import { memo, startTransition, useCallback, useContext, useEffect, useMemo, useState } from 'react';
import { graphql, useFragment, useMutation, useRefetchableFragment } from 'react-relay';
import { toast } from 'react-toastify';
import MyBookingCard from './my-booking-card';

type Props = {
  rootDataRelay: myBookings_query$key;
  rootDataBookingRelay: myBookings_bookings_query$key;
  onReloadRequired: () => void;
  organizationId: string;
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
  color?: string | null | undefined;
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

const MyBookings = ({ rootDataRelay, rootDataBookingRelay, organizationId, from, to, locationIds, teamIds, viewMode }: Props) => {
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
                customTags {
                  uniqueId
                  name
                  color
                }
                zones {
                  uniqueId
                  name
                  color
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

  const [commitDeleteBooking] = useMutation<myBookings_deleteBookingMutation>(graphql`
    mutation myBookings_deleteBookingMutation($connectionIds: [ID!]!, $input: DeleteBookingInput!) {
      deleteBooking(input: $input) {
        booking {
          id @deleteEdge(connections: $connectionIds)
        }
      }
    }
  `);

  const router = useRouter();
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const UpdateGlobalReloadId = useContext(UpdateGlobalReloadIdContext);
  const [selectedBookingId, setSelectedBookingId] = useState<null | string>(null);
  const [moreActionsAnchorEl, setMoreActionsAnchorEl] = useState<null | HTMLElement>(null);
  const moreActionsMenuOpen = Boolean(moreActionsAnchorEl);

  const moreActionsOption: MoreActionsMenuItemType[] = [
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.EditBooking],
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.DeleteBooking],
  ];

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

  const handleMoreActionsMenuItemClick = (id: MoreActionsMenuOptionType) => {
    const bookingId = selectedBookingId;
    setMoreActionsAnchorEl(null);
    setSelectedBookingId(null);

    if (!bookingId) {
      return;
    }

    switch (id) {
      case MoreActionsMenuOptionType.EditBooking:
        router.push(getModernOrganizationBookingBaseLink(organizationId, bookingId));

        break;

      case MoreActionsMenuOptionType.DeleteBooking:
        handleRemoveBookingClick(bookingId);
        break;
    }
  };

  const handleRemoveBookingClick = (id: string) => {
    const bookingDetails = myBookings.find((item) => item.id === id);
    if (!bookingDetails) {
      return;
    }

    const shortDateFormatFrom = toShortDate(bookingDetails.from);
    let bookingDetailsInfo = `for ${getCustomerFullName(bookingDetails.customer)}`;
    if (bookingDetails.location) {
      bookingDetailsInfo += ` at the "${bookingDetails.location!.name}"`;
    }

    bookingDetailsInfo += ` on ${shortDateFormatFrom}`;

    const toastId = themedToast(<NotificationContent content={`Removing booking '${bookingDetailsInfo}'...`} />, infoNotificationOptions);

    commitDeleteBooking({
      variables: {
        connectionIds,
        input: {
          clientMutationId: nanoid(),
          id: bookingDetails.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to remove booking ${bookingDetailsInfo}. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Booking ${bookingDetailsInfo} removed.`} />,
        });
        UpdateGlobalReloadId();
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove booking ${bookingDetailsInfo}.`} />,
        });
      },
    });
  };

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
      renderCell: (params) => <SmallIconTypography label={params.value?.name ?? 'N/A'} />,
      display: 'flex',
      minWidth: 200,
    },
    {
      field: 'team',
      headerName: 'Team',
      editable: false,
      renderCell: (params) => <SmallIconTypography label={params.value?.name ?? 'N/A'} />,
      display: 'flex',
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
        return <SmallIconTypography label={desks.length === 0 ? 'N/A' : desks} />;
      },
      display: 'flex',
      minWidth: 200,
    },
    {
      field: 'zones',
      headerName: 'Zones',
      editable: false,
      renderCell: (params) => (
        <Zones zones={params.value.map((zone: ZoneDetails) => ({ id: zone.uniqueId, name: zone.name, color: zone.color }))} hideIcon />
      ),
      display: 'flex',
      minWidth: 250,
    },
    {
      field: 'teammates',
      headerName: 'Teammates',
      editable: false,
      renderCell: (params) => (
        <AvatarGroup max={5}>
          {params.value?.map((customer: CustomerDetails) => (
            <CustomerAvatar key={customer?.uniqueId} name={customer} photo={{ url: customer?.photoUrl }} size="medium" showFullName />
          ))}
        </AvatarGroup>
      ),
      display: 'flex',
      minWidth: 200,
    },
    {
      field: 'date',
      headerName: 'Date',
      editable: false,
      sortable: true,
      renderCell: (params) => <SmallIconTypography label={params.value} />,
      display: 'flex',
      minWidth: 300,
    },
    {
      field: 'moreActions',
      headerName: '',
      editable: false,
      sortable: false,
      display: 'flex',
      renderCell: (params) => (
        <Box sx={{ display: 'flex', justifyContent: 'flex-end', width: '100%' }}>
          <IconButton
            onClick={(event: React.MouseEvent<HTMLElement>) => {
              setSelectedBookingId(params.id as string);
              setMoreActionsAnchorEl(event.currentTarget);
            }}
          >
            <EllipseMenuIcon />
          </IconButton>
        </Box>
      ),
      flex: 1,
    },
  ];

  if (!rootDataRefetchable.bookings) {
    return <></>;
  }

  return (
    <>
      <StackColumn sx={{ paddingLeft: defaultPadding, paddingRight: defaultPadding, paddingTop: defaultPadding }}>
        <SectionIconTypography label="My Bookings" />
        <Divider />
        <Box sx={{ paddingBottom: defaultPadding }} />

        {viewMode === 'grid' && (
          <GridContainer>
            {myBookings.map((myBooking) => {
              const key = convertDateToKey(myBooking.from);
              const otherTeammates = groupedBookingsByFromDate[key]?.filter(
                (booking) => booking.customer?.uniqueId !== rootData.me?.id && booking.location?.uniqueId === myBooking.location?.uniqueId,
              );

              return (
                <Grid key={myBooking.id}>
                  <MyBookingCard
                    bookingDetailsRelay={myBooking}
                    organizationId={organizationId}
                    connectionIds={connectionIds}
                    otherTeammates={otherTeammates!.map(({ customer }) => customer)}
                  />
                </Grid>
              );
            })}
          </GridContainer>
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
            sx={defaultGridStyle}
          />
        )}
      </StackColumn>

      <MoreActionsMenu
        anchorEl={moreActionsAnchorEl}
        open={moreActionsMenuOpen}
        onMenuItemClick={handleMoreActionsMenuItemClick}
        options={moreActionsOption}
      />
    </>
  );
};

export default memo(MyBookings);
