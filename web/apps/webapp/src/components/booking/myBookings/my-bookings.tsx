import { CustomerAvatar } from '@/components/avatars';
import { GridContainer, SectionIconTypography, SmallIconTypography, StackColumn } from '@/components/commons';
import { CustomTags } from '@/components/customTag';
import { EllipseMenuIcon } from '@/components/icons';
import { getOrganizationBookingBaseLink } from '@/components/links';
import { MoreActionsMenu, moreActionsMenuAllOptions, MoreActionsMenuItemType, MoreActionsMenuOptionType } from '@/components/moreActionsMenu';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import Resources from '@/components/resource/resources';
import { Zones } from '@/components/zone';
import { PaletteModeContext, useIntegratedPlatrform } from '@/libs/providers';
import { defaultGridStyle, defaultPadding } from '@/libs/theme';
import { dateRangeToShortDateWithAdditionalDayInfo, getCustomerFullName, getRelayErrorMessage, toShortDate } from '@/libs/utils';
import type { myBookings_bookings_query$key } from '@/queries/__generated__/myBookings_bookings_query.graphql';
import type { myBookings_bookings_refetchableFragment } from '@/queries/__generated__/myBookings_bookings_refetchableFragment.graphql';
import type { myBookings_deleteMarketplaceBookingMutation } from '@/queries/__generated__/myBookings_deleteMarketplaceBookingMutation.graphql';
import type { myBookings_deletePrivateBookingMutation } from '@/queries/__generated__/myBookings_deletePrivateBookingMutation.graphql';
import type { myBookings_query$key } from '@/queries/__generated__/myBookings_query.graphql';
import AvatarGroup from '@mui/material/AvatarGroup';
import Divider from '@mui/material/Divider';
import Grid from '@mui/material/Grid';
import IconButton from '@mui/material/IconButton';
import Box from '@mui/system/Box';
import type { GridColDef } from '@mui/x-data-grid';
import { DataGrid } from '@mui/x-data-grid';
import dayjs, { Dayjs } from 'dayjs';
import { useRouter } from 'next/navigation';
import { memo, startTransition, useCallback, useContext, useEffect, useMemo, useState } from 'react';
import { graphql, useFragment, useMutation, useRefetchableFragment } from 'react-relay';
import { toast } from 'react-toastify';
import { v7 as uuid } from 'uuid';
import MyBookingCard from './my-booking-card';

type Props = {
  rootDataRelay: myBookings_query$key;
  rootDataBookingRelay: myBookings_bookings_query$key;
  onReloadRequired: () => void;
  organizationCustomDomain: string;
  from: Dayjs;
  to: Dayjs;
  locationIds: string[];
  teamIds: string[];
  viewMode: 'list' | 'grid';
};

type CustomerDetails = {
  id: string;
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
  id: string;
  name: string | null | undefined;
  color?: string | null | undefined;
};

type ZoneDetails = {
  id: string;
  name: string | null | undefined;
  color?: string | null | undefined;
};

type ResourceDetails = {
  id: string;
  name: string | null | undefined;
  color?: string | null | undefined;
};

type TeamDetails = {
  name: string;
};

type RowType = {
  id: string;
  location?: LocationDetails | null | undefined;
  team?: TeamDetails | null | undefined;
  resources: ReadonlyArray<ResourceDetails>;
  customTags: ReadonlyArray<CustomTagDetails>;
  zones: ReadonlyArray<ZoneDetails>;
  teammates: ReadonlyArray<CustomerDetails>;
  date: string;
};

