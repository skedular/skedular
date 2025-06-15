import { BodyIconTypography, FormFieldLabel, FormStackColumn } from '@/components/commons';
import { defaultButtonStyle } from '@/libs/theme';
import { joinErrors } from '@/libs/utils';
import type { addFloorPlanDialog_addFloorPlanMutation } from '@/queries/__generated__/addFloorPlanDialog_addFloorPlanMutation.graphql';
import Button from '@mui/material/Button';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogTitle from '@mui/material/DialogTitle';
import { makeRequired, makeValidate, TextField } from 'mui-rff';
import Stack from '@mui/material/Stack';
import Box from '@mui/material/Box';
import CloudUploadIcon from '@mui/icons-material/CloudUpload';
import { nanoid } from 'nanoid';
import { memo, useState } from 'react';
import { Form } from 'react-final-form';
import { graphql, useMutation } from 'react-relay';
import { toast } from 'react-toastify';
import { number, object, string } from 'yup';
import { NotificationContent } from '@/components/notification';

type Props = {
  open: boolean;
  onClose: () => void;
  locationId: string;
  onReloadRequired: () => void;
};

type FloorPlanFormData = {
  name: string;
  floorLevel: number;
  floorName: string | null;
};

const floorPlanSchema = object({
  name: string().required('Floor plan name is required'),
  floorLevel: number().required('Floor level is required').integer('Floor level must be a whole number'),
  floorName: string().nullable(),
});

const AddFloorPlanDialog = ({ open, onClose, locationId, onReloadRequired }: Props) => {
  const [imageFile, setImageFile] = useState<File | null>(null);
  const [imageError, setImageError] = useState<string | null>(null);
  const [isUploading, setIsUploading] = useState(false);

  const [commitAddFloorPlan] = useMutation<addFloorPlanDialog_addFloorPlanMutation>(graphql`
    mutation addFloorPlanDialog_addFloorPlanMutation($input: AddFloorPlanInput!) {
      addFloorPlan(input: $input) {
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

  const handleFileChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0];
    if (!file) return;

    if (!file.type.startsWith('image/')) {
      setImageError('Please select an image file');
      return;
    }

    if (file.size > 2 * 1024 * 1024) {
      setImageError('Image size must be less than 2MB');
      return;
    }

    setImageFile(file);
    setImageError(null);
  };

  const handleSubmit = async (values: FloorPlanFormData) => {
    if (!imageFile) {
      setImageError('Please select an image file');
      return;
    }

    setIsUploading(true);

    const reader = new FileReader();
    reader.onload = () => {
      const base64String = reader.result?.toString().split(',')[1];
      if (!base64String) {
        toast.error('Failed to process image');
        setIsUploading(false);
        return;
      }

      commitAddFloorPlan({
        variables: {
          input: {
            clientMutationId: nanoid(),
            locationId,
            name: values.name,
            floorLevel: typeof values.floorLevel === 'string' ? parseInt(values.floorLevel, 10) : values.floorLevel,
            floorName: values.floorName,
            imageBase64: base64String,
            imageFileName: imageFile.name,
          },
        },
        onCompleted: (response, errors) => {
          setIsUploading(false);

          if (errors && errors.length > 0) {
            const errorMessage = joinErrors(errors);
            if (errorMessage.toLowerCase().includes('floor plan already exists for floor level')) {
              toast.error(<NotificationContent content={`A floor plan already exists for floor level ${values.floorLevel}. Please choose a different floor level.`} />);
            } else {
              toast.error(<NotificationContent content={`Failed to add floor plan. Error: ${errorMessage}`} />);
            }
            return;
          }

          toast.success(<NotificationContent content="Floor plan added successfully" />);
          onReloadRequired();
          onClose();
        },
        onError: (error) => {
          setIsUploading(false);

          let errorMessage = error.message || 'Unknown error occurred';

          const errorWithSource = error as any;
          if (errorWithSource.source && errorWithSource.source.errors && Array.isArray(errorWithSource.source.errors)) {
            const graphqlErrors = errorWithSource.source.errors;
            errorMessage = graphqlErrors.map((e: any) => e.message).join('\n');
          }

          if (errorMessage.toLowerCase().includes('floor plan already exists for floor level') || errorMessage.toLowerCase().includes('a floor plan already exists')) {
            toast.error(<NotificationContent content={errorMessage} />);
          } else {
            toast.error(<NotificationContent content={`Failed to add floor plan. Error: ${errorMessage}`} />);
          }
        },
      });
    };

    reader.readAsDataURL(imageFile);
  };

  return (
    <Dialog open={open} onClose={onClose} maxWidth="sm" fullWidth>
      <DialogTitle>Add Floor Plan</DialogTitle>
      <Form
        onSubmit={handleSubmit}
        initialValues={{
          name: '',
          floorLevel: 1,
          floorName: null,
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

                <FormFieldLabel label="Floor Plan Image *" useWiderSpace>
                  <Stack spacing={1} sx={{ width: '100%' }}>
                    <Button
                      component="label"
                      variant="outlined"
                      startIcon={<CloudUploadIcon />}
                      sx={{
                        justifyContent: 'flex-start',
                        textTransform: 'none',
                        color: imageFile ? 'success.main' : 'text.primary',
                        borderColor: imageError ? 'error.main' : imageFile ? 'success.main' : 'divider',
                      }}
                    >
                      {imageFile ? imageFile.name : 'Choose floor plan image'}
                      <input type="file" accept="image/*" onChange={handleFileChange} hidden />
                    </Button>
                    {imageError && <BodyIconTypography label={imageError} sx={{ color: 'error.main' }} />}
                    {imageFile && (
                      <Box sx={{ display: 'flex', flexDirection: 'column', gap: 0.5 }}>
                        <BodyIconTypography label={`Size: ${(imageFile.size / 1024).toFixed(0)} KB`} sx={{ color: 'text.secondary', fontSize: '0.875rem' }} />
                      </Box>
                    )}
                  </Stack>
                </FormFieldLabel>
              </Stack>
            </DialogContent>
            <DialogActions>
              <Button onClick={onClose}>Cancel</Button>
              <Button type="submit" variant="contained" sx={defaultButtonStyle} disabled={submitting || isUploading || !imageFile}>
                {isUploading ? 'Uploading...' : 'Add Floor Plan'}
              </Button>
            </DialogActions>
          </FormStackColumn>
        )}
      />
    </Dialog>
  );
};

export { AddFloorPlanDialog };
export default memo(AddFloorPlanDialog);
