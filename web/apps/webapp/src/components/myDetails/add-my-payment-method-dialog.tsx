import { DefaultDialogTitle } from '@/components/commons';
import { errorNotificationOptions, NotificationContent } from '@/components/notification';
import { DialogTransition } from '@/components/transitions';
import { PaletteModeContext } from '@/libs/providers';
import { joinErrors } from '@/libs/utils';
import type { addMyPaymentMethodDialog_addMyPaymentMethodIntentMutation } from '@/queries/__generated__/addMyPaymentMethodDialog_addMyPaymentMethodIntentMutation.graphql';
import CircularProgress from '@mui/material/CircularProgress';
import Dialog from '@mui/material/Dialog';
import DialogContent from '@mui/material/DialogContent';
import { Elements } from '@stripe/react-stripe-js';
import type { Stripe } from '@stripe/stripe-js';
import { loadStripe } from '@stripe/stripe-js';
import { nanoid } from 'nanoid';
import { memo, useContext, useEffect, useRef, useState } from 'react';
import { graphql, useMutation } from 'react-relay';
import { toast } from 'react-toastify';
import MyPaymentMethodSetupForm from './my-payment-method-setup-form';

type Props = {
  isDialogOpen: boolean;
  onCancel: () => void;
};

enum AddMyPaymentMethodState {
  WAITING_FOR_CLIENT_SECRET,
  WAITING_FOR_PAYMENT_METHOD_DETAILS,
}

const AddMyPaymentMethodDialog = ({ isDialogOpen, onCancel }: Props) => {
  const [commitAddMyPaymentMethodIntent] = useMutation<addMyPaymentMethodDialog_addMyPaymentMethodIntentMutation>(graphql`
    mutation addMyPaymentMethodDialog_addMyPaymentMethodIntentMutation($input: AddMyPaymentMethodIntentInput!) {
      addMyPaymentMethodIntent(input: $input) {
        clientMutationId
        publishedKeys
        clientSecret
      }
    }
  `);

  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const [addNewPaymentMethodState, setAddNewPaymentMethodState] = useState(AddMyPaymentMethodState.WAITING_FOR_CLIENT_SECRET);
  const [clientSecret, setClientSecret] = useState('');
  const [stripePromise, setStripePromise] = useState<Promise<Stripe | null>>();
  const hasRunRef = useRef(false);

  useEffect(() => {
    // In development, React StrictMode intentionally double-invokes useEffect to help detect side effects.
    // This guard prevents the mutation from running twice in dev mode, but has no effect in production.
    if (process.env.NODE_ENV === 'development') {
      if (hasRunRef.current) {
        return;
      }

      hasRunRef.current = true;
    }

    commitAddMyPaymentMethodIntent({
      variables: {
        input: {
          clientMutationId: nanoid(),
        },
      },
      onCompleted: (response, errors) => {
        if (errors && errors.length > 0) {
          themedToast(<NotificationContent content={`Failed to add new payment method. Error: ${joinErrors(errors)}.`} />, errorNotificationOptions);
          onCancel();

          return;
        }

        setStripePromise(loadStripe(response.addMyPaymentMethodIntent.publishedKeys));
        setClientSecret(response.addMyPaymentMethodIntent.clientSecret);
        setAddNewPaymentMethodState(AddMyPaymentMethodState.WAITING_FOR_PAYMENT_METHOD_DETAILS);
      },
      onError: (error) => {
        themedToast(<NotificationContent content={`Failed to add new payment method. Error: ${error.message}.`} />, errorNotificationOptions);
        onCancel();
      },
    });

    setAddNewPaymentMethodState(AddMyPaymentMethodState.WAITING_FOR_CLIENT_SECRET);
  }, [commitAddMyPaymentMethodIntent, onCancel, themedToast]);

  return (
    <Dialog slots={{ transition: DialogTransition }} open={isDialogOpen} onClose={onCancel} fullWidth>
      <DefaultDialogTitle title="Add Payment Method" />
      <DialogContent sx={{ marginTop: 2 }}>
        {addNewPaymentMethodState === AddMyPaymentMethodState.WAITING_FOR_CLIENT_SECRET && <CircularProgress />}
        {addNewPaymentMethodState === AddMyPaymentMethodState.WAITING_FOR_PAYMENT_METHOD_DETAILS && stripePromise && (
          <Elements stripe={stripePromise} options={{ clientSecret }}>
            <MyPaymentMethodSetupForm onCancel={onCancel} />
          </Elements>
        )}
      </DialogContent>
    </Dialog>
  );
};

export default memo(AddMyPaymentMethodDialog);
