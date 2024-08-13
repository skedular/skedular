import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Dialog from '@mui/material/Dialog';
import DialogContent from '@mui/material/DialogContent';
import DialogTitle from '@mui/material/DialogTitle';
import Stack from '@mui/material/Stack';
import { SnackbarAnchorOrigin as anchorOrigin } from '@repo/shared/libs/snackbar';
import { joinErrors } from '@repo/shared/libs/utils';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import { useSnackbar } from 'notistack';
import { memo, useState } from 'react';
import { Form } from 'react-final-form';
import { graphql, useFragment, useMutation } from 'react-relay';
import { v4 as uuidv4 } from 'uuid';
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

interface DeskDetails {
  namePrefix: string;
  count: number;
  locationTagIds: string[];
}

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

  const { enqueueSnackbar } = useSnackbar();
  const validate = makeValidate(deskSchema);
  const requiredFields = makeRequired(deskSchema);
  const [namePrefix, setNamePrefix] = useState<string>('');
  const [count, setCount] = useState(0);
  const [locationTagIds, setLocationTagIds] = useState<string[]>([]);

  const handleAddClick = ({ namePrefix, count, locationTagIds }: DeskDetails) => {
    const ids = Array.from(Array(count).keys()).map((_) => uuidv4());

    commitAddDesk({
      variables: {
        connectionIds,
        input: {
          clientMutationId: uuidv4(),
          namePrefix,
          locationId,
          count: parseInt(count.toString()),
          locationTagIds: locationTagIds ? locationTagIds : [],
          deactivated: false,
          requireBookingApproval: false,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          enqueueSnackbar(`Failed to add desk '${name}'. Error: ${joinErrors(errors)}`, {
            variant: 'error',
            anchorOrigin,
          });
        } else {
          setNamePrefix('');
          setCount(0);
          setLocationTagIds([]);

          onAddClicked();
        }
      },
      onError: (error) => {
        enqueueSnackbar(`Failed to add desk '${name}'. Error: ${error.message}`, {
          variant: 'error',
          anchorOrigin,
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
    <Dialog fullWidth={true} open={isDialogOpen}>
      <DialogTitle>Add Desk</DialogTitle>
      <DialogContent>
        <Form
          onSubmit={handleAddClick}
          initialValues={{
            namePrefix,
            count,
            locationTagIds,
          }}
          validate={validate}
          render={({ handleSubmit }) => (
            <Box
              component="form"
              sx={{
                '& > :not(style)': { m: 1 },
              }}
              autoComplete="off"
              noValidate
              onSubmit={handleSubmit}
            >
              <TextField label="Optional name prefix" name="namePrefix" required={requiredFields.namePrefix} helperText="Add your desk name prefix" />
              <TextField label="Count" name="count" required={requiredFields.count} helperText="Add number of the desks to add" />
              <DeskMultipleChoicesZones rootDataRelay={rootData} name="locationTagIds" required={requiredFields.locationTagIds} />

              <Stack sx={{ flex: 1 }} direction="row" spacing={2}>
                <Button color="secondary" variant="contained" onClick={onCancelClicked}>
                  Cancel
                </Button>
                <Button color="primary" variant="contained" type="submit">
                  Add
                </Button>
              </Stack>
            </Box>
          )}
        />
      </DialogContent>
    </Dialog>
  );
};

export default memo(BulkNewDeskDialog);
