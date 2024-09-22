import { TAG_TYPE_LOCATION_ZONE } from '@/components/zone';
import type { locationPeopleBookingsMatrix_addBookingMutation } from '@/queries/__generated__/locationPeopleBookingsMatrix_addBookingMutation.graphql';
import type { locationPeopleBookingsMatrix_addCustomerDefaultLocationMutation } from '@/queries/__generated__/locationPeopleBookingsMatrix_addCustomerDefaultLocationMutation.graphql';
import type { locationPeopleBookingsMatrix_deleteBookingMutation } from '@/queries/__generated__/locationPeopleBookingsMatrix_deleteBookingMutation.graphql';
import type { locationPeopleBookingsMatrix_deleteLocationMutation } from '@/queries/__generated__/locationPeopleBookingsMatrix_deleteLocationMutation.graphql';
import type {
  locationPeopleBookingsMatrix_query$data,
  locationPeopleBookingsMatrix_query$key,
} from '@/queries/__generated__/locationPeopleBookingsMatrix_query.graphql';
import type { locationPeopleBookingsMatrix_removeCustomerDefaultLocationMutation } from '@/queries/__generated__/locationPeopleBookingsMatrix_removeCustomerDefaultLocationMutation.graphql';
import type {
  LocationMemberOrderInput,
  locationPeopleBookingsMatrixLocationMembersPaginationQuery,
} from '@/queries/__generated__/locationPeopleBookingsMatrixLocationMembersPaginationQuery.graphql';
import { Stack, Typography } from '@mui/material';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import CardHeader from '@mui/material/CardHeader';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import DialogTitle from '@mui/material/DialogTitle';
import IconButton from '@mui/material/IconButton';
import Menu from '@mui/material/Menu';
import MenuItem from '@mui/material/MenuItem';
import ToggleButton from '@mui/material/ToggleButton';
import ToggleButtonGroup from '@mui/material/ToggleButtonGroup';
import type { GridCallbackDetails, GridCellParams, GridColDef, MuiEvent } from '@mui/x-data-grid';
import { DataGrid } from '@mui/x-data-grid';
import { CustomerAvatar } from '@repo/shared/components/avatars';
import {
  BookingIcon,
  DangerIcon,
  DeleteIcon,
  EllipseMenuIcon,
  NotPreferredIcon,
  PreferredIcon,
  SettingsIcon,
  WorkingFromHomeIcon,
  WorkingFromOfficeIcon,
} from '@repo/shared/components/icons';
import { SnackbarAnchorOrigin as anchorOrigin } from '@repo/shared/libs/snackbar';
import { endOfDay, endOfWeek, getCustomerFullName, joinErrors, startOfWeek, toShortDate } from '@repo/shared/libs/utils';
import { Dayjs } from 'dayjs';
import { nanoid } from 'nanoid';
import { useRouter } from 'next/navigation';
import { useSnackbar } from 'notistack';
import { memo, useCallback, useMemo, useState, useTransition } from 'react';
import { graphql, useMutation, usePaginationFragment } from 'react-relay';

const defaultPageSize = 10000;

type Props = {
  rootDataRelay: locationPeopleBookingsMatrix_query$key;
  organizationId?: string;
  locationId: string;
  locationName: string;
  locationsConnectionIds: string[];
  hideRemoveLocationOption?: boolean;
};

enum DateRangeType {
  ThisWeek,
  NextWeek,
}

enum MoreActionsMenuOptionType {
  SetAsPreferredLocation,
  RemoveAsPreferredLocation,
  Settings,
  RemoveLocation,
}

interface MoreActionsMenuItemType {
  id: MoreActionsMenuOptionType;
  label: String;
  icon: JSX.Element;
  color: 'inherit' | 'default' | 'primary' | 'secondary' | 'error' | 'info' | 'success' | 'warning';
}

