import { FormStackColumn, TwoButtonsDialogActions } from '@repo/shared/components/commons';
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
        <FormStackColumn onSubmit={handleSubmit}>
          <PaymentElement id="payment-element" />

          <TwoButtonsDialogActions
            onSecondaryClicked={handleCancelClick}
            primaryLabel="Add"
            secondaryLabel="Cancel"
            disabled={isAdding || !stripe || !elements}
          />
        </FormStackColumn>
      )}
    />
  );
};

export default memo(OrganizationPaymentMethodSetupForm);
