import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardActions from '@mui/material/CardActions';
import CardContent from '@mui/material/CardContent';
import CardHeader from '@mui/material/CardHeader';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import DialogTitle from '@mui/material/DialogTitle';
import Link from '@mui/material/Link';
import Stack from '@mui/material/Stack';
import Switch from '@mui/material/Switch';
import Typography from '@mui/material/Typography';
import { TeamAvatar } from '@repo/shared/components/avatars';
import { AboutIcon, DangerIcon, DeleteIcon, EditIcon, OrganizationIcon, ViewIcon } from '@repo/shared/components/icons';
import { SnackbarAnchorOrigin as anchorOrigin } from '@repo/shared/libs/snackbar';
import { joinErrors } from '@repo/shared/libs/utils';
import graphql from 'babel-plugin-relay/macro';
import { nanoid } from 'nanoid';
import { useSnackbar } from 'notistack';
import { memo, useState } from 'react';
import { useFragment, useMutation } from 'react-relay';
import type { teamCard_Query$key } from './__generated__/teamCard_Query.graphql';
import type { teamCard_TeamDetails$key } from './__generated__/teamCard_TeamDetails.graphql';
import type { teamCard_addCustomerDefaultTeamMutation } from './__generated__/teamCard_addCustomerDefaultTeamMutation.graphql';
import type { teamCard_deleteTeamMutation } from './__generated__/teamCard_deleteTeamMutation.graphql';
import type { teamCard_removeCustomerDefaultTeamMutation } from './__generated__/teamCard_removeCustomerDefaultTeamMutation.graphql';

type Props = {
  rootDataRelay: teamCard_Query$key;
  teamDetailsRelay: teamCard_TeamDetails$key;
  connectionIds: string[];
};

