import { getOrganizationBookingBaseLink } from '@/components/links';
import type { bookings_addBookingMutation } from '@/queries/__generated__/bookings_addBookingMutation.graphql';
import type { bookings_bookings_query$key } from '@/queries/__generated__/bookings_bookings_query.graphql';
import type { bookings_bookings_refetchableFragment } from '@/queries/__generated__/bookings_bookings_refetchableFragment.graphql';
import type { bookings_deleteBookingMutation } from '@/queries/__generated__/bookings_deleteBookingMutation.graphql';
import type { bookings_query$key } from '@/queries/__generated__/bookings_query.graphql';
import Button from '@mui/material/Button';
import Divider from '@mui/material/Divider';
import Grid from '@mui/material/Grid2';
import IconButton from '@mui/material/IconButton';
import Box from '@mui/system/Box';
import type { GridColDef } from '@mui/x-data-grid';
import { DataGrid } from '@mui/x-data-grid';
import { CustomerAvatar } from '@repo/shared/components/avatars';
import { GridContainer, SectionIconTypography, SmallIconTypography, StackColumn } from '@repo/shared/components/commons';
import { CustomTags } from '@repo/shared/components/customTag';
import { EllipseMenuIcon, JoinIcon } from '@repo/shared/components/icons';
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
  avatar: CustomerDetails;
  user: CustomerDetails;
  location?: LocationDetails | null | undefined;
  team?: TeamDetails | null | undefined;
  desks: ReadonlyArray<DeskDetails>;
  customTags: ReadonlyArray<CustomTagDetails>;
  zones: ReadonlyArray<ZoneDetails>;
  canJoinBooking: Boolean;
};

const Bookings = ({ rootDataRelay, rootDataBookingRelay, organizationId, from, to, locationIds, teamIds, viewMode }: Props) => {
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
              organization {
                uniqueId
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

  const [commitAddBooking] = useMutation<bookings_addBookingMutation>(graphql`
    mutation bookings_addBookingMutation($connectionIds: [ID!]!, $input: AddBookingInput!) @raw_response_type {
      addBooking(input: $input) {
        booking @appendNode(connections: $connectionIds, edgeTypeName: "BookingDetails") {
          id
          from
          to
          notes
          type
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
    const type = 'WorkingFromOffice';

    commitAddBooking({
      variables: {
        connectionIds,
        input: {
          clientMutationId: nanoid(),
          id,
          customerId: rootData.me.id,
          from: bookingDetails.from,
          to: bookingDetails.to,
          organizationId: bookingDetails.organization?.uniqueId,
          locationId: bookingDetails.location?.uniqueId,
          teamId: bookingDetails.team?.uniqueId,
          deskIds: [],
          type,
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
        let message = `Booking made for ${getCustomerFullName(booking.customer)} to work`;

        if (booking.location) {
          message += ` from the "${booking.location!.name}"`;
        }

        if (booking.desks.length > 0) {
          message += ` at desk "${booking.desks.map(({ name }) => name).join(', ')}"`;

          const zones = booking.desks.flatMap(({ zones }) => zones);
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
            to: bookingDetails.to,
            notes: null,
            type,
            customer: {
              uniqueId: rootData.me.id,
              name: rootData.me.name,
              givenName: rootData.me.givenName,
              middleName: rootData.me.middleName,
              familyName: rootData.me.familyName,
              photoUrl: rootData.me.photoUrl,
            },
            location: bookingDetails.location
              ? {
                  uniqueId: bookingDetails.location.uniqueId,
                  name: bookingDetails.location.name,
                }
              : null,
            team: bookingDetails.team
              ? {
                  uniqueId: bookingDetails.team.uniqueId,
                  name: bookingDetails.team.name,
                }
              : null,
            desks: [],
          },
        },
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

    const canJoinBooking =
      booking.customer.uniqueId === rootData.me?.id
        ? false
        : !!!bookings
            .filter((otherBooking) => otherBooking.customer.uniqueId === rootData.me?.id)
            .find((myBooking) => {
              const from = dayjs(booking.from);
              const myFrom = dayjs(myBooking.from);

              return from.year() === myFrom.year() && from.month() === myFrom.month() && from.date() === myFrom.date();
            });

    return {
      id: booking.id,
      avatar: booking.customer,
      user: booking.customer,
      location: booking.location,
      team: booking.team,
      desks: booking.desks,
      customTags,
      zones,
      date: toShortDateWithAdditionalDayInfo(dayjs(booking.from)),
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
      minWidth: 200,
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
          <Button size="small" onClick={() => handleJoinClick(params.id as string)}>
            <JoinIcon />
          </Button>
        );
      },
      display: 'flex',
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
              const canJoinBooking =
                booking.customer.uniqueId === rootData.me?.id
                  ? false
                  : !!!bookings
                      .filter((otherBooking) => otherBooking.customer.uniqueId === rootData.me?.id)
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
