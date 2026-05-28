import { DefaultDialogTitle } from '@skedular/ui';
import { errorNotificationOptions, NotificationContent } from '@/components/notification';
import { OrganizationPaymentMethodSetupForm } from '@/components/organization';
import { DialogTransition } from '@/components/transitions';
import { PaletteModeContext } from '@skedular/shared';
import { getRelayErrorMessage } from '@skedular/shared';
import type { addOrganizationPaymentMethodDialog_addOrganizationPaymentMethodIntentMutation } from '@/queries/__generated__/addOrganizationPaymentMethodDialog_addOrganizationPaymentMethodIntentMutation.graphql';
import CircularProgress from '@mui/material/CircularProgress';
import Dialog from '@mui/material/Dialog';
import DialogContent from '@mui/material/DialogContent';
import { Elements } from '@stripe/react-stripe-js';
import type { Stripe } from '@stripe/stripe-js';
import { loadStripe } from '@stripe/stripe-js';
import { memo, useCallback, useContext, useEffect, useRef, useState } from 'react';
import { graphql, useMutation } from 'react-relay';
import { toast } from 'react-toastify';
import { v7 as uuid } from 'uuid';

type Props = {
  organizationCustomDomain: string;
  isDialogOpen: boolean;
  onCancel: () => void;
};

enum AddOrganizationPaymentMethodState {
  WAITING_FOR_CLIENT_SECRET,
  WAITING_FOR_PAYMENT_METHOD_DETAILS,
}

const AddOrganizationPaymentMethodDialog = ({ organizationCustomDomain, isDialogOpen, onCancel }: Props) => {
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

  const handleDialogClose = useCallback(() => {
    hasRunRef.current = false;
    setAddNewPaymentMethodState(AddOrganizationPaymentMethodState.WAITING_FOR_CLIENT_SECRET);
    setClientSecret('');
    setStripePromise(undefined);
    onCancel();
  }, [onCancel]);

  const fetchPaymentMethodIntent = useCallback(() => {
    setAddNewPaymentMethodState(AddOrganizationPaymentMethodState.WAITING_FOR_CLIENT_SECRET);
    setClientSecret('');
    setStripePromise(undefined);

    commitAddOrganizationPaymentMethodIntent({
      variables: {
        input: {
          clientMutationId: uuid(),
          organizationCustomDomain,
        },
      },
      onCompleted: (response, errors) => {
        if (errors && errors.length > 0) {
          themedToast(<NotificationContent content={`Failed to add new payment method. Error: ${getRelayErrorMessage(errors)}.`} />, errorNotificationOptions);
          handleDialogClose();

          return;
        }

        setStripePromise(loadStripe(response.addOrganizationPaymentMethodIntent.publishedKeys));
        setClientSecret(response.addOrganizationPaymentMethodIntent.clientSecret);
        setAddNewPaymentMethodState(AddOrganizationPaymentMethodState.WAITING_FOR_PAYMENT_METHOD_DETAILS);
      },
      onError: (error) => {
        themedToast(<NotificationContent content={`Failed to add new payment method. Error: ${error.message}.`} />, errorNotificationOptions);
        handleDialogClose();
      },
    });
  }, [commitAddOrganizationPaymentMethodIntent, handleDialogClose, organizationCustomDomain, themedToast]);

  useEffect(() => {
    if (!isDialogOpen) {
      hasRunRef.current = false;
      return;
    }

    // In development, React StrictMode intentionally double-invokes useEffect to help detect side effects.
    // This guard prevents the mutation from running twice in dev mode, but has no effect in production.
    if (process.env.NODE_ENV === 'development') {
      if (hasRunRef.current) {
        return;
      }

      hasRunRef.current = true;
    }

    queueMicrotask(() => {
      fetchPaymentMethodIntent();
    });
  }, [fetchPaymentMethodIntent, isDialogOpen]);

  return (
    <Dialog slots={{ transition: DialogTransition }} open={isDialogOpen} onClose={handleDialogClose} fullWidth>
      <DefaultDialogTitle title="Add Payment Method" />
      <DialogContent sx={{ marginTop: 2 }}>
        {addNewPaymentMethodState === AddOrganizationPaymentMethodState.WAITING_FOR_CLIENT_SECRET && <CircularProgress />}
        {addNewPaymentMethodState === AddOrganizationPaymentMethodState.WAITING_FOR_PAYMENT_METHOD_DETAILS && stripePromise && (
          <Elements stripe={stripePromise} options={{ clientSecret }}>
            <OrganizationPaymentMethodSetupForm onCancel={handleDialogClose} />
          </Elements>
        )}
      </DialogContent>
    </Dialog>
  );
};

export default memo(AddOrganizationPaymentMethodDialog);
