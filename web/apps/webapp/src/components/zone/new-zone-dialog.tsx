import type { newZoneDialog_addZoneMutation } from '@/queries/__generated__/newZoneDialog_addZoneMutation.graphql';
import Dialog from '@mui/material/Dialog';
import DialogContent from '@mui/material/DialogContent';
import { ColorPicker, DefaultDialogTitle, FormFieldLabel, FormStackColumn, TwoButtonsDialogActions } from '@repo/shared/components/commons';
import {
  errorNotificationOptions,
  infoNotificationOptions,
  NotificationContent,
  successNotificationOptions,
} from '@repo/shared/components/notification';
import { DialogTransition } from '@repo/shared/components/transitions';
import { PaletteModeContext } from '@repo/shared/libs/providers';
import { joinErrors } from '@repo/shared/libs/utils';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { nanoid } from 'nanoid';
import { memo, useContext, useState } from 'react';
import { Form } from 'react-final-form';
import { graphql, useMutation } from 'react-relay';
import { toast } from 'react-toastify';
import { object, string } from 'yup';

type Props = {
  connectionIds: string[];
  isDialogOpen: boolean;
  onAddClicked: () => void;
  onCancel: () => void;
  organizationId: string;
};

type ZoneDetails = {
  name: string;
  description: string;
};

const zoneSchema = object({
  name: string().required('Zone name is required'),
  description: string().nullable(),
});

const NewZoneDialog = ({ connectionIds, isDialogOpen, onAddClicked, onCancel, organizationId }: Props) => {
  const [commitAddZone] = useMutation<newZoneDialog_addZoneMutation>(graphql`
    mutation newZoneDialog_addZoneMutation($connectionIds: [ID!]!, $input: AddZoneInput!) @raw_response_type {
      addZone(input: $input) {
        organizationTag @appendNode(connections: $connectionIds, edgeTypeName: "OrganizationTagDetails") {
          id
          name
          description
          color
        }
      }
    }
  `);

  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const validate = makeValidate(zoneSchema);
  const requiredFields = makeRequired(zoneSchema);
  const [selectedColor, setSelectedColor] = useState('');

  const handleColorChange = (color: string) => {
    setSelectedColor(color);
  };

  const handleAddClick = ({ name, description }: ZoneDetails) => {
    const id = nanoid();
    const toastId = themedToast(<NotificationContent content={`Adding zone '${name}'...`} />, infoNotificationOptions);

    commitAddZone({
      variables: {
        connectionIds,
        input: {
          clientMutationId: nanoid(),
          id,
          organizationId,
          name,
          description,
          color: selectedColor,
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
        addZone: {
          organizationTag: {
            id,
            name,
            description,
            color: selectedColor,
          },
        },
      },
    });
  };

  return (
    <Dialog TransitionComponent={DialogTransition} open={isDialogOpen} onClose={onCancel} fullWidth>
      <DefaultDialogTitle title="Add Zone" />
      <DialogContent>
        <Form
          onSubmit={handleAddClick}
          initialValues={{
            name: '',
          }}
          validate={validate}
          render={({ handleSubmit }) => (
            <FormStackColumn onSubmit={handleSubmit}>
              <FormFieldLabel label="Name" useWiderSpace>
                <TextField name="name" required={requiredFields.name} helperText="Add your zone name" />
              </FormFieldLabel>

              <FormFieldLabel label="Description" useWiderSpace>
                <TextField name="description" required={requiredFields.description} multiline rows={3} />
              </FormFieldLabel>

              <FormFieldLabel label="Color" useWiderSpace>
                <ColorPicker onChange={handleColorChange} />
              </FormFieldLabel>

              <TwoButtonsDialogActions onSecondaryClicked={onCancel} primaryLabel="Add" secondaryLabel="Cancel" />
            </FormStackColumn>
          )}
        />
      </DialogContent>
    </Dialog>
  );
};

export default memo(NewZoneDialog);
