import type { invitationToJoinLocationNotificationCard_NotificationDetails$key } from '@/queries/__generated__/invitationToJoinLocationNotificationCard_NotificationDetails.graphql';
import type { invitationToJoinLocationNotificationCard_acceptInvitationToJoinLocationMutation } from '@/queries/__generated__/invitationToJoinLocationNotificationCard_acceptInvitationToJoinLocationMutation.graphql';
import type { invitationToJoinLocationNotificationCard_rejectInvitationToJoinLocationMutation } from '@/queries/__generated__/invitationToJoinLocationNotificationCard_rejectInvitationToJoinLocationMutation.graphql';
import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardActions from '@mui/material/CardActions';
import CardContent from '@mui/material/CardContent';
import CardHeader from '@mui/material/CardHeader';
import { CustomerAvatar } from '@repo/shared/components/avatars';
import { BodyIconTypography } from '@repo/shared/components/commons';
import { CancelIcon, CheckIcon } from '@repo/shared/components/icons';
import {
  NotificationContent,
  errorNotificationOptions,
  infoNotificationOptions,
  successNotificationOptions,
} from '@repo/shared/components/notification';
import { PaletteModeContext } from '@repo/shared/libs/providers';
import { getCustomerFullName, joinErrors } from '@repo/shared/libs/utils';
import { nanoid } from 'nanoid';
import { memo, useContext, useMemo, useState } from 'react';
import { graphql, useFragment, useMutation } from 'react-relay';
import { toast } from 'react-toastify';

type Props = {
  notificationDetailsRelay: invitationToJoinLocationNotificationCard_NotificationDetails$key;
};

enum CardState {
  Pending = 1,
  Rejecting = 2,
  Rejected = 3,
  Accepting = 4,
  Accepted = 5,
}

const InvitationToJoinLocationNotificationCard = ({ notificationDetailsRelay }: Props) => {
  const notificationDetails = useFragment(
    graphql`
      fragment invitationToJoinLocationNotificationCard_NotificationDetails on Notification {
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
        location {
          name
        }
      }
    `,
    notificationDetailsRelay,
  );

  const [commitAcceptInvitationToJoinLocation] = useMutation<invitationToJoinLocationNotificationCard_acceptInvitationToJoinLocationMutation>(graphql`
    mutation invitationToJoinLocationNotificationCard_acceptInvitationToJoinLocationMutation($input: AcceptInvitationToJoinLocationInput!) {
      acceptInvitationToJoinLocation(input: $input) {
        clientMutationId
      }
    }
  `);

  const [commitRejectInvitationToJoinLocation] = useMutation<invitationToJoinLocationNotificationCard_rejectInvitationToJoinLocationMutation>(graphql`
    mutation invitationToJoinLocationNotificationCard_rejectInvitationToJoinLocationMutation($input: RejectInvitationToJoinLocationInput!) {
      rejectInvitationToJoinLocation(input: $input) {
        clientMutationId
      }
    }
  `);

  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const invitedBy = useMemo(() => notificationDetails.invitedBy, [notificationDetails]);
  const location = useMemo(() => notificationDetails.location, [notificationDetails]);
  const [cardState, setCardState] = useState<CardState>(CardState.Pending);

  const handleRejectClick = () => {
    const toastId = themedToast(
      <NotificationContent content={`Rejecting invitation to join location '${location?.name}'...`} />,
      infoNotificationOptions,
    );

    commitRejectInvitationToJoinLocation({
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
            render: (
              <NotificationContent content={`Failed to reject invitation to join location '${location?.name}'. Error: ${joinErrors(errors)}.`} />
            ),
          });

          setCardState(CardState.Pending);

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Invitation to join location '${location?.name} rejected.`} />,
        });

        setCardState(CardState.Rejected);
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to reject invitation to join location '${location?.name}'. Error: ${error.message}.`} />,
        });

        setCardState(CardState.Pending);
      },
    });

    setCardState(CardState.Rejecting);
  };

  const handleAcceptClick = () => {
    const toastId = themedToast(
      <NotificationContent content={`Accpeting invitation to join location '${location?.name}'...`} />,
      infoNotificationOptions,
    );

    commitAcceptInvitationToJoinLocation({
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
            render: (
              <NotificationContent content={`Failed to accept invitation to join location '${location?.name}'. Error: ${joinErrors(errors)}.`} />
            ),
          });

          setCardState(CardState.Pending);

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Invitation to join location '${location?.name} accepted.`} />,
        });

        setCardState(CardState.Accepted);
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to accept invitation to join location '${location?.name}'. Error: ${error.message}.`} />,
        });

        setCardState(CardState.Pending);
      },
    });

    setCardState(CardState.Accepting);
  };

  return (
    <Card sx={{ minWidth: 350, height: '100%' }}>
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
            subheader={<BodyIconTypography label={`${getCustomerFullName(invitedBy)} has invited you to join location ${location?.name}`} />}
          />

          <CardActions sx={{ justifyContent: 'flex-end' }}>
            <Button color="secondary" variant="contained" startIcon={<CancelIcon />} onClick={handleRejectClick}>
              Reject
            </Button>
            <Button color="primary" variant="contained" type="submit" startIcon={<CheckIcon />} onClick={handleAcceptClick}>
              Accept
            </Button>
          </CardActions>
        </>
      )}

      {cardState === CardState.Rejecting && (
        <CardContent>
          <BodyIconTypography label={`Rejecting invitation to join ${location?.name}`} />
        </CardContent>
      )}

      {cardState === CardState.Rejected && (
        <CardContent>
          <BodyIconTypography label={`Rejected invitation to join ${location?.name}`} />
        </CardContent>
      )}

      {cardState === CardState.Accepting && (
        <CardContent>
          <BodyIconTypography label={`Accepting invitation to join ${location?.name}`} />
        </CardContent>
      )}

      {cardState === CardState.Accepted && (
        <CardContent>
          <BodyIconTypography label={`Accepted invitation to join ${location?.name}`} />
        </CardContent>
      )}
    </Card>
  );
};

export default memo(InvitationToJoinLocationNotificationCard);
