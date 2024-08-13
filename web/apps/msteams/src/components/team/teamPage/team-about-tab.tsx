import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Grid from '@mui/material/Grid';
import Paper from '@mui/material/Paper';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import { SingleChoinceTimezone } from '@repo/shared/components/forms';
import { EditIcon } from '@repo/shared/components/icons';
import { SnackbarAnchorOrigin as anchorOrigin } from '@repo/shared/libs/snackbar';
import { joinErrors } from '@repo/shared/libs/utils';
import { OrganizationMemberSelector } from 'components/organization';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { useSnackbar } from 'notistack';
import { memo, useState } from 'react';
import { Form } from 'react-final-form';
import { graphql, useFragment, useMutation } from 'react-relay';
import { v4 as uuidv4 } from 'uuid';
import { array, object, string } from 'yup';
import type { teamAboutTab_query$key } from './__generated__/teamAboutTab_query.graphql';
import type { teamAboutTab_updateTeamMutation } from './__generated__/teamAboutTab_updateTeamMutation.graphql';

type Props = {
  rootDataRelay: teamAboutTab_query$key;
  organizationId: string | null;
};

interface TeamDetails {
  name: string;
  about: string | null;
  timezone: string;
  organizationMemberIds: string[];
}

const teamSchema = object({
  name: string().min(3, 'Team name must be at least three charcters long.').required('Team name is required'),
  about: string().nullable(),
  timezone: string().required('Timezone is required'),
  organizationMemberIds: array().nullable(),
});

const TeamAboutTab = ({ rootDataRelay, organizationId }: Props) => {
  const rootData = useFragment<teamAboutTab_query$key>(
    graphql`
      fragment teamAboutTab_query on Query {
        team(id: $teamId) {
          id
          name
          about
          timezone
          organization {
            name
          }
          canModify
          members {
            customer {
              uniqueId
            }
            organizationMember {
              uniqueId
            }
          }
        }
        ...organizationMemberSelector_query
      }
    `,
    rootDataRelay,
  );

  const [commitUpdateTeam] = useMutation<teamAboutTab_updateTeamMutation>(graphql`
    mutation teamAboutTab_updateTeamMutation($input: UpdateTeamInput!) @raw_response_type {
      updateTeam(input: $input) {
        team {
          id
          name
          about
          timezone
          organization {
            name
          }
          members {
            customer {
              uniqueId
            }
            organizationMember {
              uniqueId
            }
          }
        }
      }
    }
  `);

  const { enqueueSnackbar } = useSnackbar();
  const [editing, setEditing] = useState(false);
  const validate = makeValidate(teamSchema);
  const requiredFields = makeRequired(teamSchema);

  const handleEditClick = () => {
    setEditing(true);
  };

  const handleTeamUpdateClick = ({ name, about, timezone, organizationMemberIds }: TeamDetails) => {
    if (!rootData.team) {
      return;
    }

    commitUpdateTeam({
      variables: {
        input: {
          clientMutationId: uuidv4(),
          id: rootData.team.id,
          name,
          about,
          timezone,
          // @ts-expect-error
          customerIds: rootData.team.members.filter((member) => member.customer).map((member) => member.customer?.uniqueId),
          organizationId,
          organizationMemberIds: [...new Set(organizationMemberIds)],
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          enqueueSnackbar(`Failed to update team '${name}'. Error: ${joinErrors(errors)}`, {
            variant: 'error',
            anchorOrigin,
          });
        } else {
          setEditing(false);
        }
      },
      onError: (error) => {
        enqueueSnackbar(`Failed to update team '${name}'. Error: ${error.message}`, {
          variant: 'error',
          anchorOrigin,
        });
      },
      optimisticResponse: {
        updateTeam: {
          team: {
            id: rootData.team.id,
            name,
            about,
            timezone,
            organization: null,
            members: [],
          },
        },
      },
    });
  };

  const handleCancelClick = () => {
    setEditing(false);
  };

  if (!rootData.team) {
    return null;
  }

  const team = rootData.team;

  return (
    <>
      <Box sx={{ display: 'flex', justifyContent: 'flex-end' }}>
        {!editing && rootData.team.canModify && (
          <Button size="large" color="primary" onClick={handleEditClick}>
            <EditIcon />
          </Button>
        )}
      </Box>
      {!editing && (
        <Grid
          container
          sx={{
            display: 'flex',
            justifyContent: 'flex-start',
            alignItems: 'left',
            marginBottom: 1,
          }}
        >
          <Grid item>
            <Stack direction={'row'}>
              <Typography gutterBottom variant="h6">
                About
              </Typography>
              <Typography gutterBottom variant="body1" sx={{ whiteSpace: 'pre-line', marginLeft: 1 }}>
                {team.about}
              </Typography>
            </Stack>

            <Stack direction={'row'}>
              <Typography gutterBottom variant="h6">
                Timezone
              </Typography>
              <Typography gutterBottom variant="body1" sx={{ whiteSpace: 'pre-line', marginLeft: 1 }}>
                {team.timezone}
              </Typography>
            </Stack>

            {team.organization && (
              <Stack direction={'row'}>
                <Typography gutterBottom variant="h6">
                  Organization
                </Typography>

                <Typography gutterBottom variant="body1" sx={{ whiteSpace: 'pre-line', marginLeft: 1 }}>
                  {team.organization.name}
                </Typography>
              </Stack>
            )}
          </Grid>
        </Grid>
      )}
      {editing && (
        <Grid
          container
          sx={{
            display: 'flex',
            justifyContent: 'center',
            alignItems: 'center',
            marginBottom: 1,
          }}
        >
          <Paper elevation={24} sx={{ padding: 3 }}>
            <Form
              onSubmit={handleTeamUpdateClick}
              initialValues={{
                name: team.name,
                about: team.about,
                timezone: team.timezone,
                organizationMemberIds: rootData.team.members
                  .filter((member) => member.organizationMember)
                  // @ts-expect-error
                  .map(({ organizationMember }) => organizationMember.uniqueId),
              }}
              validate={validate}
              render={({ handleSubmit }) => (
                <Box
                  component="form"
                  sx={{
                    '& > :not(style)': { m: 1 },
                  }}
                  autoComplete="off"
                  noValidate
                  onSubmit={handleSubmit}
                >
                  <TextField label="Name" name="name" required={requiredFields.name} />
                  <TextField label="About" name="about" required={requiredFields.about} multiline={true} />
                  <SingleChoinceTimezone name="timezone" required={requiredFields.timezone} />

                  {rootData.team?.organization && (
                    <OrganizationMemberSelector
                      rootDataRelay={rootData}
                      name="organizationMemberIds"
                      required={requiredFields.organizationMemberIds}
                      multiple={true}
                      useMemberId={true}
                    />
                  )}

                  <Stack sx={{ flex: 1, justifyContent: 'flex-end' }} direction="row" spacing={2}>
                    <Button color="secondary" variant="contained" onClick={handleCancelClick}>
                      Cancel
                    </Button>
                    <Button color="primary" variant="contained" type="submit">
                      Update
                    </Button>
                  </Stack>
                </Box>
              )}
            />
          </Paper>
        </Grid>
      )}
    </>
  );
};

export default memo(TeamAboutTab);
