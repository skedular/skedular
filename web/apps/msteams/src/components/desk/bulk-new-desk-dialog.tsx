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
import { DialogTransition } from '@repo/shared/components/transitions';
import { PaletteModeContext } from '@repo/shared/libs/providers';
import { joinErrors } from '@repo/shared/libs/utils';
import graphql from 'babel-plugin-relay/macro';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { nanoid } from 'nanoid';
import { memo, useContext } from 'react';
import { Form } from 'react-final-form';
import { useFragment, useMutation } from 'react-relay';
import { toast } from 'react-toastify';
import { array, number, object, string } from 'yup';
import type { bulkNewDeskDialog_bulkAddDeskMutation } from './__generated__/bulkNewDeskDialog_bulkAddDeskMutation.graphql';
import type { bulkNewDeskDialog_query$key } from './__generated__/bulkNewDeskDialog_query.graphql';
import DeskMultipleChoicesZones from './desk-multiple-choices-zones';

type Props = {
  rootDataRelay: bulkNewDeskDialog_query$key;
  connectionIds: string[];
  isDialogOpen: boolean;
  onAddClicked: () => void;
  onCancelClicked: () => void;
  locationId: string;
};

type DeskDetails = {
  namePrefix: string;
  count: number;
  locationTagIds: string[];
};

const deskSchema = object({
  namePrefix: string(),
  count: number().positive().integer().required('Desk count is required'),
  locationTagIds: array().nullable(),
});

const BulkNewDeskDialog = ({ rootDataRelay, connectionIds, isDialogOpen, onAddClicked, onCancelClicked, locationId }: Props) => {
  const rootData = useFragment(
    graphql`
      fragment bulkNewDeskDialog_query on Query {
        ...deskMultipleChoicesZones_query
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
          locationTags {
            id
          }
        }
      }
    }
  `);

  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const validate = makeValidate(deskSchema);
  const requiredFields = makeRequired(deskSchema);

  const handleAddClick = ({ namePrefix, count, locationTagIds }: DeskDetails) => {
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
          locationTagIds: locationTagIds ? locationTagIds : [],
          organizationTagIds: [],
          deactivated: false,
          requireBookingApproval: false,
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
          desks: ids.map((id) => ({ id, name: namePrefix, locationTags: [] })),
        },
      },
    });
  };

  return (
    <Dialog TransitionComponent={DialogTransition} open={isDialogOpen}>
      <DialogTitle>Add Desk</DialogTitle>
      <DialogContent>
        <Form
          onSubmit={handleAddClick}
          initialValues={{
            namePrefix: '',
            count: 0,
            locationTagIds: [],
          }}
          validate={validate}
          render={({ handleSubmit }) => (
            <Stack direction="column" spacing={2} sx={{ paddingTop: 1 }} component="form" noValidate onSubmit={handleSubmit}>
              <TextField label="Optional name prefix" name="namePrefix" required={requiredFields.namePrefix} helperText="Add your desk name prefix" />
              <TextField label="Count" name="count" required={requiredFields.count} helperText="Add number of the desks to add" />
              <DeskMultipleChoicesZones rootDataRelay={rootData} name="locationTagIds" required={requiredFields.locationTagIds} />
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

export default memo(BulkNewDeskDialog);
