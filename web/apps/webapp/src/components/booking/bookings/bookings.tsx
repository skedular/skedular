import { CustomerAvatar } from '@/components/avatars';
import { GridContainer, SectionIconTypography, SmallIconTypography, StackColumn } from '@/components/commons';
import { CustomTags } from '@/components/customTag';
import { EllipseMenuIcon, JoinIcon } from '@/components/icons';
import { getOrganizationBookingBaseLink } from '@/components/links';
import { MoreActionsMenu, moreActionsMenuAllOptions, MoreActionsMenuItemType, MoreActionsMenuOptionType } from '@/components/moreActionsMenu';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import Resources from '@/components/resource/resources';
import { Zones } from '@/components/zone';
import { PaletteModeContext, UpdateGlobalReloadIdContext } from '@/libs/providers';
import { defaultGridStyle, defaultPadding } from '@/libs/theme';
import { dateRangeToShortDateWithAdditionalDayInfo, getCustomerFullName, joinErrors, toShortDate } from '@/libs/utils';
import type { bookings_addBookingMutation } from '@/queries/__generated__/bookings_addBookingMutation.graphql';
import type { bookings_bookings_query$key } from '@/queries/__generated__/bookings_bookings_query.graphql';
import type { bookings_bookings_refetchableFragment } from '@/queries/__generated__/bookings_bookings_refetchableFragment.graphql';
import type { bookings_deleteBookingMutation } from '@/queries/__generated__/bookings_deleteBookingMutation.graphql';
import type { bookings_query$key } from '@/queries/__generated__/bookings_query.graphql';
import Divider from '@mui/material/Divider';
import Grid from '@mui/material/Grid';
import IconButton from '@mui/material/IconButton';
import Box from '@mui/system/Box';
import type { GridColDef } from '@mui/x-data-grid';
import { DataGrid } from '@mui/x-data-grid';
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
  customerIds: string[];
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

type ResourceDetails = {
  uniqueId: string;
  name: string | null | undefined;
  color?: string | null | undefined;
};

type TeamDetails = {
  name: string;
};

type RowType = {
  id: string;
  avatar: CustomerDetails;
  user: CustomerDetails;
  location?: LocationDetails | null | undefined;
  team?: TeamDetails | null | undefined;
  resources: ReadonlyArray<ResourceDetails>;
  customTags: ReadonlyArray<CustomTagDetails>;
  zones: ReadonlyArray<ZoneDetails>;
  canJoinBooking: Boolean;
  date: string;
};

