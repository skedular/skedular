import { CustomerAvatar } from '@/components/avatars';
import { GridContainer, SectionIconTypography, SmallIconTypography, StackColumn } from '@/components/commons';
import { CustomTags } from '@/components/customTag';
import { EllipseMenuIcon, JoinIcon } from '@/components/icons';
import { getOrganizationBookingBaseLink } from '@/components/links';
import { MoreActionsMenu, moreActionsMenuAllOptions, MoreActionsMenuItemType, MoreActionsMenuOptionType } from '@/components/moreActionsMenu';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import Resources from '@/components/resource/resources';
import { Zones } from '@/components/zone';
import { PaletteModeContext, useIntegratedPlatrform } from '@/libs/providers';
import { defaultGridStyle, defaultPadding } from '@/libs/theme';
import { dateRangeToShortDateWithAdditionalDayInfo, getCustomerFullName, joinErrors, toShortDate } from '@/libs/utils';
import type { bookings_addPrivateBookingMutation } from '@/queries/__generated__/bookings_addPrivateBookingMutation.graphql';
import type { bookings_bookings_query$key } from '@/queries/__generated__/bookings_bookings_query.graphql';
import type { bookings_bookings_refetchableFragment } from '@/queries/__generated__/bookings_bookings_refetchableFragment.graphql';
import type { bookings_deleteMarketplaceBookingMutation } from '@/queries/__generated__/bookings_deleteMarketplaceBookingMutation.graphql';
import type { bookings_deletePrivateBookingMutation } from '@/queries/__generated__/bookings_deletePrivateBookingMutation.graphql';
import type { bookings_query$key } from '@/queries/__generated__/bookings_query.graphql';
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
import BookingCard from './booking-card';

type Props = {
  rootDataRelay: bookings_query$key;
  rootDataBookingRelay: bookings_bookings_query$key;
  onReloadRequired: () => void;
  organizationUniqueAlphanumericName: string;
  from: Dayjs;
  to: Dayjs;
  locationIds: string[];
  teamIds: string[];
  customerIds: string[];
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
  avatar: CustomerDetails;
  user: CustomerDetails;
  location?: LocationDetails | null | undefined;
  team?: TeamDetails | null | undefined;
  resources: ReadonlyArray<ResourceDetails>;
  customTags: ReadonlyArray<CustomTagDetails>;
  zones: ReadonlyArray<ZoneDetails>;
  canJoinBooking: boolean;
  date: string;
};

