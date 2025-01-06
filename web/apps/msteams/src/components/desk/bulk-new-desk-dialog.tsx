import Dialog from '@mui/material/Dialog';
import DialogContent from '@mui/material/DialogContent';
import { DefaultDialogTitle, FormFieldLabel, FormStackColumn, TwoButtonsDialogActions } from '@repo/shared/components/commons';
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
import { MultipleChoicesDeskTypes, MultipleChoicesZones } from 'components/organization';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { nanoid } from 'nanoid';
import { memo, useContext } from 'react';
import { Form } from 'react-final-form';
import { useFragment, useMutation } from 'react-relay';
import { toast } from 'react-toastify';
import { array, number, object, string } from 'yup';
import type { bulkNewDeskDialog_bulkAddDeskMutation } from './__generated__/bulkNewDeskDialog_bulkAddDeskMutation.graphql';
import type { bulkNewDeskDialog_query$key } from './__generated__/bulkNewDeskDialog_query.graphql';

type Props = {
  rootDataRelay: bulkNewDeskDialog_query$key;
  connectionIds: string[];
  isDialogOpen: boolean;
  onAddClicked: () => void;
  onCancel: () => void;
  locationId: string;
};

type DeskDetails = {
  namePrefix: string;
  count: number;
  deskTypeIds: string[];
  zoneIds: string[];
};

const deskSchema = object({
  namePrefix: string(),
  count: number().positive().integer().required('Desk count is required'),
  deskTypeIds: array().nullable(),
  zoneIds: array().nullable(),
});

const BulkNewDeskDialog = ({ rootDataRelay, connectionIds, isDialogOpen, onAddClicked, onCancel, locationId }: Props) => {
  const rootData = useFragment(
    graphql`
      fragment bulkNewDeskDialog_query on Query {
        ...multipleChoicesDeskTypes_query
        ...multipleChoicesZones_query
      }
    `,
    rootDataRelay,
  );

  const [commitAddDesk] = useMutation<bulkNewDeskDialog_bulkAddDeskMutation>(graphql`
    mutation bulkNewDeskDialog_bulkAddDeskMutation($connectionIds: [ID!]!, $input: BulkAddDeskInput!) @raw_response_type {
      bulkAddDesk(input: $input) {
        desks @appendNode(connections: $connectionIds, edgeTypeName: "DeskDetails") {
          id
          name
          deskTypes {
            uniqueId
          }
          zones {
            uniqueId
          }
        }
      }
    }
  `);

  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const validate = makeValidate(deskSchema);
  const requiredFields = makeRequired(deskSchema);

  const handleAddClick = ({ namePrefix, count, deskTypeIds, zoneIds }: DeskDetails) => {
    const ids = Array.from(Array(count).keys()).map((_) => nanoid());
    const toastId = themedToast(<NotificationContent content={`Adding desks...`} />, infoNotificationOptions);

    commitAddDesk({
      variables: {
        connectionIds,
        input: {
          clientMutationId: nanoid(),
          namePrefix,
          locationId,
          count: parseInt(count.toString()),
          deactivated: false,
          requireBookingApproval: false,
          deskTypeIds,
          zoneIds,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to add desks. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Desks added.`} />,
        });

        onAddClicked();
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to add desk. Error: ${error.message}.`} />,
        });
      },
      optimisticResponse: {
        bulkAddDesk: {
          desks: ids.map((id) => ({ id, name: namePrefix, deskTypes: [], zones: [] })),
        },
      },
    });
  };

  return (
    <Dialog TransitionComponent={DialogTransition} open={isDialogOpen} fullWidth>
      <DefaultDialogTitle title="Add Desk" />
      <DialogContent>
        <Form
          onSubmit={handleAddClick}
          initialValues={{
            namePrefix: '',
            count: 0,
            deskTypeIds: [],
            zoneIds: [],
          }}
          validate={validate}
          render={({ handleSubmit }) => (
            <FormStackColumn onSubmit={handleSubmit}>
              <FormFieldLabel label="Optional name prefix" useWiderSpace>
                <TextField name="namePrefix" required={requiredFields.namePrefix} helperText="Add your desk name prefix" />
              </FormFieldLabel>

              <FormFieldLabel label="Count" useWiderSpace>
                <TextField name="count" required={requiredFields.count} helperText="Add number of the desks to add" />
              </FormFieldLabel>

              <FormFieldLabel label="Desk Types" useWiderSpace>
                <MultipleChoicesDeskTypes rootDataRelay={rootData} name="deskTypeIds" required={requiredFields.deskTypeIds} />
              </FormFieldLabel>

              <FormFieldLabel label="Zones" useWiderSpace>
                <MultipleChoicesZones rootDataRelay={rootData} name="zoneIds" required={requiredFields.zoneIds} />
              </FormFieldLabel>

              <TwoButtonsDialogActions onSecondaryClicked={onCancel} primaryLabel="Add" secondaryLabel="Cancel" />
            </FormStackColumn>
          )}
        />
      </DialogContent>
    </Dialog>
  );
};

export default memo(BulkNewDeskDialog);
