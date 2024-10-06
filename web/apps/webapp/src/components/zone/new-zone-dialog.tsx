import type { newZoneDialog_addZoneMutation } from '@/queries/__generated__/newZoneDialog_addZoneMutation.graphql';
import Button from '@mui/material/Button';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogTitle from '@mui/material/DialogTitle';
import Stack from '@mui/material/Stack';
import { TAG_TYPE_LOCATION_ZONE, ZoneName } from '@repo/shared/components/zone';
import { SnackbarAnchorOrigin as anchorOrigin } from '@repo/shared/libs/snackbar';
import { joinErrors } from '@repo/shared/libs/utils';
import { makeRequired, makeValidate } from 'mui-rff';
import { nanoid } from 'nanoid';
import { useSnackbar } from 'notistack';
import { memo, useState } from 'react';
import { Form } from 'react-final-form';
import { graphql, useMutation } from 'react-relay';
import { object, string } from 'yup';

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
    <Dialog open={isDialogOpen}>
      <DialogTitle>Add Zone</DialogTitle>
      <DialogContent>
        <Form
          onSubmit={handleAddClick}
          initialValues={{
            name,
          }}
          validate={validate}
          render={({ handleSubmit }) => (
            <Stack direction="column" spacing={1} component="form" noValidate onSubmit={handleSubmit}>
              <ZoneName name="name" required={requiredFields.name} />

              <DialogActions>
                <Button color="secondary" variant="contained" onClick={onCancelClicked}>
                  Cancel
                </Button>
                <Button color="primary" variant="contained" type="submit">
                  Add
                </Button>
              </DialogActions>
            </Stack>
          )}
        />
      </DialogContent>
    </Dialog>
  );
};

export default memo(NewZoneDialog);
