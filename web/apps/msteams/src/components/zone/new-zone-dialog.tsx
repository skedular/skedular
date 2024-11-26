import Button from '@mui/material/Button';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogTitle from '@mui/material/DialogTitle';
import Stack from '@mui/material/Stack';
import {
  errorNotificationOptions,
  infoNotificationOptions,
  NotificationContent,
  successNotificationOptions,
} from '@repo/shared/components/notification';
import { LOCATION_TAG_TYPE_LOCATION_ZONE, ZoneName } from '@repo/shared/components/oldZone';
import { DialogTransition } from '@repo/shared/components/transitions';
import { PaletteModeContext } from '@repo/shared/libs/providers';
import { joinErrors } from '@repo/shared/libs/utils';
import graphql from 'babel-plugin-relay/macro';
import { makeRequired, makeValidate } from 'mui-rff';
import { nanoid } from 'nanoid';
import { memo, useContext } from 'react';
import { Form } from 'react-final-form';
import { useMutation } from 'react-relay';
import { toast } from 'react-toastify';
import { object, string } from 'yup';
import type { newZoneDialog_addZoneMutation } from './__generated__/newZoneDialog_addZoneMutation.graphql';

type Props = {
  connectionIds: string[];
  isDialogOpen: boolean;
  onAddClicked: () => void;
  onCancelClicked: () => void;
  locationId: string;
};

type ZoneDetails = {
  name: string;
};

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

  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const validate = makeValidate(zoneSchema);
  const requiredFields = makeRequired(zoneSchema);

  const handleAddClick = ({ name }: ZoneDetails) => {
    const id = nanoid();
    const toastId = themedToast(<NotificationContent content={`Adding zone '${name}'...`} />, infoNotificationOptions);

    commitAddZone({
      variables: {
        connectionIds,
        input: {
          clientMutationId: nanoid(),
          id,
          locationId,
          name,
          tagType: LOCATION_TAG_TYPE_LOCATION_ZONE,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to add zone '${name}'. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Zone ${name} added.`} />,
        });

        onAddClicked();
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to add zone '${name}'. Error: ${error.message}.`} />,
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
    <Dialog TransitionComponent={DialogTransition} open={isDialogOpen}>
      <DialogTitle>Add Zone</DialogTitle>
      <DialogContent>
        <Form
          onSubmit={handleAddClick}
          initialValues={{
            name: '',
          }}
          validate={validate}
          render={({ handleSubmit }) => (
            <Stack direction="column" spacing={2} sx={{ paddingTop: 1 }} component="form" noValidate onSubmit={handleSubmit}>
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
