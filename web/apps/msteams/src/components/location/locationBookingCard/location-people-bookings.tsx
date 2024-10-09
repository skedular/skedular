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
import Link from '@mui/material/Link';
import Menu from '@mui/material/Menu';
import MenuItem from '@mui/material/MenuItem';
import Stack from '@mui/material/Stack';
import ToggleButton from '@mui/material/ToggleButton';
import ToggleButtonGroup from '@mui/material/ToggleButtonGroup';
import Typography from '@mui/material/Typography';
import type { GetApplyQuickFilterFn, GridCallbackDetails, GridCellParams, GridColDef, MuiEvent } from '@mui/x-data-grid';
import { DataGrid, GridToolbarQuickFilter } from '@mui/x-data-grid';
import { CustomerAvatar } from '@repo/shared/components/avatars';
import { BookingIcon as BookingIconComponent } from '@repo/shared/components/booking';
import {
  BookingIcon,
  DangerIcon,
  DeleteIcon,
  DeskIcon,
  EllipseMenuIcon,
  NotPreferredIcon,
  PreferredIcon,
  SettingsIcon,
} from '@repo/shared/components/icons';
import { DialogTransition } from '@repo/shared/components/transitions';
import { TAG_TYPE_LOCATION_ZONE } from '@repo/shared/components/zone';
import { SnackbarAnchorOrigin as anchorOrigin } from '@repo/shared/libs/snackbar';
import { endOfDay, endOfWeek, getCustomerFullName, joinErrors, startOfWeek, toShortDate } from '@repo/shared/libs/utils';
import graphql from 'babel-plugin-relay/macro';
import { LocationLink, getLocationBookingsLink, getLocationSettingsLink } from 'components/location';
import { OrganizationLink } from 'components/organization';
import { Dayjs } from 'dayjs';
import { nanoid } from 'nanoid';
import { useSnackbar } from 'notistack';
import { memo, useCallback, useState, useTransition } from 'react';
import { useMutation, useRefetchableFragment } from 'react-relay';
import type { locationPeopleBookings_addBookingMutation } from './__generated__/locationPeopleBookings_addBookingMutation.graphql';
import type { locationPeopleBookings_addCustomerDefaultLocationMutation } from './__generated__/locationPeopleBookings_addCustomerDefaultLocationMutation.graphql';
import type { locationPeopleBookings_deleteBookingMutation } from './__generated__/locationPeopleBookings_deleteBookingMutation.graphql';
import type { locationPeopleBookings_deleteLocationMutation } from './__generated__/locationPeopleBookings_deleteLocationMutation.graphql';
import type { locationPeopleBookings_query$key } from './__generated__/locationPeopleBookings_query.graphql';
import type { locationPeopleBookings_removeCustomerDefaultLocationMutation } from './__generated__/locationPeopleBookings_removeCustomerDefaultLocationMutation.graphql';
import type { LocationMemberOrderInput } from './__generated__/locationPeopleBookingsLocationMembers_PaginationQuery.graphql';

type Props = {
  rootDataRelay: locationPeopleBookings_query$key;
  organizationId?: string;
  locationId: string;
  locationName?: string;
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
  RemoveLocation,
}

type MoreActionsMenuItemType = {
  id: MoreActionsMenuOptionType;
  label: String;
  icon: JSX.Element;
  color: 'inherit' | 'default' | 'primary' | 'secondary' | 'error' | 'info' | 'success' | 'warning';
};

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
  [MoreActionsMenuOptionType.RemoveLocation]: {
    id: MoreActionsMenuOptionType.RemoveLocation,
    label: 'Remove location',
    icon: <DeleteIcon />,
    color: 'warning',
  },
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