const TeamCard = ({ rootDataRelay, teamDetailsRelay: team, connectionIds }: Props) => {
  const rootData = useFragment(
    graphql`
      fragment teamCard_Query on Query {
        me {
          id
          defaultTeams {
            uniqueId
          }
        }
      }
    `,
    rootDataRelay,
  );

  const teamDetails = useFragment(
    graphql`
      fragment teamCard_TeamDetails on TeamDetails {
        id
        name
        about
        organization {
          uniqueId
          name
        }
        hasFutureBooking
        canModify
        canDelete
      }
    `,
    team,
  );

  const [commitDeleteTeam] = useMutation<teamCard_deleteTeamMutation>(graphql`
    mutation teamCard_deleteTeamMutation($connectionIds: [ID!]!, $input: DeleteTeamInput!) {
      deleteTeam(input: $input) {
        team {
          id @deleteEdge(connections: $connectionIds)
        }
      }
    }
  `);

  const [commitAddCustomerDefaultTeam] = useMutation<teamCard_addCustomerDefaultTeamMutation>(graphql`
    mutation teamCard_addCustomerDefaultTeamMutation($input: AddCustomerDefaultTeamInput!) {
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

  const [commitRemoveCustomerDefaultTeam] = useMutation<teamCard_removeCustomerDefaultTeamMutation>(graphql`
    mutation teamCard_removeCustomerDefaultTeamMutation($input: RemoveCustomerDefaultTeamInput!) {
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
  const [teamRemoveConfirmationDialogOpen, setTeamRemoveConfirmationDialogOpen] = useState(false);

  const handleDeleteClick = () => {
    setTeamRemoveConfirmationDialogOpen(true);
  };

  const handleDefaultTeamStateChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    if (!rootData.me) {
      return;
    }

    if (event.target.checked) {
      commitAddCustomerDefaultTeam({
        variables: {
          input: {
            clientMutationId: nanoid(),
            teamId: teamDetails.id,
          },
        },
        onCompleted: (_, errors) => {
          if (errors && errors.length > 0) {
            enqueueSnackbar(`Failed to set team '${teamDetails.name}' as your preferred team. Error: ${joinErrors(errors)}`, {
              variant: 'error',
              anchorOrigin,
            });
          }
        },
        onError: (error) => {
          enqueueSnackbar(`Failed to set team '${teamDetails.name}' as your preferred team. Error: ${error.message}`, {
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
                  uniqueId: teamDetails.id,
                },
              ]),
            },
          },
        },
      });
    } else {
      commitRemoveCustomerDefaultTeam({
        variables: {
          input: {
            clientMutationId: nanoid(),
            teamId: teamDetails.id,
          },
        },
        onCompleted: (_, errors) => {
          if (errors && errors.length > 0) {
            enqueueSnackbar(`Failed to remove the team '${teamDetails.name}' as your preferred team. Error: ${joinErrors(errors)}`, {
              variant: 'error',
              anchorOrigin,
            });
          }
        },
        onError: (error) => {
          enqueueSnackbar(`Failed to remove the team '${teamDetails.name}' as your preferred team. Error: ${error.message}`, {
            variant: 'error',
            anchorOrigin,
          });
        },
        optimisticResponse: {
          removeCustomerDefaultTeam: {
            customer: {
              id: rootData.me.id,
              defaultTeams: rootData.me.defaultTeams.filter(({ uniqueId }) => uniqueId === teamDetails.id),
            },
          },
        },
      });
    }
  };

  const handleCancelRemovingTeamClick = () => {
    setTeamRemoveConfirmationDialogOpen(false);
  };

  const handleConfirmRemovingTeamClick = () => {
    setTeamRemoveConfirmationDialogOpen(false);

    commitDeleteTeam({
      variables: {
        connectionIds: connectionIds,
        input: {
          clientMutationId: nanoid(),
          id: teamDetails.id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          enqueueSnackbar(`Failed to delete team '${teamDetails.name}'. Error: ${joinErrors(errors)}`, {
            variant: 'error',
            anchorOrigin,
          });
        }
      },
      onError: (error) => {
        enqueueSnackbar(`Failed to delete team '${teamDetails.name}'. Error: ${error.message}`, {
          variant: 'error',
          anchorOrigin,
        });
      },
    });
  };

  return (
    <>
      <Card elevation={24} sx={{ minWidth: 350, height: '100%' }}>
        <CardHeader
          title={
            <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
              <TeamAvatar name={{ name: teamDetails.name }} photo={{ url: null }} />
              {teamDetails.name && (
                <Typography variant="h5" noWrap={true}>
                  {teamDetails.name}
                </Typography>
              )}
            </Stack>
          }
        />

        <CardContent>
          {teamDetails.about && (
            <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
              <AboutIcon />
              <Typography variant="body1" noWrap={true}>
                {teamDetails.about}
              </Typography>
            </Stack>
          )}

          {teamDetails.organization && (
            <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
              <OrganizationIcon />
              <Typography variant="body1" noWrap={true}>
                {teamDetails.organization.name}
              </Typography>
            </Stack>
          )}
        </CardContent>

        <CardActions sx={{ justifyContent: 'flex-end' }}>
          {teamDetails.organization && (
            <Link href={`/organization/${teamDetails.organization.uniqueId}/team/${teamDetails.id}`}>
              <Button size="small" color="primary">
                {teamDetails.canModify ? <EditIcon /> : <ViewIcon />}
              </Button>
            </Link>
          )}

          {!teamDetails.organization && (
            <Link href={`/team/${teamDetails.id}`}>
              <Button size="small" color="primary">
                {teamDetails.canModify ? <EditIcon /> : <ViewIcon />}
              </Button>
            </Link>
          )}

          {teamDetails.canDelete && (
            <Button size="small" color="warning" onClick={handleDeleteClick}>
              <DeleteIcon />
            </Button>
          )}

          <Switch checked={!!rootData.me?.defaultTeams.find((team) => team.uniqueId === teamDetails.id)} onChange={handleDefaultTeamStateChange} />
        </CardActions>
      </Card>

      <Dialog fullWidth={true} open={teamRemoveConfirmationDialogOpen} onClose={handleCancelRemovingTeamClick}>
        <DialogTitle>Remove team</DialogTitle>
        <DialogContent>
          <DialogContentText>
            {teamDetails.hasFutureBooking
              ? `Bookings are scheduled for the team "${teamDetails.name}". Are you sure you want to remove it?`
              : `Are you sure you want to remove the team "${teamDetails.name}"?`}
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

export default memo(TeamCard);
