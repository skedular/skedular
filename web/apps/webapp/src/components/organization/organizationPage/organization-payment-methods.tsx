import type { organizationPaymentMethods_addOrganizationPaymentMethodIntentMutation } from '@/queries/__generated__/organizationPaymentMethods_addOrganizationPaymentMethodIntentMutation.graphql';
import type { organizationPaymentMethods_query$key } from '@/queries/__generated__/organizationPaymentMethods_query.graphql';
import type { organizationPaymentMethods_removeOrganizationPaymentMethodMutation } from '@/queries/__generated__/organizationPaymentMethods_removeOrganizationPaymentMethodMutation.graphql';
import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardActions from '@mui/material/CardActions';
import CardContent from '@mui/material/CardContent';
import CircularProgress from '@mui/material/CircularProgress';
import { BodyIconTypography, LeadIconTypography, StackRow } from '@repo/shared/components/commons';
import { AddIcon, RemoveIcon } from '@repo/shared/components/icons';
import {
  NotificationContent,
  errorNotificationOptions,
  infoNotificationOptions,
  successNotificationOptions,
} from '@repo/shared/components/notification';
import { PaletteModeContext } from '@repo/shared/libs/providers';
import { joinErrors } from '@repo/shared/libs/utils';
import { Elements } from '@stripe/react-stripe-js';
import type { Stripe } from '@stripe/stripe-js';
import { loadStripe } from '@stripe/stripe-js';
import { nanoid } from 'nanoid';
import { memo, useContext, useState } from 'react';
import { graphql, useFragment, useMutation } from 'react-relay';
import { toast } from 'react-toastify';
import OrganizationPaymentMethodSetupForm from './organization-payment-method-setup-form';

type Props = {
  rootDataRelay: organizationPaymentMethods_query$key;
  onReloadRequired: () => void;
};

enum AddOrganizationPaymentMethodState {
  NOT_STARTED = 1,
  WAITING_FOR_CLIENT_SECRET,
  WAITING_FOR_PAYMENT_METHOD_DETAILS,
  WAITING_FOR_PAYMENT_METHOD_CONFIRMATION,
  PAYMENT_METHOD_SUBMITTED,
}

