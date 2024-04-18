import { OrganizationMemberSelector } from '@/components/organization';
import { SingleChoinceTimezone } from '@repo/shared/components/forms';
import type { addTeam_addTeamMutation } from '@/queries/__generated__/addTeam_addTeamMutation.graphql';
import type { addTeam_query$key } from '@/queries/__generated__/addTeam_query.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Paper from '@mui/material/Paper';
import Stack from '@mui/material/Stack';
import { joinErrors } from '@repo/shared/libs/utils';
import { SnackbarAnchorOrigin as anchorOrigin } from '@repo/shared/libs/snackbar';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { v4 as uuidv4 } from 'uuid';
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

    const id = uuidv4();
    const customerIds = !organizationId ? [rootData.me.id] : [];

    commitAddTeam({
      variables: {
        input: {
          clientMutationId: uuidv4(),
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
    <Box
      sx={{
        display: 'flex',
        flexWrap: 'wrap',
        '& > :not(style)': {
          m: 1,
        },
        maxWidth: 600,
      }}
    >
      <Paper elevation={24} sx={{ padding: 3 }}>
        <Form
          onSubmit={handleTeamCreateClick}
          initialValues={{
            name: '',
            about: null,
            organizationMemberIds: [],
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

              {organizationId && (
                <OrganizationMemberSelector
                  rootDataRelay={rootData}
                  name="organizationMemberIds"
                  required={requiredFields.organizationMemberIds}
                  multiple={true}
                  useMemberId={true}
                />
              )}

              <Stack sx={{ flex: 1 }} direction="row" spacing={2}>
                <Button color="secondary" variant="contained" onClick={handleCancelClick}>
                  Cancel
                </Button>
                <Button color="primary" variant="contained" type="submit">
                  Create
                </Button>
              </Stack>
            </Box>
          )}
        />
      </Paper>
    </Box>
  );
};

export default memo(AddTeam);
