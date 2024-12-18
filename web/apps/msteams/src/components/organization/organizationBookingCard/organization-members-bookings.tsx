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
import ToggleButton from '@mui/material/ToggleButton';
import ToggleButtonGroup from '@mui/material/ToggleButtonGroup';
import { BodyIconTypography, StackRow, StackRowFullWidth } from '@repo/shared/components/commons';
import { BookingIcon, DangerIcon, DeleteIcon, EllipseMenuIcon, NotPreferredIcon, PreferredIcon, SettingsIcon } from '@repo/shared/components/icons';
import {
  errorNotificationOptions,
  infoNotificationOptions,
  NotificationContent,
  successNotificationOptions,
} from '@repo/shared/components/notification';
import { DialogTransition } from '@repo/shared/components/transitions';
import { PaletteModeContext } from '@repo/shared/libs/providers';
import { joinErrors, startOfDay } from '@repo/shared/libs/utils';
import graphql from 'babel-plugin-relay/macro';
import { BookingsWeekGrid } from 'components/booking';
import { getOrganizationBookingsBaseLink, getOrganizationSettingsBaseLink, OrganizationLink } from 'components/organization';
import { Dayjs } from 'dayjs';
import { nanoid } from 'nanoid';
import type { JSX } from 'react';
import { memo, useContext, useState } from 'react';
import { useFragment, useMutation } from 'react-relay';
import { toast } from 'react-toastify';
import type { organizationMembersBookings_clearCustomerDefaultOrganizationMutation } from './__generated__/organizationMembersBookings_clearCustomerDefaultOrganizationMutation.graphql';
import type { organizationMembersBookings_deleteOrganizationMutation } from './__generated__/organizationMembersBookings_deleteOrganizationMutation.graphql';
import type { organizationMembersBookings_query$key } from './__generated__/organizationMembersBookings_query.graphql';
import type { organizationMembersBookings_setCustomerDefaultOrganizationMutation } from './__generated__/organizationMembersBookings_setCustomerDefaultOrganizationMutation.graphql';

type Props = {
  rootDataRelay: organizationMembersBookings_query$key;
  organizationId: string;
  organizationName?: string;
  organizationsConnectionIds: string[];
  hideRemoveOrganizationOption?: boolean;
};

enum DateRangeType {
  ThisWeek,
  NextWeek,
}

enum MoreActionsMenuOptionType {
  MarkAsDefaultOrganization,
  ClearAsPreferredOrganization,
  RemoveOrganization,
}

type MoreActionsMenuItemType = {
  id: MoreActionsMenuOptionType;
  label: string;
  icon: JSX.Element;
};

const moreActionsMenuAllOptions: Record<MoreActionsMenuOptionType, MoreActionsMenuItemType> = {
  [MoreActionsMenuOptionType.MarkAsDefaultOrganization]: {
    id: MoreActionsMenuOptionType.MarkAsDefaultOrganization,
    label: 'Mark as default organization',
    icon: <NotPreferredIcon color="primary" />,
  },
  [MoreActionsMenuOptionType.ClearAsPreferredOrganization]: {
    id: MoreActionsMenuOptionType.ClearAsPreferredOrganization,
    label: 'Clear as default organization',
    icon: <PreferredIcon color="primary" />,
  },
  [MoreActionsMenuOptionType.RemoveOrganization]: {
    id: MoreActionsMenuOptionType.RemoveOrganization,
    label: 'Remove organization',
    icon: <DeleteIcon color="warning" />,
  },
};