const MyBookings = ({ rootDataRelay, rootDataBookingRelay, organizationCustomDomain, from, to, locationIds, teamIds, viewMode }: Props) => {
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
            organizationCustomDomain: $organizationCustomDomain
            locationIds: $locationIds
            teamIds: $teamIds
            fromGte: $bookingsSearchCriteriaFrom
            fromLte: $bookingsSearchCriteriaTo
          }
          orderBy: [{ field: FROM, direction: ASCENDING }]
        ) @connection(key: "myBookings_bookings") {
          __id
          totalCount
          edges {
            node {
              id
              from
              until
              notes
              channel {
                channel
              }
              involvedCustomers {
                id
                name
                givenName
                middleName
                familyName
                photoUrl
              }
              involvedLocations {
                uniqueId
                name
              }
              involvedTeams {
                id
                name
              }
              bookingResources {
                resource {
                  id
                  name
                  color
                  customTags {
                    id
                    name
                    color
                  }
                  zones {
                    id
                    name
                    color
                  }
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

  const [commitDeletePrivateBooking] = useMutation<myBookings_deletePrivateBookingMutation>(graphql`
    mutation myBookings_deletePrivateBookingMutation($connectionIds: [ID!]!, $input: DeletePrivateBookingInput!) {
      deletePrivateBooking(input: $input) {
        booking {
          id @deleteEdge(connections: $connectionIds)
        }
      }
    }
  `);

  const [commitDeleteMarketplaceBooking] = useMutation<myBookings_deleteMarketplaceBookingMutation>(graphql`
    mutation myBookings_deleteMarketplaceBookingMutation($connectionIds: [ID!]!, $input: DeleteMarketplaceBookingInput!) {
      deleteMarketplaceBooking(input: $input) {
        booking {
          id @deleteEdge(connections: $connectionIds)
        }
      }
    }
  `);

  const { integratedPlatrform } = useIntegratedPlatrform();
  const router = useRouter();
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const [selectedBookingId, setSelectedBookingId] = useState<null | string>(null);
  const [moreActionsAnchorEl, setMoreActionsAnchorEl] = useState<null | HTMLElement>(null);
  const moreActionsMenuOpen = Boolean(moreActionsAnchorEl);

  const moreActionsOption: MoreActionsMenuItemType[] = [
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.EditBooking],
    moreActionsMenuAllOptions[MoreActionsMenuOptionType.DeleteBooking],
  ];

  const bookings = useMemo(() => rootDataRefetchable.bookings.edges.map((edge) => edge.node), [rootDataRefetchable.bookings]);
  const connectionIds = useMemo(() => [rootDataRefetchable.bookings.__id], [rootDataRefetchable.bookings]);
  const myBookings = useMemo(() => bookings.filter((booking) => booking.involvedCustomers.some((item) => item.id === rootData.me?.id)), [bookings, rootData.me?.id]);

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
        router.push(getOrganizationBookingBaseLink(integratedPlatrform, organizationCustomDomain, bookingId));

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
    let bookingDetailsInfo = `for ${getCustomerFullName(bookingDetails.involvedCustomers[0])}`;
    if (bookingDetails.involvedLocations.length > 0) {
      bookingDetailsInfo += ` at the "${bookingDetails.involvedLocations[0]!.name}"`;
    }

    bookingDetailsInfo += ` on ${shortDateFormatFrom}`;

    const toastId = themedToast(<NotificationContent content={`Removing booking '${bookingDetailsInfo}'...`} />, infoNotificationOptions);

    if (bookingDetails.channel.channel === 'PRIVATE') {
      commitDeletePrivateBooking({
        variables: {
          connectionIds,
          input: {
            clientMutationId: uuid(),
            id: bookingDetails.id,
          },
        },
        onCompleted: (_, errors) => {
          if (errors && errors.length > 0) {
            toast.update(toastId, {
              ...errorNotificationOptions,
              render: <NotificationContent content={`Failed to remove booking ${bookingDetailsInfo}. Error: ${getRelayErrorMessage(errors)}.`} />,
            });

            return;
          }

          toast.update(toastId, {
            ...successNotificationOptions,
            render: <NotificationContent content={`Booking ${bookingDetailsInfo} removed.`} />,
          });
        },
        onError: (error) => {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to remove booking ${bookingDetailsInfo}. Error: ${getRelayErrorMessage(error)}.`} />,
          });
        },
      });
    } else {
      commitDeleteMarketplaceBooking({
        variables: {
          connectionIds,
          input: {
            clientMutationId: uuid(),
            id: bookingDetails.id,
          },
        },
        onCompleted: (_, errors) => {
          if (errors && errors.length > 0) {
            toast.update(toastId, {
              ...errorNotificationOptions,
              render: <NotificationContent content={`Failed to remove booking ${bookingDetailsInfo}. Error: ${getRelayErrorMessage(errors)}.`} />,
            });

            return;
          }

          toast.update(toastId, {
            ...successNotificationOptions,
            render: <NotificationContent content={`Booking ${bookingDetailsInfo} removed.`} />,
          });
        },
        onError: (error) => {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to remove booking ${bookingDetailsInfo}. Error: ${getRelayErrorMessage(error)}.`} />,
          });
        },
      });
    }
  };

  const rows: RowType[] = myBookings.map((myBooking) => {
    const customTags = myBooking.bookingResources
      .flatMap(({ resource }) => resource.customTags)
      .reduce((acc: CustomTagDetails[], customTag) => {
        if (!acc.some((item) => item.id === customTag.id)) {
          acc.push(customTag);
        }

        return acc;
      }, []);
    const zones = myBooking.bookingResources
      .flatMap(({ resource }) => resource.zones)
      .reduce((acc: ZoneDetails[], zone) => {
        if (!acc.some((item) => item.id === zone.id)) {
          acc.push(zone);
        }

        return acc;
      }, []);

    const key = convertDateToKey(myBooking.from);
    const teammates: CustomerDetails[] =
      groupedBookingsByFromDate[key]
        ?.filter(
          (booking) =>
            booking.involvedCustomers.some((item) => item.id !== rootData.me?.id) &&
            booking.involvedLocations.some((item) => myBooking.involvedLocations.some((item2) => item.uniqueId === item2.uniqueId)),
        )
        .flatMap((booking) => booking.involvedCustomers) ?? [];

    return {
      id: myBooking.id,
      location: myBooking.involvedLocations.length > 0 ? myBooking.involvedLocations[0] : null,
      team: myBooking.involvedTeams.length > 0 ? myBooking.involvedTeams[0] : null,
      resources: myBooking.bookingResources.map(({ resource }) => resource),
      customTags,
      zones,
      teammates,
      date: dateRangeToShortDateWithAdditionalDayInfo(dayjs(myBooking.from), dayjs(myBooking.until)),
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
      field: 'date',
      headerName: 'Date',
      editable: false,
      sortable: true,
      renderCell: (params) => <SmallIconTypography label={params.value} />,
      display: 'flex',
      minWidth: 220,
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
      field: 'resources',
      headerName: 'Resources',
      editable: false,
      renderCell: (params) => <Resources resources={params.value.map((resource: ResourceDetails) => ({ id: resource.id, name: resource.name, color: resource.color }))} hideIcon />,
      display: 'flex',
      minWidth: 150,
    },
    {
      field: 'customTags',
      headerName: 'Tags',
      editable: false,
      renderCell: (params) => <CustomTags customTags={params.value.map((zone: CustomTagDetails) => ({ id: zone.id, name: zone.name, color: zone.color }))} hideIcon />,
      display: 'flex',
      minWidth: 150,
    },
    {
      field: 'zones',
      headerName: 'Zones',
      editable: false,
      renderCell: (params) => <Zones zones={params.value.map((zone: ZoneDetails) => ({ id: zone.id, name: zone.name, color: zone.color }))} hideIcon />,
      display: 'flex',
      minWidth: 150,
    },
    {
      field: 'teammates',
      headerName: 'Teammates',
      editable: false,
      renderCell: (params) => (
        <AvatarGroup max={5}>
          {params.value?.map((customer: CustomerDetails) => (
            <CustomerAvatar key={customer?.id} name={customer} photo={{ url: customer?.photoUrl }} size="medium" showFullName />
          ))}
        </AvatarGroup>
      ),
      display: 'flex',
      minWidth: 200,
    },
    {
      field: 'More Actions',
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
    return null;
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
              const otherTeammates =
                groupedBookingsByFromDate[key]
                  ?.filter(
                    (booking) =>
                      booking.involvedCustomers.some((item) => item.id !== rootData.me?.id) &&
                      booking.involvedLocations.some((item) => myBooking.involvedLocations.some((item2) => item.uniqueId === item2.uniqueId)),
                  )
                  .flatMap((booking) => booking.involvedCustomers) ?? [];

              return (
                <Grid key={myBooking.id}>
                  <MyBookingCard
                    bookingDetailsRelay={myBooking}
                    organizationCustomDomain={organizationCustomDomain}
                    connectionIds={connectionIds}
                    otherTeammates={otherTeammates}
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
            localeText={{ noRowsLabel: 'No booking found' }}
          />
        )}
      </StackColumn>

      <MoreActionsMenu anchorEl={moreActionsAnchorEl} open={moreActionsMenuOpen} onMenuItemClick={handleMoreActionsMenuItemClick} options={moreActionsOption} />
    </>
  );
};

export default memo(MyBookings);