const Bookings = ({ rootDataRelay, rootDataBookingRelay, organizationUniqueAlphanumericName, from, to, locationIds, teamIds, customerIds, viewMode }: Props) => {
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
            organizationUniqueAlphanumericNames: [$organizationUniqueAlphanumericName]
            locationIds: $locationIds
            teamIds: $teamIds
            customerIds: $customerIds
            fromGte: $bookingsSearchCriteriaFrom
            fromLte: $bookingsSearchCriteriaTo
          }
          orderBy: [{ field: FROM, direction: ASCENDING }]
        ) @connection(key: "bookings_bookings") {
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
              category {
                category
                name
              }
              involvedCustomers {
                id
                name
                givenName
                middleName
                familyName
                photoUrl
              }
              involvedOrganizations {
                id
              }
              involvedLocations {
                id
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
              ...bookingCard_BookingDetails
            }
          }
        }
      }
    `,
    rootDataBookingRelay,
  );

  const [commitDeletePrivateBooking] = useMutation<bookings_deletePrivateBookingMutation>(graphql`
    mutation bookings_deletePrivateBookingMutation($connectionIds: [ID!]!, $input: DeletePrivateBookingInput!) {
      deletePrivateBooking(input: $input) {
        booking {
          id @deleteEdge(connections: $connectionIds)
        }
      }
    }
  `);

  const [commitDeleteMarketplaceBooking] = useMutation<bookings_deleteMarketplaceBookingMutation>(graphql`
    mutation bookings_deleteMarketplaceBookingMutation($connectionIds: [ID!]!, $input: DeleteMarketplaceBookingInput!) {
      deleteMarketplaceBooking(input: $input) {
        booking {
          id @deleteEdge(connections: $connectionIds)
        }
      }
    }
  `);

  const [commitAddPrivateBooking] = useMutation<bookings_addPrivateBookingMutation>(graphql`
    mutation bookings_addPrivateBookingMutation($connectionIds: [ID!]!, $input: AddPrivateBookingInput!) @raw_response_type {
      addPrivateBooking(input: $input) {
        booking @appendNode(connections: $connectionIds, edgeTypeName: "BookingDetails") {
          id
          from
          until
          notes
          category {
            category
            name
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
            id
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
        router.push(getOrganizationBookingBaseLink(integratedPlatrform, organizationUniqueAlphanumericName, bookingId));

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
              render: <NotificationContent content={`Failed to remove booking ${bookingDetailsInfo}. Error: ${joinErrors(errors)}.`} />,
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
            render: <NotificationContent content={`Failed to remove booking ${bookingDetailsInfo}. Error: ${error.message}.`} />,
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
              render: <NotificationContent content={`Failed to remove booking ${bookingDetailsInfo}. Error: ${joinErrors(errors)}.`} />,
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
            render: <NotificationContent content={`Failed to remove booking ${bookingDetailsInfo}. Error: ${error.message}.`} />,
          });
        },
      });
    }
  };

  const handleJoinClick = (bookingId: string) => {
    const bookingDetails = bookings.find((item) => item.id === bookingId);
    if (!bookingDetails) {
      return;
    }

    const shortDateFormatFrom = toShortDate(bookingDetails.from);
    const id = uuid();
    const toastId = themedToast(<NotificationContent content={`Joining booking on '${shortDateFormatFrom}'...`} />, infoNotificationOptions);

    commitAddPrivateBooking({
      variables: {
        connectionIds,
        input: {
          clientMutationId: uuid(),
          id,
          customerIds: [rootData.me.id],
          from: bookingDetails.from,
          until: bookingDetails.until,
          organizationIds: bookingDetails.involvedOrganizations.map(({ id }) => id),
          teamIds: [],
          resourceIds: [],
          category: bookingDetails.category.category,
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

        const booking = response.addPrivateBooking?.booking;
        let message = `Booking made for ${getCustomerFullName(booking.involvedCustomers[0])} to work`;

        if (booking.involvedLocations.length > 0) {
          message += ` from the "${booking.involvedLocations[0].name}"`;
        }

        if (booking.bookingResources.length > 0) {
          message += ` at resource "${booking.bookingResources.map(({ resource }) => resource.name).join(', ')}"`;

          const zones = booking.bookingResources.flatMap(({ resource }) => resource.zones);
          if (zones.length > 0) {
            const uniqueZones = Array.from(zones.reduce((map, zone) => map.set(zone.id, zone), new Map()).values());

            message += ` in "${uniqueZones.map(({ name }) => name).join(', ')}"`;
          }
        }

        message += ` on ${toShortDate(booking.from)}.`;

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={message} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to make a booking '${shortDateFormatFrom}'. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        addPrivateBooking: {
          booking: {
            id,
            from: bookingDetails.from,
            until: bookingDetails.until,
            notes: null,
            category: {
              category: bookingDetails.category.category,
              name: bookingDetails.category.name,
            },
            involvedCustomers: [
              {
                id: rootData.me.id,
                name: rootData.me.name,
                givenName: rootData.me.givenName,
                middleName: rootData.me.middleName,
                familyName: rootData.me.familyName,
                photoUrl: rootData.me.photoUrl,
              },
            ],
            involvedLocations: [],
            involvedTeams: [],
            bookingResources: [],
          },
        },
      },
    });
  };

  const rows: RowType[] = bookings.map((booking) => {
    const customTags = booking.bookingResources
      .flatMap(({ resource }) => resource.customTags)
      .reduce((acc: CustomTagDetails[], customTag) => {
        if (!acc.some((item) => item.id === customTag.id)) {
          acc.push(customTag);
        }

        return acc;
      }, []);
    const zones = booking.bookingResources
      .flatMap(({ resource }) => resource.zones)
      .reduce((acc: ZoneDetails[], zone) => {
        if (!acc.some((item) => item.id === zone.id)) {
          acc.push(zone);
        }

        return acc;
      }, []);

    const canJoinBooking = booking.involvedCustomers.some((item) => item.id === rootData.me?.id)
      ? false
      : !!!bookings
          .filter((otherBooking) => otherBooking.involvedCustomers.some((item) => item.id === rootData.me?.id))
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
      resources: booking.bookingResources.map(({ resource }) => resource),
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
    return null;
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
              const canJoinBooking = booking.involvedCustomers.some((item) => item.id === rootData.me?.id)
                ? false
                : !!!bookings
                    .filter((otherBooking) => otherBooking.involvedCustomers.some((item) => item.id === rootData.me?.id))
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
                    organizationUniqueAlphanumericName={organizationUniqueAlphanumericName}
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
