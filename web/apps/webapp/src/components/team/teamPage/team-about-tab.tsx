import type { teamAboutTab_rootQuery } from '@/queries/__generated__/teamAboutTab_rootQuery.graphql';
import type { teamAboutTab_updateTeamMutation } from '@/queries/__generated__/teamAboutTab_updateTeamMutation.graphql';
import Button from '@mui/material/Button';
import Stack from '@mui/material/Stack';
import { SingleChoinceTimezone } from '@repo/shared/components/forms';
import { Loading } from '@repo/shared/components/loading';
import type { RootError } from '@repo/shared/components/relayError';
import { RelayError } from '@repo/shared/components/relayError';
import { SnackbarAnchorOrigin as anchorOrigin } from '@repo/shared/libs/snackbar';
import { joinErrors } from '@repo/shared/libs/utils';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { nanoid } from 'nanoid';
import { useSnackbar } from 'notistack';
import { memo, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { Form } from 'react-final-form';
import { graphql, PreloadedQuery, useMutation, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { object, string } from 'yup';

type Props = {
  queryReference: PreloadedQuery<teamAboutTab_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationId?: string;
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
};

const teamSchema = object({
  name: string().min(3, 'Team name must be at least three charcters long.').required('Team name is required'),
  about: string().nullable(),
  timezone: string().required('Timezone is required'),
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
  const validate = makeValidate(teamSchema);
  const requiredFields = makeRequired(teamSchema);

  const handleTeamUpdateClick = ({ name, about, timezone }: TeamDetails) => {
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
          organizationMemberIds: rootData.team.members
            .filter((member) => member.organizationMember)
            .map((member) => member.organizationMember!.uniqueId),
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

        enqueueSnackbar(`Team details updated.`, {
          variant: 'success',
          anchorOrigin,
        });
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

  if (!rootData.team) {
    return null;
  }

  const team = rootData.team;

  return (
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
        <Stack direction="column" spacing={2} sx={{ paddingTop: 1 }} component="form" noValidate onSubmit={handleSubmit}>
          <TextField label="Name" name="name" required={requiredFields.name} />
          <TextField label="About" name="about" required={requiredFields.about} multiline={true} />
          <SingleChoinceTimezone name="timezone" required={requiredFields.timezone} />

          <Stack sx={{ justifyContent: 'flex-end' }} direction="row" spacing={1}>
            <Button color="primary" variant="contained" type="submit">
              Update
            </Button>
          </Stack>
        </Stack>
      )}
    />
  );
};

const MemoTeamAboutTab = memo(TeamAboutTab);

type RelayProps = {
  onReloadRequired: () => void;
  organizationId?: string;
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
