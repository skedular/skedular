import { ColorPicker, DefaultDialogTitle, FormFieldLabel, FormStackColumn, LeadIconTypography, SmallIconTypography, TwoButtonsDialogActions } from '@/components/commons';
import { errorNotificationOptions, infoNotificationOptions, NotificationContent, successNotificationOptions } from '@/components/notification';
import { DialogTransition } from '@/components/transitions';
import { PaletteModeContext } from '@/libs/providers';
import { getRelayErrorMessage } from '@/libs/utils';
import type { addOrganizationProductTagDialog_addProductTagMutation } from '@/queries/__generated__/addOrganizationProductTagDialog_addProductTagMutation.graphql';
import Dialog from '@mui/material/Dialog';
import DialogContent from '@mui/material/DialogContent';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { memo, useContext, useState } from 'react';
import { Form } from 'react-final-form';
import { graphql, useMutation } from 'react-relay';
import { toast } from 'react-toastify';
import { v7 as uuid } from 'uuid';
import { object, string } from 'yup';

type Props = {
  organizationCustomDomain: string;
  connectionIds: string[];
  isDialogOpen: boolean;
  onAddClicked: () => void;
  onCancel: () => void;
};

type ProductTagDetails = {
  name: string;
  description: string | null | undefined;
};

const productTagSchema = object({
  name: string().required('Product tag name is required'),
  description: string().nullable(),
});

const AddOrganizationProductTagDialog = ({ organizationCustomDomain, connectionIds, isDialogOpen, onAddClicked, onCancel }: Props) => {
  const [commitAddProductTag] = useMutation<addOrganizationProductTagDialog_addProductTagMutation>(graphql`
    mutation addOrganizationProductTagDialog_addProductTagMutation($connectionIds: [ID!]!, $input: AddProductTagInput!) @raw_response_type {
      addProductTag(input: $input) {
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
  const validate = makeValidate(productTagSchema);
  const requiredFields = makeRequired(productTagSchema);
  const [selectedColor, setSelectedColor] = useState('');

  const handleColorChange = (color: string) => {
    setSelectedColor(color);
  };

  const handleAddClick = ({ name, description }: ProductTagDetails) => {
    const id = uuid();
    const toastId = themedToast(<NotificationContent content={`Adding product tag '${name}'...`} />, infoNotificationOptions);

    commitAddProductTag({
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
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`We couldn't add the product tag '${name}'. ${getRelayErrorMessage(errors)}`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`The product tag '${name}' has been added.`} />,
        });

        onAddClicked();
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`We couldn't add the product tag '${name}'. ${error.message}`} />,
        });
      },
      optimisticResponse: {
        addProductTag: {
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
      <DefaultDialogTitle title="Add Product Tag" />
      <DialogContent sx={{ marginTop: 2 }}>
        <Form
          onSubmit={handleAddClick}
          initialValues={{}}
          validate={validate}
          render={({ handleSubmit }) => {
            return (
              <FormStackColumn onSubmit={handleSubmit}>
                <LeadIconTypography label="Add product tag to this organization" />
                <SmallIconTypography label="Enter the name of the product tag to add to this organization." />

                <FormFieldLabel label="Name">
                  <TextField name="name" required={requiredFields.name} />
                </FormFieldLabel>

                <FormFieldLabel label="Description">
                  <TextField name="description" required={requiredFields.description} multiline rows={3} />
                </FormFieldLabel>

                <FormFieldLabel label="Color">
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

export default memo(AddOrganizationProductTagDialog);
