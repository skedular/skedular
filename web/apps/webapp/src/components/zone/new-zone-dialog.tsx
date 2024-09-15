import { TAG_TYPE_LOCATION_ZONE } from '@/components/zone';
import type { newZoneDialog_addZoneMutation } from '@/queries/__generated__/newZoneDialog_addZoneMutation.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Dialog from '@mui/material/Dialog';
import DialogContent from '@mui/material/DialogContent';
import DialogTitle from '@mui/material/DialogTitle';
import Stack from '@mui/material/Stack';
import { SnackbarAnchorOrigin as anchorOrigin } from '@repo/shared/libs/snackbar';
import { joinErrors } from '@repo/shared/libs/utils';
import { makeRequired, makeValidate } from 'mui-rff';
import { nanoid } from 'nanoid';
import { useSnackbar } from 'notistack';
import { memo, useState } from 'react';
import { Form } from 'react-final-form';
import { graphql, useMutation } from 'react-relay';
import { object, string } from 'yup';
import ZoneName from './zone-name';

type Props = {
  connectionIds: string[];
  isDialogOpen: boolean;
  onAddClicked: () => void;
  onCancelClicked: () => void;
  locationId: string;
};

interface ZoneDetails {
  name: string;
}

const zoneSchema = object({
  name: string().required('Zone name is required'),
});

const NewZoneDialog = ({ connectionIds, isDialogOpen, onAddClicked, onCancelClicked, locationId }: Props) => {
  const [commitAddZone] = useMutation<newZoneDialog_addZoneMutation>(graphql`
    mutation newZoneDialog_addZoneMutation($connectionIds: [ID!]!, $input: AddLocationTagInput!) @raw_response_type {
      addLocationTag(input: $input) {
        locationTag @appendNode(connections: $connectionIds, edgeTypeName: "LocationTagDetails") {
          id
          name
        }
      }
    }
  `);

  const { enqueueSnackbar } = useSnackbar();
  const validate = makeValidate(zoneSchema);
  const requiredFields = makeRequired(zoneSchema);
  const [name, setName] = useState<string>('');

  const handleAddClick = ({ name }: ZoneDetails) => {
    const id = nanoid();

    commitAddZone({
      variables: {
        connectionIds,
        input: {
          clientMutationId: nanoid(),
          id,
          locationId,
          name,
          tagType: TAG_TYPE_LOCATION_ZONE,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          enqueueSnackbar(`Failed to add zone '${name}'. Error: ${joinErrors(errors)}`, {
            variant: 'error',
            anchorOrigin,
          });
        } else {
          setName('');

          onAddClicked();
        }
      },
      onError: (error) => {
        enqueueSnackbar(`Failed to add zone '${name}'. Error: ${error.message}`, {
          variant: 'error',
          anchorOrigin,
        });
      },
      optimisticResponse: {
        addLocationTag: {
          locationTag: {
            id,
            name,
          },
        },
      },
    });
  };

  return (
    <Dialog fullWidth={true} open={isDialogOpen}>
      <DialogTitle>Add Zone</DialogTitle>
      <DialogContent>
        <Form
          onSubmit={handleAddClick}
          initialValues={{
            name,
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
              <ZoneName name="name" required={requiredFields.name} />

              <Stack sx={{ flex: 1 }} direction="row" spacing={2}>
                <Button color="secondary" variant="contained" onClick={onCancelClicked}>
                  Cancel
                </Button>
                <Button color="primary" variant="contained" type="submit">
                  Add
                </Button>
              </Stack>
            </Box>
          )}
        />
      </DialogContent>
    </Dialog>
  );
};

export default memo(NewZoneDialog);
