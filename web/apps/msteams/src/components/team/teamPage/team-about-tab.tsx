import Button from '@mui/material/Button';
import Paper from '@mui/material/Paper';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import { SingleChoinceTimezone } from '@repo/shared/components/forms';
import { EditIcon } from '@repo/shared/components/icons';
import { Loading } from '@repo/shared/components/loading';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { SnackbarAnchorOrigin as anchorOrigin } from '@repo/shared/libs/snackbar';
import { joinErrors } from '@repo/shared/libs/utils';
import graphql from 'babel-plugin-relay/macro';
import { OrganizationMemberSelector } from 'components/organization';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { nanoid } from 'nanoid';
import { useSnackbar } from 'notistack';
import { memo, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { Form } from 'react-final-form';
import { PreloadedQuery, useMutation, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { array, object, string } from 'yup';
import type { teamAboutTab_rootQuery } from './__generated__/teamAboutTab_rootQuery.graphql';
import type { teamAboutTab_updateTeamMutation } from './__generated__/teamAboutTab_updateTeamMutation.graphql';

type Props = {
  queryReference: PreloadedQuery<teamAboutTab_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationId: string;
};

const RootQuery = graphql`
  query teamAboutTab_rootQuery(
    $organizationId: String!
    $organizationExists: Boolean!
    $teamId: String!
    $bookingPeopleNameSearchText: String
    $organizationMemberSelectorOrganizationMembersSortingValues: [OrganizationMemberOrderInput!]
  ) {
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
`;

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

const TeamAboutTab = ({ queryReference, organizationId }: Props) => {
  const rootData = usePreloadedQuery<teamAboutTab_rootQuery>(RootQuery, queryReference);
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

          return;
        }

        setEditing(false);
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
      {!editing && (
        <>
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
          {rootData.team.canModify && (
            <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
              <Button variant="contained" size="small" color="primary" startIcon={<EditIcon />} onClick={handleEditClick}>
                Edit
              </Button>
            </Stack>
          )}
        </>
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
                    organizationId={organizationId}
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

const MemoTeamAboutTab = memo(TeamAboutTab);

type RelayProps = {
  onReloadRequired: () => void;
  organizationId: string;
  teamId: string;
};

const TeamAboutTabWithRelay = ({ onReloadRequired, organizationId, teamId }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<teamAboutTab_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(nanoid());
  const [, startTransition] = useTransition();

  useEffect(() => {
    loadQuery(
      {
        organizationId: organizationId ?? '',
        organizationExists: !!organizationId,
        teamId,
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId, organizationId, teamId]);

  const handleReloadRequired = () => {
    startTransition(() => {
      setTriggerReloadId(nanoid());

      onReloadRequired();
    });
  };

  if (!queryReference) {
    return <Loading />;
  }

  return (
    <ErrorBoundary fallbackRender={({ error }: { error: RootError }) => <RelayError error={error} />}>
      <MemoTeamAboutTab queryReference={queryReference} onReloadRequired={handleReloadRequired} organizationId={organizationId} />
    </ErrorBoundary>
  );
};

export default memo(TeamAboutTabWithRelay);
