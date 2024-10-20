import { BookingsWeekGrid } from '@/components/booking';
import { getOrganizationBookingsBaseLink, getOrganizationSettingsBaseLink, OrganizationLink } from '@/components/organization';
import type { organizationPeopleBookings_clearCustomerDefaultOrganizationMutation } from '@/queries/__generated__/organizationPeopleBookings_clearCustomerDefaultOrganizationMutation.graphql';
import type { organizationPeopleBookings_deleteOrganizationMutation } from '@/queries/__generated__/organizationPeopleBookings_deleteOrganizationMutation.graphql';
import type { organizationPeopleBookings_query$key } from '@/queries/__generated__/organizationPeopleBookings_query.graphql';
import type { organizationPeopleBookings_setCustomerDefaultOrganizationMutation } from '@/queries/__generated__/organizationPeopleBookings_setCustomerDefaultOrganizationMutation.graphql';
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
import { BookingIcon, DangerIcon, DeleteIcon, EllipseMenuIcon, NotPreferredIcon, PreferredIcon, SettingsIcon } from '@repo/shared/components/icons';
import { DialogTransition } from '@repo/shared/components/transitions';
import { SnackbarAnchorOrigin as anchorOrigin } from '@repo/shared/libs/snackbar';
import { joinErrors, startOfDay } from '@repo/shared/libs/utils';
import { Dayjs } from 'dayjs';
import { nanoid } from 'nanoid';
import NextLink from 'next/link';
import { useSnackbar } from 'notistack';
import { memo, useState } from 'react';
import { graphql, useFragment, useMutation } from 'react-relay';

type Props = {
  rootDataRelay: organizationPeopleBookings_query$key;
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
  label: String;
  icon: JSX.Element;
  color: 'inherit' | 'default' | 'primary' | 'secondary' | 'error' | 'info' | 'success' | 'warning';
};

const moreActionsMenuAllOptions: Record<MoreActionsMenuOptionType, MoreActionsMenuItemType> = {
  [MoreActionsMenuOptionType.MarkAsDefaultOrganization]: {
    id: MoreActionsMenuOptionType.MarkAsDefaultOrganization,
    label: 'Mark as default organization',
    icon: <NotPreferredIcon />,
    color: 'primary',
  },
  [MoreActionsMenuOptionType.ClearAsPreferredOrganization]: {
    id: MoreActionsMenuOptionType.ClearAsPreferredOrganization,
    label: 'Clear as default organization',
    icon: <PreferredIcon />,
    color: 'primary',
  },
  [MoreActionsMenuOptionType.RemoveOrganization]: {
    id: MoreActionsMenuOptionType.RemoveOrganization,
    label: 'Remove organization',
    icon: <DeleteIcon />,
    color: 'warning',
  },
};

const OrganizationPeopleBookings = ({
  rootDataRelay,
  organizationId,
  organizationName,
  organizationsConnectionIds,
  hideRemoveOrganizationOption,
}: Props) => {
  const rootData = useFragment(
    graphql`
      fragment organizationPeopleBookings_query on Query {
        organizationMembers(where: { organizationId: $organizationId }, orderBy: $peopleSortingValues) {
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

  const [commitDeleteOrganization] = useMutation<organizationPeopleBookings_deleteOrganizationMutation>(graphql`
    mutation organizationPeopleBookings_deleteOrganizationMutation($connectionIds: [ID!]!, $input: DeleteOrganizationInput!) {
      deleteOrganization(input: $input) {
        organization {
          id @deleteEdge(connections: $connectionIds)
        }
      }
    }
  `);

  const [commitSetCustomerDefaultOrganization] = useMutation<organizationPeopleBookings_setCustomerDefaultOrganizationMutation>(graphql`
    mutation organizationPeopleBookings_setCustomerDefaultOrganizationMutation($input: SetCustomerDefaultOrganizationInput!) {
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

  const [commitClearCustomerDefaultOrganization] = useMutation<organizationPeopleBookings_clearCustomerDefaultOrganizationMutation>(graphql`
    mutation organizationPeopleBookings_clearCustomerDefaultOrganizationMutation($input: ClearCustomerDefaultOrganizationInput!) {
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

  const { enqueueSnackbar } = useSnackbar();
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

    commitSetCustomerDefaultOrganization({
      variables: {
        input: {
          clientMutationId: nanoid(),
          organizationId: organizationId,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          enqueueSnackbar(`Failed to mark '${organizationName}' as your default organization. Error: ${joinErrors(errors)}`, {
            variant: 'error',
            anchorOrigin,
          });

          return;
        }

        enqueueSnackbar(`'${organizationName}' is now your default organization.`, {
          variant: 'success',
          anchorOrigin,
        });
      },
      onError: (error) => {
        enqueueSnackbar(`Failed to mark '${organizationName}' as your default organization. Error: ${error.message}`, {
          variant: 'error',
          anchorOrigin,
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

    commitClearCustomerDefaultOrganization({
      variables: {
        input: {
          clientMutationId: nanoid(),
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          enqueueSnackbar(`Failed to clear '${organizationName}' as your default organization. Error: ${joinErrors(errors)}`, {
            variant: 'error',
            anchorOrigin,
          });

          return;
        }

        enqueueSnackbar(`'${organizationName}' is no longer set as your default organization.`, {
          variant: 'success',
          anchorOrigin,
        });
      },
      onError: (error) => {
        enqueueSnackbar(`Failed to clear '${organizationName}' as your default organization. Error: ${error.message}`, {
          variant: 'error',
          anchorOrigin,
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
          enqueueSnackbar(`Failed to remove organization '${organizationName}'. Error: ${joinErrors(errors)}`, {
            variant: 'error',
            anchorOrigin,
          });

          return;
        }

        enqueueSnackbar(`Organization '${organizationName}' has been successfully removed.`, {
          variant: 'success',
          anchorOrigin,
        });
      },
      onError: (error) => {
        enqueueSnackbar(`Failed to remove organization '${organizationName}'. Error: ${error.message}`, {
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
          title={<OrganizationLink id={organizationId} name={rootData.organization?.name} />}
          subheader={
            <Stack direction="row" sx={{ justifyContent: 'space-between', width: '100%', alignItems: 'center' }}>
              <ToggleButtonGroup color="primary" value={dateRangeType} exclusive onChange={handleDateRangeTypeChange} size="small">
                <ToggleButton value={DateRangeType.ThisWeek}>This week</ToggleButton>
                <ToggleButton value={DateRangeType.NextWeek}>Next week</ToggleButton>
              </ToggleButtonGroup>
              <Stack direction="row">
                <Link component={NextLink} href={getOrganizationBookingsBaseLink(organizationId)}>
                  <BookingIcon />
                </Link>

                {rootData.organization.canModify && (
                  <Link component={NextLink} href={getOrganizationSettingsBaseLink(organizationId)}>
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
          <BookingsWeekGrid
            rootDataRelay={rootData}
            rootDataAllBookingsRelay={rootData}
            organizationId={organizationId}
            startDate={startDate}
            customers={rootData.organizationMembers.map((member) => member.customer)}
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

export default memo(OrganizationPeopleBookings);
