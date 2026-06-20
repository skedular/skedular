import { errorNotificationOptions, NotificationContent } from '@/components/notification';
import { DialogTransition } from '@/components/transitions';
import type { newCustomTagDialog_addCustomTagMutation } from '@/queries/__generated__/newCustomTagDialog_addCustomTagMutation.graphql';
import Dialog from '@mui/material/Dialog';
import DialogContent from '@mui/material/DialogContent';
import { getRelayErrorMessage, PaletteModeContext } from '@skedular/shared';
import { ColorPicker, DefaultDialogTitle, FormFieldLabel, FormStackColumn, TwoButtonsDialogActions } from '@skedular/ui';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { memo, useContext, useState } from 'react';
import { Form } from 'react-final-form';
import { graphql, useMutation } from 'react-relay';
import { toast } from 'react-toastify';
import { v7 as uuid } from 'uuid';
import { object, string } from 'yup';

type Props = {
  connectionIds: string[];
  isDialogOpen: boolean;
  onAddClicked: () => void;
  onCancel: () => void;
  organizationCustomDomain: string;
};

type CustomTagDetails = {
  name: string;
  description: string | null | undefined;
};

const customTagSchema = object({
  name: string().required('Tag name is required'),
  description: string().nullable(),
});

const NewCustomTagDialog = ({ connectionIds, isDialogOpen, onAddClicked, onCancel, organizationCustomDomain }: Props) => {
  const [commitAddCustomTag] = useMutation<newCustomTagDialog_addCustomTagMutation>(graphql`
    mutation newCustomTagDialog_addCustomTagMutation($connectionIds: [ID!]!, $input: AddCustomTagInput!) @raw_response_type {
      addCustomTag(input: $input) {
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
  const validate = makeValidate(customTagSchema);
  const requiredFields = makeRequired(customTagSchema);
  const [selectedColor, setSelectedColor] = useState('');

  const handleColorChange = (color: string) => {
    setSelectedColor(color);
  };

  const handleAddClick = ({ name, description }: CustomTagDetails) => {
    const id = uuid();

    commitAddCustomTag({
      variables: {
        connectionIds,
        input: {
          clientMutationId: uuid(),
          id,
          organizationCustomDomain,
          name,
          description,
          color: selectedColor,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          themedToast(<NotificationContent content={`Failed to add tag '${name}'. Error: ${getRelayErrorMessage(errors)}.`} />, errorNotificationOptions);

          return;
        }

        onAddClicked();
      },
      onError: (error) => {
        themedToast(<NotificationContent content={`Failed to add tag '${name}'. Error: ${error.message}.`} />, errorNotificationOptions);
      },
      optimisticResponse: {
        addCustomTag: {
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
    <Dialog slots={{ transition: DialogTransition }} open={isDialogOpen} onClose={onCancel} fullWidth>
      <DefaultDialogTitle title="Add Tag" />
      <DialogContent sx={{ marginTop: 2 }}>
        <Form
          onSubmit={handleAddClick}
          initialValues={{
            name: '',
          }}
          validate={validate}
          render={({ handleSubmit }) => (
            <FormStackColumn onSubmit={handleSubmit}>
              <FormFieldLabel label="Name">
                <TextField name="name" required={requiredFields.name} helperText="Add your tag name" />
              </FormFieldLabel>

              <FormFieldLabel label="Description">
                <TextField name="description" required={requiredFields.description} multiline rows={3} />
              </FormFieldLabel>

              <FormFieldLabel label="Color">
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

export default memo(NewCustomTagDialog);
