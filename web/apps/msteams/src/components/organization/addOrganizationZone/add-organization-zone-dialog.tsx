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
import type { addOrganizationZoneDialog_addZoneMutation } from './__generated__/addOrganizationZoneDialog_addZoneMutation.graphql';

type Props = {
  organizationId: string;
  connectionIds: string[];
  isDialogOpen: boolean;
  onAddClicked: () => void;
  onCancel: () => void;
};

type ZoneDetails = {
  name: string;
  description: string;
};

const zoneSchema = object({
  name: string().required('Zone name is required'),
  description: string().nullable(),
});

const AddOrganizationZoneDialog = ({ organizationId, connectionIds, isDialogOpen, onAddClicked, onCancel }: Props) => {
  const [commitAddZone] = useMutation<addOrganizationZoneDialog_addZoneMutation>(graphql`
    mutation addOrganizationZoneDialog_addZoneMutation($connectionIds: [ID!]!, $input: AddZoneInput!) @raw_response_type {
      addZone(input: $input) {
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
  const validate = makeValidate(zoneSchema);
  const requiredFields = makeRequired(zoneSchema);

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
          },
        },
      },
    });
  };

  return (
    <Dialog TransitionComponent={DialogTransition} open={isDialogOpen} fullWidth>
      <DefaultDialogTitle title="Add Zone" />
      <DialogContent>
        <Form
          onSubmit={handleAddClick}
          initialValues={{}}
          validate={validate}
          render={({ handleSubmit }) => {
            return (
              <FormStackColumn onSubmit={handleSubmit}>
                <LeadIconTypography label="Add zone to this organization" />
                <SmallIconTypography label="Enter the name of the zone to add to this organization." />

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

export default memo(AddOrganizationZoneDialog);
