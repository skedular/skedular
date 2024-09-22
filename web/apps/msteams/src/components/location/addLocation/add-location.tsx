import Button from '@mui/material/Button';
import Paper from '@mui/material/Paper';
import Stack from '@mui/material/Stack';
import { SingleChoinceTimezone } from '@repo/shared/components/forms';
import { SnackbarAnchorOrigin as anchorOrigin } from '@repo/shared/libs/snackbar';
import { joinErrors } from '@repo/shared/libs/utils';
import graphql from 'babel-plugin-relay/macro';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { useSnackbar } from 'notistack';
import { memo } from 'react';
import { Form } from 'react-final-form';
import { useMutation } from 'react-relay';
import { useNavigate } from 'react-router-dom';
import { v4 as uuidv4 } from 'uuid';
import { object, string } from 'yup';
import type { addLocation_addLocationMutation } from './__generated__/addLocation_addLocationMutation.graphql';

type Props = {
  organizationId: string;
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
  const navigate = useNavigate();
  const validate = makeValidate(locationSchema);
  const requiredFields = makeRequired(locationSchema);

  const handleCancelClick = () => {
    navigate(-1);
  };

  const handleLocationCreateClick = ({ name, about, timezone }: LocationDetails) => {
    const id = uuidv4();

    commitAddLocation({
      variables: {
        input: {
          clientMutationId: uuidv4(),
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
          navigate(-1);
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
          <Stack direction="column" component="form" noValidate onSubmit={handleSubmit} spacing={2}>
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
