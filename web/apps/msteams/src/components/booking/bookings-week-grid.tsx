import Box from '@mui/material/Box';
import type { GetApplyQuickFilterFn, GridCallbackDetails, GridCellParams, GridColDef, MuiEvent } from '@mui/x-data-grid';
import { DataGrid, GridToolbarQuickFilter } from '@mui/x-data-grid';
import { CustomerAvatar } from '@repo/shared/components/avatars';
import { BookingIcon as BookingIconComponent } from '@repo/shared/components/booking';
import { TAG_TYPE_LOCATION_ZONE } from '@repo/shared/components/zone';
import { GlobalReloadIdContext, UpdateGlobalReloadIdContext } from '@repo/shared/libs/providers';
import { SnackbarAnchorOrigin as anchorOrigin } from '@repo/shared/libs/snackbar';
import { endOfDay, getCustomerFullName, joinErrors, toShortDate } from '@repo/shared/libs/utils';
import graphql from 'babel-plugin-relay/macro';
import { Dayjs } from 'dayjs';
import { nanoid } from 'nanoid';
import { useSnackbar } from 'notistack';
import { memo, useCallback, useContext, useEffect, useMemo, useTransition } from 'react';
import { useFragment, useMutation, usePaginationFragment } from 'react-relay';
import type { bookingsWeekGrid_addBookingMutation } from './__generated__/bookingsWeekGrid_addBookingMutation.graphql';
import type { bookingsWeekGrid_allBookings_query$key } from './__generated__/bookingsWeekGrid_allBookings_query.graphql';
import type { bookingsWeekGrid_allBookings_refetchableFragment } from './__generated__/bookingsWeekGrid_allBookings_refetchableFragment.graphql';
import type { bookingsWeekGrid_deleteBookingMutation } from './__generated__/bookingsWeekGrid_deleteBookingMutation.graphql';
import type { bookingsWeekGrid_query$key } from './__generated__/bookingsWeekGrid_query.graphql';

type Props = {
  rootDataRelay: bookingsWeekGrid_query$key;
  rootDataAllBookingsRelay: bookingsWeekGrid_allBookings_query$key;
  organizationId: string;
  locationId?: string;
  teamId?: string;
  customers: CustomerDetails[];
  startDate: Dayjs;
};

type CustomerDetails = {
  readonly uniqueId: string;
  readonly givenName?: string | null | undefined;
  readonly middleName?: string | null | undefined;
  readonly familyName?: string | null | undefined;
  readonly name?: string | null | undefined;
  readonly photoUrl?: string | null | undefined;
};

type LocationDetails = {
  readonly name?: string | null | undefined;
};

type LocationTagDetails = {
  readonly uniqueId: string;
  readonly name?: string | null | undefined;
  readonly tagType?: string | null | undefined;
};

type DeskDetails = {
  readonly name?: string | null | undefined;
  readonly locationTags: ReadonlyArray<LocationTagDetails>;
};

type TeamDetails = {
  readonly name?: string | null | undefined;
};

type BookingDetails = {
  readonly id: string;
  readonly customer: CustomerDetails;
  readonly location?: LocationDetails | null | undefined;
  readonly team?: TeamDetails | null | undefined;
  readonly desks: ReadonlyArray<DeskDetails>;
  readonly from: any;
  readonly to: any;
};

type BookingAndCustomerDetails = {
  customer: CustomerDetails;
  booking: BookingDetails | null | undefined;
};

type RowType = {
  id: string;
  person: CustomerDetails;
  Mon: BookingAndCustomerDetails;
  Tue: BookingAndCustomerDetails;
  Wed: BookingAndCustomerDetails;
  Thu: BookingAndCustomerDetails;
  Fri: BookingAndCustomerDetails;
  Sat: BookingAndCustomerDetails;
  Sun: BookingAndCustomerDetails;
};

const QuickSearchToolbar = () => <GridToolbarQuickFilter placeholder="Find a person..." />;

