import Box from '@mui/material/Box';
import type { GetApplyQuickFilterFn, GridCallbackDetails, GridCellParams, GridColDef, MuiEvent } from '@mui/x-data-grid';
import { DataGrid, GridToolbarQuickFilter } from '@mui/x-data-grid';
import { CustomerAvatar } from '@repo/shared/components/avatars';
import { BookingIcon as BookingIconComponent } from '@repo/shared/components/booking';
import { TAG_TYPE_LOCATION_ZONE } from '@repo/shared/components/zone';
import { GlobalReloadIdContext, UpdateGlobalReloadIdContext } from '@repo/shared/libs/providers';
import { SnackbarAnchorOrigin as anchorOrigin } from '@repo/shared/libs/snackbar';
import { endOfDay, endOfIsoWeek, getCustomerFullName, joinErrors, toShortDate } from '@repo/shared/libs/utils';
import graphql from 'babel-plugin-relay/macro';
import { Dayjs } from 'dayjs';
import { nanoid } from 'nanoid';
import { useSnackbar } from 'notistack';
import { memo, useCallback, useContext, useEffect, useTransition } from 'react';
import { useFragment, useMutation, useRefetchableFragment } from 'react-relay';
import type { bookingsWeekGrid_addBookingMutation } from './__generated__/bookingsWeekGrid_addBookingMutation.graphql';
import type { bookingsWeekGrid_allBookings_query$key } from './__generated__/bookingsWeekGrid_allBookings_query.graphql';
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
  mon: BookingAndCustomerDetails;
  tue: BookingAndCustomerDetails;
  wed: BookingAndCustomerDetails;
  thu: BookingAndCustomerDetails;
  fri: BookingAndCustomerDetails;
  sat: BookingAndCustomerDetails;
  sun: BookingAndCustomerDetails;
};

const dayIndex: { [key: string]: number } = { mon: 0, tue: 1, wed: 2, thu: 3, fri: 4, sat: 5, sun: 6 };

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

  const [rootDataAllBookings, refetch] = useRefetchableFragment(
    graphql`
      fragment bookingsWeekGrid_allBookings_query on Query @refetchable(queryName: "bookingsWeekGrid_allBookings_refetchableFragment") {
        allBookings(where: { organizationIds: [$organizationId], locationIds: [$locationId], teamIds: [$teamId], fromGTE: $from, toLT: $to }) {
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
    `,
    rootDataAllBookingsRelay,
  );

  const [commitAddBooking] = useMutation<bookingsWeekGrid_addBookingMutation>(graphql`
    mutation bookingsWeekGrid_addBookingMutation($input: AddBookingInput!) {
      addBooking(input: $input) {
        booking {
          id
          from
          customer {
            name
            givenName
            middleName
            familyName
          }
          location {
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
    mutation bookingsWeekGrid_deleteBookingMutation($input: DeleteBookingInput!) {
      deleteBooking(input: $input) {
        booking {
          id
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
        const endDate = endOfIsoWeek(startDate);

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

  if (!rootData.me) {
    return <></>;
  }

  const meAsMember = customers.find((customer) => customer.uniqueId === rootData.me!.id);
  const otherMembers = customers.filter((customer) => customer.uniqueId !== rootData.me!.id);
  let finalMembersList = otherMembers;
  if (meAsMember) {
    finalMembersList = [meAsMember, ...otherMembers];
  }

  const rows: RowType[] = finalMembersList
    .map((customer) => {
      if (!rootDataAllBookings.allBookings) {
        return null;
      }

      const customerId = customer.uniqueId;

      return {
        id: customerId,
        person: customer,
        mon: {
          customer,
          booking: rootDataAllBookings.allBookings.find(
            (booking) => booking.customer!.uniqueId === customerId && booking.from === startDate.toISOString(),
          ),
        },
        tue: {
          customer,
          booking: rootDataAllBookings.allBookings.find(
            (booking) => booking.customer!.uniqueId === customerId && booking.from === startDate.add(1, 'day').toISOString(),
          ),
        },
        wed: {
          customer,
          booking: rootDataAllBookings.allBookings.find(
            (booking) => booking.customer!.uniqueId === customerId && booking.from === startDate.add(2, 'day').toISOString(),
          ),
        },
        thu: {
          customer,
          booking: rootDataAllBookings.allBookings.find(
            (booking) => booking.customer!.uniqueId === customerId && booking.from === startDate.add(3, 'day').toISOString(),
          ),
        },
        fri: {
          customer,
          booking: rootDataAllBookings.allBookings.find(
            (booking) => booking.customer!.uniqueId === customerId && booking.from === startDate.add(4, 'day').toISOString(),
          ),
        },
        sat: {
          customer,
          booking: rootDataAllBookings.allBookings.find(
            (booking) => booking.customer!.uniqueId === customerId && booking.from === startDate.add(5, 'day').toISOString(),
          ),
        },
        sun: {
          customer,
          booking: rootDataAllBookings.allBookings.find(
            (booking) => booking.customer!.uniqueId === customerId && booking.from === startDate.add(6, 'day').toISOString(),
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
      field: 'mon',
      headerName: 'Mon',
      width: 50,
      editable: false,
      renderCell: (params) => <BookingIconComponent booking={params.value.booking} />,
      align: 'center',
      display: 'flex',
    },
    {
      field: 'tue',
      headerName: 'Tue',
      width: 50,
      editable: false,
      renderCell: (params) => <BookingIconComponent booking={params.value.booking} />,
      align: 'center',
      display: 'flex',
    },
    {
      field: 'wed',
      headerName: 'Wed',
      width: 50,
      editable: false,
      renderCell: (params) => <BookingIconComponent booking={params.value.booking} />,
      align: 'center',
      display: 'flex',
    },
    {
      field: 'thu',
      headerName: 'Thu',
      width: 50,
      editable: false,
      renderCell: (params) => <BookingIconComponent booking={params.value.booking} />,
      align: 'center',
      display: 'flex',
    },
    {
      field: 'fri',
      headerName: 'Fri',
      width: 50,
      editable: false,
      renderCell: (params) => <BookingIconComponent booking={params.value.booking} />,
      align: 'center',
      display: 'flex',
    },
    {
      field: 'sat',
      headerName: 'Sat',
      width: 50,
      editable: false,
      renderCell: (params) => <BookingIconComponent booking={params.value.booking} />,
      align: 'center',
      display: 'flex',
    },
    {
      field: 'sun',
      headerName: 'Sun',
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
    const index = dayIndex[params.field]!;
    const startOfDay = startDate.add(index, 'day');
    const from = startOfDay.toISOString();
    const to = endOfDay(startOfDay).toISOString();
    const fromToPrint = toShortDate(startOfDay);

    if (booking) {
      commitDeleteBooking({
        variables: {
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

          handleRefetch(startDate);
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

          handleRefetch(startDate);
          enqueueSnackbar(message, { variant: 'success', anchorOrigin });
          UpdateGlobalReloadId();
        },
        onError: (error) => {
          enqueueSnackbar(`Failed to make a booking '${fromToPrint}'. Error: ${error.message}`, {
            variant: 'error',
            anchorOrigin,
          });
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