const OrganizationMembersBookings = ({
  rootDataRelay,
  organizationId,
  organizationName,
  organizationsConnectionIds,
  hideRemoveOrganizationOption,
}: Props) => {
  const rootData = useFragment(
    graphql`
      fragment organizationMembersBookings_query on Query {
        organizationMembers(where: { organizationId: $organizationId }, orderBy: $peopleSortingValues) {
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
          defaultOrganization {
            uniqueId
          }
        }
        organization(id: $organizationId) {
          name
          logoUrl
          hasFutureBooking
          canModify
          canDelete
        }
        ...bookingsWeekGrid_query
        ...bookingsWeekGrid_allBookings_query
      }
    `,
    rootDataRelay,
  );

  const [commitDeleteOrganization] = useMutation<organizationMembersBookings_deleteOrganizationMutation>(graphql`
    mutation organizationMembersBookings_deleteOrganizationMutation($connectionIds: [ID!]!, $input: DeleteOrganizationInput!) {
      deleteOrganization(input: $input) {
        organization {
          id @deleteEdge(connections: $connectionIds)
        }
      }
    }
  `);

  const [commitSetCustomerDefaultOrganization] = useMutation<organizationMembersBookings_setCustomerDefaultOrganizationMutation>(graphql`
    mutation organizationMembersBookings_setCustomerDefaultOrganizationMutation($input: SetCustomerDefaultOrganizationInput!) {
      setCustomerDefaultOrganization(input: $input) {
        customer {
          id
          defaultOrganization {
            uniqueId
          }
        }
      }
    }
  `);

  const [commitClearCustomerDefaultOrganization] = useMutation<organizationMembersBookings_clearCustomerDefaultOrganizationMutation>(graphql`
    mutation organizationMembersBookings_clearCustomerDefaultOrganizationMutation($input: ClearCustomerDefaultOrganizationInput!) {
      clearCustomerDefaultOrganization(input: $input) {
        customer {
          id
          defaultOrganization {
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
  const [organizationRemoveConfirmationDialogOpen, setOrganizationRemoveConfirmationDialogOpen] = useState(false);
  const [startDate, setStartDate] = useState<Dayjs>(startOfDay());

  if (!rootData.me || !rootData.organization || !rootData.organizationMembers) {
    return <></>;
  }

  const handleDateRangeTypeChange = (_: React.MouseEvent<HTMLElement>, value: DateRangeType) => {
    let start = startOfDay();
    if (value === DateRangeType.NextWeek) {
      start = start.add(1, 'week');
    }

    setStartDate(start);
    setDateRangeType(value);
  };

  let moreActionsOption: MoreActionsMenuItemType[] = [];
  if (rootData.me.defaultOrganization?.uniqueId === organizationId) {
    moreActionsOption = moreActionsOption.concat(moreActionsMenuAllOptions[MoreActionsMenuOptionType.ClearAsPreferredOrganization]);
  } else {
    moreActionsOption = moreActionsOption.concat(moreActionsMenuAllOptions[MoreActionsMenuOptionType.MarkAsDefaultOrganization]);
  }

  if (rootData.organization.canDelete && !hideRemoveOrganizationOption) {
    moreActionsOption = moreActionsOption.concat(moreActionsMenuAllOptions[MoreActionsMenuOptionType.RemoveOrganization]);
  }

  const handleMoreActionsMenuClick = (event: React.MouseEvent<HTMLElement>) => {
    setMoreActionsAnchorEl(event.currentTarget);
  };
  const handleMoreActionsMenuItemClick = (id: MoreActionsMenuOptionType) => {
    setMoreActionsAnchorEl(null);

    switch (id) {
      case MoreActionsMenuOptionType.MarkAsDefaultOrganization:
        handleMarkAsDefaultOrganizationClicked();
        break;

      case MoreActionsMenuOptionType.ClearAsPreferredOrganization:
        handleClearAsDefaultOrganizationClicked();
        break;

      case MoreActionsMenuOptionType.RemoveOrganization:
        handleRemoveOrganizationClicked();
        break;
    }
  };

  const handleMarkAsDefaultOrganizationClicked = () => {
    if (!rootData.me) {
      return;
    }

    const toastId = themedToast(
      <NotificationContent content={`Setting organization '${organizationName}' as your default organization...`} />,
      infoNotificationOptions,
    );

    commitSetCustomerDefaultOrganization({
      variables: {
        input: {
          clientMutationId: nanoid(),
          organizationId: organizationId,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: (
              <NotificationContent content={`Failed to set '${organizationName}' as your default organization. Error: ${joinErrors(errors)}.`} />
            ),
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`'${organizationName}' is now your default organization.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to set '${organizationName}' as your default organization. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        setCustomerDefaultOrganization: {
          customer: {
            id: rootData.me.id,
            defaultOrganizations: { uniqueId: organizationId },
          },
        },
      },
    });
  };

  const handleClearAsDefaultOrganizationClicked = () => {
    if (!rootData.me) {
      return;
    }

    const toastId = themedToast(
      <NotificationContent content={`Unsetting organization '${organizationName}' as your default organization...`} />,
      infoNotificationOptions,
    );

    commitClearCustomerDefaultOrganization({
      variables: {
        input: {
          clientMutationId: nanoid(),
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: (
              <NotificationContent content={`Failed to unset '${organizationName}' as your default organization. Error: ${joinErrors(errors)}.`} />
            ),
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`'${organizationName}' is no longer set as your default organization.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to unset '${organizationName}' as your default organization. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        setCustomerDefaultOrganization: {
          customer: {
            id: rootData.me.id,
            defaultOrganization: null,
          },
        },
      },
    });
  };

  const handleRemoveOrganizationClicked = () => {
    setOrganizationRemoveConfirmationDialogOpen(true);
  };

  const handleCancelRemovingOrganizationClick = () => {
    setOrganizationRemoveConfirmationDialogOpen(false);
  };

  const handleConfirmRemovingOrganizationClick = () => {
    if (!rootData.me) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Removing organization '${organizationName}'...`} />, infoNotificationOptions);

    commitDeleteOrganization({
      variables: {
        connectionIds: organizationsConnectionIds,
        input: {
          clientMutationId: nanoid(),
          id: organizationId,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to remove organization '${organizationName}'. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Organization '${organizationName}' has been successfully removed.`} />,
        });
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove organization '${organizationName}'. Error: ${error.message}.`} />,
        });
      },
    });
  };

  return (
    <>
      <Card sx={{ maxWidth: 500, height: '100%' }}>
        <CardHeader
          title={<OrganizationLink id={organizationId} name={rootData.organization?.name} />}
          subheader={
            <StackRowFullWidth>
              <ToggleButtonGroup color="primary" value={dateRangeType} exclusive onChange={handleDateRangeTypeChange} size="small">
                <ToggleButton value={DateRangeType.ThisWeek}>This week</ToggleButton>
                <ToggleButton value={DateRangeType.NextWeek}>Next week</ToggleButton>
              </ToggleButtonGroup>
              <StackRow>
                <Link href={getOrganizationBookingsBaseLink(organizationId)}>
                  <BookingIcon />
                </Link>

                {rootData.organization.canModify && (
                  <Link href={getOrganizationSettingsBaseLink(organizationId)}>
                    <SettingsIcon color="secondary" />
                  </Link>
                )}
              </StackRow>
            </StackRowFullWidth>
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
          <BookingsWeekGrid
            rootDataRelay={rootData}
            rootDataAllBookingsRelay={rootData}
            organizationId={organizationId}
            startDate={startDate}
            customers={rootData.organizationMembers.edges.map(({ node }) => node.customer)}
          />
        </CardContent>
      </Card>
      <Menu anchorEl={moreActionsAnchorEl} open={moreActionsMenuOpen} onClose={handleMoreActionsMenuItemClick}>
        {moreActionsOption.map((option) => (
          <MenuItem key={option.id} onClick={() => handleMoreActionsMenuItemClick(option.id)}>
            <BodyIconTypography label={option.label} startElement={option.icon} />
          </MenuItem>
        ))}
      </Menu>

      <Dialog TransitionComponent={DialogTransition} open={organizationRemoveConfirmationDialogOpen} onClose={handleCancelRemovingOrganizationClick}>
        <DialogTitle>Remove organization</DialogTitle>
        <DialogContent>
          <DialogContentText>
            {rootData.organization.hasFutureBooking
              ? `Bookings are scheduled for the organization "${organizationName}". Are you sure you want to remove it?`
              : `Are you sure you want to remove the organization "${organizationName}"?`}
          </DialogContentText>

          <DialogActions>
            <Button color="secondary" variant="outlined" onClick={handleCancelRemovingOrganizationClick}>
              Cancel
            </Button>
            <Button color="warning" variant="contained" startIcon={<DangerIcon />} onClick={handleConfirmRemovingOrganizationClick}>
              Remove
            </Button>
          </DialogActions>
        </DialogContent>
      </Dialog>
    </>
  );
};

export default memo(OrganizationMembersBookings);
