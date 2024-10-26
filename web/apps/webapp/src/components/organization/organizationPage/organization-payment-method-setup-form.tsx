import Button from '@mui/material/Button';
import Stack from '@mui/material/Stack';
import { NotificationContent, errorNotificationOptions } from '@repo/shared/components/notification';
import { PaletteModeContext } from '@repo/shared/libs/providers';
import { PaymentElement, useElements, useStripe } from '@stripe/react-stripe-js';
import { memo, useContext, useState } from 'react';
import { Form } from 'react-final-form';
import { toast } from 'react-toastify';

type Props = {
  onCancelClick: () => void;
};

const OrganizationPaymentMethodSetupForm = ({ onCancelClick }: Props) => {
  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const stripe = useStripe();
  const elements = useElements();
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

    themedToast(<NotificationContent content={error.message} />, errorNotificationOptions);

    setIsAdding(false);
  };

  const handleCancelClick = () => {
    onCancelClick();
  };

  return (
    <Form
      onSubmit={handleAddClick}
      render={({ handleSubmit }) => (
        <Stack direction="column" spacing={2} sx={{ paddingTop: 1 }} component="form" noValidate onSubmit={handleSubmit}>
          <PaymentElement id="payment-element" />

          <Stack sx={{ justifyContent: 'flex-end' }} direction="row" spacing={1}>
            <Button color="secondary" variant="contained" onClick={handleCancelClick} disabled={isAdding || !stripe || !elements}>
              Cancel
            </Button>
            <Button color="primary" variant="contained" type="submit" disabled={isAdding || !stripe || !elements}>
              Add
            </Button>
          </Stack>
        </Stack>
      )}
    />
  );
};

export default memo(OrganizationPaymentMethodSetupForm);
