import { ColorPicker, DefaultDialogTitle, FormFieldLabel, FormStackColumn, LeadIconTypography, SmallIconTypography, TwoButtonsDialogActions } from '@/components/commons';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import { DialogTransition } from '@/components/transitions';
import { PaletteModeContext } from '@/libs/providers';
import { joinErrors } from '@/libs/utils';
import type { addOrganizationLocationTagDialog_addLocationTagMutation } from '@/queries/__generated__/addOrganizationLocationTagDialog_addLocationTagMutation.graphql';
import Dialog from '@mui/material/Dialog';
import DialogContent from '@mui/material/DialogContent';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { nanoid } from 'nanoid';
import { memo, useContext, useState } from 'react';
import { Form } from 'react-final-form';
import { graphql, useMutation } from 'react-relay';
import { toast } from 'react-toastify';
import { object, string } from 'yup';

type Props = {
  organizationId: string;
  connectionIds: string[];
  isDialogOpen: boolean;
  onAddClicked: () => void;
  onCancel: () => void;
};

type LocationTagDetails = {
  name: string;
  description: string | null | undefined;
};

const locationTagSchema = object({
  name: string().required('Location tag name is required'),
  description: string().nullable(),
});

const AddOrganizationLocationTagDialog = ({ organizationId, connectionIds, isDialogOpen, onAddClicked, onCancel }: Props) => {
  const [commitAddLocationTag] = useMutation<addOrganizationLocationTagDialog_addLocationTagMutation>(graphql`
    mutation addOrganizationLocationTagDialog_addLocationTagMutation($connectionIds: [ID!]!, $input: AddLocationTagInput!) @raw_response_type {
      addLocationTag(input: $input) {
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
  const validate = makeValidate(locationTagSchema);
  const requiredFields = makeRequired(locationTagSchema);
  const [selectedColor, setSelectedColor] = useState('');

  const handleColorChange = (color: string) => {
    setSelectedColor(color);
  };

  const handleAddClick = ({ name, description }: LocationTagDetails) => {
    const id = nanoid();
    const toastId = themedToast(<NotificationContent content={`Adding location tag '${name}'...`} />, infoNotificationOptions);

    commitAddLocationTag({
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
            render: <NotificationContent content={`Failed to add location tag '${name}'. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Location tag ${name} added.`} />,
        });

        onAddClicked();
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to add location tag '${name}'. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        addLocationTag: {
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
      <DefaultDialogTitle title="Add Location Tag" />
      <DialogContent sx={{ marginTop: 2 }}>
        <Form
          onSubmit={handleAddClick}
          initialValues={{}}
          validate={validate}
          render={({ handleSubmit }) => {
            return (
              <FormStackColumn onSubmit={handleSubmit}>
                <LeadIconTypography label="Add location tag to this organization" />
                <SmallIconTypography label="Enter the name of the location tag to add to this organization." />

                <FormFieldLabel label="Name" useWiderSpace>
                  <TextField name="name" required={requiredFields.name} />
                </FormFieldLabel>

                <FormFieldLabel label="Description" useWiderSpace>
                  <TextField name="description" required={requiredFields.description} multiline rows={3} />
                </FormFieldLabel>

                <FormFieldLabel label="Color" useWiderSpace>
                  <ColorPicker onChange={handleColorChange} />
                </FormFieldLabel>

                <TwoButtonsDialogActions onSecondaryClicked={onCancel} primaryLabel="Add" secondaryLabel="Cancel" />
              </FormStackColumn>
            );
          }}
        />
      </DialogContent>
    </Dialog>
  );
};

export default memo(AddOrganizationLocationTagDialog);
