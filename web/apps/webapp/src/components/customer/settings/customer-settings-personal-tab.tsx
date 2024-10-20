import type { customerSettingsPersonalTab_query$key } from '@/queries/__generated__/customerSettingsPersonalTab_query.graphql';
import type { customerSettingsPersonalTab_updateMyCustomerDetailsMutation } from '@/queries/__generated__/customerSettingsPersonalTab_updateMyCustomerDetailsMutation.graphql';
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
import { object, string } from 'yup';

type Props = {
  rootDataRelay: customerSettingsPersonalTab_query$key;
};

type SettingsDetails = {
  designation: string | null;
  title: string | null;
  name: string | null;
  givenName: string | null;
  middleName: string | null;
  familyName: string | null;
  timezone: string;
};

const settingsSchema = object({
  designation: string().nullable(),
  title: string().nullable(),
  name: string().nullable(),
  givenName: string().nullable(),
  middleName: string().nullable(),
  familyName: string().nullable(),
  timezone: string().required('Timezone is required'),
});

const CustomerSettingsPersonalTab = ({ rootDataRelay }: Props) => {
  const rootData = useFragment<customerSettingsPersonalTab_query$key>(
    graphql`
      fragment customerSettingsPersonalTab_query on Query {
        me {
          id
          timezone
          designation
          title
          name
          givenName
          middleName
          familyName
        }
      }
    `,
    rootDataRelay,
  );

  const [commitUpdateMyCustomerDetails] = useMutation<customerSettingsPersonalTab_updateMyCustomerDetailsMutation>(graphql`
    mutation customerSettingsPersonalTab_updateMyCustomerDetailsMutation($input: UpdateMyCustomerDetailsInput!) @raw_response_type {
      updateMyCustomerDetails(input: $input) {
        customer {
          id
          timezone
          designation
          title
          name
          givenName
          middleName
          familyName
        }
      }
    }
  `);

  const { enqueueSnackbar } = useSnackbar();
  const [editing, setEditing] = useState(false);
  const validate = makeValidate(settingsSchema);
  const requiredFields = makeRequired(settingsSchema);

  const handleEditClick = () => {
    setEditing(true);
  };

  const handleSettingsUpdateClick = ({ timezone, designation, title, name, givenName, middleName, familyName }: SettingsDetails) => {
    if (!rootData.me) {
      return;
    }

    commitUpdateMyCustomerDetails({
      variables: {
        input: {
          clientMutationId: nanoid(),
          timezone,
          designation,
          title,
          name,
          givenName,
          middleName,
          familyName,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          enqueueSnackbar(`Failed to update personal details. Error: ${joinErrors(errors)}`, {
            variant: 'error',
            anchorOrigin,
          });

          return;
        }

        setEditing(false);
      },
      onError: (error) => {
        enqueueSnackbar(`Failed to update personal details. Error: ${error.message}`, {
          variant: 'error',
          anchorOrigin,
        });
      },
      optimisticResponse: {
        updateMyCustomerDetails: {
          customer: {
            id: rootData.me.id,
            timezone,
            designation,
            title,
            name,
            givenName,
            middleName,
            familyName,
          },
        },
      },
    });
  };

  const handleCancelClick = () => {
    setEditing(false);
  };

  if (!rootData.me) {
    return null;
  }

  return (
    <>
      {!editing && (
        <>
          <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
            <Typography variant="h6">Designation</Typography>
            <Typography variant="body1">{rootData.me.designation}</Typography>
          </Stack>

          <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
            <Typography variant="h6">Title</Typography>
            <Typography variant="body1">{rootData.me.title}</Typography>
          </Stack>

          <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
            <Typography variant="h6">Name</Typography>
            <Typography variant="body1">{rootData.me.name}</Typography>
          </Stack>

          <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
            <Typography variant="h6">Given Name</Typography>
            <Typography variant="body1">{rootData.me.givenName}</Typography>
          </Stack>

          <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
            <Typography variant="h6">Middle Name</Typography>
            <Typography variant="body1">{rootData.me.middleName}</Typography>
          </Stack>

          <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
            <Typography variant="h6">Family Name</Typography>
            <Typography variant="body1">{rootData.me.familyName}</Typography>
          </Stack>

          <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
            <Typography variant="h6">Timezone</Typography>
            <Typography variant="body1">{rootData.me.timezone}</Typography>
          </Stack>
          <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
            <Button variant="contained" size="small" color="primary" startIcon={<EditIcon />} onClick={handleEditClick}>
              Edit
            </Button>
          </Stack>
        </>
      )}
      {editing && (
        <Paper elevation={24} sx={{ padding: 2 }}>
          <Form
            onSubmit={handleSettingsUpdateClick}
            initialValues={{
              timezone: rootData.me.timezone,
              designation: rootData.me.designation,
              title: rootData.me.title,
              name: rootData.me.name,
              givenName: rootData.me.givenName,
              middleName: rootData.me.middleName,
              familyName: rootData.me.familyName,
            }}
            validate={validate}
            render={({ handleSubmit }) => (
              <Stack direction="column" spacing={1} sx={{ paddingTop: 1 }} component="form" noValidate onSubmit={handleSubmit}>
                <TextField label="Designation" name="designation" required={requiredFields.designation} />
                <TextField label="Title" name="title" required={requiredFields.title} />
                <TextField label="Name" name="name" required={requiredFields.name} />
                <TextField label="Given Name" name="givenName" required={requiredFields.givenName} />
                <TextField label="Middle Name" name="middleName" required={requiredFields.middleName} />
                <TextField label="Family Name" name="familyName" required={requiredFields.familyName} />
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

export default memo(CustomerSettingsPersonalTab);