const OrganizationPaymentMethods = ({ rootDataRelay, onReloadRequired }: Props) => {
  const rootData = useFragment<organizationPaymentMethods_query$key>(
    graphql`
      fragment organizationPaymentMethods_query on Query {
        organization(id: $organizationId) {
          id
        }
        organizationPaymentMethodsDetails(organizationId: $organizationId) {
          id
          cardBrand
          cardExpiryMonth
          cardExpiryYear
          cardLastFourDigit
        }
      }
    `,
    rootDataRelay,
  );

  const [commitAddOrganizationPaymentMethodIntent] = useMutation<organizationPaymentMethods_addOrganizationPaymentMethodIntentMutation>(graphql`
    mutation organizationPaymentMethods_addOrganizationPaymentMethodIntentMutation($input: AddOrganizationPaymentMethodIntentInput!) {
      addOrganizationPaymentMethodIntent(input: $input) {
        clientMutationId
        publishedKeys
        clientSecret
      }
    }
  `);

  const [commitRemoveOrganizationPaymentMethod] = useMutation<organizationPaymentMethods_removeOrganizationPaymentMethodMutation>(graphql`
    mutation organizationPaymentMethods_removeOrganizationPaymentMethodMutation($input: RemoveOrganizationPaymentMethodInput!) {
      removeOrganizationPaymentMethod(input: $input) {
        clientMutationId
      }
    }
  `);

  const paletteMode = useContext(PaletteModeContext);
  const themedToast = paletteMode === 'dark' ? toast.dark : toast;
  const [addNewPaymentMethodState, setAddNewPaymentMethodState] = useState(AddOrganizationPaymentMethodState.NOT_STARTED);
  const [clientSecret, setClientSecret] = useState('');
  const [stripePromise, setStripePromise] = useState<Promise<Stripe | null>>();

  const handleAddNewPaymentMethodClick = () => {
    if (!rootData.organization) {
      return;
    }

    commitAddOrganizationPaymentMethodIntent({
      variables: {
        input: {
          clientMutationId: nanoid(),
          organizationId: rootData.organization.id,
        },
      },
      onCompleted: (response, errors) => {
        if (errors && errors.length > 0) {
          themedToast(<NotificationContent content={`Failed to add new payment method. Error: ${joinErrors(errors)}.`} />, errorNotificationOptions);
          setAddNewPaymentMethodState(AddOrganizationPaymentMethodState.NOT_STARTED);

          return;
        }

        if (response.addOrganizationPaymentMethodIntent) {
          setStripePromise(loadStripe(response.addOrganizationPaymentMethodIntent?.publishedKeys));
          setClientSecret(response.addOrganizationPaymentMethodIntent?.clientSecret);
          setAddNewPaymentMethodState(AddOrganizationPaymentMethodState.WAITING_FOR_PAYMENT_METHOD_DETAILS);
        } else {
          themedToast(<NotificationContent content={`Payment method added.`} />, successNotificationOptions);
          setAddNewPaymentMethodState(AddOrganizationPaymentMethodState.NOT_STARTED);
        }
      },
      onError: (error) => {
        themedToast(<NotificationContent content={`Failed to add new payment method. Error: ${error.message}.`} />, errorNotificationOptions);
        setAddNewPaymentMethodState(AddOrganizationPaymentMethodState.NOT_STARTED);
      },
    });

    setAddNewPaymentMethodState(AddOrganizationPaymentMethodState.WAITING_FOR_CLIENT_SECRET);
  };

  const handleCancelAddPaymentMethodClick = () => {
    setAddNewPaymentMethodState(AddOrganizationPaymentMethodState.NOT_STARTED);
  };

  const handleRemovePaymentMethodClick = (id: string) => {
    if (!rootData.organization) {
      return;
    }

    const toastId = themedToast(<NotificationContent content={`Removing payment method...`} />, infoNotificationOptions);

    commitRemoveOrganizationPaymentMethod({
      variables: {
        input: {
          clientMutationId: nanoid(),
          id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          toast.update(toastId, {
            ...errorNotificationOptions,
            render: <NotificationContent content={`Failed to remove payment method. Error: ${joinErrors(errors)}.`} />,
          });

          return;
        }

        toast.update(toastId, {
          ...successNotificationOptions,
          render: <NotificationContent content={`Payment method removed.`} />,
        });

        setAddNewPaymentMethodState(AddOrganizationPaymentMethodState.NOT_STARTED);
        onReloadRequired();
      },
      onError: (error) => {
        toast.update(toastId, {
          ...errorNotificationOptions,
          render: <NotificationContent content={`Failed to remove payment method. Error: ${error.message}.`} />,
        });

        onReloadRequired();
      },
    });

    setAddNewPaymentMethodState(AddOrganizationPaymentMethodState.WAITING_FOR_CLIENT_SECRET);
  };

  if (!rootData.organizationPaymentMethodsDetails) {
    return <></>;
  }

  const paymentMethodExist = rootData.organizationPaymentMethodsDetails.length > 0;

  return (
    <>
      <LeadIconTypography label="Payment methods" />
      {paymentMethodExist && (
        <>
          {rootData.organizationPaymentMethodsDetails.map(({ id, cardBrand, cardExpiryMonth, cardExpiryYear, cardLastFourDigit }) => {
            return (
              <Card elevation={24} key={id}>
                <CardContent>
                  <BodyIconTypography label={`${cardBrand} •••• ${cardLastFourDigit}`} />
                  <BodyIconTypography label={`Expires ${cardExpiryMonth}/${cardExpiryYear?.toString().slice(-2)}`} />
                  <CardActions sx={{ justifyContent: 'flex-end' }}>
                    <Button startIcon={<RemoveIcon />} onClick={() => handleRemovePaymentMethodClick(id)}>
                      Remove
                    </Button>
                  </CardActions>
                </CardContent>
              </Card>
            );
          })}
        </>
      )}
      {!paymentMethodExist && addNewPaymentMethodState === AddOrganizationPaymentMethodState.NOT_STARTED && (
        <>
          <BodyIconTypography label="No payment method setup yet" />
          <StackRow>
            <Button variant="contained" size="small" startIcon={<AddIcon />} onClick={handleAddNewPaymentMethodClick}>
              Add payment method
            </Button>
          </StackRow>
        </>
      )}
      {!paymentMethodExist && addNewPaymentMethodState === AddOrganizationPaymentMethodState.WAITING_FOR_CLIENT_SECRET && <CircularProgress />}
      {!paymentMethodExist && addNewPaymentMethodState === AddOrganizationPaymentMethodState.WAITING_FOR_PAYMENT_METHOD_DETAILS && stripePromise && (
        <Elements stripe={stripePromise} options={{ clientSecret }}>
          <OrganizationPaymentMethodSetupForm onCancelClick={handleCancelAddPaymentMethodClick} />
        </Elements>
      )}
      {!paymentMethodExist && addNewPaymentMethodState === AddOrganizationPaymentMethodState.WAITING_FOR_PAYMENT_METHOD_CONFIRMATION && (
        <CircularProgress />
      )}
    </>
  );
};

export default memo(OrganizationPaymentMethods);
