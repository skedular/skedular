import { FormFieldLabel, FormStackColumn } from '@/components/commons';
import { defaultButtonStyle } from '@/libs/theme';
import { joinErrors } from '@/libs/utils';
import type { editFloorPlanDialog_updateFloorPlanMutation } from '@/queries/__generated__/editFloorPlanDialog_updateFloorPlanMutation.graphql';
import Button from '@mui/material/Button';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogTitle from '@mui/material/DialogTitle';
import { makeRequired, makeValidate, TextField, Switches } from 'mui-rff';
import Stack from '@mui/material/Stack';
import { nanoid } from 'nanoid';
import { memo } from 'react';
import { Form } from 'react-final-form';
import { graphql, useMutation } from 'react-relay';
import { toast } from 'react-toastify';
import { boolean, number, object, string } from 'yup';
import { NotificationContent } from '@/components/notification';

type Props = {
  open: boolean;
  onClose: () => void;
  floorPlan: {
    id: string;
    name: string;
    floorLevel: number;
    floorName: string | null | undefined;
    imagePath: string;
    thumbnailPath: string | null | undefined;
    width: number;
    height: number;
    isActive: boolean;
  };
  onReloadRequired: () => void;
};

type FloorPlanFormData = {
  name: string;
  floorLevel: number;
  floorName: string | null;
  isActive: boolean;
};

const floorPlanSchema = object({
  name: string().required('Floor plan name is required'),
  floorLevel: number().required('Floor level is required').integer('Floor level must be a whole number'),
  floorName: string().nullable(),
  isActive: boolean().required(),
});

const EditFloorPlanDialog = ({ open, onClose, floorPlan, onReloadRequired }: Props) => {
  const [commitUpdateFloorPlan] = useMutation<editFloorPlanDialog_updateFloorPlanMutation>(graphql`
    mutation editFloorPlanDialog_updateFloorPlanMutation($input: UpdateFloorPlanInput!) {
      updateFloorPlan(input: $input) {
        floorPlan {
          id
          name
          floorLevel
          floorName
          imagePath
          thumbnailPath
          width
          height
          isActive
        }
      }
    }
  `);

  const validateFloorPlan = makeValidate(floorPlanSchema);
  const requiredFields = makeRequired(floorPlanSchema);

  const handleSubmit = (values: FloorPlanFormData) => {
    commitUpdateFloorPlan({
      variables: {
        input: {
          clientMutationId: nanoid(),
          id: floorPlan.id,
          name: values.name,
          floorLevel: typeof values.floorLevel === 'string' ? parseInt(values.floorLevel, 10) : values.floorLevel,
          floorName: values.floorName,
          isActive: values.isActive,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.error(<NotificationContent content={`Failed to update floor plan. Error: ${joinErrors(errors)}`} />);
          return;
        }

        toast.success(<NotificationContent content="Floor plan updated successfully" />);
        onReloadRequired();
        onClose();
      },
      onError: (error) => {
        toast.error(<NotificationContent content={`Failed to update floor plan. Error: ${error.message}`} />);
      },
    });
  };

  return (
    <Dialog open={open} onClose={onClose} maxWidth="sm" fullWidth>
      <DialogTitle>Edit Floor Plan</DialogTitle>
      <Form
        onSubmit={handleSubmit}
        initialValues={{
          name: floorPlan.name,
          floorLevel: floorPlan.floorLevel,
          floorName: floorPlan.floorName,
          isActive: floorPlan.isActive,
        }}
        validate={validateFloorPlan}
        render={({ handleSubmit, submitting }) => (
          <FormStackColumn onSubmit={handleSubmit}>
            <DialogContent sx={{ marginTop: 2 }}>
              <Stack spacing={2}>
                <FormFieldLabel label="Name *" useWiderSpace>
                  <TextField name="name" required={requiredFields.name} placeholder="e.g., Main Floor, Building A - Level 2" fullWidth />
                </FormFieldLabel>

                <FormFieldLabel label="Floor Level *" useWiderSpace>
                  <TextField
                    name="floorLevel"
                    type="number"
                    required={requiredFields.floorLevel}
                    placeholder="e.g., 1, 2, -1 for basement"
                    helperText="Each floor level must be unique. Only one floor plan allowed per level."
                    fullWidth
                  />
                </FormFieldLabel>

                <FormFieldLabel label="Floor Name" useWiderSpace>
                  <TextField name="floorName" required={requiredFields.floorName} placeholder="e.g., Ground Floor, Mezzanine" fullWidth />
                </FormFieldLabel>

                <FormFieldLabel label="Status" useWiderSpace>
                  <Switches name="isActive" data={{ label: 'Active', value: true }} />
                </FormFieldLabel>

                {floorPlan.thumbnailPath && (
                  <FormFieldLabel label="Current Image" useWiderSpace>
                    <img
                      src={floorPlan.thumbnailPath}
                      alt={floorPlan.name}
                      style={{
                        maxWidth: '100%',
                        height: 'auto',
                        border: '1px solid #e0e0e0',
                        borderRadius: 4,
                      }}
                    />
                  </FormFieldLabel>
                )}
              </Stack>
            </DialogContent>
            <DialogActions>
              <Button onClick={onClose}>Cancel</Button>
              <Button type="submit" variant="contained" sx={defaultButtonStyle} disabled={submitting}>
                Update Floor Plan
              </Button>
            </DialogActions>
          </FormStackColumn>
        )}
      />
    </Dialog>
  );
};

export { EditFloorPlanDialog };
export default memo(EditFloorPlanDialog);
