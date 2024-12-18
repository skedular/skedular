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
import { CustomerAvatar } from '@repo/shared/components/avatars';
import { BodyIconTypography } from '@repo/shared/components/commons';
import { DangerIcon, DeleteIcon } from '@repo/shared/components/icons';
import {
  NotificationContent,
  errorNotificationOptions,
  infoNotificationOptions,
  successNotificationOptions,
} from '@repo/shared/components/notification';
import { DialogTransition } from '@repo/shared/components/transitions';
import { PaletteModeContext } from '@repo/shared/libs/providers';
import { getCustomerFullName, joinErrors } from '@repo/shared/libs/utils';
import { nanoid } from 'nanoid';
import { memo, useContext, useMemo, useState } from 'react';
import { graphql, useFragment, useMutation } from 'react-relay';
import { toast } from 'react-toastify';

type Props = {
  rootDataRelay: teamMemberCard_query$key;
  teamMemberDetailsRelay: teamMemberCard_TeamMemberDetails$key;
  organizationId?: string;
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

  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
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

  const handleConfirmRemovingTeamMemberClick = () => {
    if (!rootData.team) {
      return;
    }

    setTeamMemberRemoveConfirmationDialogOpen(false);

    const toastId = themedToast(<NotificationContent content={`Removing team member...`} />, infoNotificationOptions);

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
            .map((member) => member.organizationMember!.uniqueId),
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to remove team member'. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Team member removed.`} />,
        });

        onRefetchNeeded();
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove team member. Error: ${error.message}.`} />,
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
      <Card sx={{ minWidth: 200, height: '100%' }}>
        <CardHeader
          title={
            <BodyIconTypography
              label={getCustomerFullName(customer)}
              startElement={<CustomerAvatar name={customer} photo={{ url: customer.photoUrl }} />}
            />
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

      <Dialog TransitionComponent={DialogTransition} open={teamMemberRemoveConfirmationDialogOpen} onClose={handleCancelRemovingTeamMemberClick}>
        <DialogTitle>Remove desk</DialogTitle>
        <DialogContent>
          <DialogContentText>{`Are you sure you want to remove "${getCustomerFullName(customer)}"?`}</DialogContentText>
          <DialogActions>
            <Button color="secondary" variant="outlined" onClick={handleCancelRemovingTeamMemberClick}>
              Cancel
            </Button>
            <Button color="warning" variant="contained" startIcon={<DangerIcon />} onClick={handleConfirmRemovingTeamMemberClick}>
              Remove
            </Button>
          </DialogActions>
        </DialogContent>
      </Dialog>
    </>
  );
};

export default memo(TeamMemberCard);
