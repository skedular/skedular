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
import { SnackbarAnchorOrigin as anchorOrigin } from '@repo/shared/libs/snackbar';
import { joinErrors, startOfDay } from '@repo/shared/libs/utils';
import graphql from 'babel-plugin-relay/macro';
import { BookingsWeekGrid } from 'components/booking';
import { LocationLink, getLocationBookingsLink, getLocationSettingsLink } from 'components/location';
import { Dayjs } from 'dayjs';
import { nanoid } from 'nanoid';
import { useSnackbar } from 'notistack';
import { memo, useState } from 'react';
import { useFragment, useMutation } from 'react-relay';
import type { locationPeopleBookings_addCustomerDefaultLocationMutation } from './__generated__/locationPeopleBookings_addCustomerDefaultLocationMutation.graphql';
import type { locationPeopleBookings_deleteLocationMutation } from './__generated__/locationPeopleBookings_deleteLocationMutation.graphql';
import type { locationPeopleBookings_query$key } from './__generated__/locationPeopleBookings_query.graphql';
import type { locationPeopleBookings_removeCustomerDefaultLocationMutation } from './__generated__/locationPeopleBookings_removeCustomerDefaultLocationMutation.graphql';

type Props = {
  rootDataRelay: locationPeopleBookings_query$key;
  organizationId: string;
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

const LocationPeopleBookings = ({
  rootDataRelay,
  organizationId,
  locationId,
  locationName,
  locationsConnectionIds,
  hideRemoveLocationOption,
}: Props) => {
  const rootData = useFragment(
    graphql`
      fragment locationPeopleBookings_query on Query {
        locationMembers(where: { locationId: $locationId }, orderBy: $peopleSortingValues) {
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
        customersByDefaultLocation(where: { locationId: $locationId }) {
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
        ...bookingsWeekGrid_query
        ...bookingsWeekGrid_allBookings_query
      }
    `,
    rootDataRelay,
  );

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
  const [locationRemoveConfirmationDialogOpen, setLocationRemoveConfirmationDialogOpen] = useState(false);
  const [startDate, setStartDate] = useState<Dayjs>(startOfDay());

  if (!rootData.me || !rootData.location || !rootData.locationMembers || !rootData.customersByDefaultLocation) {
    return <></>;
  }

  const allMembers = rootData.location?.organization
    ? rootData.customersByDefaultLocation.map((customer) => ({ ...customer, uniqueId: customer.id }))
    : rootData.locationMembers.map((member) => member.customer);

  const handleDateRangeTypeChange = (_: React.MouseEvent<HTMLElement>, value: DateRangeType) => {
    let start = startOfDay();
    if (value === DateRangeType.NextWeek) {
      start = start.add(1, 'week');
    }

    setStartDate(start);
    setDateRangeType(value);
  };

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

          return;
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

          return;
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

          return;
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

          <BookingsWeekGrid
            rootDataRelay={rootData}
            rootDataAllBookingsRelay={rootData}
            organizationId={organizationId}
            startDate={startDate}
            customers={allMembers}
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