const moreActionsMenuAllOptions: Record<MoreActionsMenuOptionType, MoreActionsMenuItemType> = {
  [MoreActionsMenuOptionType.SetAsPreferredLocation]: {
    id: MoreActionsMenuOptionType.SetAsPreferredLocation,
    label: 'Set as preferred location',
    icon: <NotPreferredIcon />,
    color: 'primary',
  },
  [MoreActionsMenuOptionType.RemoveAsPreferredLocation]: {
    id: MoreActionsMenuOptionType.RemoveAsPreferredLocation,
    label: 'Remove as preferred location',
    icon: <PreferredIcon />,
    color: 'primary',
  },
  [MoreActionsMenuOptionType.Settings]: {
    id: MoreActionsMenuOptionType.Settings,
    label: 'Settings',
    icon: <SettingsIcon />,
    color: 'secondary',
  },
  [MoreActionsMenuOptionType.RemoveLocation]: {
    id: MoreActionsMenuOptionType.RemoveLocation,
    label: 'Remove location',
    icon: <DeleteIcon />,
    color: 'warning',
  },
};

type CustomerDetails = {
  uniqueId: string;
  name: string | null | undefined;
  givenName: string | null | undefined;
  middleName: string | null | undefined;
  familyName: string | null | undefined;
  photoUrl: string | null | undefined;
};

type BookingDetails = {
  customer: CustomerDetails;
  booking: locationPeopleBookingsMatrix_query$data['allBookings'][number] | undefined;
};

type RowType = {
  id: string;
  customer: CustomerDetails;
  mon: BookingDetails;
  tue: BookingDetails;
  wed: BookingDetails;
  thu: BookingDetails;
  fri: BookingDetails;
  sat: BookingDetails;
  sun: BookingDetails;
};

const dayIndex: { [key: string]: number } = { mon: 0, tue: 1, wed: 2, thu: 3, fri: 4, sat: 5, sun: 6 };

const getBookingIcon = ({ booking }: BookingDetails) => {
  let tip = '';

  if (booking) {
    tip = `Working`;
    if (booking.location) {
      tip += ` from the "${booking.location!.name}"`;
    }

    if (booking.desks.length > 0) {
      tip += ` at desk "${booking.desks.map(({ name }) => name).join(', ')}"`;

      const zones = booking.desks.flatMap(({ locationTags }) => locationTags).filter(({ tagType }) => tagType === TAG_TYPE_LOCATION_ZONE);
      if (zones.length > 0) {
        const uniqueZones = Array.from(zones.reduce((map, zone) => map.set(zone.uniqueId, zone), new Map()).values());

        tip += ` in "${uniqueZones.map(({ name }) => name).join(', ')}"`;
      }
    }
  }

  return booking ? <WorkingFromOfficeIcon tip={tip} /> : <WorkingFromHomeIcon />;
};

