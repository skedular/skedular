import type { newDeskTypeDialog_addDeskTypeMutation } from '@/queries/__generated__/newDeskTypeDialog_addDeskTypeMutation.graphql';
import Button from '@mui/material/Button';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogTitle from '@mui/material/DialogTitle';
import { FormStackColumn } from '@repo/shared/components/commons';
import { DeskTypeName } from '@repo/shared/components/deskType';
import {
  errorNotificationOptions,
  infoNotificationOptions,
  NotificationContent,
  successNotificationOptions,
} from '@repo/shared/components/notification';
import { DialogTransition } from '@repo/shared/components/transitions';
import { PaletteModeContext } from '@repo/shared/libs/providers';
import { joinErrors } from '@repo/shared/libs/utils';
import { makeRequired, makeValidate } from 'mui-rff';
import { nanoid } from 'nanoid';
import { memo, useContext } from 'react';
import { Form } from 'react-final-form';
import { graphql, useMutation } from 'react-relay';
import { toast } from 'react-toastify';
import { object, string } from 'yup';

type Props = {
  connectionIds: string[];
  isDialogOpen: boolean;
  onAddClicked: () => void;
  onCancelClicked: () => void;
  organizationId: string;
};

type DeskTypeDetails = {
  name: string;
};

const deskTypeSchema = object({
  name: string().required('Desk type name is required'),
});

const NewDeskTypeDialog = ({ connectionIds, isDialogOpen, onAddClicked, onCancelClicked, organizationId }: Props) => {
  const [commitAddDeskType] = useMutation<newDeskTypeDialog_addDeskTypeMutation>(graphql`
    mutation newDeskTypeDialog_addDeskTypeMutation($connectionIds: [ID!]!, $input: AddDeskTypeInput!) @raw_response_type {
      addDeskType(input: $input) {
        organizationTag @appendNode(connections: $connectionIds, edgeTypeName: "OrganizationTagDetails") {
          id
          name
        }
      }
    }
  `);

  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const validate = makeValidate(deskTypeSchema);
  const requiredFields = makeRequired(deskTypeSchema);

  const handleAddClick = ({ name }: DeskTypeDetails) => {
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
          },
        },
      },
    });
  };

  return (
    <Dialog TransitionComponent={DialogTransition} open={isDialogOpen}>
      <DialogTitle>Add desk type</DialogTitle>
      <DialogContent>
        <Form
          onSubmit={handleAddClick}
          initialValues={{
            name: '',
          }}
          validate={validate}
          render={({ handleSubmit }) => (
            <FormStackColumn onSubmit={handleSubmit}>
              <DeskTypeName name="name" required={requiredFields.name} />

              <DialogActions>
                <Button color="secondary" variant="contained" onClick={onCancelClicked}>
                  Cancel
                </Button>
                <Button color="primary" variant="contained" type="submit">
                  Add
                </Button>
              </DialogActions>
            </FormStackColumn>
          )}
        />
      </DialogContent>
    </Dialog>
  );
};

export default memo(NewDeskTypeDialog);