const Bookings = ({ rootDataRelay, rootDataBookingRelay, organizationId, from, to, locationIds, teamIds, customerIds, viewMode }: Props) => {
  const rootData = useFragment<bookings_query$key>(
    graphql`
      fragment bookings_query on Query {
        me {
          id
          name
          givenName
          middleName
          familyName
          photoUrl
        }
        ...bookingCard_query
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
            customerIds: $customerIds
            fromGte: $bookingsSearchCriteriaFrom
            fromLte: $bookingsSearchCriteriaTo
          }
          orderBy: [{ field: From, direction: Ascending }]
        ) @connection(key: "bookings_bookings") {
          __id
          totalCount
          edges {
            node {
              id
              from
              until
              notes
              type {
                type
                name
              }
              involvedCustomers {
                uniqueId
                name
                givenName
                middleName
                familyName
                photoUrl
              }
              involvedOrganizations {
                uniqueId
              }
              involvedLocations {
                uniqueId
                name
              }
              involvedTeams {
                uniqueId
                name
              }
              resources {
                uniqueId
                name
                color
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

  const [commitAddBooking] = useMutation<bookings_addBookingMutation>(graphql`
    mutation bookings_addBookingMutation($connectionIds: [ID!]!, $input: AddBookingInput!) @raw_response_type {
      addBooking(input: $input) {
        booking @appendNode(connections: $connectionIds, edgeTypeName: "BookingDetails") {
          id
          from
          until
          notes
          type {
            type
            name
          }
          involvedCustomers {
            uniqueId
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
            uniqueId
            name
          }
          resources {
            uniqueId
            name
            color
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

  const bookings = useMemo(() => rootDataRefetchable.bookings.edges.map((edge) => edge.node), [rootDataRefetchable.bookings]);
  const connectionIds = useMemo(() => [rootDataRefetchable.bookings.__id], [rootDataRefetchable.bookings]);

  const convertDateToKey = (date: Dayjs) => dayjs(date).format('YYYY-MM-DD');

  const handleRefetch = useCallback(
    (from: Dayjs, to: Dayjs, locationIds: string[], teamIds: string[], customerIds: string[]) => {
      startTransition(() => {
        refetch(
          {
            bookingsSearchCriteriaFrom: from.toISOString(),
            bookingsSearchCriteriaTo: to.toISOString(),
            locationIds,
            teamIds,
            customerIds,
          },
          {
            fetchPolicy: 'store-and-network',
          },
        );
      });
    },
    [refetch],
  );

  useEffect(() => handleRefetch(from, to, locationIds, teamIds, customerIds), [handleRefetch, from, to, locationIds, teamIds, customerIds]);

  const handleMoreActionsMenuItemClick = (id: MoreActionsMenuOptionType) => {
    const bookingId = selectedBookingId;
    setMoreActionsAnchorEl(null);
    setSelectedBookingId(null);

    if (!bookingId) {
      return;
    }

    switch (id) {
      case MoreActionsMenuOptionType.EditBooking:
        router.push(getOrganizationBookingBaseLink(organizationId, bookingId));

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
    let bookingDetailsInfo = `for ${getCustomerFullName(bookingDetails.involvedCustomers[0])}`;
    if (bookingDetails.involvedLocations.length > 0) {
      bookingDetailsInfo += ` at the "${bookingDetails.involvedLocations[0].name}"`;
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

  const handleJoinClick = (bookingId: string) => {
    if (!rootData.me) {
      return;
    }

    const bookingDetails = bookings.find((item) => item.id === bookingId);
    if (!bookingDetails) {
      return;
    }

    const shortDateFormatFrom = toShortDate(bookingDetails.from);
    const id = nanoid();
    const toastId = themedToast(<NotificationContent content={`Joining booking on '${shortDateFormatFrom}'...`} />, infoNotificationOptions);

    commitAddBooking({
      variables: {
        connectionIds,
        input: {
          clientMutationId: nanoid(),
          id,
          customerIds: [rootData.me.id],
          from: bookingDetails.from,
          until: bookingDetails.until,
          organizationIds: bookingDetails.involvedOrganizations.map(({ uniqueId }) => uniqueId),
          teamIds: [],
          resourceIds: [],
          productVersionIds: [],
          type: bookingDetails.type.type,
        },
      },
      onCompleted: (response, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to make a booking '${shortDateFormatFrom}'. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        const booking = response.addBooking?.booking!;
        let message = `Booking made for ${getCustomerFullName(booking.involvedCustomers[0])} to work`;

        if (booking.involvedLocations.length > 0) {
          message += ` from the "${booking.involvedLocations[0].name}"`;
        }

        if (booking.resources.length > 0) {
          message += ` at resource "${booking.resources.map(({ name }) => name).join(', ')}"`;

          const zones = booking.resources.flatMap(({ zones }) => zones);
          if (zones.length > 0) {
            const uniqueZones = Array.from(zones.reduce((map, zone) => map.set(zone.uniqueId, zone), new Map()).values());

            message += ` in "${uniqueZones.map(({ name }) => name).join(', ')}"`;
          }
        }

        message += ` on ${toShortDate(booking.from)}.`;

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={message} />,
        });

        UpdateGlobalReloadId();
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to make a booking '${shortDateFormatFrom}'. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        addBooking: {
          booking: {
            id,
            from: bookingDetails.from,
            until: bookingDetails.until,
            notes: null,
            type: {
              type: bookingDetails.type.type,
              name: bookingDetails.type.name,
            },
            involvedCustomers: [
              {
                uniqueId: rootData.me.id,
                name: rootData.me.name,
                givenName: rootData.me.givenName,
                middleName: rootData.me.middleName,
                familyName: rootData.me.familyName,
                photoUrl: rootData.me.photoUrl,
              },
            ],
            involvedLocations: [],
            involvedTeams: [],
            resources: [],
          },
        },
      },
    });
  };

  const rows: RowType[] = bookings.map((booking) => {
    const customTags = booking.resources
      .flatMap(({ customTags }) => customTags)
      .reduce((acc: CustomTagDetails[], customTag) => {
        if (!acc.some((item) => item.uniqueId === customTag.uniqueId)) {
          acc.push(customTag);
        }

        return acc;
      }, []);
    const zones = booking.resources
      .flatMap(({ zones }) => zones)
      .reduce((acc: ZoneDetails[], zone) => {
        if (!acc.some((item) => item.uniqueId === zone.uniqueId)) {
          acc.push(zone);
        }

        return acc;
      }, []);

    const canJoinBooking = booking.involvedCustomers.some((item) => item.uniqueId === rootData.me?.id)
      ? false
      : !!!bookings
          .filter((otherBooking) => otherBooking.involvedCustomers.some((item) => item.uniqueId === rootData.me?.id))
          .find((myBooking) => {
            const from = dayjs(booking.from);
            const myFrom = dayjs(myBooking.from);

            return from.year() === myFrom.year() && from.month() === myFrom.month() && from.date() === myFrom.date();
          });

    return {
      id: booking.id,
      avatar: booking.involvedCustomers[0],
      user: booking.involvedCustomers[0],
      location: booking.involvedLocations.length > 0 ? booking.involvedLocations[0] : null,
      team: booking.involvedTeams.length > 0 ? booking.involvedTeams[0] : null,
      resources: booking.resources,
      customTags,
      zones,
      date: dateRangeToShortDateWithAdditionalDayInfo(dayjs(booking.from), dayjs(booking.until)),
      canJoinBooking,
    };
  });

  const columns: GridColDef<(typeof rows)[number]>[] = [
    {
      field: 'avatar',
      headerName: '',
      editable: false,
      renderCell: (params) => <CustomerAvatar name={params.value} photo={{ url: params.value?.photoUrl }} size="medium" showFullName />,
      display: 'flex',
      maxWidth: 20,
    },
    {
      field: 'user',
      headerName: 'User',
      editable: false,
      renderCell: (params) => <SmallIconTypography label={getCustomerFullName(params.value)} />,
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
      field: 'resources',
      headerName: 'Resources',
      editable: false,
      renderCell: (params) => (
        <Resources resources={params.value.map((resource: ResourceDetails) => ({ id: resource.uniqueId, name: resource.name, color: resource.color }))} hideIcon />
      ),
      display: 'flex',
      minWidth: 150,
    },
    {
      field: 'customTags',
      headerName: 'Tags',
      editable: false,
      renderCell: (params) => <CustomTags customTags={params.value.map((zone: CustomTagDetails) => ({ id: zone.uniqueId, name: zone.name, color: zone.color }))} hideIcon />,
      display: 'flex',
      minWidth: 150,
    },
    {
      field: 'zones',
      headerName: 'Zones',
      editable: false,
      renderCell: (params) => <Zones zones={params.value.map((zone: ZoneDetails) => ({ id: zone.uniqueId, name: zone.name, color: zone.color }))} hideIcon />,
      display: 'flex',
      minWidth: 150,
    },
    {
      field: 'canJoinBooking',
      headerName: 'Join',
      editable: false,
      renderCell: (params) => {
        if (!params.value) {
          return <></>;
        }

        return (
          <IconButton onClick={() => handleJoinClick(params.id as string)}>
            <JoinIcon />
          </IconButton>
        );
      },
      display: 'flex',
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
              const canJoinBooking = booking.involvedCustomers.some((item) => item.uniqueId === rootData.me?.id)
                ? false
                : !!!bookings
                    .filter((otherBooking) => otherBooking.involvedCustomers.some((item) => item.uniqueId === rootData.me?.id))
                    .find((myBooking) => {
                      const from = dayjs(booking.from);
                      const myFrom = dayjs(myBooking.from);

                      return from.year() === myFrom.year() && from.month() === myFrom.month() && from.date() === myFrom.date();
                    });

              return (
                <Grid key={booking.id}>
                  <BookingCard
                    rootDataRelay={rootData}
                    bookingDetailsRelay={booking}
                    organizationId={organizationId}
                    connectionIds={connectionIds}
                    canJoinBooking={canJoinBooking}
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

export default memo(Bookings);
