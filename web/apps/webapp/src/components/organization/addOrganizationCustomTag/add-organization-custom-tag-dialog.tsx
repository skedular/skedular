import type { addOrganizationCustomTagDialog_addCustomTagMutation } from '@/queries/__generated__/addOrganizationCustomTagDialog_addCustomTagMutation.graphql';
import Dialog from '@mui/material/Dialog';
import DialogContent from '@mui/material/DialogContent';
import {
  ColorPicker,
  DefaultDialogTitle,
  FormFieldLabel,
  FormStackColumn,
  LeadIconTypography,
  SmallIconTypography,
  TwoButtonsDialogActions,
} from '@repo/shared/components/commons';
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
  organizationId: string;
  connectionIds: string[];
  isDialogOpen: boolean;
  onAddClicked: () => void;
  onCancel: () => void;
};

type CustomTagDetails = {
  name: string;
  description: string;
};

const customTagSchema = object({
  name: string().required('Tag name is required'),
  description: string().nullable(),
});

const AddOrganizationCustomTagDialog = ({ organizationId, connectionIds, isDialogOpen, onAddClicked, onCancel }: Props) => {
  const [commitAddCustomTag] = useMutation<addOrganizationCustomTagDialog_addCustomTagMutation>(graphql`
    mutation addOrganizationCustomTagDialog_addCustomTagMutation($connectionIds: [ID!]!, $input: AddCustomTagInput!) @raw_response_type {
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
    const id = nanoid();
    const toastId = themedToast(<NotificationContent content={`Adding tag '${name}'...`} />, infoNotificationOptions);

    commitAddCustomTag({
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
            render: <NotificationContent content={`Failed to add tag '${name}'. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Tag ${name} added.`} />,
        });

        onAddClicked();
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to add tag '${name}'. Error: ${error.message}.`} />,
        });
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
      <DefaultDialogTitle title="Add CustomTag" />
      <DialogContent>
        <Form
          onSubmit={handleAddClick}
          initialValues={{}}
          validate={validate}
          render={({ handleSubmit }) => {
            return (
              <FormStackColumn onSubmit={handleSubmit}>
                <LeadIconTypography label="Add tag to this organization" />
                <SmallIconTypography label="Enter the name of the tag to add to this organization." />

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

export default memo(AddOrganizationCustomTagDialog);
