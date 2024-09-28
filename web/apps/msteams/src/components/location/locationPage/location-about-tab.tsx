import Button from '@mui/material/Button';
import Paper from '@mui/material/Paper';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import { SingleChoinceTimezone } from '@repo/shared/components/forms';
import { EditIcon } from '@repo/shared/components/icons';
import { SnackbarAnchorOrigin as anchorOrigin } from '@repo/shared/libs/snackbar';
import { joinErrors } from '@repo/shared/libs/utils';
import graphql from 'babel-plugin-relay/macro';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { nanoid } from 'nanoid';
import { useSnackbar } from 'notistack';
import { memo, useState } from 'react';
import { Form } from 'react-final-form';
import { useFragment, useMutation } from 'react-relay';
import { object, string } from 'yup';
import type { locationAboutTab_query$key } from './__generated__/locationAboutTab_query.graphql';
import type { locationAboutTab_updateLocationMutation } from './__generated__/locationAboutTab_updateLocationMutation.graphql';

type Props = {
  rootDataRelay: locationAboutTab_query$key;
  organizationId: string | null;
};

interface LocationDetails {
  name: string;
  about: string | null;
  timezone: string;
}

const locationSchema = object({
  name: string().min(3, 'Location name must be at least three charcters long.').required('Location name is required'),
  about: string().nullable(),
  timezone: string().required('Timezone is required'),
});

const LocationAboutTab = ({ rootDataRelay, organizationId }: Props) => {
  const rootData = useFragment<locationAboutTab_query$key>(
    graphql`
      fragment locationAboutTab_query on Query {
        location(id: $locationId) {
          id
          name
          about
          timezone
          organization {
            name
          }
          canModify
        }
      }
    `,
    rootDataRelay,
  );

  const [commitUpdateLocation] = useMutation<locationAboutTab_updateLocationMutation>(graphql`
    mutation locationAboutTab_updateLocationMutation($input: UpdateLocationInput!) @raw_response_type {
      updateLocation(input: $input) {
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
  const [editing, setEditing] = useState(false);
  const validate = makeValidate(locationSchema);
  const requiredFields = makeRequired(locationSchema);

  const handleEditClick = () => {
    setEditing(true);
  };

  const handleLocationUpdateClick = ({ name, about, timezone }: LocationDetails) => {
    if (!rootData.location) {
      return;
    }

    commitUpdateLocation({
      variables: {
        input: {
          clientMutationId: nanoid(),
          id: rootData.location.id,
          name,
          about,
          timezone,
          organizationId,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          enqueueSnackbar(`Failed to update location '${name}'. Error: ${joinErrors(errors)}`, {
            variant: 'error',
            anchorOrigin,
          });
        } else {
          setEditing(false);
        }
      },
      onError: (error) => {
        enqueueSnackbar(`Failed to update location '${name}'. Error: ${error.message}`, {
          variant: 'error',
          anchorOrigin,
        });
      },
      optimisticResponse: {
        updateLocation: {
          location: {
            id: rootData.location.id,
            name,
            about,
            timezone,
          },
        },
      },
    });
  };

  const handleCancelClick = () => {
    setEditing(false);
  };

  if (!rootData.location) {
    return null;
  }

  const location = rootData.location;

  return (
    <>
      <Stack direction="row" sx={{ justifyContent: 'flex-end' }} spacing={1}>
        {!editing && rootData.location.canModify && (
          <Button size="large" color="primary" onClick={handleEditClick}>
            <EditIcon />
          </Button>
        )}
      </Stack>
      {!editing && (
        <Stack direction="column" spacing={1}>
          <Stack direction="row" spacing={1} sx={{ alignItems: 'center' }}>
            <Typography variant="h6">About</Typography>
            <Typography variant="body1">{location.about}</Typography>
          </Stack>

          <Stack direction="row" spacing={1} sx={{ alignItems: 'center' }}>
            <Typography variant="h6">Timezone</Typography>
            <Typography variant="body1">{location.timezone}</Typography>
          </Stack>

          {location.organization && (
            <Stack direction="row" spacing={1} sx={{ alignItems: 'center' }}>
              <Typography variant="h6">Organization</Typography>

              <Typography variant="body1">{location.organization.name}</Typography>
            </Stack>
          )}
        </Stack>
      )}
      {editing && (
        <Paper elevation={24} sx={{ padding: 2 }}>
          <Form
            onSubmit={handleLocationUpdateClick}
            initialValues={{
              name: location.name,
              about: location.about,
              timezone: location.timezone,
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

export default memo(LocationAboutTab);
