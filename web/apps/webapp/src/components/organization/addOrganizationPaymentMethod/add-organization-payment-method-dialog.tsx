import { v7 as uuid } from 'uuid';
import { DefaultDialogTitle } from '@/components/commons';
import { errorNotificationOptions, NotificationContent } from '@/components/notification';
import { OrganizationPaymentMethodSetupForm } from '@/components/organization';
import { DialogTransition } from '@/components/transitions';
import { PaletteModeContext } from '@/libs/providers';
import { joinErrors } from '@/libs/utils';
import type { addOrganizationPaymentMethodDialog_addOrganizationPaymentMethodIntentMutation } from '@/queries/__generated__/addOrganizationPaymentMethodDialog_addOrganizationPaymentMethodIntentMutation.graphql';
import CircularProgress from '@mui/material/CircularProgress';
import Dialog from '@mui/material/Dialog';
import DialogContent from '@mui/material/DialogContent';
import { Elements } from '@stripe/react-stripe-js';
import type { Stripe } from '@stripe/stripe-js';
import { loadStripe } from '@stripe/stripe-js';
import { memo, useContext, useEffect, useRef, useState } from 'react';
import { graphql, useMutation } from 'react-relay';
import { toast } from 'react-toastify';

type Props = {
  organizationId: string;
  isDialogOpen: boolean;
  onCancel: () => void;
};

enum AddOrganizationPaymentMethodState {
  WAITING_FOR_CLIENT_SECRET,
  WAITING_FOR_PAYMENT_METHOD_DETAILS,
}

const AddOrganizationPaymentMethodDialog = ({ organizationId, isDialogOpen, onCancel }: Props) => {
  const [commitAddOrganizationPaymentMethodIntent] = useMutation<addOrganizationPaymentMethodDialog_addOrganizationPaymentMethodIntentMutation>(graphql`
    mutation addOrganizationPaymentMethodDialog_addOrganizationPaymentMethodIntentMutation($input: AddOrganizationPaymentMethodIntentInput!) {
      addOrganizationPaymentMethodIntent(input: $input) {
        clientMutationId
        publishedKeys
        clientSecret
      }
    }
  `);

  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const [addNewPaymentMethodState, setAddNewPaymentMethodState] = useState(AddOrganizationPaymentMethodState.WAITING_FOR_CLIENT_SECRET);
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

    commitAddOrganizationPaymentMethodIntent({
      variables: {
        input: {
          clientMutationId: uuid(),
          organizationId,
        },
      },
      onCompleted: (response, errors) => {
        if (errors && errors.length > 0) {
          themedToast(<NotificationContent content={`Failed to add new payment method. Error: ${joinErrors(errors)}.`} />, errorNotificationOptions);
          onCancel();

          return;
        }

        setStripePromise(loadStripe(response.addOrganizationPaymentMethodIntent.publishedKeys));
        setClientSecret(response.addOrganizationPaymentMethodIntent.clientSecret);
        setAddNewPaymentMethodState(AddOrganizationPaymentMethodState.WAITING_FOR_PAYMENT_METHOD_DETAILS);
      },
      onError: (error) => {
        themedToast(<NotificationContent content={`Failed to add new payment method. Error: ${error.message}.`} />, errorNotificationOptions);
        onCancel();
      },
    });

    setAddNewPaymentMethodState(AddOrganizationPaymentMethodState.WAITING_FOR_CLIENT_SECRET);
  }, [commitAddOrganizationPaymentMethodIntent, onCancel, organizationId, themedToast]);

  return (
    <Dialog slots={{ transition: DialogTransition }} open={isDialogOpen} onClose={onCancel} fullWidth>
      <DefaultDialogTitle title="Add Payment Method" />
      <DialogContent sx={{ marginTop: 2 }}>
        {addNewPaymentMethodState === AddOrganizationPaymentMethodState.WAITING_FOR_CLIENT_SECRET && <CircularProgress />}
        {addNewPaymentMethodState === AddOrganizationPaymentMethodState.WAITING_FOR_PAYMENT_METHOD_DETAILS && stripePromise && (
          <Elements stripe={stripePromise} options={{ clientSecret }}>
            <OrganizationPaymentMethodSetupForm onCancel={onCancel} />
          </Elements>
        )}
      </DialogContent>
    </Dialog>
  );
};

export default memo(AddOrganizationPaymentMethodDialog);
