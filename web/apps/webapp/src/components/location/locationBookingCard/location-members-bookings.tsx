import { BookingsWeekGrid } from '@/components/booking';
import { getLocationBookingsLink, getLocationSettingsLink, LocationLink } from '@/components/location';
import { OrganizationLink } from '@/components/organization';
import type { locationMembersBookings_addCustomerDefaultLocationMutation } from '@/queries/__generated__/locationMembersBookings_addCustomerDefaultLocationMutation.graphql';
import type { locationMembersBookings_deleteLocationMutation } from '@/queries/__generated__/locationMembersBookings_deleteLocationMutation.graphql';
import type { locationMembersBookings_query$key } from '@/queries/__generated__/locationMembersBookings_query.graphql';
import type { locationMembersBookings_removeCustomerDefaultLocationMutation } from '@/queries/__generated__/locationMembersBookings_removeCustomerDefaultLocationMutation.graphql';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import CardHeader from '@mui/material/CardHeader';
import Dialog from '@mui/material/Dialog';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import DialogTitle from '@mui/material/DialogTitle';
import IconButton from '@mui/material/IconButton';
import Link from '@mui/material/Link';
import ToggleButton from '@mui/material/ToggleButton';
import ToggleButtonGroup from '@mui/material/ToggleButtonGroup';
import Box from '@mui/system/Box';
import { BodyIconTypography, PushToRight, StackColumn, StackRow, TwoButtonsDialogActions } from '@repo/shared/components/commons';
import { BookingIcon, DeskIcon, EllipseMenuIcon, SettingsIcon } from '@repo/shared/components/icons';
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
import { DialogTransition } from '@repo/shared/components/transitions';
import { PaletteModeContext } from '@repo/shared/libs/providers';
import { coal, sandstone } from '@repo/shared/libs/theme';
import { joinErrors, startOfDay } from '@repo/shared/libs/utils';
import { Dayjs } from 'dayjs';
import { nanoid } from 'nanoid';
import NextLink from 'next/link';
import { memo, useContext, useState } from 'react';
import { graphql, useFragment, useMutation } from 'react-relay';
import { toast } from 'react-toastify';

