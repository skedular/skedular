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
import graphql from 'babel-plugin-relay/macro';
import { BookingsWeekGrid } from 'components/booking';
import { TeamLink, getTeamBookingsLink, getTeamSettingsLink } from 'components/team';
import { Dayjs } from 'dayjs';
import { nanoid } from 'nanoid';
import { useSnackbar } from 'notistack';
import { memo, useState } from 'react';
import { useFragment, useMutation } from 'react-relay';
import type { teamPeopleBookings_addCustomerDefaultTeamMutation } from './__generated__/teamPeopleBookings_addCustomerDefaultTeamMutation.graphql';
import type { teamPeopleBookings_deleteTeamMutation } from './__generated__/teamPeopleBookings_deleteTeamMutation.graphql';
import type { teamPeopleBookings_query$key } from './__generated__/teamPeopleBookings_query.graphql';
import type { teamPeopleBookings_removeCustomerDefaultTeamMutation } from './__generated__/teamPeopleBookings_removeCustomerDefaultTeamMutation.graphql';

type Props = {
  rootDataRelay: teamPeopleBookings_query$key;
  organizationId: string;
  teamId: string;
  teamName?: string;
  teamsConnectionIds: string[];
  hideRemoveTeamOption?: boolean;
};

enum DateRangeType {
  ThisWeek,
  NextWeek,
}

enum MoreActionsMenuOptionType {
  SetAsPreferredTeam,
  RemoveAsPreferredTeam,
  RemoveTeam,
}

type MoreActionsMenuItemType = {
  id: MoreActionsMenuOptionType;
  label: String;
  icon: JSX.Element;
  color: 'inherit' | 'default' | 'primary' | 'secondary' | 'error' | 'info' | 'success' | 'warning';
};

const moreActionsMenuAllOptions: Record<MoreActionsMenuOptionType, MoreActionsMenuItemType> = {
  [MoreActionsMenuOptionType.SetAsPreferredTeam]: {
    id: MoreActionsMenuOptionType.SetAsPreferredTeam,
    label: 'Set as preferred team',
    icon: <NotPreferredIcon />,
    color: 'primary',
  },
  [MoreActionsMenuOptionType.RemoveAsPreferredTeam]: {
    id: MoreActionsMenuOptionType.RemoveAsPreferredTeam,
    label: 'Remove as preferred team',
    icon: <PreferredIcon />,
    color: 'primary',
  },
  [MoreActionsMenuOptionType.RemoveTeam]: {
    id: MoreActionsMenuOptionType.RemoveTeam,
    label: 'Remove team',
    icon: <DeleteIcon />,
    color: 'warning',
  },
};

