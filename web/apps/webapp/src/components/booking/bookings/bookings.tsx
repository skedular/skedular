import { getModernOrganizationBookingBaseLink } from '@/components/organization';
import type { bookings_bookings_query$key } from '@/queries/__generated__/bookings_bookings_query.graphql';
import type { bookings_bookings_refetchableFragment } from '@/queries/__generated__/bookings_bookings_refetchableFragment.graphql';
import type { bookings_deleteBookingMutation } from '@/queries/__generated__/bookings_deleteBookingMutation.graphql';
import type { bookings_query$key } from '@/queries/__generated__/bookings_query.graphql';
import Divider from '@mui/material/Divider';
import Grid from '@mui/material/Grid2';
import IconButton from '@mui/material/IconButton';
import Box from '@mui/system/Box';
import type { GridColDef } from '@mui/x-data-grid';
import { DataGrid } from '@mui/x-data-grid';
import { GridContainer, SectionIconTypography, SmallIconTypography, StackColumn } from '@repo/shared/components/commons';
import { CustomTags } from '@repo/shared/components/customTag';
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
import BookingCard from './booking-card';

type Props = {
  rootDataRelay: bookings_query$key;
  rootDataBookingRelay: bookings_bookings_query$key;
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

type CustomTagDetails = {
  uniqueId: string;
  name: string | null | undefined;
  color?: string | null | undefined;
};

type ZoneDetails = {
  uniqueId: string;
  name: string | null | undefined;
  color?: string | null | undefined;
};

type DeskDetails = {
  name: string;
};

type TeamDetails = {
  name: string;
};

type RowType = {
  id: string;
  user: CustomerDetails;
  location?: LocationDetails | null | undefined;
  team?: TeamDetails | null | undefined;
  desks: ReadonlyArray<DeskDetails>;
  customTags: ReadonlyArray<CustomTagDetails>;
  zones: ReadonlyArray<ZoneDetails>;
};

const Bookings = ({ rootDataRelay, rootDataBookingRelay, organizationId, from, to, locationIds, teamIds, viewMode }: Props) => {
  const rootData = useFragment<bookings_query$key>(
    graphql`
      fragment bookings_query on Query {
        me {
          id
        }
      }
    `,
    rootDataRelay,
  );

  const [rootDataRefetchable, refetch] = useRefetchableFragment<bookings_bookings_refetchableFragment, bookings_bookings_query$key>(
    graphql`
      fragment bookings_bookings_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: null })
      @refetchable(queryName: "bookings_bookings_refetchableFragment") {
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
        ) @connection(key: "bookings_bookings") {
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
              ...bookingCard_BookingDetails
            }
          }
        }
      }
    `,
    rootDataBookingRelay,
  );

  const [commitDeleteBooking] = useMutation<bookings_deleteBookingMutation>(graphql`
    mutation bookings_deleteBookingMutation($connectionIds: [ID!]!, $input: DeleteBookingInput!) {
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

  const convertDateToKey = (date: Dayjs) => dayjs(date).format('YYYY-MM-DD');

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
    const bookingDetails = bookings.find((item) => item.id === id);
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

  const rows: RowType[] = bookings.map((booking) => {
    const customTags = booking.desks
      .flatMap(({ customTags }) => customTags)
      .reduce((acc: CustomTagDetails[], customTag) => {
        if (!acc.some((item) => item.uniqueId === customTag.uniqueId)) {
          acc.push(customTag);
        }

        return acc;
      }, []);
    const zones = booking.desks
      .flatMap(({ zones }) => zones)
      .reduce((acc: ZoneDetails[], zone) => {
        if (!acc.some((item) => item.uniqueId === zone.uniqueId)) {
          acc.push(zone);
        }

        return acc;
      }, []);

    return {
      id: booking.id,
      user: booking.customer,
      location: booking.location,
      team: booking.team,
      desks: booking.desks,
      customTags,
      zones,
      date: toShortDateWithAdditionalDayInfo(dayjs(booking.from)),
    };
  });

  const columns: GridColDef<(typeof rows)[number]>[] = [
    {
      field: 'user',
      headerName: 'User',
      editable: false,
      renderCell: (params) => <SmallIconTypography label={getCustomerFullName(params.value)} />,
      display: 'flex',
      minWidth: 200,
    },
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
      minWidth: 150,
    },
    {
      field: 'customTags',
      headerName: 'Tags',
      editable: false,
      renderCell: (params) => (
        <CustomTags customTags={params.value.map((zone: CustomTagDetails) => ({ id: zone.uniqueId, name: zone.name, color: zone.color }))} hideIcon />
      ),
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
        <SectionIconTypography label="Bookings" />
        <Divider />
        <Box sx={{ paddingBottom: defaultPadding }} />

        {viewMode === 'grid' && (
          <GridContainer>
            {bookings.map((booking) => {
              const key = convertDateToKey(booking.from);

              return (
                <Grid key={booking.id}>
                  <BookingCard bookingDetailsRelay={booking} organizationId={organizationId} connectionIds={connectionIds} />
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

export default memo(Bookings);
