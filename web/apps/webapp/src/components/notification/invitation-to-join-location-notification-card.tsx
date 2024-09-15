import { CustomerAvatar } from '@/components/customer';
import type { invitationToJoinLocationNotificationCard_NotificationDetails$key } from '@/queries/__generated__/invitationToJoinLocationNotificationCard_NotificationDetails.graphql';
import type { invitationToJoinLocationNotificationCard_acceptInvitationToJoinLocationMutation } from '@/queries/__generated__/invitationToJoinLocationNotificationCard_acceptInvitationToJoinLocationMutation.graphql';
import type { invitationToJoinLocationNotificationCard_rejectInvitationToJoinLocationMutation } from '@/queries/__generated__/invitationToJoinLocationNotificationCard_rejectInvitationToJoinLocationMutation.graphql';
import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardActions from '@mui/material/CardActions';
import CardContent from '@mui/material/CardContent';
import Paper from '@mui/material/Paper';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import { CancelIcon, CheckIcon } from '@repo/shared/components/icons';
import { SnackbarAnchorOrigin as anchorOrigin } from '@repo/shared/libs/snackbar';
import { getCustomerFullName, joinErrors } from '@repo/shared/libs/utils';
import { nanoid } from 'nanoid';
import { useSnackbar } from 'notistack';
import { memo, useMemo, useState } from 'react';
import { graphql, useFragment, useMutation } from 'react-relay';

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

  const invitedBy = useMemo(() => notificationDetails.invitedBy, [notificationDetails]);
  const location = useMemo(() => notificationDetails.location, [notificationDetails]);
  const { enqueueSnackbar } = useSnackbar();
  const [cardState, setCardState] = useState<CardState>(CardState.Pending);

  const handleRejectClick = () => {
    commitRejectInvitationToJoinLocation({
      variables: {
        input: {
          clientMutationId: nanoid(),
          id: notificationDetails.sourceId,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          enqueueSnackbar(`Failed to reject invitation to join location '${location?.name}'. Error: ${joinErrors(errors)}`, {
            variant: 'error',
            anchorOrigin,
          });

          setCardState(CardState.Pending);
        } else {
          setCardState(CardState.Rejected);
        }
      },
      onError: (error) => {
        enqueueSnackbar(`Failed to reject invitation to join location '${location?.name}'. Error: ${error.message}`, {
          variant: 'error',
          anchorOrigin,
        });

        setCardState(CardState.Pending);
      },
    });

    setCardState(CardState.Rejecting);
  };

  const handleAcceptClick = () => {
    commitAcceptInvitationToJoinLocation({
      variables: {
        input: {
          clientMutationId: nanoid(),
          id: notificationDetails.sourceId,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          enqueueSnackbar(`Failed to accept invitation to join location '${location?.name}'. Error: ${joinErrors(errors)}`, {
            variant: 'error',
            anchorOrigin,
          });

          setCardState(CardState.Pending);
        } else {
          setCardState(CardState.Accepted);
        }
      },
      onError: (error) => {
        enqueueSnackbar(`Failed to accept invitation to join location '${location?.name}'. Error: ${error.message}`, {
          variant: 'error',
          anchorOrigin,
        });

        setCardState(CardState.Pending);
      },
    });

    setCardState(CardState.Accepting);
  };

  return (
    <Paper
      elevation={24}
      sx={{
        minWidth: 350,
        maxWidth: 350,
      }}
    >
      <Card
        sx={{
          minWidth: 350,
          maxWidth: 350,
        }}
      >
        {cardState === CardState.Pending && (
          <CardContent>
            <Stack direction="row" spacing={2} sx={{ marginBottom: 1 }}>
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

            <Stack direction="row" spacing={2} sx={{ marginBottom: 1 }}>
              <Typography gutterBottom variant="body1">
                {`${getCustomerFullName(invitedBy)} has invited you to join location ${location?.name}`}
              </Typography>
            </Stack>

            <CardActions>
              <Stack sx={{ flex: 1, justifyContent: 'flex-end' }} direction="row" spacing={2}>
                <Button color="secondary" variant="contained" startIcon={<CancelIcon />} onClick={handleRejectClick}>
                  Reject
                </Button>
                <Button color="primary" variant="contained" type="submit" startIcon={<CheckIcon />} onClick={handleAcceptClick}>
                  Accept
                </Button>
              </Stack>
            </CardActions>
          </CardContent>
        )}

        {cardState === CardState.Rejecting && (
          <CardContent>
            <Typography gutterBottom variant="body1">
              {`Rejecting invitation to join ${location?.name}`}
            </Typography>
          </CardContent>
        )}

        {cardState === CardState.Rejected && (
          <CardContent>
            <Typography gutterBottom variant="body1">
              {`Rejected invitation to join ${location?.name}`}
            </Typography>
          </CardContent>
        )}

        {cardState === CardState.Accepting && (
          <CardContent>
            <Typography gutterBottom variant="body1">
              {`Accepting invitation to join ${location?.name}`}
            </Typography>
          </CardContent>
        )}

        {cardState === CardState.Accepted && (
          <CardContent>
            <Typography gutterBottom variant="body1">
              {`Accepted invitation to join ${location?.name}`}
            </Typography>
          </CardContent>
        )}
      </Card>
    </Paper>
  );
};

export default memo(InvitationToJoinLocationNotificationCard);
