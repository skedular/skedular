import { OrganizationPaymentMethodSetupForm } from '@/components/organization';
import type { addOrganizationPaymentMethodDialog_addOrganizationPaymentMethodIntentMutation } from '@/queries/__generated__/addOrganizationPaymentMethodDialog_addOrganizationPaymentMethodIntentMutation.graphql';
import CircularProgress from '@mui/material/CircularProgress';
import Dialog from '@mui/material/Dialog';
import DialogContent from '@mui/material/DialogContent';
import { DefaultDialogTitle } from '@repo/shared/components/commons';
import { errorNotificationOptions, NotificationContent, successNotificationOptions } from '@repo/shared/components/notification';
import { DialogTransition } from '@repo/shared/components/transitions';
import { PaletteModeContext } from '@repo/shared/libs/providers';
import { joinErrors } from '@repo/shared/libs/utils';
import { Elements } from '@stripe/react-stripe-js';
import type { Stripe } from '@stripe/stripe-js';
import { loadStripe } from '@stripe/stripe-js';
import { nanoid } from 'nanoid';
import { memo, useContext, useEffect, useState } from 'react';
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
  const [commitAddOrganizationPaymentMethodIntent] = useMutation<addOrganizationPaymentMethodDialog_addOrganizationPaymentMethodIntentMutation>(
    graphql`
      mutation addOrganizationPaymentMethodDialog_addOrganizationPaymentMethodIntentMutation($input: AddOrganizationPaymentMethodIntentInput!) {
        addOrganizationPaymentMethodIntent(input: $input) {
          clientMutationId
          publishedKeys
          clientSecret
        }
      }
    `,
  );

  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const [addNewPaymentMethodState, setAddNewPaymentMethodState] = useState(AddOrganizationPaymentMethodState.WAITING_FOR_CLIENT_SECRET);
  const [clientSecret, setClientSecret] = useState('');
  const [stripePromise, setStripePromise] = useState<Promise<Stripe | null>>();

  useEffect(() => {
    commitAddOrganizationPaymentMethodIntent({
      variables: {
        input: {
          clientMutationId: nanoid(),
          organizationId,
        },
      },
      onCompleted: (response, errors) => {
        if (errors && errors.length > 0) {
          themedToast(<NotificationContent content={`Failed to add new payment method. Error: ${joinErrors(errors)}.`} />, errorNotificationOptions);
          onCancel();

          return;
        }

        if (response.addOrganizationPaymentMethodIntent) {
          setStripePromise(loadStripe(response.addOrganizationPaymentMethodIntent?.publishedKeys));
          setClientSecret(response.addOrganizationPaymentMethodIntent?.clientSecret);
          setAddNewPaymentMethodState(AddOrganizationPaymentMethodState.WAITING_FOR_PAYMENT_METHOD_DETAILS);
        } else {
          themedToast(<NotificationContent content={`Payment method added.`} />, successNotificationOptions);
          onCancel();
        }
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
