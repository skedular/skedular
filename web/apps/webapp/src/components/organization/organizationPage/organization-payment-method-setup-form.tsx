import Button from '@mui/material/Button';
import Stack from '@mui/material/Stack';
import { SnackbarAnchorOrigin as anchorOrigin } from '@repo/shared/libs/snackbar';
import { PaymentElement, useElements, useStripe } from '@stripe/react-stripe-js';
import { useSnackbar } from 'notistack';
import { memo, useState } from 'react';
import { Form } from 'react-final-form';

type Props = {
  onCancelClick: () => void;
};

const OrganizationPaymentMethodSetupForm = ({ onCancelClick }: Props) => {
  const stripe = useStripe();
  const elements = useElements();
  const { enqueueSnackbar } = useSnackbar();
  const [isAdding, setIsAdding] = useState(false);

  const handleAddClick = async () => {
    if (!stripe || !elements) {
      return;
    }

    setIsAdding(true);

    const { error } = await stripe.confirmSetup({
      elements,
      confirmParams: {
        return_url: `${window.location.origin}/api/payment/v1/organization/add-payment-method`,
      },
    });

    enqueueSnackbar(error.message, {
      variant: 'error',
      anchorOrigin,
    });

    setIsAdding(false);
  };

  const handleCancelClick = () => {
    onCancelClick();
  };

  return (
    <Form
      onSubmit={handleAddClick}
      render={({ handleSubmit }) => (
        <Stack direction="column" component="form" noValidate onSubmit={handleSubmit} spacing={2}>
          <PaymentElement id="payment-element" />

          <Stack sx={{ justifyContent: 'flex-end' }} direction="row" spacing={1}>
            <Button color="primary" variant="contained" type="submit" disabled={isAdding || !stripe || !elements}>
              Add
            </Button>
            <Button color="secondary" variant="contained" onClick={handleCancelClick} disabled={isAdding || !stripe || !elements}>
              Cancel
            </Button>
          </Stack>
        </Stack>
      )}
    />
  );
};

export default memo(OrganizationPaymentMethodSetupForm);
