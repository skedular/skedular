import type { invitationToJoinOrganizationNotificationCard_NotificationDetails$key } from '@/queries/__generated__/invitationToJoinOrganizationNotificationCard_NotificationDetails.graphql';
import type { invitationToJoinOrganizationNotificationCard_acceptInvitationToJoinOrganizationMutation } from '@/queries/__generated__/invitationToJoinOrganizationNotificationCard_acceptInvitationToJoinOrganizationMutation.graphql';
import type { invitationToJoinOrganizationNotificationCard_rejectInvitationToJoinOrganizationMutation } from '@/queries/__generated__/invitationToJoinOrganizationNotificationCard_rejectInvitationToJoinOrganizationMutation.graphql';
import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardActions from '@mui/material/CardActions';
import CardContent from '@mui/material/CardContent';
import CardHeader from '@mui/material/CardHeader';
import { CustomerAvatar } from '@repo/shared/components/avatars';
import { BodyIconTypography } from '@repo/shared/components/commons';
import { CancelIcon, CheckIcon } from '@repo/shared/components/icons';
import {
  errorNotificationOptions,
  infoNotificationOptions,
  NotificationContent,
  successNotificationOptions,
} from '@repo/shared/components/notification';
import { PaletteModeContext } from '@repo/shared/libs/providers';
import { getCustomerFullName, joinErrors } from '@repo/shared/libs/utils';
import { nanoid } from 'nanoid';
import { memo, useContext, useMemo, useState } from 'react';
import { graphql, useFragment, useMutation } from 'react-relay';
import { toast } from 'react-toastify';

type Props = {
  notificationDetailsRelay: invitationToJoinOrganizationNotificationCard_NotificationDetails$key;
};

enum CardState {
  Pending = 1,
  Rejecting = 2,
  Rejected = 3,
  Accepting = 4,
  Accepted = 5,
}

const InvitationToJoinOrganizationNotificationCard = ({ notificationDetailsRelay }: Props) => {
  const notificationDetails = useFragment(
    graphql`
      fragment invitationToJoinOrganizationNotificationCard_NotificationDetails on Notification {
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
        organization {
          name
        }
      }
    `,
    notificationDetailsRelay,
  );

  const [commitAcceptInvitationToJoinOrganization] =
    useMutation<invitationToJoinOrganizationNotificationCard_acceptInvitationToJoinOrganizationMutation>(graphql`
      mutation invitationToJoinOrganizationNotificationCard_acceptInvitationToJoinOrganizationMutation(
        $input: AcceptInvitationToJoinOrganizationInput!
      ) {
        acceptInvitationToJoinOrganization(input: $input) {
          clientMutationId
        }
      }
    `);

  const [commitRejectInvitationToJoinOrganization] =
    useMutation<invitationToJoinOrganizationNotificationCard_rejectInvitationToJoinOrganizationMutation>(graphql`
      mutation invitationToJoinOrganizationNotificationCard_rejectInvitationToJoinOrganizationMutation(
        $input: RejectInvitationToJoinOrganizationInput!
      ) {
        rejectInvitationToJoinOrganization(input: $input) {
          clientMutationId
        }
      }
    `);

  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const invitedBy = useMemo(() => notificationDetails.invitedBy, [notificationDetails]);
  const organization = useMemo(() => notificationDetails.organization, [notificationDetails]);
  const [cardState, setCardState] = useState<CardState>(CardState.Pending);

  const handleRejectClick = () => {
    const toastId = themedToast(
      <NotificationContent content={`Rejecting invitation to join organization '${organization?.name}'...`} />,
      infoNotificationOptions,
    );

    commitRejectInvitationToJoinOrganization({
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
              <NotificationContent
                content={`Failed to reject invitation to join organization '${organization?.name}'. Error: ${joinErrors(errors)}.`}
              />
            ),
          });

          setCardState(CardState.Pending);

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Invitation to join organization '${organization?.name} rejected.`} />,
        });

        setCardState(CardState.Rejected);
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: (
            <NotificationContent content={`Failed to reject invitation to join organization '${organization?.name}'. Error: ${error.message}.`} />
          ),
        });

        setCardState(CardState.Pending);
      },
    });

    setCardState(CardState.Rejecting);
  };

  const handleAcceptClick = () => {
    const toastId = themedToast(
      <NotificationContent content={`Accpeting invitation to join organization '${organization?.name}'...`} />,
      infoNotificationOptions,
    );

    commitAcceptInvitationToJoinOrganization({
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
              <NotificationContent
                content={`Failed to accept invitation to join organization '${organization?.name}'. Error: ${joinErrors(errors)}`}
              />
            ),
          });

          setCardState(CardState.Pending);

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Invitation to join organization '${organization?.name} accepted.`} />,
        });

        setCardState(CardState.Accepted);
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: (
            <NotificationContent content={`Failed to accept invitation to join organization '${organization?.name}'. Error: ${error.message}.`} />
          ),
        });

        setCardState(CardState.Pending);
      },
    });

    setCardState(CardState.Accepting);
  };

  return (
    <Card elevation={24} sx={{ minWidth: 350, height: '100%' }}>
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
            subheader={<BodyIconTypography label={`${getCustomerFullName(invitedBy)} has invited you to join organization ${organization?.name}`} />}
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
          <BodyIconTypography label={`Rejecting invitation to join ${organization?.name}`} />
        </CardContent>
      )}

      {cardState === CardState.Rejected && (
        <CardContent>
          <BodyIconTypography label={`Rejected invitation to join ${organization?.name}`} />
        </CardContent>
      )}

      {cardState === CardState.Accepting && (
        <CardContent>
          <BodyIconTypography label={`Accepting invitation to join ${organization?.name}`} />
        </CardContent>
      )}

      {cardState === CardState.Accepted && (
        <CardContent>
          <BodyIconTypography label={`Accepted invitation to join ${organization?.name}`} />
        </CardContent>
      )}
    </Card>
  );
};

export default memo(InvitationToJoinOrganizationNotificationCard);
