'use client';

import Box from '@mui/material/Box';
import Button from '@mui/material/Button';
import CircularProgress from '@mui/material/CircularProgress';
import Grid from '@mui/material/Grid';
import TextField from '@mui/material/TextField';
import Typography from '@/components/commons/Typography';
import { Field, Form } from 'react-final-form';
import * as yup from 'yup';

export type ProductFormValues = {
  name: string;
  price: number;
  description?: string;
  capacity?: number;
};

export type ProductFormProps = {
  initialValues?: Partial<ProductFormValues>;
  submitting?: boolean;
  onSubmit: (values: ProductFormValues) => void | Promise<void>;
  onCancel?: () => void;
  title?: string;
};

const schema: yup.ObjectSchema<ProductFormValues> = yup.object({
  name: yup.string().trim().required('Product name is required'),
  price: yup.number().typeError('Price must be a number').moreThan(0, 'Price must be greater than 0').required('Price is required'),
  description: yup.string().trim().optional(),
  capacity: yup.number().typeError('Capacity must be a number').integer('Capacity must be a whole number').min(1, 'Capacity must be at least 1').optional(),
});

const validate = (values: Partial<ProductFormValues>): Partial<Record<keyof ProductFormValues, string>> => {
  try {
    schema.validateSync(values, { abortEarly: false });
    return {};
  } catch (error) {
    if (error instanceof yup.ValidationError) {
      return error.inner.reduce<Partial<Record<keyof ProductFormValues, string>>>((accumulator, current) => {
        if (current.path && !(current.path in accumulator)) {
          accumulator[current.path as keyof ProductFormValues] = current.message;
        }
        return accumulator;
      }, {});
    }
    return {};
  }
};

const requiredAsterisk = (
  <Typography component="span" color="error" aria-hidden="true">
    {' '}
    *
  </Typography>
);

const ProductForm = ({ initialValues, submitting = false, onSubmit, onCancel, title = 'Product details' }: ProductFormProps) => (
  <Form<ProductFormValues>
    initialValues={{
      name: initialValues?.name ?? '',
      price: initialValues?.price ?? 0,
      description: initialValues?.description ?? '',
      capacity: initialValues?.capacity ?? undefined,
    }}
    validate={validate}
    onSubmit={onSubmit}
  >
    {({ handleSubmit: handleFormSubmit, submitting: formSubmitting, dirty, hasValidationErrors }) => (
      <Box component="form" onSubmit={handleFormSubmit} noValidate sx={{ maxWidth: 720 }}>
        <Typography variant="h6" component="h2" sx={{ fontWeight: 600, mb: 2 }}>
          {title}
        </Typography>
        <Grid container spacing={2}>
          <Grid size={{ xs: 12, sm: 8 }}>
            <Field name="name">
              {({ input, meta }) => (
                <TextField
                  {...input}
                  label={
                    <>
                      Name
                      {requiredAsterisk}
                    </>
                  }
                  placeholder="e.g. Hot Desk — Daily Pass"
                  fullWidth
                  required
                  error={meta.touched && Boolean(meta.error)}
                  helperText={meta.touched && meta.error ? meta.error : ' '}
                />
              )}
            </Field>
          </Grid>
          <Grid size={{ xs: 12, sm: 4 }}>
            <Field name="price">
              {({ input, meta }) => (
                <TextField
                  {...input}
                  label={
                    <>
                      Price (USD)
                      {requiredAsterisk}
                    </>
                  }
                  type="number"

                  fullWidth
                  required
                  error={meta.touched && Boolean(meta.error)}
                  helperText={meta.touched && meta.error ? meta.error : ' '}
                />
              )}
            </Field>
          </Grid>
          <Grid size={{ xs: 12 }}>
            <Field name="description">
              {({ input, meta }) => (
                <TextField
                  {...input}
                  label="Description"
                  placeholder="Describe the product for your customers"
                  multiline
                  minRows={3}
                  maxRows={6}
                  fullWidth
                  error={meta.touched && Boolean(meta.error)}
                  helperText={meta.touched && meta.error ? meta.error : 'Optional'}
                />
              )}
            </Field>
          </Grid>
          <Grid size={{ xs: 12, sm: 6 }}>
            <Field name="capacity">
              {({ input, meta }) => (
                <TextField
                  {...input}
                  label="Capacity"
                  type="number"

                  fullWidth
                  error={meta.touched && Boolean(meta.error)}
                  helperText={meta.touched && meta.error ? meta.error : 'Maximum number of guests'}
                />
              )}
            </Field>
          </Grid>
        </Grid>

        <Box sx={{ display: 'flex', justifyContent: 'flex-end', gap: 1, mt: 3 }}>
          {onCancel && (
            <Button type="button" variant="text" onClick={onCancel} disabled={formSubmitting || submitting}>
              Cancel
            </Button>
          )}
          <Button
            type="submit"
            variant="contained"
            color="primary"
            disabled={formSubmitting || submitting || (dirty && hasValidationErrors)}
            startIcon={formSubmitting || submitting ? <CircularProgress size={16} color="inherit" /> : undefined}
          >
            {formSubmitting || submitting ? 'Saving…' : 'Save'}
          </Button>
        </Box>
      </Box>
    )}
  </Form>
);

export default ProductForm;
