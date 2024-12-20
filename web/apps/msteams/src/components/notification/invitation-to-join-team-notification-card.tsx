import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import CardHeader from '@mui/material/CardHeader';
import { CustomerAvatar } from '@repo/shared/components/avatars';
import { BodyIconTypography, TwoButtonsCardActions } from '@repo/shared/components/commons';
import {
  NotificationContent,
  errorNotificationOptions,
  infoNotificationOptions,
  successNotificationOptions,
} from '@repo/shared/components/notification';
import { PaletteModeContext } from '@repo/shared/libs/providers';
import { getCustomerFullName, joinErrors } from '@repo/shared/libs/utils';
import graphql from 'babel-plugin-relay/macro';
import { nanoid } from 'nanoid';
import { memo, useContext, useMemo, useState } from 'react';
import { useFragment, useMutation } from 'react-relay';
import { toast } from 'react-toastify';
import type { invitationToJoinTeamNotificationCard_NotificationDetails$key } from './__generated__/invitationToJoinTeamNotificationCard_NotificationDetails.graphql';
import type { invitationToJoinTeamNotificationCard_acceptInvitationToJoinTeamMutation } from './__generated__/invitationToJoinTeamNotificationCard_acceptInvitationToJoinTeamMutation.graphql';
import type { invitationToJoinTeamNotificationCard_rejectInvitationToJoinTeamMutation } from './__generated__/invitationToJoinTeamNotificationCard_rejectInvitationToJoinTeamMutation.graphql';

type Props = {
  notificationDetailsRelay: invitationToJoinTeamNotificationCard_NotificationDetails$key;
};

enum CardState {
  Pending = 1,
  Rejecting = 2,
  Rejected = 3,
  Accepting = 4,
  Accepted = 5,
}

const InvitationToJoinTeamNotificationCard = ({ notificationDetailsRelay }: Props) => {
  const notificationDetails = useFragment(
    graphql`
      fragment invitationToJoinTeamNotificationCard_NotificationDetails on Notification {
        id
        sourceId
        invitedBy {
          name
          givenName
          middleName
          familyName
          photoUrl
        }
        invitee {
          name
          givenName
          middleName
          familyName
          photoUrl
        }
        team {
          name
        }
      }
    `,
    notificationDetailsRelay,
  );

  const [commitAcceptInvitationToJoinTeam] = useMutation<invitationToJoinTeamNotificationCard_acceptInvitationToJoinTeamMutation>(graphql`
    mutation invitationToJoinTeamNotificationCard_acceptInvitationToJoinTeamMutation($input: AcceptInvitationToJoinTeamInput!) {
      acceptInvitationToJoinTeam(input: $input) {
        clientMutationId
      }
    }
  `);

  const [commitRejectInvitationToJoinTeam] = useMutation<invitationToJoinTeamNotificationCard_rejectInvitationToJoinTeamMutation>(graphql`
    mutation invitationToJoinTeamNotificationCard_rejectInvitationToJoinTeamMutation($input: RejectInvitationToJoinTeamInput!) {
      rejectInvitationToJoinTeam(input: $input) {
        clientMutationId
      }
    }
  `);

  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const invitedBy = useMemo(() => notificationDetails.invitedBy, [notificationDetails]);
  const team = useMemo(() => notificationDetails.team, [notificationDetails]);
  const [cardState, setCardState] = useState<CardState>(CardState.Pending);

  const handleRejectClick = () => {
    const toastId = themedToast(<NotificationContent content={`Rejecting invitation to join team '${team?.name}'...`} />, infoNotificationOptions);

    commitRejectInvitationToJoinTeam({
      variables: {
        input: {
          clientMutationId: nanoid(),
          id: notificationDetails.sourceId,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to reject invitation to join team '${team?.name}'. Error: ${joinErrors(errors)}.`} />,
          });

          setCardState(CardState.Pending);

          return;
        }

        setCardState(CardState.Rejected);
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to reject invitation to join team '${team?.name}'. Error: ${error.message}.`} />,
        });

        setCardState(CardState.Pending);
      },
    });

    setCardState(CardState.Rejecting);
  };

  const handleAcceptClick = () => {
    const toastId = themedToast(<NotificationContent content={`Accpeting invitation to join team '${team?.name}'...`} />, infoNotificationOptions);

    commitAcceptInvitationToJoinTeam({
      variables: {
        input: {
          clientMutationId: nanoid(),
          id: notificationDetails.sourceId,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to accept invitation to join team '${team?.name}'. Error: ${joinErrors(errors)}.`} />,
          });

          setCardState(CardState.Pending);

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Invitation to join team '${team?.name} accepted.`} />,
        });

        setCardState(CardState.Accepted);
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to accept invitation to join team '${team?.name}'. Error: ${error.message}.`} />,
        });

        setCardState(CardState.Pending);
      },
    });

    setCardState(CardState.Accepting);
  };

  return (
    <Card sx={{ minWidth: 400, height: '100%' }}>
      {cardState === CardState.Pending && (
        <>
          <CardHeader
            title={
              <CustomerAvatar
                name={{
                  name: null,
                  givenName: invitedBy?.givenName,
                  middleName: invitedBy?.middleName,
                  familyName: invitedBy?.familyName,
                }}
                photo={{
                  url: invitedBy?.photoUrl,
                }}
              />
            }
          />
          <TwoButtonsCardActions
            onPrimaryClicked={handleAcceptClick}
            onSecondaryClicked={handleRejectClick}
            primaryLabel="Accept"
            secondaryLabel="Reject"
          />
        </>
      )}

      <CardContent>
        <BodyIconTypography label={`${getCustomerFullName(invitedBy)} has invited you to join team ${team?.name}`} />
        {cardState === CardState.Rejecting && <BodyIconTypography label={`Rejecting invitation to join ${team?.name}`} />}
        {cardState === CardState.Rejected && <BodyIconTypography label={`Rejected invitation to join ${team?.name}`} />}
        {cardState === CardState.Accepting && <BodyIconTypography label={`Accepting invitation to join ${team?.name}`} />}
        {cardState === CardState.Accepted && <BodyIconTypography label={`Accepted invitation to join ${team?.name}`} />}
      </CardContent>
    </Card>
  );
};

export default memo(InvitationToJoinTeamNotificationCard);