const LocationPeopleBookingsMatrix = ({
  rootDataRelay,
  organizationId,
  locationId,
  locationName,
  locationsConnectionIds,
  hideRemoveLocationOption,
}: Props) => {
  const { data: rootData, refetch } = usePaginationFragment<
    locationPeopleBookingsMatrixLocationMembersPaginationQuery,
    locationPeopleBookingsMatrix_query$key
  >(
    graphql`
      fragment locationPeopleBookingsMatrix_query on Query
      @argumentDefinitions(cursor: { type: "String" }, count: { type: "Int", defaultValue: 10000 })
      @refetchable(queryName: "locationPeopleBookingsMatrixLocationMembersPaginationQuery") {
        paginatedLocationMembers(
          first: $count
          after: $cursor
          where: { locationId: $locationId, nameContains: $peopleNameSearchText }
          orderBy: $peopleSortingValues
        ) @connection(key: "locationPeopleBookingsMatrix_paginatedLocationMembers") {
          __id
          totalCount
          pageInfo {
            hasNextPage
          }
          edges {
            node {
              id
              customer {
                uniqueId
                name
                givenName
                middleName
                familyName
                photoUrl
              }
            }
          }
        }
        customersByDefaultLocation(where: { locationId: $locationId, nameContains: $peopleNameSearchText }) {
          id
          name
          givenName
          middleName
          familyName
          photoUrl
        }
        me {
          id
          defaultLocations {
            uniqueId
          }
        }
        location(id: $locationId) {
          deskCapacity
          hasFutureBooking
          canModify
          canDelete
          organization {
            uniqueId
          }
        }
        organizationBookingPermissions(organizationId: $organizationId) @include(if: $fetchBookingPermission) {
          canAddBookingOnBehalf
        }
        allBookings(where: { locationIds: [$locationId], fromGTE: $from, toLT: $to }) {
          id
          from
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
    rootDataRelay,
  );

  const [commitAddBooking] = useMutation<locationPeopleBookingsMatrix_addBookingMutation>(graphql`
    mutation locationPeopleBookingsMatrix_addBookingMutation($input: AddBookingInput!) {
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

  const [commitDeleteBooking] = useMutation<locationPeopleBookingsMatrix_deleteBookingMutation>(graphql`
    mutation locationPeopleBookingsMatrix_deleteBookingMutation($input: DeleteBookingInput!) {
      deleteBooking(input: $input) {
        booking {
          id
        }
      }
    }
  `);

  const [commitDeleteLocation] = useMutation<locationPeopleBookingsMatrix_deleteLocationMutation>(graphql`
    mutation locationPeopleBookingsMatrix_deleteLocationMutation($connectionIds: [ID!]!, $input: DeleteLocationInput!) {
      deleteLocation(input: $input) {
        location {
          id @deleteEdge(connections: $connectionIds)
        }
      }
    }
  `);

  const [commitAddCustomerDefaultLocation] = useMutation<locationPeopleBookingsMatrix_addCustomerDefaultLocationMutation>(graphql`
    mutation locationPeopleBookingsMatrix_addCustomerDefaultLocationMutation($input: AddCustomerDefaultLocationInput!) {
      addCustomerDefaultLocation(input: $input) {
        customer {
          id
          defaultLocations {
            uniqueId
          }
        }
      }
    }
  `);

  const [commitRemoveCustomerDefaultLocation] = useMutation<locationPeopleBookingsMatrix_removeCustomerDefaultLocationMutation>(graphql`
    mutation locationPeopleBookingsMatrix_removeCustomerDefaultLocationMutation($input: RemoveCustomerDefaultLocationInput!) {
      removeCustomerDefaultLocation(input: $input) {
        customer {
          id
          defaultLocations {
            uniqueId
          }
        }
      }
    }
  `);

  const { enqueueSnackbar } = useSnackbar();
  const router = useRouter();
  const [moreActionsAnchorEl, setMoreActionsAnchorEl] = useState<null | HTMLElement>(null);
  const moreActionsMenuOpen = Boolean(moreActionsAnchorEl);
  const [dateRangeType, setDateRangeType] = useState(DateRangeType.ThisWeek);
  const [, startTransition] = useTransition();
  const [sortingLocationMemberOrder] = useState<LocationMemberOrderInput>({
    direction: 'Ascending',
    field: 'name',
  });
  const [locationRemoveConfirmationDialogOpen, setLocationRemoveConfirmationDialogOpen] = useState(false);
  const [startDate, setStartDate] = useState<Dayjs>(startOfWeek(null));
  const [peopleNameSearchText] = useState<string>('');
  const [page, setPage] = useState(0);
  const [pageSize] = useState(defaultPageSize);

  const handleRefetch = useCallback(
    (pageSize: number, startDate: Dayjs) => {
      startTransition(() => {
        const endDate = endOfWeek(startDate);

        refetch(
          {
            count: pageSize,
            peopleSortingValues: [sortingLocationMemberOrder],
            peopleNameSearchText,
            organizationId: organizationId ?? '',
            fetchBookingPermission: !!organizationId,
            locationId,
            from: startDate.toISOString(),
            to: endDate.toISOString(),
          },
          {
            fetchPolicy: 'store-and-network',
            onComplete: () => {
              setPage(0);
            },
          },
        );
      });
    },
    [refetch, sortingLocationMemberOrder, peopleNameSearchText, organizationId, locationId],
  );

  const memebrs = useMemo(() => rootData.paginatedLocationMembers, [rootData.paginatedLocationMembers]);
  const slicedEdges = memebrs.edges?.slice(
    page * pageSize,
    page * pageSize + pageSize > memebrs.edges.length ? memebrs.edges.length : page * pageSize + pageSize,
  );
  const allMembers = rootData.location?.organization
    ? rootData.customersByDefaultLocation.map((customer) => ({ ...customer, uniqueId: customer.id }))
    : slicedEdges.map((member) => member.node.customer);
  const meAsMember = allMembers.find((customer) => customer.uniqueId === rootData.me!.id);
  const otherMembers = allMembers.filter((customer) => customer.uniqueId !== rootData.me!.id);
  let finalMembersList = otherMembers;
  if (meAsMember) {
    finalMembersList = [meAsMember, ...otherMembers];
  }

  const rows: RowType[] = finalMembersList.map((customer) => {
    const customerId = customer.uniqueId;

    return {
      id: customerId,
      customer,
      mon: {
        customer,
        booking: rootData.allBookings.find((booking) => booking.customer!.uniqueId === customerId && booking.from === startDate.toISOString()),
      },
      tue: {
        customer,
        booking: rootData.allBookings.find(
          (booking) => booking.customer!.uniqueId === customerId && booking.from === startDate.add(1, 'day').toISOString(),
        ),
      },
      wed: {
        customer,
        booking: rootData.allBookings.find(
          (booking) => booking.customer!.uniqueId === customerId && booking.from === startDate.add(2, 'day').toISOString(),
        ),
      },
      thu: {
        customer,
        booking: rootData.allBookings.find(
          (booking) => booking.customer!.uniqueId === customerId && booking.from === startDate.add(3, 'day').toISOString(),
        ),
      },
      fri: {
        customer,
        booking: rootData.allBookings.find(
          (booking) => booking.customer!.uniqueId === customerId && booking.from === startDate.add(4, 'day').toISOString(),
        ),
      },
      sat: {
        customer,
        booking: rootData.allBookings.find(
          (booking) => booking.customer!.uniqueId === customerId && booking.from === startDate.add(5, 'day').toISOString(),
        ),
      },
      sun: {
        customer,
        booking: rootData.allBookings.find(
          (booking) => booking.customer!.uniqueId === customerId && booking.from === startDate.add(6, 'day').toISOString(),
        ),
      },
    };
  });

  const columns: GridColDef<(typeof rows)[number]>[] = [
    {
      field: 'customer',
      headerName: '',
      renderCell: (params) => (
        // TODO: 20240919 - Morteza: I don't like below 80% custom height setup, get rid of it in future.
        <Box display="flex" justifyContent="center" alignItems="center" height="80%">
          <CustomerAvatar
            name={{
              name: params.value.name,
              givenName: params.value.givenName,
              middleName: params.value.middleName,
              familyName: params.value.familyName,
            }}
            photo={{
              url: params.value.photoUrl,
            }}
            size="small"
            showFullName={true}
          />
        </Box>
      ),
    },
    {
      field: 'mon',
      headerName: 'Mon',
      width: 50,
      editable: false,
      renderCell: (params) => getBookingIcon(params.value),
      align: 'center',
    },
    {
      field: 'tue',
      headerName: 'Tue',
      width: 50,
      editable: false,
      renderCell: (params) => getBookingIcon(params.value),
      align: 'center',
    },
    {
      field: 'wed',
      headerName: 'Wed',
      width: 50,
      editable: false,
      renderCell: (params) => getBookingIcon(params.value),
      align: 'center',
    },
    {
      field: 'thu',
      headerName: 'Thu',
      width: 50,
      editable: false,
      renderCell: (params) => getBookingIcon(params.value),
      align: 'center',
    },
    {
      field: 'fri',
      headerName: 'Fri',
      width: 50,
      editable: false,
      renderCell: (params) => getBookingIcon(params.value),
      align: 'center',
    },
    {
      field: 'sat',
      headerName: 'Sat',
      width: 50,
      editable: false,
      renderCell: (params) => getBookingIcon(params.value),
      align: 'center',
    },
    {
      field: 'sun',
      headerName: 'Sun',
      width: 50,
      editable: false,
      renderCell: (params) => getBookingIcon(params.value),
      align: 'center',
    },
  ];

  const handleCellClick = (params: GridCellParams, event: MuiEvent, details: GridCallbackDetails) => {
    const { customer, booking } = params.value as BookingDetails;
    if (!rootData.organizationBookingPermissions?.canAddBookingOnBehalf && rootData.me?.id !== customer.uniqueId) {
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
          }

          let message = `Booking removed for ${getCustomerFullName(booking.customer)}`;

          if (booking.location) {
            message += ` at the "${booking.location!.name}"`;
          }

          message += ` on ${toShortDate(booking.from)}`;

          handleRefetch(pageSize, startDate);
          enqueueSnackbar(message, { variant: 'success', anchorOrigin });
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
            deskIds: [],
          },
        },
        onCompleted: (response, errors) => {
          if (errors && errors.length > 0) {
            enqueueSnackbar(`Failed to add booking '${fromToPrint}'. Error: ${joinErrors(errors)}`, {
              variant: 'error',
              anchorOrigin,
            });
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

          handleRefetch(pageSize, startDate);
          enqueueSnackbar(message, { variant: 'success', anchorOrigin });
        },
        onError: (error) => {
          enqueueSnackbar(`Failed to add booking '${fromToPrint}'. Error: ${error.message}`, {
            variant: 'error',
            anchorOrigin,
          });
        },
      });
    }
  };

  const handleDateRangeTypeChange = (event: React.MouseEvent<HTMLElement>, value: DateRangeType) => {
    let start = startOfWeek(null);
    if (value === DateRangeType.NextWeek) {
      start = start.add(1, 'week');
    }

    setStartDate(start);
    setDateRangeType(value);

    handleRefetch(pageSize, start);
  };

  if (!rootData.me || !rootData.location) {
    return <></>;
  }

  const rowCount = rootData.location.organization ? rootData.customersByDefaultLocation.length : (rootData.paginatedLocationMembers?.totalCount ?? 0);

  let moreActionsOption: MoreActionsMenuItemType[] = [];
  if (rootData.me.defaultLocations.some((location) => location.uniqueId === locationId)) {
    moreActionsOption = moreActionsOption.concat(moreActionsMenuAllOptions[MoreActionsMenuOptionType.RemoveAsPreferredLocation]);
  } else {
    moreActionsOption = moreActionsOption.concat(moreActionsMenuAllOptions[MoreActionsMenuOptionType.SetAsPreferredLocation]);
  }

  if (rootData.location.canModify) {
    moreActionsOption = moreActionsOption.concat(moreActionsMenuAllOptions[MoreActionsMenuOptionType.Settings]);
  }

  if (rootData.location.canDelete && !hideRemoveLocationOption) {
    moreActionsOption = moreActionsOption.concat(moreActionsMenuAllOptions[MoreActionsMenuOptionType.RemoveLocation]);
  }

  const handleMoreActionsMenuClick = (event: React.MouseEvent<HTMLElement>) => {
    setMoreActionsAnchorEl(event.currentTarget);
  };
  const handleMoreActionsMenuItemClick = (id: MoreActionsMenuOptionType) => {
    setMoreActionsAnchorEl(null);

    switch (id) {
      case MoreActionsMenuOptionType.SetAsPreferredLocation:
        handleSetAsPreferredLocationClicked();
        break;

      case MoreActionsMenuOptionType.RemoveAsPreferredLocation:
        handleSetAsNotPreferredLocationClicked();
        break;

      case MoreActionsMenuOptionType.Settings:
        handleSettingsClicked();
        break;

      case MoreActionsMenuOptionType.RemoveLocation:
        handleRemoveLocationClicked();
        break;
    }
  };

  const handleBookingsClicked = () => {
    router.push(organizationId ? `/organization/${organizationId}/location/${locationId}?tab=bookings` : `/location/${locationId}?tab=bookings`);
  };

  const handleSettingsClicked = () => {
    router.push(organizationId ? `/organization/${organizationId}/location/${locationId}?tab=about` : `/location/${locationId}?tab=about`);
  };

  const handleSetAsPreferredLocationClicked = () => {
    if (!rootData.me) {
      return;
    }

    commitAddCustomerDefaultLocation({
      variables: {
        input: {
          clientMutationId: nanoid(),
          locationId: locationId,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          enqueueSnackbar(`Failed to set location '${locationName}' as the preferred location. Error: ${joinErrors(errors)}`, {
            variant: 'error',
            anchorOrigin,
          });
        }

        enqueueSnackbar(`Location '${locationName}' has been set as the preferred location.`, {
          variant: 'success',
          anchorOrigin,
        });
      },
      onError: (error) => {
        enqueueSnackbar(`Failed to set location '${locationName}' as the preferred location. Error: ${error.message}`, {
          variant: 'error',
          anchorOrigin,
        });
      },
      optimisticResponse: {
        addCustomerDefaultLocation: {
          customer: {
            id: rootData.me.id,
            defaultLocations: rootData.me.defaultLocations.concat([
              {
                uniqueId: locationId,
              },
            ]),
          },
        },
      },
    });
  };

  const handleSetAsNotPreferredLocationClicked = () => {
    if (!rootData.me) {
      return;
    }

    commitRemoveCustomerDefaultLocation({
      variables: {
        input: {
          clientMutationId: nanoid(),
          locationId: locationId,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          enqueueSnackbar(`Failed to remove the location '${locationName}' as your preferred location. Error: ${joinErrors(errors)}`, {
            variant: 'error',
            anchorOrigin,
          });
        }

        enqueueSnackbar(`Location '${locationName}' has been removed as your preferred location.`, {
          variant: 'success',
          anchorOrigin,
        });
      },
      onError: (error) => {
        enqueueSnackbar(`Failed to remove the location '${locationName}' as your preferred location. Error: ${error.message}`, {
          variant: 'error',
          anchorOrigin,
        });
      },
      optimisticResponse: {
        addCustomerDefaultLocation: {
          customer: {
            id: rootData.me.id,
            defaultLocations: rootData.me.defaultLocations.filter(({ uniqueId }) => uniqueId === locationId),
          },
        },
      },
    });
  };

  const handleRemoveLocationClicked = () => {
    setLocationRemoveConfirmationDialogOpen(true);
  };

  const handleCancelRemovingLocationClick = () => {
    setLocationRemoveConfirmationDialogOpen(false);
  };

  const handleConfirmRemovingLocationClick = () => {
    if (!rootData.me) {
      return;
    }

    commitDeleteLocation({
      variables: {
        connectionIds: locationsConnectionIds,
        input: {
          clientMutationId: nanoid(),
          id: locationId,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          enqueueSnackbar(`Failed to remove location '${locationName}'. Error: ${joinErrors(errors)}`, {
            variant: 'error',
            anchorOrigin,
          });
        }

        enqueueSnackbar(`Location '${locationName}' has been successfully removed.`, {
          variant: 'success',
          anchorOrigin,
        });
      },
      onError: (error) => {
        enqueueSnackbar(`Failed to remove location '${locationName}'. Error: ${error.message}`, {
          variant: 'error',
          anchorOrigin,
        });
      },
    });
  };

  return (
    <>
      <Card sx={{ maxWidth: 500, height: '100%' }}>
        <CardHeader
          title={locationName}
          subheader={
            <Stack direction="row" justifyContent="space-between" width="100%">
              <ToggleButtonGroup color="primary" value={dateRangeType} exclusive onChange={handleDateRangeTypeChange} size="small">
                <ToggleButton value={DateRangeType.ThisWeek}>This week</ToggleButton>
                <ToggleButton value={DateRangeType.NextWeek}>Next week</ToggleButton>
              </ToggleButtonGroup>
              <Stack direction="row">
                <IconButton color="primary" onClick={handleBookingsClicked}>
                  <BookingIcon />
                </IconButton>
              </Stack>
            </Stack>
          }
          action={
            <>
              {moreActionsOption.length > 0 && (
                <IconButton onClick={handleMoreActionsMenuClick}>
                  <EllipseMenuIcon />
                </IconButton>
              )}
            </>
          }
        />
        <CardContent>
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
            disableRowSelectionOnClick
            density="compact"
            onCellClick={handleCellClick}
          />
        </CardContent>
      </Card>
      <Menu anchorEl={moreActionsAnchorEl} open={moreActionsMenuOpen} onClose={handleMoreActionsMenuItemClick}>
        {moreActionsOption.map((option) => (
          <MenuItem key={option.id} onClick={() => handleMoreActionsMenuItemClick(option.id)}>
            <Stack direction="row" spacing={1} sx={{ alignItems: 'center' }}>
              <IconButton color={option.color}>{option.icon}</IconButton>
              <Typography variant="body1">{option.label}</Typography>
            </Stack>
          </MenuItem>
        ))}
      </Menu>

      <Dialog fullWidth={true} open={locationRemoveConfirmationDialogOpen} onClose={handleCancelRemovingLocationClick}>
        <DialogTitle>Remove location</DialogTitle>
        <DialogContent>
          <DialogContentText>
            {rootData.location.hasFutureBooking
              ? `Bookings are scheduled for the location "${locationName}". Are you sure you want to remove it?`
              : `Are you sure you want to remove the location "${locationName}"?`}
          </DialogContentText>

          <DialogActions>
            <Button color="warning" variant="contained" startIcon={<DangerIcon />} onClick={handleConfirmRemovingLocationClick}>
              Remove
            </Button>
            <Button color="secondary" variant="outlined" onClick={handleCancelRemovingLocationClick}>
              Cancel
            </Button>
          </DialogActions>
        </DialogContent>
      </Dialog>
    </>
  );
};

export default memo(LocationPeopleBookingsMatrix);
