import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardActions from '@mui/material/CardActions';
import CardContent from '@mui/material/CardContent';
import CircularProgress from '@mui/material/CircularProgress';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import { AddIcon, RemoveIcon } from '@repo/shared/components/icons';
import { SnackbarAnchorOrigin as anchorOrigin } from '@repo/shared/libs/snackbar';
import { joinErrors } from '@repo/shared/libs/utils';
import { Elements } from '@stripe/react-stripe-js';
import type { Stripe } from '@stripe/stripe-js';
import { loadStripe } from '@stripe/stripe-js';
import graphql from 'babel-plugin-relay/macro';
import { nanoid } from 'nanoid';
import { useSnackbar } from 'notistack';
import { memo, useState } from 'react';
import { useFragment, useMutation } from 'react-relay';
import type { organizationPaymentMethods_addOrganizationPaymentMethodIntentMutation } from './__generated__/organizationPaymentMethods_addOrganizationPaymentMethodIntentMutation.graphql';
import type { organizationPaymentMethods_query$key } from './__generated__/organizationPaymentMethods_query.graphql';
import type { organizationPaymentMethods_removeOrganizationPaymentMethodMutation } from './__generated__/organizationPaymentMethods_removeOrganizationPaymentMethodMutation.graphql';
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

  const { enqueueSnackbar } = useSnackbar();
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
          enqueueSnackbar(`Failed to add new payment method. Error: ${joinErrors(errors)}`, {
            variant: 'error',
            anchorOrigin,
          });

          setAddNewPaymentMethodState(AddOrganizationPaymentMethodState.NOT_STARTED);

          return;
        }

        if (response.addOrganizationPaymentMethodIntent) {
          setStripePromise(loadStripe(response.addOrganizationPaymentMethodIntent?.publishedKeys));

          setClientSecret(response.addOrganizationPaymentMethodIntent?.clientSecret);

          setAddNewPaymentMethodState(AddOrganizationPaymentMethodState.WAITING_FOR_PAYMENT_METHOD_DETAILS);
        } else {
          enqueueSnackbar(`Returned payment intent is null`, {
            variant: 'error',
            anchorOrigin,
          });

          setAddNewPaymentMethodState(AddOrganizationPaymentMethodState.NOT_STARTED);
        }
      },
      onError: (error) => {
        enqueueSnackbar(`Failed to add new payment method. Error: ${error.message}`, {
          variant: 'error',
          anchorOrigin,
        });

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

    commitRemoveOrganizationPaymentMethod({
      variables: {
        input: {
          clientMutationId: nanoid(),
          id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          enqueueSnackbar(`Failed to remove payment method. Error: ${joinErrors(errors)}`, {
            variant: 'error',
            anchorOrigin,
          });

          return;
        }

        setAddNewPaymentMethodState(AddOrganizationPaymentMethodState.NOT_STARTED);
        onReloadRequired();
      },
      onError: (error) => {
        enqueueSnackbar(`Failed to remove payment method. Error: ${error.message}`, {
          variant: 'error',
          anchorOrigin,
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
      <Typography variant="h6">Payment methods</Typography>
      {paymentMethodExist && (
        <>
          {rootData.organizationPaymentMethodsDetails.map(({ id, cardBrand, cardExpiryMonth, cardExpiryYear, cardLastFourDigit }) => {
            return (
              <Card elevation={24} key={id}>
                <CardContent>
                  <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
                    <Typography variant="body1">{`${cardBrand} •••• ${cardLastFourDigit}`}</Typography>
                  </Stack>

                  <Stack direction="row" spacing={1} sx={{ alignItems: 'center', flexWrap: 'wrap' }}>
                    <Typography variant="body1">{`Expires ${cardExpiryMonth}/${cardExpiryYear?.toString().slice(-2)}`}</Typography>
                  </Stack>

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
          <Typography variant="h6">Payment method</Typography>
          <Typography>No payment method setup yet</Typography>
          <Button variant="contained" startIcon={<AddIcon />} onClick={handleAddNewPaymentMethodClick}>
            Add payment method
          </Button>
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