const LocationPeopleBookings = ({
  rootDataRelay,
  organizationId,
  locationId,
  locationName,
  locationsConnectionIds,
  hideRemoveLocationOption,
}: Props) => {
  const [rootData, refetch] = useRefetchableFragment(
    graphql`
      fragment locationPeopleBookings_query on Query @refetchable(queryName: "locationPeopleBookingsLocationMembers_PaginationQuery") {
        locationMembers(where: { locationId: $locationId, nameContains: $peopleNameSearchText }, orderBy: $peopleSortingValues)
          @include(if: $locationExists) {
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
        customersByDefaultLocation(where: { locationId: $locationId, nameContains: $peopleNameSearchText }) @include(if: $locationExists) {
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
          name
          deskCapacity
          hasFutureBooking
          canModify
          canDelete
          organization {
            uniqueId
            name
          }
        }
        locationBookingPermissions(locationId: $locationId) @include(if: $locationExists) {
          canAddBookingOnBehalf
          canDeleteBookingOnBehalf
        }
        allBookings(where: { locationIds: [$locationId], fromGTE: $from, toLT: $to }) {
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
    rootDataRelay,
  );

  const [commitAddBooking] = useMutation<locationPeopleBookings_addBookingMutation>(graphql`
    mutation locationPeopleBookings_addBookingMutation($input: AddBookingInput!) {
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

  const [commitDeleteBooking] = useMutation<locationPeopleBookings_deleteBookingMutation>(graphql`
    mutation locationPeopleBookings_deleteBookingMutation($input: DeleteBookingInput!) {
      deleteBooking(input: $input) {
        booking {
          id
        }
      }
    }
  `);

  const [commitDeleteLocation] = useMutation<locationPeopleBookings_deleteLocationMutation>(graphql`
    mutation locationPeopleBookings_deleteLocationMutation($connectionIds: [ID!]!, $input: DeleteLocationInput!) {
      deleteLocation(input: $input) {
        location {
          id @deleteEdge(connections: $connectionIds)
        }
      }
    }
  `);

  const [commitAddCustomerDefaultLocation] = useMutation<locationPeopleBookings_addCustomerDefaultLocationMutation>(graphql`
    mutation locationPeopleBookings_addCustomerDefaultLocationMutation($input: AddCustomerDefaultLocationInput!) {
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

  const [commitRemoveCustomerDefaultLocation] = useMutation<locationPeopleBookings_removeCustomerDefaultLocationMutation>(graphql`
    mutation locationPeopleBookings_removeCustomerDefaultLocationMutation($input: RemoveCustomerDefaultLocationInput!) {
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
  const [moreActionsAnchorEl, setMoreActionsAnchorEl] = useState<null | HTMLElement>(null);
  const moreActionsMenuOpen = Boolean(moreActionsAnchorEl);
  const [dateRangeType, setDateRangeType] = useState(DateRangeType.ThisWeek);
  const [, startTransition] = useTransition();
  const [sortingLocationMemberOrder] = useState<LocationMemberOrderInput>({
    direction: 'Ascending',
    field: 'name',
  });
  const [locationRemoveConfirmationDialogOpen, setLocationRemoveConfirmationDialogOpen] = useState(false);
  const [startDate, setStartDate] = useState<Dayjs>(startOfWeek());
  const [peopleNameSearchText] = useState<string>('');

  const handleRefetch = useCallback(
    (startDate: Dayjs) => {
      startTransition(() => {
        const endDate = endOfWeek(startDate);

        refetch(
          {
            peopleSortingValues: [sortingLocationMemberOrder],
            peopleNameSearchText,
            organizationId: organizationId ?? '',
            fetchBookingPermission: !!organizationId,
            locationId,
            locationExists: !!locationId,
            from: startDate.toISOString(),
            to: endDate.toISOString(),
          },
          {
            fetchPolicy: 'store-and-network',
          },
        );
      });
    },
    [refetch, sortingLocationMemberOrder, peopleNameSearchText, organizationId, locationId],
  );

  if (!rootData.me || !rootData.location || !rootData.locationMembers || !rootData.customersByDefaultLocation) {
    return <></>;
  }

  const allMembers = rootData.location?.organization
    ? rootData.customersByDefaultLocation.map((customer) => ({ ...customer, uniqueId: customer.id }))
    : rootData.locationMembers.map((member) => member.customer);
  const meAsMember = allMembers.find((customer) => customer.uniqueId === rootData.me!.id);
  const otherMembers = allMembers.filter((customer) => customer.uniqueId !== rootData.me!.id);
  let finalMembersList = otherMembers;
  if (meAsMember) {
    finalMembersList = [meAsMember, ...otherMembers];
  }

  const rows: RowType[] = finalMembersList
    .map((customer) => {
      if (!rootData.allBookings) {
        return null;
      }

      const customerId = customer.uniqueId;

      return {
        id: customerId,
        person: customer,
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
    if (!booking && !rootData.locationBookingPermissions?.canAddBookingOnBehalf && rootData.me?.id !== customer.uniqueId) {
      enqueueSnackbar(`You are not authorized to make a booking on behalf of someone else`, {
        variant: 'error',
        anchorOrigin,
      });

      return;
    }

    if (booking && !rootData.locationBookingPermissions?.canDeleteBookingOnBehalf && rootData.me?.id !== customer.uniqueId) {
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
          }

          let message = `Booking removed for ${getCustomerFullName(booking.customer)}`;

          if (booking.location) {
            message += ` at the "${booking.location!.name}"`;
          }

          message += ` on ${toShortDate(booking.from)}`;

          handleRefetch(startDate);
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
            enqueueSnackbar(`Failed to make a booking '${fromToPrint}'. Error: ${joinErrors(errors)}`, {
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

          handleRefetch(startDate);
          enqueueSnackbar(message, { variant: 'success', anchorOrigin });
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

  const handleDateRangeTypeChange = (event: React.MouseEvent<HTMLElement>, value: DateRangeType) => {
    let start = startOfWeek();
    if (value === DateRangeType.NextWeek) {
      start = start.add(1, 'week');
    }

    setStartDate(start);
    setDateRangeType(value);

    handleRefetch(start);
  };

  const rowCount = rootData.location.organization ? rootData.customersByDefaultLocation.length : rootData.locationMembers.length;

  let moreActionsOption: MoreActionsMenuItemType[] = [];
  if (rootData.me.defaultLocations.some((location) => location.uniqueId === locationId)) {
    moreActionsOption = moreActionsOption.concat(moreActionsMenuAllOptions[MoreActionsMenuOptionType.RemoveAsPreferredLocation]);
  } else {
    moreActionsOption = moreActionsOption.concat(moreActionsMenuAllOptions[MoreActionsMenuOptionType.SetAsPreferredLocation]);
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
        handleRemoveAsPreferredLocationClicked();
        break;

      case MoreActionsMenuOptionType.RemoveLocation:
        handleRemoveLocationClicked();
        break;
    }
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
          enqueueSnackbar(`Failed to set location '${locationName}' as your preferred location. Error: ${joinErrors(errors)}`, {
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
        enqueueSnackbar(`Failed to set location '${locationName}' as your preferred location. Error: ${error.message}`, {
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

  const handleRemoveAsPreferredLocationClicked = () => {
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
          title={
            <Stack direction="column">
              <LocationLink organizationId={organizationId} id={locationId} name={locationName} />
              {rootData.location.organization && (
                <OrganizationLink id={rootData.location.organization.uniqueId} name={rootData.location.organization.name} />
              )}
            </Stack>
          }
          subheader={
            <Stack direction="row" sx={{ justifyContent: 'space-between', width: '100%', alignItems: 'center' }}>
              <ToggleButtonGroup color="primary" value={dateRangeType} exclusive onChange={handleDateRangeTypeChange} size="small">
                <ToggleButton value={DateRangeType.ThisWeek}>This week</ToggleButton>
                <ToggleButton value={DateRangeType.NextWeek}>Next week</ToggleButton>
              </ToggleButtonGroup>
              <Stack direction="row">
                <Link href={getLocationBookingsLink(locationId, organizationId)}>
                  <BookingIcon />
                </Link>

                {rootData.location.canModify && (
                  <Link href={getLocationSettingsLink(locationId, organizationId)}>
                    <SettingsIcon color="secondary" />
                  </Link>
                )}
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
          <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
            <DeskIcon />
            <Typography variant="body1">
              {rootData.location.deskCapacity === 0 ? 'No desk available' : `Desk Capacity: ${rootData.location.deskCapacity}`}
            </Typography>
          </Stack>

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
        </CardContent>
      </Card>
      <Menu anchorEl={moreActionsAnchorEl} open={moreActionsMenuOpen} onClose={handleMoreActionsMenuItemClick}>
        {moreActionsOption.map((option) => (
          <MenuItem key={option.id} onClick={() => handleMoreActionsMenuItemClick(option.id)}>
            <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
              <IconButton color={option.color}>{option.icon}</IconButton>
              <Typography variant="body1">{option.label}</Typography>
            </Stack>
          </MenuItem>
        ))}
      </Menu>

      <Dialog TransitionComponent={DialogTransition} open={locationRemoveConfirmationDialogOpen} onClose={handleCancelRemovingLocationClick}>
        <DialogTitle>Remove location</DialogTitle>
        <DialogContent>
          <DialogContentText>
            {rootData.location.hasFutureBooking
              ? `Bookings are scheduled for the location "${locationName}". Are you sure you want to remove it?`
              : `Are you sure you want to remove the location "${locationName}"?`}
          </DialogContentText>

          <DialogActions>
            <Button color="secondary" variant="outlined" onClick={handleCancelRemovingLocationClick}>
              Cancel
            </Button>
            <Button color="warning" variant="contained" startIcon={<DangerIcon />} onClick={handleConfirmRemovingLocationClick}>
              Remove
            </Button>
          </DialogActions>
        </DialogContent>
      </Dialog>
    </>
  );
};

export default memo(LocationPeopleBookings);