const BookingsWeekGrid = ({ rootDataRelay, rootDataAllBookingsRelay, organizationId, locationId, teamId, customers, startDate }: Props) => {
  const rootData = useFragment(
    graphql`
      fragment bookingsWeekGrid_query on Query {
        me {
          id
        }
        organizationBookingPermissions(organizationId: $organizationId) {
          canAddBookingOnBehalf
          canDeleteBookingOnBehalf
        }
        locationBookingPermissions(locationId: $locationId) @include(if: $locationExists) {
          canAddBookingOnBehalf
          canDeleteBookingOnBehalf
        }
        teamBookingPermissions(teamId: $teamId) @include(if: $teamExists) {
          canAddBookingOnBehalf
          canDeleteBookingOnBehalf
        }
      }
    `,
    rootDataRelay,
  );

  const { data: rootDataAllBookings, refetch } = usePaginationFragment<
    bookingsWeekGrid_allBookings_refetchableFragment,
    bookingsWeekGrid_allBookings_query$key
  >(
    graphql`
      fragment bookingsWeekGrid_allBookings_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: null })
      @refetchable(queryName: "bookingsWeekGrid_allBookings_refetchableFragment") {
        bookings(
          first: $count
          after: $cursor
          where: { organizationIds: [$organizationId], locationIds: [$locationId], teamIds: [$teamId], fromGTE: $from, toLT: $to }
        ) @connection(key: "bookingsWeekGrid_bookings") {
          __id
          totalCount
          edges {
            node {
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
                name
              }
              team {
                name
              }
              desks {
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
    rootDataAllBookingsRelay,
  );

  const [commitAddBooking] = useMutation<bookingsWeekGrid_addBookingMutation>(graphql`
    mutation bookingsWeekGrid_addBookingMutation($connectionIds: [ID!]!, $input: AddBookingInput!) @raw_response_type {
      addBooking(input: $input) {
        booking @appendNode(connections: $connectionIds, edgeTypeName: "BookingDetails") {
          id
          from
          to
          customer {
            uniqueId
            name
            givenName
            middleName
            familyName
          }
          organization {
            uniqueId
            name
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
  `);

  const [commitDeleteBooking] = useMutation<bookingsWeekGrid_deleteBookingMutation>(graphql`
    mutation bookingsWeekGrid_deleteBookingMutation($connectionIds: [ID!]!, $input: DeleteBookingInput!) {
      deleteBooking(input: $input) {
        booking {
          id @deleteEdge(connections: $connectionIds)
        }
      }
    }
  `);

  const globalReloadId = useContext(GlobalReloadIdContext);
  const UpdateGlobalReloadId = useContext(UpdateGlobalReloadIdContext);
  const { enqueueSnackbar } = useSnackbar();
  const [, startTransition] = useTransition();
  const handleRefetch = useCallback(
    (startDate: Dayjs) => {
      startTransition(() => {
        const endDate = startDate.add(1, 'week');

        refetch(
          {
            from: startDate.toISOString(),
            to: endDate.toISOString(),
          },
          {
            fetchPolicy: 'store-and-network',
          },
        );
      });
    },
    [refetch],
  );

  useEffect(() => {
    handleRefetch(startDate);
  }, [handleRefetch, globalReloadId, startDate]);

  const connectionIds = useMemo(() => (rootDataAllBookings.bookings ? [rootDataAllBookings.bookings.__id] : []), [rootDataAllBookings.bookings]);
  const allBookings = useMemo(
    () => (rootDataAllBookings.bookings?.edges ? rootDataAllBookings.bookings.edges.map(({ node }) => node) : []),
    [rootDataAllBookings.bookings],
  );

  if (!rootData.me || !rootDataAllBookings.bookings) {
    return <></>;
  }

  const day = startDate.day();
  const daysOfWeek = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'];
  const correctedDaysOfWeek = [...daysOfWeek.slice(day), ...daysOfWeek.slice(0, day)];
  const daysOfWeekMap = correctedDaysOfWeek.reduce(
    (acc, day, index) => {
      acc[day] = index;

      return acc;
    },
    {} as Record<string, number>,
  );

  const meAsMember = customers.find((customer) => customer.uniqueId === rootData.me!.id);
  const otherMembers = customers.filter((customer) => customer.uniqueId !== rootData.me!.id);
  let finalMembersList = otherMembers;
  if (meAsMember) {
    finalMembersList = [meAsMember, ...otherMembers];
  }

  const rows: RowType[] = finalMembersList
    .map((customer) => {
      const customerId = customer.uniqueId;

      return {
        id: customerId,
        person: customer,
        Mon: {
          customer,
          booking: allBookings.find(
            (booking) => booking.customer!.uniqueId === customerId && booking.from === startDate.add(daysOfWeekMap['Mon']!, 'day').toISOString(),
          ),
        },
        Tue: {
          customer,
          booking: allBookings.find(
            (booking) => booking.customer!.uniqueId === customerId && booking.from === startDate.add(daysOfWeekMap['Tue']!, 'day').toISOString(),
          ),
        },
        Wed: {
          customer,
          booking: allBookings.find(
            (booking) => booking.customer!.uniqueId === customerId && booking.from === startDate.add(daysOfWeekMap['Wed']!, 'day').toISOString(),
          ),
        },
        Thu: {
          customer,
          booking: allBookings.find(
            (booking) => booking.customer!.uniqueId === customerId && booking.from === startDate.add(daysOfWeekMap['Thu']!, 'day').toISOString(),
          ),
        },
        Fri: {
          customer,
          booking: allBookings.find(
            (booking) => booking.customer!.uniqueId === customerId && booking.from === startDate.add(daysOfWeekMap['Fri']!, 'day').toISOString(),
          ),
        },
        Sat: {
          customer,
          booking: allBookings.find(
            (booking) => booking.customer!.uniqueId === customerId && booking.from === startDate.add(daysOfWeekMap['Sat']!, 'day').toISOString(),
          ),
        },
        Sun: {
          customer,
          booking: allBookings.find(
            (booking) => booking.customer!.uniqueId === customerId && booking.from === startDate.add(daysOfWeekMap['Sun']!, 'day').toISOString(),
          ),
        },
      };
    })
    .filter((row) => !!row);

  const getApplyQuickFilterNameSearch: GetApplyQuickFilterFn<any, unknown> = (value) => {
    return (cellValue) => {
      const lowercaseValue = value.toLowerCase();
      const customer = cellValue as CustomerDetails;

      return Object.entries(customer).some(
        ([key, value]) => key !== 'uniqueId' && key !== 'photoUrl' && typeof value === 'string' && value.toLowerCase().includes(lowercaseValue),
      );
    };
  };

  const columns: GridColDef<(typeof rows)[number]>[] = [
    {
      field: 'person',
      headerName: '',
      renderCell: (params) => (
        <Box display="flex" justifyContent="center" alignItems="center" height="100%">
          <CustomerAvatar name={params.value} photo={{ url: params.value.photoUrl }} size="small" showFullName={true} />
        </Box>
      ),
      getApplyQuickFilterFn: getApplyQuickFilterNameSearch,
    },
    {
      field: correctedDaysOfWeek[0]!,
      headerName: correctedDaysOfWeek[0]!,
      width: 50,
      editable: false,
      renderCell: (params) => <BookingIconComponent booking={params.value.booking} />,
      align: 'center',
      display: 'flex',
    },
    {
      field: correctedDaysOfWeek[1]!,
      headerName: correctedDaysOfWeek[1]!,
      width: 50,
      editable: false,
      renderCell: (params) => <BookingIconComponent booking={params.value.booking} />,
      align: 'center',
      display: 'flex',
    },
    {
      field: correctedDaysOfWeek[2]!,
      headerName: correctedDaysOfWeek[2]!,
      width: 50,
      editable: false,
      renderCell: (params) => <BookingIconComponent booking={params.value.booking} />,
      align: 'center',
      display: 'flex',
    },
    {
      field: correctedDaysOfWeek[3]!,
      headerName: correctedDaysOfWeek[3]!,
      width: 50,
      editable: false,
      renderCell: (params) => <BookingIconComponent booking={params.value.booking} />,
      align: 'center',
      display: 'flex',
    },
    {
      field: correctedDaysOfWeek[4]!,
      headerName: correctedDaysOfWeek[4]!,
      width: 50,
      editable: false,
      renderCell: (params) => <BookingIconComponent booking={params.value.booking} />,
      align: 'center',
      display: 'flex',
    },
    {
      field: correctedDaysOfWeek[5]!,
      headerName: correctedDaysOfWeek[5]!,
      width: 50,
      editable: false,
      renderCell: (params) => <BookingIconComponent booking={params.value.booking} />,
      align: 'center',
      display: 'flex',
    },
    {
      field: correctedDaysOfWeek[6]!,
      headerName: correctedDaysOfWeek[6]!,
      width: 50,
      editable: false,
      renderCell: (params) => <BookingIconComponent booking={params.value.booking} />,
      align: 'center',
      display: 'flex',
    },
  ];

  const handleCellClick = (params: GridCellParams, event: MuiEvent, details: GridCallbackDetails) => {
    const { customer, booking } = params.value as BookingAndCustomerDetails;
    if (!booking && !rootData.organizationBookingPermissions?.canAddBookingOnBehalf && rootData.me?.id !== customer.uniqueId) {
      enqueueSnackbar(`You are not authorized to make a booking on behalf of someone else`, {
        variant: 'error',
        anchorOrigin,
      });

      return;
    }

    if (booking && !rootData.organizationBookingPermissions?.canDeleteBookingOnBehalf && rootData.me?.id !== customer.uniqueId) {
      enqueueSnackbar(`You are not authorized to remove this booking on behalf of someone else`, {
        variant: 'error',
        anchorOrigin,
      });

      return;
    }

    const id = booking ? booking.id : nanoid();
    const index = daysOfWeekMap[params.field]!;
    const startOfDay = startDate.add(index, 'day');
    const from = startOfDay.toISOString();
    const to = endOfDay(startOfDay).toISOString();
    const fromToPrint = toShortDate(startOfDay);

    if (booking) {
      commitDeleteBooking({
        variables: {
          connectionIds,
          input: {
            clientMutationId: nanoid(),
            id,
          },
        },
        onCompleted: (_, errors) => {
          if (errors && errors.length > 0) {
            enqueueSnackbar(`Failed to delete booking '${fromToPrint}'. Error: ${joinErrors(errors)}`, {
              variant: 'error',
              anchorOrigin,
            });

            return;
          }

          let message = `Booking removed for ${getCustomerFullName(booking.customer)}`;

          if (booking.location) {
            message += ` at the "${booking.location!.name}"`;
          }

          message += ` on ${toShortDate(booking.from)}`;

          enqueueSnackbar(message, { variant: 'success', anchorOrigin });
          UpdateGlobalReloadId();
        },
        onError: (error) => {
          enqueueSnackbar(`Failed to delete booking '${fromToPrint}'. Error: ${error.message}`, {
            variant: 'error',
            anchorOrigin,
          });
        },
      });
    } else {
      commitAddBooking({
        variables: {
          connectionIds,
          input: {
            clientMutationId: nanoid(),
            id,
            customerId: customer.uniqueId,
            from,
            to,
            organizationId,
            locationId,
            teamId,
            deskIds: [],
          },
        },
        onCompleted: (response, errors) => {
          if (errors && errors.length > 0) {
            enqueueSnackbar(`Failed to make a booking '${fromToPrint}'. Error: ${joinErrors(errors)}`, {
              variant: 'error',
              anchorOrigin,
            });

            return;
          }

          const booking = response.addBooking?.booking!;
          let message = `Booking added for ${getCustomerFullName(booking.customer)} to work`;

          if (booking.location) {
            message += ` from the "${booking.location!.name}"`;
          }

          if (booking.desks.length > 0) {
            message += ` at desk "${booking.desks.map(({ name }) => name).join(', ')}"`;

            const zones = booking.desks.flatMap(({ locationTags }) => locationTags).filter(({ tagType }) => tagType === TAG_TYPE_LOCATION_ZONE);
            if (zones.length > 0) {
              const uniqueZones = Array.from(zones.reduce((map, zone) => map.set(zone.uniqueId, zone), new Map()).values());

              message += ` in "${uniqueZones.map(({ name }) => name).join(', ')}"`;
            }
          }

          message += ` on ${toShortDate(booking.from)}`;

          enqueueSnackbar(message, { variant: 'success', anchorOrigin });
          UpdateGlobalReloadId();
        },
        onError: (error) => {
          enqueueSnackbar(`Failed to make a booking '${fromToPrint}'. Error: ${error.message}`, {
            variant: 'error',
            anchorOrigin,
          });
        },
        optimisticResponse: {
          addBooking: {
            booking: {
              id,
              from,
              to,
              customer: {
                uniqueId: customer.uniqueId,
                name: '',
                givenName: '',
                middleName: '',
                familyName: '',
              },
              organization: organizationId
                ? {
                    uniqueId: organizationId,
                    name: '',
                  }
                : null,
              location: locationId
                ? {
                    uniqueId: locationId,
                    name: '',
                  }
                : null,
              team: teamId
                ? {
                    uniqueId: teamId,
                    name: '',
                  }
                : null,
              desks: [],
            },
          },
        },
      });
    }
  };

  const rowCount = customers.length;

  return (
    <DataGrid
      rows={rows}
      columns={columns}
      hideFooterPagination={rowCount <= 10}
      initialState={{
        pagination: {
          rowCount,
          paginationModel: {
            pageSize: 10,
          },
        },
      }}
      pageSizeOptions={[10]}
      ignoreDiacritics
      disableRowSelectionOnClick
      density="compact"
      onCellClick={handleCellClick}
      slots={{ toolbar: QuickSearchToolbar }}
    />
  );
};

export default memo(BookingsWeekGrid);
