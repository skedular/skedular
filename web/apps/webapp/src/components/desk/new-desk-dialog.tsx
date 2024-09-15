import type { newDeskDialog_addDeskMutation } from '@/queries/__generated__/newDeskDialog_addDeskMutation.graphql';
import type { newDeskDialog_query$key } from '@/queries/__generated__/newDeskDialog_query.graphql';
import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import Dialog from '@mui/material/Dialog';
import DialogContent from '@mui/material/DialogContent';
import DialogTitle from '@mui/material/DialogTitle';
import Stack from '@mui/material/Stack';
import { SnackbarAnchorOrigin as anchorOrigin } from '@repo/shared/libs/snackbar';
import { joinErrors } from '@repo/shared/libs/utils';
import { makeRequired, makeValidate } from 'mui-rff';
import { nanoid } from 'nanoid';
import { useSnackbar } from 'notistack';
import { memo, useState } from 'react';
import { Form } from 'react-final-form';
import { graphql, useFragment, useMutation } from 'react-relay';
import { array, object, string } from 'yup';
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

interface DeskDetails {
  name: string;
  locationTagIds: string[];
}

const deskSchema = object({
  name: string().required('Desk name is required'),
  locationTagIds: array().nullable(),
});

const NewDeskDialog = ({ rootDataRelay, connectionIds, isDialogOpen, onAddClicked, onCancelClicked, locationId }: Props) => {
  const rootData = useFragment(
    graphql`
      fragment newDeskDialog_query on Query {
        ...deskMultipleChoicesZones_query
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
        }
      }
    }
  `);

  const { enqueueSnackbar } = useSnackbar();
  const validate = makeValidate(deskSchema);
  const requiredFields = makeRequired(deskSchema);
  const [name, setName] = useState<string>('');
  const [locationTagIds, setLocationTagIds] = useState<string[]>([]);

  const handleAddClick = ({ name, locationTagIds }: DeskDetails) => {
    const id = nanoid();

    commitAddDesk({
      variables: {
        connectionIds,
        input: {
          clientMutationId: nanoid(),
          id,
          locationId,
          name,
          locationTagIds,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          enqueueSnackbar(`Failed to add desk '${name}'. Error: ${joinErrors(errors)}`, {
            variant: 'error',
            anchorOrigin,
          });
        } else {
          setName('');
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
        addDesk: {
          desk: {
            id,
            name,
            locationTags: [],
          },
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
            name,
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
              <DeskName name="name" required={requiredFields.name} />
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

export default memo(NewDeskDialog);
