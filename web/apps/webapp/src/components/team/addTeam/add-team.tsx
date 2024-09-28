import { OrganizationMemberSelector } from '@/components/organization';
import type { addTeam_addTeamMutation } from '@/queries/__generated__/addTeam_addTeamMutation.graphql';
import type { addTeam_query$key } from '@/queries/__generated__/addTeam_query.graphql';
import Button from '@mui/material/Button';
import Paper from '@mui/material/Paper';
import Stack from '@mui/material/Stack';
import { SingleChoinceTimezone } from '@repo/shared/components/forms';
import { SnackbarAnchorOrigin as anchorOrigin } from '@repo/shared/libs/snackbar';
import { joinErrors } from '@repo/shared/libs/utils';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { nanoid } from 'nanoid';
import { useRouter } from 'next/navigation';
import { useSnackbar } from 'notistack';
import { memo } from 'react';
import { Form } from 'react-final-form';
import { graphql, useFragment, useMutation } from 'react-relay';
import { array, object, string } from 'yup';

type Props = {
  rootDataRelay: addTeam_query$key;
  organizationId: string | null;
};

interface TeamDetails {
  name: string;
  about: string | null;
  timezone: string | null;
  organizationMemberIds: string[];
}

const teamSchema = object({
  name: string().min(3, 'Team name must be at least three charcters long.').required('Team name is required'),
  about: string().nullable(),
  timezone: string().nullable(),
  organizationMemberIds: array().nullable(),
});

const AddTeam = ({ rootDataRelay, organizationId }: Props) => {
  const rootData = useFragment(
    graphql`
      fragment addTeam_query on Query {
        me {
          id
        }
        ...organizationMemberSelector_query
      }
    `,
    rootDataRelay,
  );

  const [commitAddTeam] = useMutation<addTeam_addTeamMutation>(graphql`
    mutation addTeam_addTeamMutation($input: AddTeamInput!) @raw_response_type {
      addTeam(input: $input) {
        team {
          id
          name
          about
          timezone
        }
      }
    }
  `);

  const { enqueueSnackbar } = useSnackbar();
  const router = useRouter();
  const validate = makeValidate(teamSchema);
  const requiredFields = makeRequired(teamSchema);

  const handleCancelClick = () => {
    router.back();
  };

  const handleTeamCreateClick = ({ name, about, timezone, organizationMemberIds }: TeamDetails) => {
    if (!rootData.me) {
      return;
    }

    const id = nanoid();
    const customerIds = !organizationId ? [rootData.me.id] : [];

    commitAddTeam({
      variables: {
        input: {
          clientMutationId: nanoid(),
          id,
          name,
          about,
          timezone,
          customerIds,
          organizationId,
          organizationMemberIds: [...new Set(organizationMemberIds)],
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          enqueueSnackbar(`Failed to add new team '${name}'. Error: ${joinErrors(errors)}`, {
            variant: 'error',
            anchorOrigin,
          });
        } else {
          router.back();
        }
      },
      onError: (error) => {
        enqueueSnackbar(`Failed to add new team '${name}'. Error: ${error.message}`, {
          variant: 'error',
          anchorOrigin,
        });
      },
      optimisticResponse: {
        addTeam: {
          team: {
            id,
            name,
            about,
            timezone,
          },
        },
      },
    });
  };

  if (!rootData.me) {
    return <></>;
  }

  return (
    <Paper elevation={24} sx={{ padding: 2 }}>
      <Form
        onSubmit={handleTeamCreateClick}
        initialValues={{
          name: '',
          about: null,
          organizationMemberIds: [],
        }}
        validate={validate}
        render={({ handleSubmit }) => (
          <Stack direction="column" spacing={1} component="form" noValidate onSubmit={handleSubmit}>
            <TextField label="Name" name="name" required={requiredFields.name} />
            <TextField label="About" name="about" required={requiredFields.about} multiline={true} />
            <SingleChoinceTimezone name="timezone" required={requiredFields.timezone} />

            {organizationId && (
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
                Create
              </Button>
            </Stack>
          </Stack>
        )}
      />
    </Paper>
  );
};

export default memo(AddTeam);
