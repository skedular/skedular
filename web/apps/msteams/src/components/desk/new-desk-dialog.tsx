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
import { MultipleChoicesDeskTypes, MultipleChoicesZones } from 'components/organization';
import { makeRequired, makeValidate } from 'mui-rff';
import { nanoid } from 'nanoid';
import { memo, useContext } from 'react';
import { Form } from 'react-final-form';
import { useFragment, useMutation } from 'react-relay';
import { toast } from 'react-toastify';
import { array, object, string } from 'yup';
import type { newDeskDialog_addDeskMutation } from './__generated__/newDeskDialog_addDeskMutation.graphql';
import type { newDeskDialog_query$key } from './__generated__/newDeskDialog_query.graphql';
import DeskMultipleChoicesZones from './desk-multiple-choices-zones';
import DeskName from './desk-name';

type Props = {
  rootDataRelay: newDeskDialog_query$key;
  connectionIds: string[];
  isDialogOpen: boolean;
  onAddClicked: () => void;
  onCancelClicked: () => void;
  locationId: string;
};

type DeskDetails = {
  name: string;
  locationTagIds: string[];
  deskTypeIds: string[];
  zoneIds: string[];
};

const deskSchema = object({
  name: string().required('Desk name is required'),
  locationTagIds: array().nullable(),
  deskTypeIds: array().nullable(),
  zoneIds: array().nullable(),
});

const NewDeskDialog = ({ rootDataRelay, connectionIds, isDialogOpen, onAddClicked, onCancelClicked, locationId }: Props) => {
  const rootData = useFragment(
    graphql`
      fragment newDeskDialog_query on Query {
        ...deskMultipleChoicesZones_query
        ...multipleChoicesDeskTypes_query
        ...multipleChoicesZones_query
      }
    `,
    rootDataRelay,
  );

  const [commitAddDesk] = useMutation<newDeskDialog_addDeskMutation>(graphql`
    mutation newDeskDialog_addDeskMutation($connectionIds: [ID!]!, $input: AddDeskInput!) @raw_response_type {
      addDesk(input: $input) {
        desk @appendNode(connections: $connectionIds, edgeTypeName: "DeskDetails") {
          id
          name
          locationTags {
            id
          }
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

  const handleAddClick = ({ name, locationTagIds, deskTypeIds, zoneIds }: DeskDetails) => {
    const id = nanoid();
    const toastId = themedToast(<NotificationContent content={`Adding desk '${name}'...`} />, infoNotificationOptions);

    commitAddDesk({
      variables: {
        connectionIds,
        input: {
          clientMutationId: nanoid(),
          id,
          locationId,
          name,
          locationTagIds,
          deskTypeIds,
          zoneIds,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to add desk '${name}'. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Desk ${name} added.`} />,
        });

        onAddClicked();
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to add desk '${name}'. Error: ${error.message}.`} />,
        });
      },

      optimisticResponse: {
        addDesk: {
          desk: {
            id,
            name,
            locationTags: [],
            deskTypes: [],
            zones: [],
          },
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
            name: '',
            locationTagIds: [],
            deskTypeIds: [],
          }}
          validate={validate}
          render={({ handleSubmit }) => (
            <Stack direction="column" spacing={2} sx={{ paddingTop: 1 }} component="form" noValidate onSubmit={handleSubmit}>
              <DeskName name="name" required={requiredFields.name} />
              <DeskMultipleChoicesZones rootDataRelay={rootData} name="locationTagIds" required={requiredFields.locationTagIds} />
              <MultipleChoicesDeskTypes rootDataRelay={rootData} name="deskTypeIds" required={requiredFields.deskTypeIds} />
              <MultipleChoicesZones rootDataRelay={rootData} name="zoneIds" required={requiredFields.zoneIds} />

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

export default memo(NewDeskDialog);
