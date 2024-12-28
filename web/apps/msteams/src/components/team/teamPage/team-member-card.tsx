import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardActions from '@mui/material/CardActions';
import CardHeader from '@mui/material/CardHeader';
import Dialog from '@mui/material/Dialog';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import DialogTitle from '@mui/material/DialogTitle';
import { CustomerAvatar } from '@repo/shared/components/avatars';
import { BodyIconTypography, TwoButtonsDialogActions } from '@repo/shared/components/commons';
import { DeleteIcon } from '@repo/shared/components/icons';
import {
  NotificationContent,
  errorNotificationOptions,
  infoNotificationOptions,
  successNotificationOptions,
} from '@repo/shared/components/notification';
import { DialogTransition } from '@repo/shared/components/transitions';
import { PaletteModeContext } from '@repo/shared/libs/providers';
import { getCustomerFullName, joinErrors } from '@repo/shared/libs/utils';
import graphql from 'babel-plugin-relay/macro';
import { nanoid } from 'nanoid';
import { memo, useContext, useMemo, useState } from 'react';
import { useFragment, useMutation } from 'react-relay';
import { toast } from 'react-toastify';
import type { teamMemberCard_TeamMemberDetails$key } from './__generated__/teamMemberCard_TeamMemberDetails.graphql';
import type { teamMemberCard_deleteTeamMemberMutation } from './__generated__/teamMemberCard_deleteTeamMemberMutation.graphql';
import type { teamMemberCard_query$key } from './__generated__/teamMemberCard_query.graphql';

type Props = {
  rootDataRelay: teamMemberCard_query$key;
  teamMemberDetailsRelay: teamMemberCard_TeamMemberDetails$key;
  connectionIds: string[];
  onRefetchNeeded: () => void;
};

const TeamMemberCard = ({ rootDataRelay, teamMemberDetailsRelay, connectionIds, onRefetchNeeded }: Props) => {
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

  const [commitDeleteTeamMember] = useMutation<teamMemberCard_deleteTeamMemberMutation>(graphql`
    mutation teamMemberCard_deleteTeamMemberMutation($connectionIds: [ID!]!, $input: DeleteTeamMemberInput!) {
      deleteTeamMember(input: $input) {
        teamMember {
          id @deleteEdge(connections: $connectionIds)
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

    commitDeleteTeamMember({
      variables: {
        connectionIds,
        input: {
          clientMutationId: nanoid(),
          id: teamMemberDetails.id,
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
              invertDefaultColor
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
          <TwoButtonsDialogActions
            onPrimaryClicked={handleConfirmRemovingTeamMemberClick}
            onSecondaryClicked={handleCancelRemovingTeamMemberClick}
            primaryLabel="Remove"
            secondaryLabel="Cancel"
          />
        </DialogContent>
      </Dialog>
    </>
  );
};

export default memo(TeamMemberCard);