const TeamPeopleBookings = ({ rootDataRelay, organizationId, teamId, teamName, teamsConnectionIds, hideRemoveTeamOption }: Props) => {
  const rootData = useFragment(
    graphql`
      fragment teamPeopleBookings_query on Query {
        teamMembers(where: { teamId: $teamId }, orderBy: $peopleSortingValues) {
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
          defaultTeams {
            uniqueId
          }
        }
        team(id: $teamId) {
          name
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

  const [commitDeleteTeam] = useMutation<teamPeopleBookings_deleteTeamMutation>(graphql`
    mutation teamPeopleBookings_deleteTeamMutation($connectionIds: [ID!]!, $input: DeleteTeamInput!) {
      deleteTeam(input: $input) {
        team {
          id @deleteEdge(connections: $connectionIds)
        }
      }
    }
  `);

  const [commitAddCustomerDefaultTeam] = useMutation<teamPeopleBookings_addCustomerDefaultTeamMutation>(graphql`
    mutation teamPeopleBookings_addCustomerDefaultTeamMutation($input: AddCustomerDefaultTeamInput!) {
      addCustomerDefaultTeam(input: $input) {
        customer {
          id
          defaultTeams {
            uniqueId
          }
        }
      }
    }
  `);

  const [commitRemoveCustomerDefaultTeam] = useMutation<teamPeopleBookings_removeCustomerDefaultTeamMutation>(graphql`
    mutation teamPeopleBookings_removeCustomerDefaultTeamMutation($input: RemoveCustomerDefaultTeamInput!) {
      removeCustomerDefaultTeam(input: $input) {
        customer {
          id
          defaultTeams {
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
  const [teamRemoveConfirmationDialogOpen, setTeamRemoveConfirmationDialogOpen] = useState(false);
  const [startDate, setStartDate] = useState<Dayjs>(startOfDay());

  if (!rootData.me || !rootData.team || !rootData.teamMembers) {
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
  if (rootData.me.defaultTeams.some((team) => team.uniqueId === teamId)) {
    moreActionsOption = moreActionsOption.concat(moreActionsMenuAllOptions[MoreActionsMenuOptionType.RemoveAsPreferredTeam]);
  } else {
    moreActionsOption = moreActionsOption.concat(moreActionsMenuAllOptions[MoreActionsMenuOptionType.SetAsPreferredTeam]);
  }

  if (rootData.team.canDelete && !hideRemoveTeamOption) {
    moreActionsOption = moreActionsOption.concat(moreActionsMenuAllOptions[MoreActionsMenuOptionType.RemoveTeam]);
  }

  const handleMoreActionsMenuClick = (event: React.MouseEvent<HTMLElement>) => {
    setMoreActionsAnchorEl(event.currentTarget);
  };
  const handleMoreActionsMenuItemClick = (id: MoreActionsMenuOptionType) => {
    setMoreActionsAnchorEl(null);

    switch (id) {
      case MoreActionsMenuOptionType.SetAsPreferredTeam:
        handleSetAsPreferredTeamClicked();
        break;

      case MoreActionsMenuOptionType.RemoveAsPreferredTeam:
        handleRemoveAsPreferredTeamClicked();
        break;

      case MoreActionsMenuOptionType.RemoveTeam:
        handleRemoveTeamClicked();
        break;
    }
  };

  const handleSetAsPreferredTeamClicked = () => {
    if (!rootData.me) {
      return;
    }

    commitAddCustomerDefaultTeam({
      variables: {
        input: {
          clientMutationId: nanoid(),
          teamId: teamId,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          enqueueSnackbar(`Failed to set team '${teamName}' as your preferred team. Error: ${joinErrors(errors)}`, {
            variant: 'error',
            anchorOrigin,
          });

          return;
        }

        enqueueSnackbar(`Team '${teamName}' has been set as the preferred team.`, {
          variant: 'success',
          anchorOrigin,
        });
      },
      onError: (error) => {
        enqueueSnackbar(`Failed to set team '${teamName}' as your preferred team. Error: ${error.message}`, {
          variant: 'error',
          anchorOrigin,
        });
      },
      optimisticResponse: {
        addCustomerDefaultTeam: {
          customer: {
            id: rootData.me.id,
            defaultTeams: rootData.me.defaultTeams.concat([
              {
                uniqueId: teamId,
              },
            ]),
          },
        },
      },
    });
  };

  const handleRemoveAsPreferredTeamClicked = () => {
    if (!rootData.me) {
      return;
    }

    commitRemoveCustomerDefaultTeam({
      variables: {
        input: {
          clientMutationId: nanoid(),
          teamId: teamId,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          enqueueSnackbar(`Failed to remove the team '${teamName}' as your preferred team. Error: ${joinErrors(errors)}`, {
            variant: 'error',
            anchorOrigin,
          });

          return;
        }

        enqueueSnackbar(`Team '${teamName}' has been removed as your preferred team.`, {
          variant: 'success',
          anchorOrigin,
        });
      },
      onError: (error) => {
        enqueueSnackbar(`Failed to remove the team '${teamName}' as your preferred team. Error: ${error.message}`, {
          variant: 'error',
          anchorOrigin,
        });
      },
      optimisticResponse: {
        addCustomerDefaultTeam: {
          customer: {
            id: rootData.me.id,
            defaultTeams: rootData.me.defaultTeams.filter(({ uniqueId }) => uniqueId === teamId),
          },
        },
      },
    });
  };

  const handleRemoveTeamClicked = () => {
    setTeamRemoveConfirmationDialogOpen(true);
  };

  const handleCancelRemovingTeamClick = () => {
    setTeamRemoveConfirmationDialogOpen(false);
  };

  const handleConfirmRemovingTeamClick = () => {
    if (!rootData.me) {
      return;
    }

    commitDeleteTeam({
      variables: {
        connectionIds: teamsConnectionIds,
        input: {
          clientMutationId: nanoid(),
          id: teamId,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          enqueueSnackbar(`Failed to remove team '${teamName}'. Error: ${joinErrors(errors)}`, {
            variant: 'error',
            anchorOrigin,
          });

          return;
        }

        enqueueSnackbar(`Team '${teamName}' has been successfully removed.`, {
          variant: 'success',
          anchorOrigin,
        });
      },
      onError: (error) => {
        enqueueSnackbar(`Failed to remove team '${teamName}'. Error: ${error.message}`, {
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
              <TeamLink organizationId={organizationId} id={teamId} name={teamName} />
            </Stack>
          }
          subheader={
            <Stack direction="row" sx={{ justifyContent: 'space-between', width: '100%', alignItems: 'center' }}>
              <ToggleButtonGroup color="primary" value={dateRangeType} exclusive onChange={handleDateRangeTypeChange} size="small">
                <ToggleButton value={DateRangeType.ThisWeek}>This week</ToggleButton>
                <ToggleButton value={DateRangeType.NextWeek}>Next week</ToggleButton>
              </ToggleButtonGroup>
              <Stack direction="row">
                <Link href={getTeamBookingsLink(teamId, organizationId)}>
                  <BookingIcon />
                </Link>

                {rootData.team.canModify && (
                  <Link href={getTeamSettingsLink(teamId, organizationId)}>
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
            customers={rootData.teamMembers.map((member) => member.customer)}
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

      <Dialog TransitionComponent={DialogTransition} open={teamRemoveConfirmationDialogOpen} onClose={handleCancelRemovingTeamClick}>
        <DialogTitle>Remove team</DialogTitle>
        <DialogContent>
          <DialogContentText>
            {rootData.team.hasFutureBooking
              ? `Bookings are scheduled for the team "${teamName}". Are you sure you want to remove it?`
              : `Are you sure you want to remove the team "${teamName}"?`}
          </DialogContentText>

          <DialogActions>
            <Button color="secondary" variant="outlined" onClick={handleCancelRemovingTeamClick}>
              Cancel
            </Button>
            <Button color="warning" variant="contained" startIcon={<DangerIcon />} onClick={handleConfirmRemovingTeamClick}>
              Remove
            </Button>
          </DialogActions>
        </DialogContent>
      </Dialog>
    </>
  );
};

export default memo(TeamPeopleBookings);
