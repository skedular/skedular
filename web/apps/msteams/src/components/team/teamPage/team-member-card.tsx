import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardActions from '@mui/material/CardActions';
import CardContent from '@mui/material/CardContent';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import DialogTitle from '@mui/material/DialogTitle';
import Paper from '@mui/material/Paper';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import { useTheme } from '@mui/material/styles';
import { DangerIcon, DeleteIcon } from '@repo/shared/components/icons';
import { SnackbarAnchorOrigin as anchorOrigin } from '@repo/shared/libs/snackbar';
import { getCustomerFullName, joinErrors } from '@repo/shared/libs/utils';
import { CustomerAvatar } from 'components/customer';
import { useSnackbar } from 'notistack';
import { memo, useMemo, useState } from 'react';
import { graphql, useFragment, useMutation } from 'react-relay';
import { v4 as uuidv4 } from 'uuid';
import type { teamMemberCard_TeamMemberDetails$key } from './__generated__/teamMemberCard_TeamMemberDetails.graphql';
import type { teamMemberCard_query$key } from './__generated__/teamMemberCard_query.graphql';
import type { teamMemberCard_updateTeamMutation } from './__generated__/teamMemberCard_updateTeamMutation.graphql';

type Props = {
  rootDataRelay: teamMemberCard_query$key;
  teamMemberDetailsRelay: teamMemberCard_TeamMemberDetails$key;
  organizationId: string | null;
  onRefetchNeeded: () => void;
};

interface CustomerDetails {
  uniqueId: string;
  name: string | null | undefined;
  givenName: string | null | undefined;
  middleName: string | null | undefined;
  familyName: string | null | undefined;
  photoUrl: string | null | undefined;
}

interface OrganizationMemberDetails {
  customer: CustomerDetails;
}

interface TeamMemberDetails {
  id: string;
  member: CustomerDetails | null | undefined;
  organizationMember: OrganizationMemberDetails | null | undefined;
}

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

  const theme = useTheme();
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
          clientMutationId: uuidv4(),
          id: rootData.team.id,
          name: rootData.team.name,
          about: rootData.team.about,
          customerIds: rootData.team.members
            .filter((member) => member.customer && member.id !== teamMemberDetails.id)
            // @ts-expect-error
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
      <Paper
        elevation={24}
        sx={{
          minWidth: 300,
          maxWidth: 300,
        }}
      >
        <Card
          sx={{
            minWidth: 300,
            maxWidth: 300,
          }}
        >
          <CardContent>
            <Stack direction="row" spacing={2} sx={{ marginBottom: 1 }}>
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
            </Stack>

            <Stack direction="row" spacing={2} sx={{ marginBottom: 1 }}>
              <Typography gutterBottom variant="body1">
                {getCustomerFullName(customer)}
              </Typography>
            </Stack>

            <CardActions>
              {rootData.team.canModify && (
                <Button size="small" color="warning" onClick={handleDeleteClick}>
                  <DeleteIcon />
                </Button>
              )}
            </CardActions>
          </CardContent>
        </Card>
      </Paper>

      <Dialog fullWidth={true} open={teamMemberRemoveConfirmationDialogOpen} onClose={handleCancelRemovingTeamMemberClick}>
        <DialogTitle color={theme.palette.warning.main}>Remove desk</DialogTitle>
        <DialogContent>
          <DialogContentText
            color={theme.palette.warning.main}
          >{`Are you sure you want to remove "${getCustomerFullName(customer)}"?`}</DialogContentText>

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
