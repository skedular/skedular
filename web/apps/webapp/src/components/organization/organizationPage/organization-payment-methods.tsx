import { AddIcon, RemoveIcon } from '@repo/shared/components/icons';
import type { organizationPaymentMethods_addOrganizationPaymentMethodIntentMutation } from '@/queries/__generated__/organizationPaymentMethods_addOrganizationPaymentMethodIntentMutation.graphql';
import type { organizationPaymentMethods_query$key } from '@/queries/__generated__/organizationPaymentMethods_query.graphql';
import type { organizationPaymentMethods_removeOrganizationPaymentMethodMutation } from '@/queries/__generated__/organizationPaymentMethods_removeOrganizationPaymentMethodMutation.graphql';
import Button from '@mui/material/Button';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import CircularProgress from '@mui/material/CircularProgress';
import Container from '@mui/material/Container';
import Paper from '@mui/material/Paper';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import { joinErrors } from '@repo/shared/libs/utils';
import { SnackbarAnchorOrigin as anchorOrigin } from '@repo/shared/libs/snackbar';
import { Elements } from '@stripe/react-stripe-js';
import type { Stripe } from '@stripe/stripe-js';
import { loadStripe } from '@stripe/stripe-js';
import { v4 as uuidv4 } from 'uuid';
import { useSnackbar } from 'notistack';
import { memo, useState } from 'react';
import { graphql, useFragment, useMutation } from 'react-relay';
import OrganizationPaymentMethodSetupForm from './organization-payment-method-setup-form';

type Props = {
  rootDataRelay: organizationPaymentMethods_query$key;
  onRefetchRequired: () => void;
};

enum AddOrganizationPaymentMethodState {
  NOT_STARTED = 1,
  WAITING_FOR_CLIENT_SECRET,
  WAITING_FOR_PAYMENT_METHOD_DETAILS,
  WAITING_FOR_PAYMENT_METHOD_CONFIRMATION,
  PAYMENT_METHOD_SUBMITTED,
}

const OrganizationPaymentMethods = ({ rootDataRelay, onRefetchRequired }: Props) => {
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
          clientMutationId: uuidv4(),
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
        } else {
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
          clientMutationId: uuidv4(),
          id,
        },
      },
      onCompleted: (_, errors) => {
        if (errors && errors.length > 0) {
          enqueueSnackbar(`Failed to remove payment method. Error: ${joinErrors(errors)}`, {
            variant: 'error',
            anchorOrigin,
          });
        } else {
          setAddNewPaymentMethodState(AddOrganizationPaymentMethodState.NOT_STARTED);
        }

        onRefetchRequired();
      },
      onError: (error) => {
        enqueueSnackbar(`Failed to remove payment method. Error: ${error.message}`, {
          variant: 'error',
          anchorOrigin,
        });

        onRefetchRequired();
      },
    });

    setAddNewPaymentMethodState(AddOrganizationPaymentMethodState.WAITING_FOR_CLIENT_SECRET);
  };

  const paymentMethodExist = rootData.organizationPaymentMethodsDetails.length > 0;

  return (
    <Container>
      <Typography variant="h6" gutterBottom>
        Payment method
      </Typography>
      {paymentMethodExist && (
        <>
          {rootData.organizationPaymentMethodsDetails.map(({ id, cardBrand, cardExpiryMonth, cardExpiryYear, cardLastFourDigit }) => {
            return (
              <Paper
                elevation={24}
                sx={{
                  minWidth: 300,
                  maxWidth: 300,
                }}
                key={id}
              >
                <Card
                  sx={{
                    minWidth: 300,
                    maxWidth: 300,
                  }}
                >
                  <CardContent>
                    <Stack direction="row" spacing={2} sx={{ marginBottom: 1 }}>
                      <Typography gutterBottom variant="body1">
                        {`${cardBrand} •••• ${cardLastFourDigit}`}
                      </Typography>
                    </Stack>

                    <Stack direction="row" spacing={2} sx={{ marginBottom: 1 }}>
                      <Typography gutterBottom variant="body1">
                        {`Expires ${cardExpiryMonth}/${cardExpiryYear?.toString().slice(-2)}`}
                      </Typography>
                    </Stack>

                    <Button startIcon={<RemoveIcon />} onClick={() => handleRemovePaymentMethodClick(id)}>
                      Remove
                    </Button>
                  </CardContent>
                </Card>
              </Paper>
            );
          })}
        </>
      )}
      {!paymentMethodExist && addNewPaymentMethodState === AddOrganizationPaymentMethodState.NOT_STARTED && (
        <>
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
    </Container>
  );
};

export default memo(OrganizationPaymentMethods);
