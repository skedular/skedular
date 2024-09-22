import type { teamMemberCard_TeamMemberDetails$key } from '@/queries/__generated__/teamMemberCard_TeamMemberDetails.graphql';
import type { teamMemberCard_query$key } from '@/queries/__generated__/teamMemberCard_query.graphql';
import type { teamMemberCard_updateTeamMutation } from '@/queries/__generated__/teamMemberCard_updateTeamMutation.graphql';
import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardActions from '@mui/material/CardActions';
import CardHeader from '@mui/material/CardHeader';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import DialogTitle from '@mui/material/DialogTitle';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import { CustomerAvatar } from '@repo/shared/components/avatars';
import { DangerIcon, DeleteIcon } from '@repo/shared/components/icons';
import { SnackbarAnchorOrigin as anchorOrigin } from '@repo/shared/libs/snackbar';
import { getCustomerFullName, joinErrors } from '@repo/shared/libs/utils';
import { nanoid } from 'nanoid';
import { useSnackbar } from 'notistack';
import { memo, useMemo, useState } from 'react';
import { graphql, useFragment, useMutation } from 'react-relay';

type Props = {
  rootDataRelay: teamMemberCard_query$key;
  teamMemberDetailsRelay: teamMemberCard_TeamMemberDetails$key;
  organizationId: string | null;
  onRefetchNeeded: () => void;
};

const TeamMemberCard = ({ teamMemberDetailsRelay, rootDataRelay, organizationId, onRefetchNeeded }: Props) => {
  const rootData = useFragment<teamMemberCard_query$key>(
    graphql`
      fragment teamMemberCard_query on Query {
        team(id: $teamId) {
          id
          name
          about
          canModify
          members {
            id
            customer {
              uniqueId
            }
            organizationMember {
              uniqueId
            }
          }
        }
      }
    `,
    rootDataRelay,
  );

  const teamMemberDetails = useFragment(
    graphql`
      fragment teamMemberCard_TeamMemberDetails on TeamMemberDetails {
        id
        customer {
          name
          givenName
          middleName
          familyName
          photoUrl
        }
        organizationMember {
          customer {
            name
            givenName
            middleName
            familyName
            photoUrl
          }
        }
      }
    `,
    teamMemberDetailsRelay,
  );

  const [commitUpdateTeam] = useMutation<teamMemberCard_updateTeamMutation>(graphql`
    mutation teamMemberCard_updateTeamMutation($input: UpdateTeamInput!) @raw_response_type {
      updateTeam(input: $input) {
        team {
          id
        }
      }
    }
  `);

  const { enqueueSnackbar } = useSnackbar();
  const [teamMemberRemoveConfirmationDialogOpen, setTeamMemberRemoveConfirmationDialogOpen] = useState(false);
  const customer = useMemo(() => {
    if (teamMemberDetails.customer) {
      return teamMemberDetails.customer;
    }

    if (teamMemberDetails.organizationMember) {
      return teamMemberDetails.organizationMember.customer;
    }

    return null;
  }, [teamMemberDetails.customer, teamMemberDetails.organizationMember]);

  const handleDeleteClick = () => {
    setTeamMemberRemoveConfirmationDialogOpen(true);
  };

  const handleCancelRemovingTeamMemberClick = () => {
    setTeamMemberRemoveConfirmationDialogOpen(false);
  };

  const handleConfirmRemovingDeskClick = () => {
    if (!rootData.team) {
      return;
    }

    setTeamMemberRemoveConfirmationDialogOpen(false);

    commitUpdateTeam({
      variables: {
        input: {
          clientMutationId: nanoid(),
          id: rootData.team.id,
          name: rootData.team.name,
          about: rootData.team.about,
          customerIds: rootData.team.members
            .filter((member) => member.customer && member.id !== teamMemberDetails.id)
            .map((member) => member.customer.uniqueId),
          organizationId,
          organizationMemberIds: rootData.team.members
            .filter((member) => member.organizationMember && member.id !== teamMemberDetails.id)
            // @ts-expect-error
            .map((member) => member.organizationMember.uniqueId),
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          enqueueSnackbar(`Failed to update team '${rootData.team?.name}'. Error: ${joinErrors(errors)}`, {
            variant: 'error',
            anchorOrigin,
          });
        } else {
          onRefetchNeeded();
        }
      },
      onError: (error) => {
        enqueueSnackbar(`Failed to update team '${rootData.team?.name}'. Error: ${error.message}`, {
          variant: 'error',
          anchorOrigin,
        });
      },
      optimisticResponse: {
        updateTeam: {
          team: {
            id: rootData.team.id,
          },
        },
      },
    });
  };

  if (!customer || !rootData.team) {
    return <></>;
  }

  return (
    <>
      <Card elevation={24} sx={{ minWidth: 200, height: '100%' }}>
        <CardHeader
          title={
            <Stack direction="row" spacing={1} sx={{ alignItems: 'center' }}>
              <CustomerAvatar
                name={{
                  name: customer.name,
                  givenName: customer.givenName,
                  middleName: customer.middleName,
                  familyName: customer.familyName,
                }}
                photo={{
                  url: customer.photoUrl,
                }}
              />
              <Typography variant="body1">{getCustomerFullName(customer)}</Typography>
            </Stack>
          }
        />

        {rootData.team.canModify && (
          <CardActions sx={{ justifyContent: 'flex-end' }}>
            <Button size="small" color="warning" onClick={handleDeleteClick}>
              <DeleteIcon />
            </Button>
          </CardActions>
        )}
      </Card>

      <Dialog fullWidth={true} open={teamMemberRemoveConfirmationDialogOpen} onClose={handleCancelRemovingTeamMemberClick}>
        <DialogTitle>Remove desk</DialogTitle>
        <DialogContent>
          <DialogContentText>{`Are you sure you want to remove "${getCustomerFullName(customer)}"?`}</DialogContentText>
          <DialogActions>
            <Button color="secondary" variant="outlined" onClick={handleCancelRemovingTeamMemberClick}>
              Cancel
            </Button>
            <Button color="warning" variant="contained" startIcon={<DangerIcon />} onClick={handleConfirmRemovingDeskClick}>
              Remove
            </Button>
          </DialogActions>
        </DialogContent>
      </Dialog>
    </>
  );
};

export default memo(TeamMemberCard);
