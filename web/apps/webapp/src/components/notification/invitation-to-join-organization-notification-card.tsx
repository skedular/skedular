import type { invitationToJoinOrganizationNotificationCard_NotificationDetails$key } from '@/queries/__generated__/invitationToJoinOrganizationNotificationCard_NotificationDetails.graphql';
import type { invitationToJoinOrganizationNotificationCard_acceptInvitationToJoinOrganizationMutation } from '@/queries/__generated__/invitationToJoinOrganizationNotificationCard_acceptInvitationToJoinOrganizationMutation.graphql';
import type { invitationToJoinOrganizationNotificationCard_rejectInvitationToJoinOrganizationMutation } from '@/queries/__generated__/invitationToJoinOrganizationNotificationCard_rejectInvitationToJoinOrganizationMutation.graphql';
import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardActions from '@mui/material/CardActions';
import CardContent from '@mui/material/CardContent';
import CardHeader from '@mui/material/CardHeader';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import { CustomerAvatar } from '@repo/shared/components/avatars';
import { CancelIcon, CheckIcon } from '@repo/shared/components/icons';
import { SnackbarAnchorOrigin as anchorOrigin } from '@repo/shared/libs/snackbar';
import { getCustomerFullName, joinErrors } from '@repo/shared/libs/utils';
import { nanoid } from 'nanoid';
import { useSnackbar } from 'notistack';
import { memo, useMemo, useState } from 'react';
import { graphql, useFragment, useMutation } from 'react-relay';

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

  const invitedBy = useMemo(() => notificationDetails.invitedBy, [notificationDetails]);
  const organization = useMemo(() => notificationDetails.organization, [notificationDetails]);

  const { enqueueSnackbar } = useSnackbar();
  const [cardState, setCardState] = useState<CardState>(CardState.Pending);

  const handleRejectClick = () => {
    commitRejectInvitationToJoinOrganization({
      variables: {
        input: {
          clientMutationId: nanoid(),
          id: notificationDetails.sourceId,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          enqueueSnackbar(`Failed to reject invitation to join organization '${organization?.name}'. Error: ${joinErrors(errors)}`, {
            variant: 'error',
            anchorOrigin,
          });

          setCardState(CardState.Pending);

          return;
        }

        setCardState(CardState.Rejected);
      },
      onError: (error) => {
        enqueueSnackbar(`Failed to reject invitation to join organization '${organization?.name}'. Error: ${error.message}`, {
          variant: 'error',
          anchorOrigin,
        });

        setCardState(CardState.Pending);
      },
    });

    setCardState(CardState.Rejecting);
  };

  const handleAcceptClick = () => {
    commitAcceptInvitationToJoinOrganization({
      variables: {
        input: {
          clientMutationId: nanoid(),
          id: notificationDetails.sourceId,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          enqueueSnackbar(`Failed to accept invitation to join organization '${organization?.name}'. Error: ${joinErrors(errors)}`, {
            variant: 'error',
            anchorOrigin,
          });

          setCardState(CardState.Pending);

          return;
        }

        setCardState(CardState.Accepted);
      },
      onError: (error) => {
        enqueueSnackbar(`Failed to accept invitation to join organization '${organization?.name}'. Error: ${error.message}`, {
          variant: 'error',
          anchorOrigin,
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
              <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
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
              </Stack>
            }
            subheader={
              <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
                <Typography variant="body1">
                  {`${getCustomerFullName(invitedBy)} has invited you to join organization ${organization?.name}`}
                </Typography>
              </Stack>
            }
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
          <Typography variant="body1">{`Rejecting invitation to join ${organization?.name}`}</Typography>
        </CardContent>
      )}

      {cardState === CardState.Rejected && (
        <CardContent>
          <Typography variant="body1">{`Rejected invitation to join ${organization?.name}`}</Typography>
        </CardContent>
      )}

      {cardState === CardState.Accepting && (
        <CardContent>
          <Typography variant="body1">{`Accepting invitation to join ${organization?.name}`}</Typography>
        </CardContent>
      )}

      {cardState === CardState.Accepted && (
        <CardContent>
          <Typography variant="body1">{`Accepted invitation to join ${organization?.name}`}</Typography>
        </CardContent>
      )}
    </Card>
  );
};

export default memo(InvitationToJoinOrganizationNotificationCard);
