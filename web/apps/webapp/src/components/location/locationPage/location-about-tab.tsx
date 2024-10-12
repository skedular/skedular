import type { locationAboutTab_rootQuery } from '@/queries/__generated__/locationAboutTab_rootQuery.graphql';
import type { locationAboutTab_updateLocationMutation } from '@/queries/__generated__/locationAboutTab_updateLocationMutation.graphql';
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
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { nanoid } from 'nanoid';
import { useSnackbar } from 'notistack';
import { memo, useEffect, useState, useTransition } from 'react';
import { ErrorBoundary } from 'react-error-boundary';
import { Form } from 'react-final-form';
import { graphql, PreloadedQuery, useMutation, usePreloadedQuery, useQueryLoader } from 'react-relay';
import { object, string } from 'yup';

type Props = {
  queryReference: PreloadedQuery<locationAboutTab_rootQuery, Record<string, unknown>>;
  onReloadRequired: () => void;
  organizationId?: string;
};

const RootQuery = graphql`
  query locationAboutTab_rootQuery($locationId: String!) {
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
`;

type LocationDetails = {
  name: string;
  about: string | null;
  timezone: string;
};

const locationSchema = object({
  name: string().min(3, 'Location name must be at least three charcters long.').required('Location name is required'),
  about: string().nullable(),
  timezone: string().required('Timezone is required'),
});

const LocationAboutTab = ({ queryReference, organizationId }: Props) => {
  const rootData = usePreloadedQuery<locationAboutTab_rootQuery>(RootQuery, queryReference);
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
          <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
            <Typography variant="h6">About</Typography>
            <Typography variant="body1">{location.about}</Typography>
          </Stack>

          <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
            <Typography variant="h6">Timezone</Typography>
            <Typography variant="body1">{location.timezone}</Typography>
          </Stack>

          {location.organization && (
            <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
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
              <Stack direction="column" spacing={1} sx={{ paddingTop: 1 }} component="form" noValidate onSubmit={handleSubmit}>
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

const MemoLocationAboutTab = memo(LocationAboutTab);

type RelayProps = {
  onReloadRequired: () => void;
  organizationId?: string;
  locationId: string;
};

const LocationAboutTabWithRelay = ({ onReloadRequired, organizationId, locationId }: RelayProps) => {
  const [queryReference, loadQuery] = useQueryLoader<locationAboutTab_rootQuery>(RootQuery);
  const [triggerReloadId, setTriggerReloadId] = useState(nanoid());
  const [, startTransition] = useTransition();

  useEffect(() => {
    loadQuery(
      {
        locationId,
      },
      {
        fetchPolicy: 'store-and-network',
      },
    );
  }, [loadQuery, triggerReloadId, locationId]);

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
      <MemoLocationAboutTab queryReference={queryReference} onReloadRequired={handleReloadRequired} organizationId={organizationId} />
    </ErrorBoundary>
  );
};

export default memo(LocationAboutTabWithRelay);
