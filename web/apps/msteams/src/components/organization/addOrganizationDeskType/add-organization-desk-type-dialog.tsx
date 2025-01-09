import Dialog from '@mui/material/Dialog';
import DialogContent from '@mui/material/DialogContent';
import {
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
import graphql from 'babel-plugin-relay/macro';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { nanoid } from 'nanoid';
import { memo, useContext } from 'react';
import { Form } from 'react-final-form';
import { useMutation } from 'react-relay';
import { toast } from 'react-toastify';
import { object, string } from 'yup';
import type { addOrganizationDeskTypeDialog_addDeskTypeMutation } from './__generated__/addOrganizationDeskTypeDialog_addDeskTypeMutation.graphql';

type Props = {
  organizationId: string;
  connectionIds: string[];
  isDialogOpen: boolean;
  onAddClicked: () => void;
  onCancel: () => void;
};

type DeskTypeDetails = {
  name: string;
  description: string;
};

const deskTypeSchema = object({
  name: string().required('Desk type name is required'),
  description: string().nullable(),
});

const AddOrganizationDeskTypeDialog = ({ organizationId, connectionIds, isDialogOpen, onAddClicked, onCancel }: Props) => {
  const [commitAddDeskType] = useMutation<addOrganizationDeskTypeDialog_addDeskTypeMutation>(graphql`
    mutation addOrganizationDeskTypeDialog_addDeskTypeMutation($connectionIds: [ID!]!, $input: AddDeskTypeInput!) @raw_response_type {
      addDeskType(input: $input) {
        organizationTag @appendNode(connections: $connectionIds, edgeTypeName: "OrganizationTagDetails") {
          id
          name
          description
        }
      }
    }
  `);

  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const validate = makeValidate(deskTypeSchema);
  const requiredFields = makeRequired(deskTypeSchema);

  const handleAddClick = ({ name, description }: DeskTypeDetails) => {
    const id = nanoid();
    const toastId = themedToast(<NotificationContent content={`Adding desk type '${name}'...`} />, infoNotificationOptions);

    commitAddDeskType({
      variables: {
        connectionIds,
        input: {
          clientMutationId: nanoid(),
          id,
          organizationId,
          name,
          description,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to add desk type '${name}'. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Desk type ${name} added.`} />,
        });

        onAddClicked();
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to add desk type '${name}'. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        addDeskType: {
          organizationTag: {
            id,
            name,
            description,
          },
        },
      },
    });
  };

  return (
    <Dialog TransitionComponent={DialogTransition} open={isDialogOpen} fullWidth>
      <DefaultDialogTitle title="Add DeskType" />
      <DialogContent>
        <Form
          onSubmit={handleAddClick}
          initialValues={{}}
          validate={validate}
          render={({ handleSubmit }) => {
            return (
              <FormStackColumn onSubmit={handleSubmit}>
                <LeadIconTypography label="Add desk type to this organization" />
                <SmallIconTypography label="Enter the name of the desk type to add to this organization." />

                <FormFieldLabel label="Name" useWiderSpace>
                  <TextField name="name" required={requiredFields.name} />
                </FormFieldLabel>

                <FormFieldLabel label="Description" useWiderSpace>
                  <TextField name="description" required={requiredFields.description} multiline rows={3} />
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

export default memo(AddOrganizationDeskTypeDialog);
