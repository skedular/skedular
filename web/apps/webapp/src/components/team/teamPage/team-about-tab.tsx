import { OrganizationMemberSelector } from '@/components/organization';
import type { teamAboutTab_query$key } from '@/queries/__generated__/teamAboutTab_query.graphql';
import type { teamAboutTab_updateTeamMutation } from '@/queries/__generated__/teamAboutTab_updateTeamMutation.graphql';
import Button from '@mui/material/Button';
import Paper from '@mui/material/Paper';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import { SingleChoinceTimezone } from '@repo/shared/components/forms';
import { EditIcon } from '@repo/shared/components/icons';
import { SnackbarAnchorOrigin as anchorOrigin } from '@repo/shared/libs/snackbar';
import { joinErrors } from '@repo/shared/libs/utils';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { nanoid } from 'nanoid';
import { useSnackbar } from 'notistack';
import { memo, useState } from 'react';
import { Form } from 'react-final-form';
import { graphql, useFragment, useMutation } from 'react-relay';
import { array, object, string } from 'yup';

type Props = {
  rootDataRelay: teamAboutTab_query$key;
  organizationId: string | null;
};

type TeamDetails = {
  name: string;
  about: string | null;
  timezone: string;
  organizationMemberIds: string[];
};

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
          clientMutationId: nanoid(),
          id: rootData.team.id,
          name,
          about,
          timezone,
          customerIds: rootData.team.members.filter((member) => member.customer).map((member) => member.customer.uniqueId),
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
      <Stack direction="row" sx={{ justifyContent: 'flex-end' }} spacing={1}>
        {!editing && rootData.team.canModify && (
          <Button size="large" color="primary" onClick={handleEditClick}>
            <EditIcon />
          </Button>
        )}
      </Stack>
      {!editing && (
        <Stack direction="column" spacing={1}>
          <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
            <Typography variant="h6">About</Typography>
            <Typography variant="body1">{team.about}</Typography>
          </Stack>

          <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
            <Typography variant="h6">Timezone</Typography>
            <Typography variant="body1">{team.timezone}</Typography>
          </Stack>

          {team.organization && (
            <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
              <Typography variant="h6">Organization</Typography>

              <Typography variant="body1">{team.organization.name}</Typography>
            </Stack>
          )}
        </Stack>
      )}
      {editing && (
        <Paper elevation={24} sx={{ padding: 2 }}>
          <Form
            onSubmit={handleTeamUpdateClick}
            initialValues={{
              name: team.name,
              about: team.about,
              timezone: team.timezone,
              organizationMemberIds: rootData.team.members
                .filter((member) => member.organizationMember)
                .map(({ organizationMember }) => organizationMember!.uniqueId),
            }}
            validate={validate}
            render={({ handleSubmit }) => (
              <Stack direction="column" spacing={1} sx={{ paddingTop: 1 }} component="form" noValidate onSubmit={handleSubmit}>
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

                <Stack sx={{ justifyContent: 'flex-end' }} direction="row" spacing={1}>
                  <Button color="secondary" variant="contained" onClick={handleCancelClick}>
                    Cancel
                  </Button>
                  <Button color="primary" variant="contained" type="submit">
                    Update
                  </Button>
                </Stack>
              </Stack>
            )}
          />
        </Paper>
      )}
    </>
  );
};

export default memo(TeamAboutTab);