type Props = {
  rootDataRelay: locationMembersBookings_query$key;
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

const LocationMembersBookings = ({
  rootDataRelay,
  organizationId,
  locationId,
  locationName,
  locationsConnectionIds,
  hideRemoveLocationOption,
}: Props) => {
  const rootData = useFragment(
    graphql`
      fragment locationMembersBookings_query on Query {
        locationMembers(where: { locationId: $locationId }, orderBy: $locationPeopleSortingValues) {
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
        organizationMembers(where: { organizationId: $organizationId }, orderBy: $organizationPeopleSortingValues) @include(if: $organizationExists) {
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

  const [commitDeleteLocation] = useMutation<locationMembersBookings_deleteLocationMutation>(graphql`
    mutation locationMembersBookings_deleteLocationMutation($connectionIds: [ID!]!, $input: DeleteLocationInput!) {
      deleteLocation(input: $input) {
        location {
          id @deleteEdge(connections: $connectionIds)
        }
      }
    }
  `);

  const [commitAddCustomerDefaultLocation] = useMutation<locationMembersBookings_addCustomerDefaultLocationMutation>(graphql`
    mutation locationMembersBookings_addCustomerDefaultLocationMutation($input: AddCustomerDefaultLocationInput!) {
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

  const [commitRemoveCustomerDefaultLocation] = useMutation<locationMembersBookings_removeCustomerDefaultLocationMutation>(graphql`
    mutation locationMembersBookings_removeCustomerDefaultLocationMutation($input: RemoveCustomerDefaultLocationInput!) {
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

  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const [moreActionsAnchorEl, setMoreActionsAnchorEl] = useState<null | HTMLElement>(null);
  const moreActionsMenuOpen = Boolean(moreActionsAnchorEl);
  const [dateRangeType, setDateRangeType] = useState(DateRangeType.ThisWeek);
  const [locationRemoveConfirmationDialogOpen, setLocationRemoveConfirmationDialogOpen] = useState(false);
  const [startDate, setStartDate] = useState<Dayjs>(startOfDay());

  if (!rootData.me || !rootData.location) {
    return <></>;
  }

  const allMembers = rootData.location?.organization
    ? rootData.organizationMembers
      ? rootData.organizationMembers.edges.map(({ node }) => node.customer)
      : []
    : rootData.locationMembers
      ? rootData.locationMembers.edges.map(({ node }) => node.customer)
      : [];

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

    const toastId = themedToast(
      <NotificationContent content={`Setting location '${locationName}' as your preferred location...`} />,
      infoNotificationOptions,
    );

    commitAddCustomerDefaultLocation({
      variables: {
        input: {
          clientMutationId: nanoid(),
          locationId: locationId,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: (
              <NotificationContent content={`Failed to set location '${locationName}' as your preferred location. Error: ${joinErrors(errors)}.`} />
            ),
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Location '${locationName}' has been set as the preferred location.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to set location '${locationName}' as your preferred location. Error: ${error.message}.`} />,
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

    const toastId = themedToast(
      <NotificationContent content={`Removing location '${locationName}' as your preferred location...`} />,
      infoNotificationOptions,
    );

    commitRemoveCustomerDefaultLocation({
      variables: {
        input: {
          clientMutationId: nanoid(),
          locationId: locationId,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: (
              <NotificationContent
                content={`Failed to remove the location '${locationName}' as your preferred location. Error: ${joinErrors(errors)}.`}
              />
            ),
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Location '${locationName}' has been removed as your preferred location.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: (
            <NotificationContent content={`Failed to remove the location '${locationName}' as your preferred location. Error: ${error.message}.`} />
          ),
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

    const toastId = themedToast(<NotificationContent content={`Removing location '${locationName}'...`} />, infoNotificationOptions);

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
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to remove location '${locationName}'. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Location '${locationName}' has been successfully removed.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove location '${locationName}'. Error: ${error.message}.`} />,
        });
      },
    });
  };

  return (
    <>
      <Card sx={{ maxWidth: 500, height: '100%' }}>
        <CardHeader
          title={
            <StackColumn>
              <LocationLink organizationId={organizationId} id={locationId} name={locationName} />
              {rootData.location.organization && (
                <OrganizationLink id={rootData.location.organization.uniqueId} name={rootData.location.organization.name} />
              )}
            </StackColumn>
          }
          action={
            <>
              {moreActionsOption.length > 0 && (
                <Box color={paletteMode === 'dark' ? coal : sandstone}>
                  <IconButton onClick={handleMoreActionsMenuClick} color="inherit">
                    <EllipseMenuIcon />
                  </IconButton>
                </Box>
              )}
            </>
          }
        />
        <CardContent>
          <StackRow>
            <ToggleButtonGroup color="primary" value={dateRangeType} exclusive onChange={handleDateRangeTypeChange} size="small">
              <ToggleButton value={DateRangeType.ThisWeek}>This week</ToggleButton>
              <ToggleButton value={DateRangeType.NextWeek}>Next week</ToggleButton>
            </ToggleButtonGroup>
            <PushToRight />
            <StackRow>
              <Link component={NextLink} href={getLocationBookingsLink(locationId, organizationId)}>
                <BookingIcon />
              </Link>

              {rootData.location.canModify && (
                <Link component={NextLink} href={getLocationSettingsLink(locationId, organizationId)}>
                  <SettingsIcon color="secondary" />
                </Link>
              )}
            </StackRow>
          </StackRow>

          <BodyIconTypography
            label={rootData.location.deskCapacity === 0 ? 'No desk available' : `Desk Capacity: ${rootData.location.deskCapacity}`}
            startElement={<DeskIcon />}
          />

          <BookingsWeekGrid
            rootDataRelay={rootData}
            rootDataAllBookingsRelay={rootData}
            organizationId={organizationId}
            startDate={startDate}
            customers={allMembers}
            locationId={locationId}
          />
        </CardContent>
      </Card>

      <MoreActionsMenu
        anchorEl={moreActionsAnchorEl}
        open={moreActionsMenuOpen}
        onMenuItemClick={handleMoreActionsMenuItemClick}
        options={moreActionsOption}
      />

      <Dialog TransitionComponent={DialogTransition} open={locationRemoveConfirmationDialogOpen} onClose={handleCancelRemovingLocationClick}>
        <DialogTitle>Remove Location</DialogTitle>
        <DialogContent>
          <DialogContentText>
            {rootData.location.hasFutureBooking
              ? `Bookings are scheduled for the location "${locationName}". Are you sure you want to remove it?`
              : `Are you sure you want to remove the location "${locationName}"?`}
          </DialogContentText>
          <TwoButtonsDialogActions
            onPrimaryClicked={handleConfirmRemovingLocationClick}
            onSecondaryClicked={handleCancelRemovingLocationClick}
            primaryLabel="Remove"
            secondaryLabel="Cancel"
          />
        </DialogContent>
      </Dialog>
    </>
  );
};

export default memo(LocationMembersBookings);
