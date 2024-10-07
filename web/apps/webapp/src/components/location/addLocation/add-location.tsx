import type { addLocation_addLocationMutation } from '@/queries/__generated__/addLocation_addLocationMutation.graphql';
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
import { graphql, useMutation } from 'react-relay';
import { object, string } from 'yup';

type Props = {
  organizationId: string | null;
};

interface LocationDetails {
  name: string;
  about: string | null;
  timezone: string | null;
}

const locationSchema = object({
  name: string().min(3, 'Location name must be at least three charcters long.').required('Location name is required'),
  about: string().nullable(),
  timezone: string().nullable(),
});

const AddLocation = ({ organizationId }: Props) => {
  const [commitAddLocation] = useMutation<addLocation_addLocationMutation>(graphql`
    mutation addLocation_addLocationMutation($input: AddLocationInput!) @raw_response_type {
      addLocation(input: $input) {
        location {
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
  const validate = makeValidate(locationSchema);
  const requiredFields = makeRequired(locationSchema);

  const handleCancelClick = () => {
    router.back();
  };

  const handleLocationCreateClick = ({ name, about, timezone }: LocationDetails) => {
    const id = nanoid();

    commitAddLocation({
      variables: {
        input: {
          clientMutationId: nanoid(),
          id,
          name,
          about,
          organizationId,
          timezone,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          enqueueSnackbar(`Failed to add new location '${name}'. Error: ${joinErrors(errors)}`, {
            variant: 'error',
            anchorOrigin,
          });
        } else {
          router.back();
        }
      },
      onError: (error) => {
        enqueueSnackbar(`Failed to add new location '${name}'. Error: ${error.message}`, {
          variant: 'error',
          anchorOrigin,
        });
      },
      optimisticResponse: {
        addLocation: {
          location: {
            id,
            name,
            about,
            timezone,
          },
        },
      },
    });
  };

  return (
    <Paper elevation={24} sx={{ padding: 2 }}>
      <Form
        onSubmit={handleLocationCreateClick}
        initialValues={{
          name: '',
          about: null,
          organizationId,
        }}
        validate={validate}
        render={({ handleSubmit }) => (
          <Stack direction="column" spacing={1} sx={{ paddingTop: 1 }} component="form" noValidate onSubmit={handleSubmit}>
            <TextField label="Name" name="name" required={requiredFields.name} />
            <TextField label="About" name="about" required={requiredFields.about} multiline={true} />
            <SingleChoinceTimezone name="timezone" required={requiredFields.timezone} />

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

export default memo(AddLocation);
